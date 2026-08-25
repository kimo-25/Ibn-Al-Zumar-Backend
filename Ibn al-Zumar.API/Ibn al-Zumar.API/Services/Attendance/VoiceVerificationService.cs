using System.Numerics;
using System.Text;

namespace IbnAlZumar.API.Services.Attendance;

/// <summary>
/// خدمة تحليل الصوت محلياً (بدون أي اعتماد على Hugging Face أو أي API خارجي).
/// تستقبل ملف WAV (PCM) - وهو ما يرسله الفرونت إند فعلياً بعد audioToWav.js -
/// وتستخرج منه متجه ميزات (float[]) ثابت الطول يمكن استخدامه لاحقاً في مقارنة
/// المتحدثين عبر Cosine Similarity.
///
/// الميزات المستخرجة (55 قيمة ثابتة بغض النظر عن طول التسجيل):
///   - 26 قيمة: متوسط طاقة كل فلتر Mel (Log Mel Filterbank Energies - مثل أساس MFCC)
///   - 26 قيمة: الانحراف المعياري لنفس الفلاتر عبر زمن التسجيل
///   - 1 قيمة: متوسط طاقة الإشارة (Energy)
///   - 1 قيمة: متوسط معدل عبور الصفر (Zero Crossing Rate)
///   - 1 قيمة: متوسط تردد النغمة الأساسية (Pitch) للأطر المصوّتة
///
/// ⚠️ ملاحظة مهمة: هذا النهج (Mel filterbank + إحصائيات) أبسط بكثير من نموذج
/// عصبي متخصص مثل ECAPA-TDNN، لذا يُنصح بإعادة ضبط MatchThreshold في
/// AttendanceService بعد التجربة الفعلية مع موظفيكم (ابدأ بقيمة أقل، مثلاً 0.55-0.65،
/// وارفعها تدريجياً حسب معدلات القبول/الرفض الخاطئة).
/// </summary>
public class VoiceVerificationService : IVoiceVerificationService
{
    private const int NumMelFilters = 26;
    private const double FrameDurationSeconds = 0.025; // 25ms
    private const double HopDurationSeconds = 0.010;   // 10ms
    private const double PreEmphasisCoefficient = 0.97;

    public Task<float[]> ExtractVoiceEmbeddingAsync(Stream audioStream, string fileName, CancellationToken cancellationToken = default)
    {
        // العمل حسابي (CPU-bound) بحت وسريع جداً (ميلي ثواني) - نلفّه في Task.Run
        // فقط للحفاظ على التوقيع async الأصلي للواجهة دون تغييرها.
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var memoryStream = new MemoryStream();
            audioStream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            if (bytes.Length == 0)
            {
                return Array.Empty<float>();
            }

            WavAudio wav;
            try
            {
                wav = ParseWav(bytes);
            }
            catch
            {
                // ملف تالف أو صيغة غير مدعومة - نرجع مصفوفة فارغة ليتعامل معها الكولر
                // (AttendanceService يتحقق من embedding.Length == 0 بالفعل).
                return Array.Empty<float>();
            }

            // نطلب على الأقل ~0.3 ثانية صوت حتى تكون الميزات ذات معنى
            if (wav.Samples.Length < wav.SampleRate * 0.3)
            {
                return Array.Empty<float>();
            }

            return ExtractFeatures(wav.Samples, wav.SampleRate);
        }, cancellationToken);
    }

    public double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA == null || vectorB == null || vectorA.Length == 0 || vectorA.Length != vectorB.Length)
        {
            return 0d;
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (var i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0d;
        }

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }

    // =========================================================
    //                    WAV Parsing
    // =========================================================

    private sealed class WavAudio
    {
        public float[] Samples { get; init; } = Array.Empty<float>();
        public int SampleRate { get; init; }
    }

    private static WavAudio ParseWav(byte[] data)
    {
        if (data.Length < 44 ||
            data[0] != 'R' || data[1] != 'I' || data[2] != 'F' || data[3] != 'F' ||
            data[8] != 'W' || data[9] != 'A' || data[10] != 'V' || data[11] != 'E')
        {
            throw new InvalidOperationException("الملف الصوتي ليس بصيغة WAV صالحة.");
        }

        int pos = 12; // بعد رأس RIFF/WAVE
        int audioFormat = 1;
        int numChannels = 1;
        int sampleRate = 16000;
        int bitsPerSample = 16;
        byte[]? audioData = null;

        while (pos + 8 <= data.Length)
        {
            var chunkId = Encoding.ASCII.GetString(data, pos, 4);
            var chunkSize = BitConverter.ToInt32(data, pos + 4);
            var chunkStart = pos + 8;

            if (chunkSize < 0 || chunkStart + chunkSize > data.Length)
            {
                // حماية من ملفات مشوهة الحجم - نتوقف بأمان بما جُمع حتى الآن
                break;
            }

            if (chunkId == "fmt ")
            {
                audioFormat = BitConverter.ToInt16(data, chunkStart);
                numChannels = BitConverter.ToInt16(data, chunkStart + 2);
                sampleRate = BitConverter.ToInt32(data, chunkStart + 4);
                bitsPerSample = BitConverter.ToInt16(data, chunkStart + 14);
            }
            else if (chunkId == "data")
            {
                audioData = new byte[chunkSize];
                Array.Copy(data, chunkStart, audioData, 0, chunkSize);
            }

            // الـ chunks في WAV محاذاة على كلمة (word-aligned)
            pos = chunkStart + chunkSize + (chunkSize % 2);
        }

        if (audioData == null || audioData.Length == 0)
        {
            throw new InvalidOperationException("لم يتم العثور على بيانات صوتية (data chunk) داخل ملف WAV.");
        }

        if (numChannels <= 0) numChannels = 1;
        if (bitsPerSample <= 0) bitsPerSample = 16;

        var mono = DecodeSamplesToMono(audioData, numChannels, bitsPerSample, audioFormat);
        return new WavAudio { Samples = mono, SampleRate = sampleRate };
    }

    private static float[] DecodeSamplesToMono(byte[] audioData, int numChannels, int bitsPerSample, int audioFormat)
    {
        var bytesPerSample = Math.Max(1, bitsPerSample / 8);
        var totalSamples = audioData.Length / bytesPerSample;
        var frameCount = totalSamples / Math.Max(1, numChannels);

        var mono = new float[Math.Max(0, frameCount)];

        for (var frame = 0; frame < frameCount; frame++)
        {
            double sum = 0;

            for (var ch = 0; ch < numChannels; ch++)
            {
                var byteOffset = (frame * numChannels + ch) * bytesPerSample;
                if (byteOffset + bytesPerSample > audioData.Length) continue;

                double sample = bitsPerSample switch
                {
                    8 => (audioData[byteOffset] - 128) / 128.0,
                    16 => BitConverter.ToInt16(audioData, byteOffset) / 32768.0,
                    24 => Decode24BitSample(audioData, byteOffset),
                    32 when audioFormat == 3 => BitConverter.ToSingle(audioData, byteOffset), // IEEE float
                    32 => BitConverter.ToInt32(audioData, byteOffset) / 2147483648.0,
                    _ => 0
                };

                sum += sample;
            }

            mono[frame] = (float)(sum / numChannels);
        }

        return mono;
    }

    private static double Decode24BitSample(byte[] data, int offset)
    {
        int val = (data[offset + 2] << 16) | (data[offset + 1] << 8) | data[offset];
        if ((val & 0x800000) != 0)
        {
            val = unchecked((int)(val | 0xFF000000));
        }
        return val / 8388608.0;
    }

    // =========================================================
    //                 Feature Extraction
    // =========================================================

    private static float[] ExtractFeatures(float[] samples, int sampleRate)
    {
        // Pre-emphasis: يبرز الترددات العالية التي تحمل خصائص الصوت المميزة
        var pre = new float[samples.Length];
        pre[0] = samples[0];
        for (var i = 1; i < samples.Length; i++)
        {
            pre[i] = samples[i] - (float)PreEmphasisCoefficient * samples[i - 1];
        }

        var frameLength = Math.Max(16, (int)(sampleRate * FrameDurationSeconds));
        var hopLength = Math.Max(8, (int)(sampleRate * HopDurationSeconds));
        var fftSize = NextPowerOfTwo(frameLength);

        var melFilters = BuildMelFilterbank(NumMelFilters, fftSize, sampleRate);

        var melEnergiesPerFrame = new List<double[]>();
        var frameEnergies = new List<double>();
        var frameZcr = new List<double>();
        var voicedPitches = new List<double>();

        for (var start = 0; start + frameLength <= pre.Length; start += hopLength)
        {
            var frame = new float[frameLength];
            Array.Copy(pre, start, frame, 0, frameLength);

            double energy = 0;
            for (var i = 0; i < frameLength; i++)
            {
                // نافذة Hamming لتقليل تسرب الطيف (spectral leakage)
                var w = 0.54 - 0.46 * Math.Cos(2 * Math.PI * i / (frameLength - 1));
                frame[i] = (float)(frame[i] * w);
                energy += frame[i] * frame[i];
            }

            var avgEnergy = energy / frameLength;
            frameEnergies.Add(avgEnergy);
            frameZcr.Add(ZeroCrossingRate(frame));

            var complexBuffer = new Complex[fftSize];
            for (var i = 0; i < fftSize; i++)
            {
                complexBuffer[i] = i < frameLength ? new Complex(frame[i], 0) : Complex.Zero;
            }

            FFT(complexBuffer);

            var powerSpectrum = new double[fftSize / 2 + 1];
            for (var i = 0; i <= fftSize / 2; i++)
            {
                powerSpectrum[i] = complexBuffer[i].Magnitude * complexBuffer[i].Magnitude;
            }

            var melEnergies = new double[NumMelFilters];
            for (var m = 0; m < NumMelFilters; m++)
            {
                double sum = 0;
                var filter = melFilters[m];
                for (var i = 0; i < powerSpectrum.Length; i++)
                {
                    sum += powerSpectrum[i] * filter[i];
                }
                melEnergies[m] = Math.Log(sum + 1e-10);
            }
            melEnergiesPerFrame.Add(melEnergies);

            // نقدّر الـ pitch فقط للأطر ذات طاقة كافية (أطر مصوّتة فعلياً)
            if (avgEnergy > 1e-5)
            {
                var pitch = EstimatePitch(frame, sampleRate);
                if (pitch > 0) voicedPitches.Add(pitch);
            }
        }

        if (melEnergiesPerFrame.Count == 0)
        {
            return Array.Empty<float>();
        }

        var meanMel = new double[NumMelFilters];
        var stdMel = new double[NumMelFilters];

        for (var m = 0; m < NumMelFilters; m++)
        {
            var mean = melEnergiesPerFrame.Sum(e => e[m]) / melEnergiesPerFrame.Count;
            var variance = melEnergiesPerFrame.Sum(e => (e[m] - mean) * (e[m] - mean)) / melEnergiesPerFrame.Count;
            meanMel[m] = mean;
            stdMel[m] = Math.Sqrt(variance);
        }

        var avgFrameEnergy = frameEnergies.Count > 0 ? frameEnergies.Average() : 0;
        var avgZcr = frameZcr.Count > 0 ? frameZcr.Average() : 0;
        var avgPitch = voicedPitches.Count > 0 ? voicedPitches.Average() : 0;

        var features = new List<float>(NumMelFilters * 2 + 3);
        features.AddRange(meanMel.Select(v => (float)v));
        features.AddRange(stdMel.Select(v => (float)v));
        features.Add((float)avgFrameEnergy);
        features.Add((float)avgZcr);
        features.Add((float)avgPitch);

        return features.ToArray();
    }

    private static double ZeroCrossingRate(float[] frame)
    {
        var crossings = 0;
        for (var i = 1; i < frame.Length; i++)
        {
            if ((frame[i] >= 0) != (frame[i - 1] >= 0)) crossings++;
        }
        return (double)crossings / frame.Length;
    }

    private static double EstimatePitch(float[] frame, int sampleRate)
    {
        // تقدير النغمة الأساسية عبر Autocorrelation في مدى صوت بشري نموذجي (50-500Hz)
        var minLag = sampleRate / 500;
        var maxLag = Math.Min(frame.Length - 1, sampleRate / 50);
        if (minLag < 1) minLag = 1;
        if (minLag >= maxLag) return 0;

        double bestCorr = 0;
        var bestLag = -1;

        for (var lag = minLag; lag <= maxLag; lag++)
        {
            double corr = 0;
            for (var i = 0; i < frame.Length - lag; i++)
            {
                corr += frame[i] * frame[i + lag];
            }

            if (corr > bestCorr)
            {
                bestCorr = corr;
                bestLag = lag;
            }
        }

        return bestLag > 0 ? (double)sampleRate / bestLag : 0;
    }

    // =========================================================
    //                 Mel Filterbank + FFT
    // =========================================================

    private static double[][] BuildMelFilterbank(int numFilters, int fftSize, int sampleRate)
    {
        var melMin = HzToMel(0);
        var melMax = HzToMel(sampleRate / 2.0);

        var melPoints = new double[numFilters + 2];
        for (var i = 0; i < melPoints.Length; i++)
        {
            melPoints[i] = melMin + (melMax - melMin) * i / (numFilters + 1);
        }

        var hzPoints = melPoints.Select(MelToHz).ToArray();
        var binPoints = hzPoints
            .Select(hz => (int)Math.Floor((fftSize + 1) * hz / sampleRate))
            .ToArray();

        var specSize = fftSize / 2 + 1;
        var filters = new double[numFilters][];

        for (var m = 0; m < numFilters; m++)
        {
            filters[m] = new double[specSize];
            var left = binPoints[m];
            var center = binPoints[m + 1];
            var right = binPoints[m + 2];

            for (var k = Math.Max(0, left); k < Math.Min(specSize, center); k++)
            {
                if (center > left) filters[m][k] = (double)(k - left) / (center - left);
            }
            for (var k = Math.Max(0, center); k < Math.Min(specSize, right); k++)
            {
                if (right > center) filters[m][k] = (double)(right - k) / (right - center);
            }
        }

        return filters;
    }

    private static double HzToMel(double hz) => 2595.0 * Math.Log10(1.0 + hz / 700.0);
    private static double MelToHz(double mel) => 700.0 * (Math.Pow(10.0, mel / 2595.0) - 1.0);

    private static int NextPowerOfTwo(int n)
    {
        var power = 1;
        while (power < n) power <<= 1;
        return power;
    }

    /// <summary>
    /// FFT تكراري (Cooley-Tukey, radix-2) - لا يعتمد على أي مكتبة خارجية.
    /// </summary>
    private static void FFT(Complex[] buffer)
    {
        var n = buffer.Length;
        if (n <= 1) return;

        for (int i = 1, j = 0; i < n; i++)
        {
            var bit = n >> 1;
            for (; (j & bit) != 0; bit >>= 1)
            {
                j ^= bit;
            }
            j ^= bit;

            if (i < j)
            {
                (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
            }
        }

        for (var len = 2; len <= n; len <<= 1)
        {
            var angle = -2 * Math.PI / len;
            var wlen = new Complex(Math.Cos(angle), Math.Sin(angle));

            for (var i = 0; i < n; i += len)
            {
                var w = Complex.One;
                for (var j = 0; j < len / 2; j++)
                {
                    var u = buffer[i + j];
                    var v = buffer[i + j + len / 2] * w;
                    buffer[i + j] = u + v;
                    buffer[i + j + len / 2] = u - v;
                    w *= wlen;
                }
            }
        }
    }
}