/* ======================================================================
   Jadever Products - Pearl Group Exclusive Agent Price List
   Auto-generated INSERT statements for SQL Server (SSMS)
   Source: »Ì—· Ã—Ê» ÃœÌœ Ã«œÌ›— 23-04-2026 (Exclusive Agent Price List)

   Notes:
   - CurrentCostPrice = unit price from the agent price list («·”⁄—)
   - SellingPrice was NOT present in the catalog, so it was estimated
     using a 30% markup over CurrentCostPrice (round to 2 decimals).
     Adjust as needed for your actual pricing strategy.
   - Barcode was not provided in the catalog -> inserted as NULL.
   - CategoryId = 1 and BrandId = 1 for all rows as requested.
   - QuantityPerCarton fixed at 1 as requested (catalog carton qty
     is kept in a comment next to each row for reference).
   ====================================================================== */

SET NOCOUNT ON;
BEGIN TRAN;

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDS520', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-Ion cordless drill', -- Name
    N'‘‰ÌÊ— »ÿ«—ÌÂ 12 ›Ê·  ”—⁄«  1 »ÿ«—ÌÂ (Type C) »œÊ‰ ‘«Õ‰', -- NameAr
    N'Voltage:12V; No-load speed:0-400/0-1500rpm; Max torque:20Nm; Plastic chuck; Chuck capacity:0.8-10mm; Torque settings:15+1; Mechanical 2-speed gear; Integrated LED work light; Includes 1 Pcs 1.5Ah battery pack(JDLBS5150); Battery charging port: USB type-C | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    1157.52, -- SellingPrice (estimated: cost + 30% markup)
    890.4, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDT1B77', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'77 Pcs tools set', -- Name
    N'‘‰ÿ… »·«” Ìﬂ 16 »Ê’… 77 ﬁÿ⁄… ‘‰ÌÊ— 12 ›Ê·  1 »ÿ«—Ì… 75 ﬁÿ⁄… ⁄œ… Ê ≈ﬂ””Ê«—« ', -- NameAr
    N'77 Pcs tools set includes: 1Pcs 12V cordless drill, battery pack, USB type-A to type-C cable, 1/4 inch*100mm magnetic shank, 10Pcs 2 inch screwdriver bits (SL4/SL5/SL6/PH1/PH2/PH3/T10/T15/T20/T30), 6 inch combination pliers, 6 inch adjustable wrench, 140mm screwdriver tester | Unit: SET | Catalog Qty/Carton: 6', -- Description
    1856.24, -- SellingPrice (estimated: cost + 30% markup)
    1427.88, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDT4B77', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'77 Pcs tools set', -- Name
    N'‘‰ÿ… »·«” Ìﬂ 16 »Ê’… 77 ﬁÿ⁄… ‘‰ÌÊ— 12 ›Ê·  1 »ÿ«—Ì… 75 ﬁÿ⁄… ⁄œ… Ê ≈ﬂ””Ê«—« ', -- NameAr
    N'77 Pcs tools set includes: 1Pcs 12V cordless drill(JDCDS518), battery pack, USB type-A to type-C cable, 1/4 inch*100mm magnetic shank, 10Pcs 2 inch screwdriver bits, 6 inch combination pliers, 6 inch adjustable wrench, 140mm screwdriver tester | Unit: SET | Catalog Qty/Carton: 6', -- Description
    1856.4, -- SellingPrice (estimated: cost + 30% markup)
    1428.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDP521', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-Ion impact drill', -- Name
    N'‘‰ÌÊ— 20 ›Ê·  œﬁ«ﬁ + »ÿ«—Ì… Ê‘«Õ‰', -- NameAr
    N'Voltage:20V; No-load speed:0-400/0-1500rpm; Max impact rate:22500bpm; Max torque:35Nm; Plastic chuck; Chuck capacity:0.8-10mm; Torque settings:18+1+1; Mechanical 2-speed gear; Integrated LED work light; Includes 1 Pcs 1.5Ah battery pack(JDLBP515) | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    2091.8, -- SellingPrice (estimated: cost + 30% markup)
    1609.08, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDP5282', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Compact Brushless Cordless Impact Drill', -- Name
    N'‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 52 ‰ÌÊ ‰ 1 »ÿ«—Ì… + ‘«Õ‰', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:52Nm; Plastic chuck; Chuck capacity:13mm; Torque settings:22+1+1; Mechanical 2-speed gear; Spindle lock function; Integrated LED work light | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    1978.6, -- SellingPrice (estimated: cost + 30% markup)
    1522.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDP5281', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Compact Brushless Cordless Impact Drill', -- Name
    N'‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 52 ‰ÌÊ ‰ 2 »ÿ«—Ì… + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:52Nm; Plastic chuck; Chuck capacity:13mm; Torque settings:22+1+1; Mechanical 2-speed gear; Spindle lock function; Integrated LED work light | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    2843.1, -- SellingPrice (estimated: cost + 30% markup)
    2187.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDP6281', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Compact Brushless Cordless Impact Drill', -- Name
    N'‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 62 ‰ÌÊ ‰ 2 »ÿ«—Ì… ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:62Nm; Plastic chuck; Chuck capacity:13mm; Torque settings:22+1+1; Mechanical 2-speed gear; Spindle lock function; Integrated LED work light | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    3183.7, -- SellingPrice (estimated: cost + 30% markup)
    2449.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDP7281', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Compact Brushless Cordless Impact Drill', -- Name
    N'‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 72 ‰ÌÊ ‰ Ÿ—› „⁄œ‰ 2 »ÿ«—Ì… + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:72Nm; Metal chuck; Chuck capacity:13mm; Torque settings:22+1+1; Mechanical 2-speed gear; Spindle lock function; Integrated LED work light | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    3372.2, -- SellingPrice (estimated: cost + 30% markup)
    2594.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCDP9281', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Brushless Cordless Impact Drill', -- Name
    N'‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 92 ‰ÌÊ ‰ Ÿ—› „⁄œ‰ 2 »ÿ«—Ì… 4 √„»Ì— + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:92Nm; Metal chuck; Chuck capacity:13mm; Torque settings:22+1+1; Mechanical 2-speed gear; Spindle lock function; Integrated LED work light | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    5057.0, -- SellingPrice (estimated: cost + 30% markup)
    3890.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDT4B91', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'91 Pcs Tools set', -- Name
    N'ﬂÌ  ‘‰ÌÊ— 91 ﬁÿ⁄… ‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 62 ‰ÌÊ ‰ 2 »ÿ«—Ì… ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Includes 1 Pcs 20V Lithium-ion impact drill(JDCDP6281); Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:62Nm; Metal chuck; Chuck capacity:13mm; Torque settings:22+1+1; Spindle lock function | Unit: SET | Catalog Qty/Carton: 4', -- Description
    3807.7, -- SellingPrice (estimated: cost + 30% markup)
    2929.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDT4B119', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'119 Pcs Tools set', -- Name
    N'ﬂÌ  ‘‰ÌÊ— 119 ﬁÿ⁄… ‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 62 ‰ÌÊ ‰ 2 »ÿ«—Ì… ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'1 Pcs 20V Lithium-ion impact drill(JDCDP6281); Brushless motor; Voltage:20V; No-load speed:0-500/0-2000rpm; Max impact rate:30000bpm; Max torque:62Nm; Metal chuck; Chuck capacity:13mm; Torque settings:22+1+1; Spindle lock function; 2 Pcs 2.0Ah battery pack(JDLBP520) | Unit: SET | Catalog Qty/Carton: 6', -- Description
    4105.4, -- SellingPrice (estimated: cost + 30% markup)
    3158.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCK20273', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless 2 pcs combo kit', -- Name
    N'ÿﬁ„ ﬂÊ„»Ê »—«‘·Ì” ‘‰ÌÊ— »—«‘·Ì” 13 „„ 20 ›Ê·  œﬁ«ﬁ 62 ‰ÌÊ ‰ + ’«—ÊŒ »—«‘·Ì” 4.5 »Ê’… 20 ›Ê·  + 2 »ÿ«—Ì… 4 √„»Ì— + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Voltage:20V; With 1pcs Compact Brushless Cordless Impact Drill(JDCDP6281); With 1pcs Cordless angle grinder(JDLAP5322); Impact Drill: Brushless motor, No-load speed 0-500/0-2000rpm, Max impact rate 30000bpm, Max torque 62Nm, Metal chuck, Chuck capacity 13mm, Torque settings 22+1+1 | Unit: SET | Catalog Qty/Carton: 4', -- Description
    5525.0, -- SellingPrice (estimated: cost + 30% markup)
    4250.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLAP5421', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless angle grinder', -- Name
    N'’«—ÊŒ »—«‘·Ì” 4.5 »Ê’… 20 ›Ê·  ”—⁄«  1 »ÿ«—ÌÂ 4 «„»Ì— + ‘«Õ‰', -- NameAr
    N'Brushless motor; Voltage:20V; Max input power:850W; No-load speed:3000/6000/9000rpm; Disc diameter:115mm; Spindle thread:M14; Includes 1 Pcs 4.0Ah battery pack(JDLBP540), 1 Pcs charger(JDFCP518); Charge volts:220-240V~50/60Hz; Packed by color box | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    2502.5, -- SellingPrice (estimated: cost + 30% markup)
    1925.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLAP5322', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless angle grinder', -- Name
    N'’«—ÊŒ »—«‘·Ì” 4.5 »Ê’… 20 ›Ê·  ”—⁄«  2 »ÿ«—ÌÂ 4 «„»Ì— + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; Max input power:950W; No-load speed:3000/6000/9000rpm; Disc diameter:115mm; Spindle thread:M14; Includes 2 Pcs 4.0Ah battery pack(JDLBP540), 1 Pcs charger(JDFCP518); Packed by carrying case | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    4063.8, -- SellingPrice (estimated: cost + 30% markup)
    3126.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLAPB522', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless angle grinder', -- Name
    N'’«—ÊŒ »—«‘·Ì” 4.5 »Ê’… 20 ›Ê·  ”—⁄«  2 »ÿ«—ÌÂ 4 «„»Ì— + ‘«Õ‰ + 5 ÕÃ— ﬁÿ⁄Ì… 5 »Ê’… ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless Motor; Voltage:20V; Max input power:1150W; No-load speed:3000/6000/9000rpm; Disc diameter:115mm; Spindle thread:M14; Includes 1 Set abrasive metal cutting disc(5 Pcs), 2 Pcs 4.0Ah battery pack(JDLBP540), 1 Pcs charger(JDFCP518) | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    4570.8, -- SellingPrice (estimated: cost + 30% markup)
    3516.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLM1516', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless rotary hammer', -- Name
    N'‘«ﬂÊ‘ 20 ›Ê·  16 „„ »œÊ‰ «·»ÿ«—Ì… Ê «·‘«Õ‰', -- NameAr
    N'Voltage:20V; No-load speed:0-850rpm; Impact rate:0-5100bpm; Impact energy:1.5J; Max drilling capacity:16mm in concrete; SDS plus chuck system; Integrated LED work light; Includes 3 Pcs drills; Charger sold separately | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    2016.3, -- SellingPrice (estimated: cost + 30% markup)
    1551.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLM15225', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless rotary hammer', -- Name
    N'‘«ﬂÊ‘ »ÿ«—Ì… »—«‘·Ì” 20 ›Ê·  22 „„ + ÿﬁ„ »‰ÿ + «Ã‰… + 2 »ÿ«—Ì… + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-1100rpm; Impact rate:0-5400bpm; Impact energy:2.0J; Max drilling capacity: Concrete 22mm, Steel 13mm, Wood 28mm; SDS Plus chuck system; Integrated LED work light | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    4716.4, -- SellingPrice (estimated: cost + 30% markup)
    3628.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLM1B283', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless rotary hammer', -- Name
    N'‘«ﬂÊ‘ »ÿ«—Ì… »—«‘·Ì” 20 ›Ê·  28 „„ + ÿﬁ„ »‰ÿ + «Ã‰… + 2 »ÿ«—Ì… + ‘«Õ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-930rpm; Impact rate:0-4400bpm; Impact energy:4.5J; Max drilling capacity: Concrete 28mm, Steel 13mm, Wood 40mm; SDS Plus chuck system | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    6630.0, -- SellingPrice (estimated: cost + 30% markup)
    5100.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLM1528', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless rotary hammer', -- Name
    N'‘«ﬂÊ‘ »—«‘·Ì” 20 ›Ê·  28 „„ »‰ÿ ÂÌ· Ì + 2 √Ã‰… »œÊ‰ »ÿ«—Ì… Ê ‘«Õ‰', -- NameAr
    N'Brushless motor; Voltage:20V; No-load speed:0-930rpm; Impact rate:0-4400bpm; Impact energy:4.5J; Max drilling capacity: Concrete 28mm, Steel 13mm, Wood 40mm; SDS Plus chuck system | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    4452.5, -- SellingPrice (estimated: cost + 30% markup)
    3425.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCD3B21', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless impact driver&wrench', -- Name
    N'œ—Ì· 1/2 »Ê’… Ê „›ﬂ »ÿ«—Ì… 1/4 »Ê’… 2*1 »—«‘·Ì” 20 ›Ê·  3 ”—⁄«  ⁄“„ 210 ‰ÌÊ ‰ 2 »ÿ«—ÌÂ 2 «„»Ì— + ‘«Õ‰ + ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; Square drive:1/2 inch; Hex shank:6.35mm; No-load speed:0-2000/0-2400/0-2600rpm; Impact rate:0-2100/0-2500/0-2900bpm; Max torque:210Nm; Integrated LED work light; Includes 1 Pcs screwdriver bit, 1 Pcs 19mm socket | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    3428.1, -- SellingPrice (estimated: cost + 30% markup)
    2637.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCD1B48', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless impact wrench', -- Name
    N'œ—Ì· 1/2 »Ê’… »—«‘·Ì” 20 ›Ê·  3 ”—⁄«  (⁄“„ 480:600 ‰ÌÊ ‰) 2 »ÿ«—ÌÂ 3 «„»Ì— + ‘«Õ‰ 3 ·ﬁ„ —»«ÿ + ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; Square drive:1/2 inch; No-load speed:0-1200/0-1800/0-2200rpm; Impact rate:0-2400/0-2800/0-3300bpm; Fastening torque:480Nm; Nut-Busting torque:600Nm; Integrated work light; Includes 3 Pcs sockets(21,22,24mm), 2 Pcs 3.0Ah battery pack(JDLBP530) | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    5075.2, -- SellingPrice (estimated: cost + 30% markup)
    3904.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCD1B128', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless impact wrench', -- Name
    N'œ—Ì· 3/4 »Ê’… »—«‘·Ì” 20 ›Ê·  3 ”—⁄«  (⁄“„ 1280:1800 ‰ÌÊ ‰) 2 »ÿ«—ÌÂ 5 «„»Ì— + ‘«Õ‰ + 2 ·ﬁ„ —»«ÿ + ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; Square drive:3/4 inch; No-load speed:0-900/0-1200/0-1800rpm; Impact rate:0-1800/0-2000/0-2200bpm; Fastening torque:1280Nm; Nut-Busting torque:1800Nm; Includes 2 Pcs 5.0Ah battery pack(JDLBP550), 1 Pcs charger(JDFCP518) | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    10511.8, -- SellingPrice (estimated: cost + 30% markup)
    8086.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCV4401', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-Ion cordless screwdriver', -- Name
    N'„›ﬂ “«ÊÌ… »ÿ«—Ì… 4 ›Ê·  + 2 ”‰ „›ﬂ', -- NameAr
    N'Voltage:4V; Hex Shank:1/4 inch; No-load speed:240rpm; Max torque:4Nm; Charging via USB type-C cable(cable sold separately); Integrated LED work light; Includes 2 Pcs 50mm Cr-V bits | Unit: PCS | Catalog Qty/Carton: 20', -- Description
    451.1, -- SellingPrice (estimated: cost + 30% markup)
    347.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCD2B21', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless impact driver', -- Name
    N'„›ﬂ »ÿ«—Ì… 1/4 »Ê’… »—«‘·Ì” 20 ›Ê·  3 ”—⁄«  ⁄“„ 210 ‰ÌÊ ‰ 2 »ÿ«—ÌÂ 2 «„»Ì— + ‘«Õ‰ + ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Brushless motor; Voltage:20V; Hex shank:6.35mm; No-load speed:0-2000/0-2400/0-2600rpm; Impact rate:0-2100/0-2500/0-2900bpm; Max torque:210Nm; Includes 1 Pcs screwdriver bit, 3 Pcs nut setters, 2 Pcs 2.0Ah battery pack(JDLBP520) | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    2809.3, -- SellingPrice (estimated: cost + 30% markup)
    2161.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDQX0120', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless pressure washer', -- Name
    N'„ﬂ‰… €”Ì· »—«‘·Ì” 20 ›Ê·  25 »«— + „‘ „·«  »œÊ‰ »ÿ«—Ì… Ê ‘«Õ‰', -- NameAr
    N'Voltage:20V; Max pressure:24.5Bar; Flow rate:2.5L/min; Auto stop system; Includes 1 Set 2-pattern spray gun, 1 Pcs 3m water inlet hose with quick connector, 1 Pcs 300ml foam producer; Battery and charger sold separately | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    1368.9, -- SellingPrice (estimated: cost + 30% markup)
    1053.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDQX01203', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless pressure washer', -- Name
    N'„ﬂ‰… €”Ì· »—«‘·Ì” 20 ›Ê·  25 »«— + „‘ „·«  + »ÿ«—Ì… 2 √„»Ì— + ‘«Õ‰', -- NameAr
    N'Voltage:20V; Max pressure:24.5Bar; Flow rate:2.5L/min; Auto stop system; Includes spray gun set, water inlet hose, foam producer, 1 Pcs 2Ah 20V battery pack(JDLBP520), 1 Pcs charger(JDFCP518) | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    2260.7, -- SellingPrice (estimated: cost + 30% markup)
    1739.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLV0801', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless vacuum cleaner', -- Name
    N'„ﬂ‰”… »ÿ«—Ì… ‘Õ‰ 8 ›Ê·  Type C »ÿ«—Ì… 2000 «„»Ì—', -- NameAr
    N'Voltage:8V; Dust capacity:0.45L; Vacuum pressure:>=6kPa; Battery type:2000mAh; USB type-C charger system; Includes 1 Pcs charger cable, 1 Pcs crevice nozzle | Unit: PCS | Catalog Qty/Carton: 8', -- Description
    998.4, -- SellingPrice (estimated: cost + 30% markup)
    768.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLV20201', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless vacuum cleaner', -- Name
    N'„ﬂ‰”Â »ÿ«—ÌÂ 20 ›Ê·  „⁄ »ÿ«—ÌÂ Ê ‘«Õ‰', -- NameAr
    N'Voltage:20V; Dust capacity:0.45L; Vacuum pressure:>=8kpa; Includes 1 Pcs crevice nozzle, 1 Pcs 2Ah battery pack(JDLBP520), 1 Pcs charger(JDFCP518) | Unit: PCS | Catalog Qty/Carton: 8', -- Description
    1680.9, -- SellingPrice (estimated: cost + 30% markup)
    1293.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLV2020', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless vacuum cleaner', -- Name
    N'„ﬂ‰”… »ÿ«—Ì… 20 ›Ê·  »œÊ‰ »ÿ«—Ì… Ê ‘«Õ‰', -- NameAr
    N'Voltage:20V; Dust Capacity:0.45L; Vacuum Pressure:>=8kpa; Battery and charger sold separately; Includes 1 pcs crevice nozzle | Unit: PCS | Catalog Qty/Carton: 8', -- Description
    808.6, -- SellingPrice (estimated: cost + 30% markup)
    622.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLY1508', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-ion jump starter', -- Name
    N'»«Ê— »‰ﬂ ·»ÿ«—Ì… «·”Ì«—… Jump Starter 8000MAH »œ«Ì…  Ì«— «·ÿ«ﬁ… 200 √„»Ì— √ﬁ’Ï ﬁœ—… ·· Ì«— 400 √„»Ì—', -- NameAr
    N'Voltage:12V; Battery Capacity:8000mAh; Start Amp:200A; Peak Amp:400A; Suitable for upto 3L gasoline engines; Built-in LED flashlight with strobe and SOS; Accessories: Micro USB, Smart clamps, travel pouch | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    3086.2, -- SellingPrice (estimated: cost + 30% markup)
    2374.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLY1512', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-ion jump starter', -- Name
    N'»«Ê— »‰ﬂ ·»ÿ«—Ì… «·”Ì«—… Jump Starter 12000MAH »œ«Ì…  Ì«— «·ÿ«ﬁ… 300 √„»Ì— √ﬁ’Ï ﬁœ—… ·· Ì«— 600 √„»Ì—', -- NameAr
    N'Voltage:12V; Battery Capacity:12000mAh; Start Amp:300A; Peak Amp:600A; Suitable for upto 4L gasoline engines; LCD display; Dual 2.4A USB ports; Built-in LED flashlight with strobe and SOS | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    3666.0, -- SellingPrice (estimated: cost + 30% markup)
    2820.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLN2520', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless auto air compressor', -- Name
    N'ﬂ„»ÌÊ”Ì— ”Ì«—Â 20 ›Ê·  10.5 »«— »œÊ‰ »ÿ«—Ì… Ê »œÊ‰ ‘«Õ‰', -- NameAr
    N'Voltage:20V; Max pressure:150PSI/10.5Bar; With 3pcs adaptors; Battery and charger sold separately | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    1028.3, -- SellingPrice (estimated: cost + 30% markup)
    791.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAAC511', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Auto air compressor', -- Name
    N'ﬂ„»—Ê”Ì— ”Ì«—Â œÌÃÌ «· 12 ›Ê·  11 »«— Ì⁄„· ⁄·Ì «·Ê·«⁄Â', -- NameAr
    N'Voltage:12V; Max pressure:160PSI/11Bar; Max air flow:35L/min; Integrated work light; With 1pcs 3m cord with cigarette lighter; With 3pcs adaptors | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    1560.0, -- SellingPrice (estimated: cost + 30% markup)
    1200.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDBLP521', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless blower', -- Name
    N'»·«Ê— Õœ«∆ﬁ »ÿ«—Ì… 20 ›Ê·  + »ÿ«—Ì… 2 √„»Ì— + ‘«Õ‰', -- NameAr
    N'Voltage:20V; No-load speed:15000rpm; Average air volume:8.5m3/min; Max air speed:115km/h; Includes 1 Pcs 2.0Ah 20V battery pack(JDLBP520), 1 Pcs charger(JDFCP518) | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    2228.2, -- SellingPrice (estimated: cost + 30% markup)
    1714.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDJFP525', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless gauge straight shear', -- Name
    N'„ﬁ’ Õœ«∆ﬁ »ÿ«—Ì… 20 ›Ê·  »œÊ‰ »ÿ«—Ì… Ê‘«Õ‰', -- NameAr
    N'Brushless motor; Voltage:20V; Cutting diameter:25mm; Includes 1 Pcs oil bottle(empty), 1 Pcs sharpening stone; Battery and charger sold separately | Unit: PCS | Catalog Qty/Carton: 12', -- Description
    3010.8, -- SellingPrice (estimated: cost + 30% markup)
    2316.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSU3066', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless spray gun', -- Name
    N'„”œ” œÊﬂÊ »ÿ«—Ì… 20 ›Ê·  »‰ … »·«” Ìﬂ ⁄œ· ”⁄… 800 „· »œÊ‰ »ÿ«—Ì… Ê ‘«Õ‰', -- NameAr
    N'Voltage:20V; Spraying pressure:0.1-0.2Bar; Max flow:650mL/min; Max viscosity:100DIN-s; Container capacity:800ml; Includes viscosity measuring cup, nozzle cleaning needle | Unit: PCS | Catalog Qty/Carton: 8', -- Description
    1198.6, -- SellingPrice (estimated: cost + 30% markup)
    922.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDUB1501', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'USB type-A to type-C cable', -- Name
    N'Ê’·Â  «Ì» ”Ì', -- NameAr
    N'Cable length:1m; Max charge current:3A; Packed by plastic bag | Unit: PCS | Catalog Qty/Carton: 120', -- Description
    33.8, -- SellingPrice (estimated: cost + 30% markup)
    26.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLBP520', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-Ion battery pack', -- Name
    N'»ÿ«—ÌÂ 20 ›Ê·  2 √„»Ì—', -- NameAr
    N'Voltage:20V; Lithium-Ion 2.0Ah battery; LED battery power indicator; One battery fits all P20S multiple tools | Unit: PCS | Catalog Qty/Carton: 20', -- Description
    777.4, -- SellingPrice (estimated: cost + 30% markup)
    598.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLBP540', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Lithium-Ion battery pack', -- Name
    N'»ÿ«—ÌÂ 20 ›Ê·  4 √„»Ì—', -- NameAr
    N'Voltage:20V; Lithium-Ion 4.0Ah battery; LED battery power indicator; One battery fits all P20S multiple tools | Unit: PCS | Catalog Qty/Carton: 12', -- Description
    1366.3, -- SellingPrice (estimated: cost + 30% markup)
    1051.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDMD15651', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Impact drill', -- Name
    N'‘‰ÌÊ— 13 „„ œﬁ«ﬁ ≈·ﬂ —Ê‰Ì 650 Ê«ÿ Ì„Ì‰/‘„«·', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:650W; No-load speed:0-3000rpm; Max drilling capacity:13mm; Variable speed control; Forward/Reverse switch; Hammer function | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    1058.2, -- SellingPrice (estimated: cost + 30% markup)
    814.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRH1D26', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Rotary hammer', -- Name
    N'‘«ﬂÊ‘ ⁄œ· 26 „„ 800 Ê«  ‰Œ—Ì„  ﬂ”Ì—', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:800W; No-load speed:0-1100rpm; Impact rate:0-4000bpm; Impact energy:2.5J; Max drilling capacity: Concrete 26mm, Steel 13mm, Wood 30mm; SDS plus chuck system; With 3 drills and 2 chisels | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    2429.7, -- SellingPrice (estimated: cost + 30% markup)
    1869.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRH1D26-2', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Rotary hammer', -- Name
    N'‘«ﬂÊ‘ ⁄œ·  Œ—Ì„  ﬂ”Ì— 2 Ÿ—› 26 „„ 800 Ê«ÿ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:800W; No-load speed:0-1100rpm; Impact rate:0-4000bpm; Impact energy:2.5J; Max drilling capacity: Concrete 26mm, Steel 13mm, Wood 30mm; SDS plus chuck system; 1 Pcs keyless quick-change chuck | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    2847.0, -- SellingPrice (estimated: cost + 30% markup)
    2190.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRH3D38', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Rotary hammer', -- Name
    N'‘«ﬂÊ‘ 38 „„ „«ﬂ” 1600 Ê«  ‰Œ—Ì„ Ê ﬂ”Ì—', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:1600W; No-load speed:630rpm; Impact rate:3850bpm; Impact energy:10J; Max drilling capacity: Concrete 40mm, Core bit 100mm; SDS MAX chuck system; Chisel-locking system; Anti-vibration system | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    5478.2, -- SellingPrice (estimated: cost + 30% markup)
    4214.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAG851801', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Angle grinder', -- Name
    N'’«—ÊŒ 7 »Ê’… 1800 Ê«ÿ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:1800W; No-load speed:8480rpm; Disc diameter:180mm; Spindle thread:M14; With 1pcs auxiliary handle; Disc not included | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    3118.7, -- SellingPrice (estimated: cost + 30% markup)
    2399.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAG852001', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Angle grinder', -- Name
    N'’«—ÊŒ 9 »Ê’… 2000 Ê«ÿ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:2000W; No-load speed:6500rpm; Disc diameter:230mm; Spindle thread:M14; With 1pcs auxiliary handle; Disc not included | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    3152.5, -- SellingPrice (estimated: cost + 30% markup)
    2425.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRY1D131', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Mini grinder', -- Name
    N'„Ì‰Ì ﬂ—«›  130 Ê«  ”—⁄«  35000 ·›Â/œ + ›·Ìﬂ”«»· + 52 ﬁÿ⁄… «ﬂ””Ê«—', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:130W; No-load speed:8000-35000rpm; Collet size:3.2mm; Variable speed control; Includes 1 Pcs flexible shaft, 52 Pcs accessories | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    1080.3, -- SellingPrice (estimated: cost + 30% markup)
    831.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRR5032', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Glass and stone working set for mini drill', -- Name
    N'ÿﬁ„ «ﬂ””Ê«— „Ì‰Ì ﬂ—«›  32 ﬁÿ⁄…', -- NameAr
    N'32Pcs set for grinding and sanding; Cut-Off wheel, abrasive buff, grinding stones, sanding bands, sanding discs, mandrels, sanding drum | Unit: SET | Catalog Qty/Carton: 40', -- Description
    140.4, -- SellingPrice (estimated: cost + 30% markup)
    108.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRR7080', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Accessories of mini drill', -- Name
    N'ÿﬁ„ «ﬂ””Ê«— „Ì‰Ì ﬂ—«›  80 ﬁÿ⁄…', -- NameAr
    N'80Pcs general purpose set; sanding bands, grinding stones, nylon brush, mandrels, high speed cutter, polishing wheels, reinforced cut-off wheels, polishing compound, cut-off wheels, sanding discs | Unit: SET | Catalog Qty/Carton: 40', -- Description
    201.5, -- SellingPrice (estimated: cost + 30% markup)
    155.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLT155001', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Laminate trimmer', -- Name
    N'—«Ê — 500 Ê«ÿ 6 „„', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:500W; No-load speed:28000rpm; Collet size:6mm and 1/4 inch; Includes wrenches, template guide, horizontal guide rail, linear guide, roller components | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    1609.4, -- SellingPrice (estimated: cost + 30% markup)
    1238.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDER1516001', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Electric router', -- Name
    N'—«Ê — 1600 Ê«ÿ 6-8-12 „„', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:1600W; No-load speed:22000rpm; Collet size:6mm,8mm,12mm,1/4 inch,1/2 inch; Plunge capacity:0-60mm; Includes guide holder, template guide, trimmer guide, straight guide | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    3472.3, -- SellingPrice (estimated: cost + 30% markup)
    2671.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAB15401', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Aspirator blower', -- Name
    N'»·«Ê— 400 Ê«ÿ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:400W; No-load speed:14000rpm; Max blowing rate:3.0m3/min; With 1pcs blowing pipe | Unit: PCS | Catalog Qty/Carton: 8', -- Description
    824.2, -- SellingPrice (estimated: cost + 30% markup)
    634.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHG1516', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Heat gun', -- Name
    N'„”œ”  ”ŒÌ‰ 2 œ—Ã… 1600 Ê«ÿ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:1600W; Temperature:400C/500C; Airflow:250/480 L/min; Includes 1 Pcs reduction nozzle | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    617.5, -- SellingPrice (estimated: cost + 30% markup)
    475.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDEG2A50', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Spray gun', -- Name
    N'„”œ” œÊﬂÊ 550 Ê«ÿ 0.2 »«— 800 „·Ì + „‘ „·« ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:550W; Spraying pressure:0.1-0.2Bar; Max flow:700ml/min; Max viscosity:120DIN-s; Container capacity:800ml; Power cord length:2.0m; Accessories: viscosity measuring cup, nozzle cleaning needle, shoulder strap | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    1393.6, -- SellingPrice (estimated: cost + 30% markup)
    1072.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDM6501', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Digital AC clamp meter', -- Name
    N'»‰”… √„»Ì— 200 A', -- NameAr
    N'Display:True RMS 2000counts; AC Current:2A/20A/200A; AC Voltage:2V/20V/200V/600V; DC Voltage:200mV/2V/20V/200V/600V; Resistance range up to 20MOhm; Data Hold; Diode test; Low battery indication; Auto power off; With 2pcs R03 AAA batteries | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    661.7, -- SellingPrice (estimated: cost + 30% markup)
    509.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTP3501', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'AC voltage detector', -- Name
    N'ÃÂ«“ ﬂ‘› «· Ì«— «·ﬂÂ—»«∆Ì 12 ›Ê·  1000 ›Ê· / Ì«— „ —œœ', -- NameAr
    N'AC Voltage:12V~1000V(High sensitivity), 48V~1000V(Low sensitivity); Frequency:50/60Hz; Alarm mode: Sound and light alarm; Flash light: white LED; Auto power off; With 2pcs R03 AAA batteries | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    175.5, -- SellingPrice (estimated: cost + 30% markup)
    135.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDM2501', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Digital multimeter', -- Name
    N'„«· Ì„Ì — œÌÃÌ «· 600 ›Ê· ', -- NameAr
    N'Display:True RMS 4000counts; Temperature measurement; DC/AC Voltage up to 600V; DC/AC Current up to 10A; Resistance up to 40MOhm; Capacitance up to 4mF; Frequency up to 4mHz; Non-contact voltage detection; Diode test | Unit: PCS | Catalog Qty/Carton: 30', -- Description
    763.1, -- SellingPrice (estimated: cost + 30% markup)
    587.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDDM94011', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Electrical test kit', -- Name
    N'ÿﬁ„ „⁄œ«  ﬁÌ«” ﬂÂ—»«¡ 3 ﬁÿ⁄ »‰”… ﬂ·«„» √„»Ì— 200 √„»Ì— + ÃÂ«“ „«· Ì„Ì — œÌÃÌ «· 600 ›Ê·  + ÃÂ«“ ﬂ‘› «· Ì«— «·ﬂÂ—»«∆Ì 12 ›Ê·  1000 ›Ê· / Ì«— „ —œœ', -- NameAr
    N'3pcs/set includes: digital AC clamp meter(JDDM6501), digital multimeter(JDDM1501), AC voltage detector; Display True RMS 2000counts; Data Hold; Diode test | Unit: SET | Catalog Qty/Carton: 20', -- Description
    958.1, -- SellingPrice (estimated: cost + 30% markup)
    737.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLE1M03', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Self-leveling line laser', -- Name
    N'„Ì“«‰ ·Ì“— 15 „ — 2 Œÿ √Õ„—', -- NameAr
    N'Red laser level; Working Range:0~15m; Levelling accuracy:+-2mm@5m; Line accuracy:+-2mm@5m; Levelling time:<=3s; Self-levelling angle:<=4deg; Laser type:635+-5nm; Laser Class II <1mW; With laser cross lock function; With horizontal and vertical line function | Unit: PCS | Catalog Qty/Carton: 30', -- Description
    1463.8, -- SellingPrice (estimated: cost + 30% markup)
    1126.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLE8M12', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Cordless 3D laser level', -- Name
    N'„Ì“«‰ ·Ì“— 30 „ — 12 Œÿ «Œ÷— 360 œ—Ã… À·«ÀÏ «·«»⁄«œ »ÿ«—Ì…', -- NameAr
    N'Green laser level; Voltage:12V; Working Range:0~30m; Self-levelling angle<=4deg; Laser class II <1mW; With 1*360deg horizontal and 2*360deg vertical laser line; 1 Pcs 1.5Ah battery pack; Battery charging port: USB type-C; Charger sold separately | Unit: PCS | Catalog Qty/Carton: 8', -- Description
    6163.3, -- SellingPrice (estimated: cost + 30% markup)
    4741.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAY1A10', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Airless paint sprayer', -- Name
    N'„ﬂ‰… ≈Ì—·Ì” 1200 Ê«ÿ  ’—Ì› 1.6 · —/œ ﬁÊ… 315 »«—', -- NameAr
    N'Voltage:220-240V~50/60Hz; Motor rating power:1200W; Max pressure:20.7MPa; Max Fluid delivery:1600ml/min; 7.5m black pipe; Weight:11kg; Automatic oiling system; With 517 nozzle spray gun | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    10656.1, -- SellingPrice (estimated: cost + 30% markup)
    8197.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDVR6520', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Vacuum cleaner', -- Name
    N'„ﬂ‰”… ÂÊ›— Œœ„… ‘«ﬁ… 1600 Ê«ÿ', -- NameAr
    N'Voltage:220V-240V~50/60Hz; Power:1600W; Pure copper wire motor; Suction power:260~330W; Capacity:1.8L; With speed control function; With automatic cable rewinder; HEPA filter; Air flow:2.0~2.3 m3/min; Vacuum pressure(kPa):>=20 kPa; With soft start | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    3468.4, -- SellingPrice (estimated: cost + 30% markup)
    2668.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHP1A12', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High pressure washer', -- Name
    N'„ﬂ‰… €”Ì· ÷€ÿ ⁄«·Ì 90 »«— „Ê Ê— 1200 Ê«ÿ Œ—ÿÊ„ 8 „ — + „‘ „·«  ( Õ÷Ì— –« Ì)', -- NameAr
    N'Voltage:220-240V~50Hz; Aluminum wire induction motor; Input power:1200W; Max pressure:90Bar(1305PSI); Flow rate:5.0L/min; Auto stop system; Includes water spray gun, 8m high pressure metal hose, 2m inlet hose, quick connecter | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    3959.8, -- SellingPrice (estimated: cost + 30% markup)
    3046.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHP3A14', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High pressure washer', -- Name
    N'„ﬂ‰… €”Ì· ÷€ÿ ⁄«·Ì 110 »«— „Ê Ê— 1400 Ê«ÿ Œ—ÿÊ„ 5 „ — + „‘ „·« ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Carbon brush motor; Aluminum wire; Input power:1400W; Max pressure:110Bar(1595PSI); Flow rate:5.6L/min; Auto stop system; Includes water spray gun, 5m high pressure hose, water inlet screw | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    2926.3, -- SellingPrice (estimated: cost + 30% markup)
    2251.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHP3A18', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High pressure washer', -- Name
    N'„ﬂ‰… €”Ì· ÷€ÿ ⁄«·Ì 130 »«— „Ê Ê— 1800 Ê«ÿ Œ—ÿÊ„ 5 „ — + „‘ „·« ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Carbon brush motor; Aluminum wire; Input power:1800W; Max pressure:130Bar(1885PSI); Flow rate:5.8L/min; Auto stop system; Includes soap bottle, water spray gun, 5m high pressure hose | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    3772.6, -- SellingPrice (estimated: cost + 30% markup)
    2902.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDXN1536', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Integrated rotary nozzle', -- Name
    N'›ÊÂ… œÊ«—… 360 œ—Ã… ·„«ﬂÌ‰… «·€”Ì·', -- NameAr
    N'360deg nozzle; Integrated rotary nozzle; Suitable for JDHP1A11/JDHP3A12/JDHP3A14/JDHP3A18/JDHP1A12 high pressure washer | Unit: PCS | Catalog Qty/Carton: 50', -- Description
    197.6, -- SellingPrice (estimated: cost + 30% markup)
    152.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWDH1301', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Inverter MMA welding machine', -- Name
    N'„«ﬂÌ‰Â ·Õ«„ ÃœÌœ… «‰›— — 130 «„»Ì—', -- NameAr
    N'IGBT inverter technology; Input voltage:1~220-240V; Frequency:50/60Hz; Output current:15-130A; Duty cycle:130A@20%; No-load voltage:62V; Max output current:130A; Diameter of electrode:1.6-3.2mm | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    3439.8, -- SellingPrice (estimated: cost + 30% markup)
    2646.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWD11301', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Inverter MMA welding machine', -- Name
    N'„«ﬂÌ‰… ·Õ«„ ≈‰›Ì— — 130 √„»Ì— + ÿﬁ„ ≈ﬂ””Ê«—« ', -- NameAr
    N'IGBT inverter technology; Input voltage:1~220-240V; Frequency:50/60Hz; Output current:10-130A; Duty cycle:130A@30%; LED display; No-load voltage:65V; Max output current:130A; Diameter of electrode:1.6-3.2mm; Anti-stick/Hot start/Arc-force; With electrode holder with cable | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    3113.5, -- SellingPrice (estimated: cost + 30% markup)
    2395.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWD31601', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Inverter MMA welding machine', -- Name
    N'„«ﬂÌ‰… ·Õ«„ ≈‰›Ì— — 160 √„»Ì— + ÿﬁ„ ≈ﬂ””Ê«—«  MINI', -- NameAr
    N'IGBT inverter technology; Input voltage:1~220-240V; Frequency:50/60Hz; Output current:20-160A; Duty cycle:160A@60%; LED display; No-load voltage:65V; Max output current:160A; Diameter of electrode:1.6-4.0mm; Anti-stick/Hot start/Arc-force | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    4462.9, -- SellingPrice (estimated: cost + 30% markup)
    3433.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDEL5606', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Soldering gun with solder feeder', -- Name
    N'„”œ” ·Õ«„ ﬁ’œÌ— 60 Ê«ÿ', -- NameAr
    N'Voltage:220-240V~50/60Hz; Input power:60W; Preheat time:1~2minutes; Straight, tip head; With Solder Wire Feeder; Built-in ceramic heating element; Long life replaceable tip | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    338.0, -- SellingPrice (estimated: cost + 30% markup)
    260.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDEH1A03', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Electrode holder', -- Name
    N'»‰”… ·Õ«„ 300 √„»Ì—', -- NameAr
    N'Rated current:300A; Length:230mm; New design; Suitable for MMA welding machine | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    176.8, -- SellingPrice (estimated: cost + 30% markup)
    136.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPG1801', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'PU coated gloves', -- Name
    N'ÃÊ«‰ Ì Õ„«ÌÂ „ﬁ«” XL', -- NameAr
    N'Liner Material:Polyester; Coating material/Finish:PU; Gauge:13; Protection level:3131X; 12pairs/paper; seamless knit glove with polyurethane coated smooth grip, ideal for general duty work | Unit: PAIR | Catalog Qty/Carton: 300', -- Description
    24.7, -- SellingPrice (estimated: cost + 30% markup)
    19.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDGV2801', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Nitrile glove', -- Name
    N'ÃÊ«‰ Ï ·«⁄„«· «·‘ÕÊ„«  Ê«·“Ì  „ﬁ«” XL', -- NameAr
    N'For oil environment worker etc.; Nitrile coated palm, smooth and rough-texture palm finish | Unit: PAIR | Catalog Qty/Carton: 240', -- Description
    26.0, -- SellingPrice (estimated: cost + 30% markup)
    20.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDXG3801', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Latex gloves', -- Name
    N'ÃÊ«‰ Ï ·« Ìﬂ” „ﬁ«” XL', -- NameAr
    N'Liner Material:Polyester; Coating material/Finish:Latex; Gauge:10; 12pairs/paper; Anti-slip and anti-tear latex material, good grip and anti-slip effect, strong tear resistance | Unit: PAIR | Catalog Qty/Carton: 240', -- Description
    37.7, -- SellingPrice (estimated: cost + 30% markup)
    29.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLG2114', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Welding leather gloves', -- Name
    N'ÃÊ«‰ Ï Ã·œ ·√⁄„«· «··Õ«„ Ê «·Õœ«œ… ÿÊ· 14 »Ê’…', -- NameAr
    N'Length:14 inch; Material:cowhide; Gauntlet cuff for better protection; Reinforced at stress points with strong stitches; Ideal for gardening, cycling, welding, woodworking, cutting, construction | Unit: PAIR | Catalog Qty/Carton: 48', -- Description
    258.7, -- SellingPrice (estimated: cost + 30% markup)
    199.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTR8510', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Tile cutter', -- Name
    N'„«ﬂÌ‰…  ﬁÿÌ⁄ ”Ì—«„Ìﬂ 100 ”„', -- NameAr
    N'Max cutting length:1000mm; Max cutting thickness:14mm; Steel base size:1200X200mm; Product weight:13.7kg; Blade size:22X6X2mm | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    3251.3, -- SellingPrice (estimated: cost + 30% markup)
    2501.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDGEAA01', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Gasoline generator', -- Name
    N'„Ê·œ ﬂÂ—»«¡ »‰“Ì‰ 0.650-0.850 ﬂÌ·Ê', -- NameAr
    N'Rated voltage:220-240V; Rated frequency:50Hz; Max output:0.8kW; Rated output:0.65kW; Rated speed:3000rpm; Engine:2 stroke; Displacement:63cc; Cooling system:air-cooled; Starting system:recoil; Fuel tank:4.0L | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    5850.0, -- SellingPrice (estimated: cost + 30% markup)
    4500.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDGEAA056D', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Gasoline generator', -- Name
    N'„Ê·œ ﬂÂ—»«¡ »‰“Ì‰ / „«—‘ 2.5-2.8 ﬂÌ·Ê', -- NameAr
    N'Rated voltage(V):220-240; Rated frequency(Hz):50; Max output(kW):2.8; Rated output(kW):2.5; Rated speed(rpm):3000; Engine:4 stroke,OHV; Displacement(mL):212; Cooling system:Air-cooled; Ignition system:T.C.I; Starting system:recoil+electric | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    14275.3, -- SellingPrice (estimated: cost + 30% markup)
    10981.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDGEAA096', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Gasoline generator', -- Name
    N'„Ê·œ ﬂÂ—»«¡ 7 ﬂÌ·Ê ’«›Ï 6 ﬂÌ·Ê „«—‘', -- NameAr
    N'Rated voltage:220-240V; Rated frequency:50Hz; Max output:7.0kW; Rated output:6.0kW; Rated speed:3000rpm; Engine:4 stroke, OHV; Displacement:420cc; Cooling system:air-cooled; Ignition system:T.C.I; Starting system:recoil+electric; Copper coil alternator | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    30199.0, -- SellingPrice (estimated: cost + 30% markup)
    23230.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDGEAA11', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Gasoline generator', -- Name
    N'„Ê·œ ﬂÂ—»«¡ »‰“Ì‰ „“Êœ »“—  ‘€Ì· 8.5-9 ﬂÌ·Ê + ⁄Ã·', -- NameAr
    N'Rated voltage:220-240V; Rated frequency:50Hz; Max output:9.0kW; Rated output:8.5kW; Rated speed:3000rpm; Engine:4 stroke,OHV; Displacement:457cc; Cooling system:air-cooled; Ignition system:T.C.I; Starting system:one push start; Copper coil alternator | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    38932.4, -- SellingPrice (estimated: cost + 30% markup)
    29948.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPC1A01', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Automatic pump control', -- Name
    N'›·Ê„«ﬂ 1.5:10 »«— 10 «„»Ì—', -- NameAr
    N'Automatic pump control; Rated voltage:220-240V; Frequency:50/60Hz; Starting pressure:1.5bar; Max current:10A; Max pressure:10bar; Pipe diameter:1 inch x1 inch; Protection degree:IP65 | Unit: PCS | Catalog Qty/Carton: 12', -- Description
    923.0, -- SellingPrice (estimated: cost + 30% markup)
    710.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWPJA04', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Water pump', -- Name
    N'„Ê Ê— „Ì«… ﬁÊ… 1.5 Õ’«‰ ”«ﬁÌ… ” «‰·Ì” ” Ì·', -- NameAr
    N'Self-priming Jet Pump; Voltage:220-240V~50Hz; Rated power:1100W(1.5HP); Max head:55m; Max flow:63L/min; Max suction:9m; Pipe diameter:1 inch x1 inch; Aluminum wire motor; Stainless steel impeller | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    3300.3, -- SellingPrice (estimated: cost + 30% markup)
    2538.69, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAX1505', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'5 Pcs Air tools set', -- Name
    N'ÿﬁ„ „”œ”«  ÂÊ«¡ „”œ” œÊﬂÊ „ﬁ·Ê» 600 ”„° „”œ” €”Ì· Ã«“° Œ—ÿÊ„ ”Ê” … 5 „ —° „”œ” ÂÊ«° „”œ”  “ÊÌœ ﬂ«Ê ‘ »«·⁄œ«œ', -- NameAr
    N'Air tools 5pcs set; Recoil hose length 5m diameter 8mm; Air blow gun nozzle length 16mm; Air spray gun operating pressure 3-4bar, paint capacity 600cc, pattern width 180-250mm; Air washing gun | Unit: SET | Catalog Qty/Carton: 10', -- Description
    972.4, -- SellingPrice (estimated: cost + 30% markup)
    748.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDQG1910', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Air hose', -- Name
    N'Œ—ÿÊ„ ÂÊ«¡ 5*8 „„ 10 „ — ”Ê” Â', -- NameAr
    N'Material:PE; Length:10M; Only with threaded connectors; Inner diameter:5mm; External diameter:8mm | Unit: PCS | Catalog Qty/Carton: 25', -- Description
    141.7, -- SellingPrice (estimated: cost + 30% markup)
    109.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDYP1E20', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'1/2 inch PVC hose', -- Name
    N'Œ—ÿÊ„ 20 „ — 1/2 »Ê’…', -- NameAr
    N'20Mx1/2 inch; 5-ply construction; Thickness:1.7mm; Packed by paper hanger | Unit: PCS | Catalog Qty/Carton: 5', -- Description
    655.2, -- SellingPrice (estimated: cost + 30% markup)
    504.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDYP1E12', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'1/2 inch PVC hose', -- Name
    N'Œ—ÿÊ„ 50 „ — 1/2 »Ê’…', -- NameAr
    N'50Mx1/2 inch; 5-ply construction; Thickness:1.7mm; Packed by paper hanger | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    1241.5, -- SellingPrice (estimated: cost + 30% markup)
    955.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHJ1510', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hydraulic bottle jack', -- Name
    N'ﬂÊ—Ìﬂ »«ﬂ„ 10 ÿ‰', -- NameAr
    N'10Ton; Min height:190mm; Lifting height:110mm; Adjustable Height:60mm; Weight:5.35KG | Unit: PCS | Catalog Qty/Carton: 4', -- Description
    1086.8, -- SellingPrice (estimated: cost + 30% markup)
    836.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHJ1520', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hydraulic bottle jack', -- Name
    N'ﬂÊ—Ìﬂ »«ﬂ„ 20 ÿ‰', -- NameAr
    N'20Ton; Min height:220mm; Lifting height:140mm; Adjustable Height:60mm; Weight:8.28KG | Unit: PCS | Catalog Qty/Carton: 2', -- Description
    1666.6, -- SellingPrice (estimated: cost + 30% markup)
    1282.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHJ25021', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hydraulic floor jack', -- Name
    N'ﬂÊ—Ìﬂ  „”«Õ 2 ÿ‰ ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'2Ton; Min height:130mm; Max height:300mm; Travel:170mm; Net weight:6.8kg; Packed by BMC | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    1639.3, -- SellingPrice (estimated: cost + 30% markup)
    1261.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHJ25251', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hydraulic floor jack', -- Name
    N'ﬂÊ—Ìﬂ  „”«Õ 2.5 ÿ‰ 12.6 ﬂÃ„ Œœ„… ‘«ﬁ… ‘‰ÿ… »·«” Ìﬂ', -- NameAr
    N'2.5Ton; Min height:85mm; Max height:380mm; Travel:295mm; Weight:12.6KG | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    2715.7, -- SellingPrice (estimated: cost + 30% markup)
    2089.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHJ2503', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hydraulic garage jack', -- Name
    N'ﬂÊ—Ìﬂ  „”«Õ 3 ÿ‰ Ê“‰ 28.9 ﬂÃ„ Œœ„… ‘«ﬁ…', -- NameAr
    N'3Ton; Min height:130mm; Max height:465mm; Travel:335mm; Weight:28.9KG | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    5691.4, -- SellingPrice (estimated: cost + 30% markup)
    4378.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWB9A15', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Foldable platform hand truck', -- Name
    N'⁄—»… ÌœÊÌ… ﬁ«»·… ··ÿÌ   Õ„· Õ Ì 150 ﬂÃ„ «·ÕÃ„ «·„„ œ 950x420x710 „„ «·ÕÃ„ «·„ÿÊÌ 530x420x530 „„', -- NameAr
    N'Load capacity:150kg; Material:Aluminium+steel; Extended size:710x420x950mm; Folded size:530x420x230mm; Wheels:100mm(PP+TPR); Including elastic bungee cord | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    2285.4, -- SellingPrice (estimated: cost + 30% markup)
    1758.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDNH1R20', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hand pallet truck', -- Name
    N'⁄—»Ì… »«·Ì … 2 ÿ‰ - Œœ„… ‘«ﬁ… „ﬁ«” 1150*550 „„', -- NameAr
    N'Load capacity:2000kg; Fork size:550*1150mm; Min fork height:75mm; Max fork height:190mm; Distance between forks:250mm; Fork width:150mm; Polyurethane wheel | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    12025.0, -- SellingPrice (estimated: cost + 30% markup)
    9250.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPD5560L', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Long shackle iron padlock', -- Name
    N'ﬁ›· ÕœÌœ 63 „„ 400 Ã„ - ”·Ì‰œ— ‰Õ«” - Õ·ﬁ… ÿÊÌ·…', -- NameAr
    N'Long lock beam; Size:63mm; Weight:400g; Lock body material:iron; Brass lock cylinder; With 3 pcs iron keys | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    123.5, -- SellingPrice (estimated: cost + 30% markup)
    95.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPD5575L', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Long shackle iron padlock', -- Name
    N'ﬁ›· ÕœÌœ 75 „„ Ê“‰ 660 Ã„ ”·Ì‰œ— ‰Õ«” - Õ·ﬁ… ÿÊÌ·…', -- NameAr
    N'Long lock beam; Size:75mm; Weight:660g; Lock body material:iron; Brass lock cylinder; With 3 pcs iron keys | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    193.7, -- SellingPrice (estimated: cost + 30% markup)
    149.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPDD470', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Stainless steel disc padlock', -- Name
    N'ﬁ›· œ«∆—Ì 70 „„ ” «‰·” ” Ì· Ê“‰ 440 Ã„ ”·Ì‰œ— ‰Õ«”', -- NameAr
    N'Size:70mm; Weight:555g; Lock body material:stainless steel; Steel close shackle; Brass lock cylinder; Super polished surface; With 2 pcs iron keys | Unit: PCS | Catalog Qty/Carton: 72', -- Description
    211.9, -- SellingPrice (estimated: cost + 30% markup)
    163.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPD8460', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Anti-prying steel padlock', -- Name
    N'ﬁ›· 60 „„ ÕœÌœ - Ê“‰ 440 Ã„ ÷œ «·”—ﬁ…', -- NameAr
    N'Anti-prying; Size:60mm; Weight:440g; Lock body material:iron; Brass lock cylinder; Special lock cylinder design; With 4 pcs iron keys | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    235.3, -- SellingPrice (estimated: cost + 30% markup)
    181.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDEC1503', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Electronic scale', -- Name
    N'„Ì“«‰ ≈·ﬂ —Ê‰Ì œÌÃÌ «· Õ„Ê·… 30 ﬂÃ„', -- NameAr
    N'Charging voltage:220-240V 50/60Hz; Max weight:30kg; With 2g graduation; Display:LED screen; Table size:235*190mm; 10 Seconds enter into Energy-saving mode | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    1125.8, -- SellingPrice (estimated: cost + 30% markup)
    866.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDEC1510', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Electronic scale', -- Name
    N'„Ì“«‰ ﬂÂ—»«∆Ì œÌÃÌ «· Õ„Ê·… 100 ﬂÃ„', -- NameAr
    N'Charging voltage:220-240V 50/60Hz; Max weight:100kg; With 20g graduation; Display:LED screen; Table size:300*400mm | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    2009.8, -- SellingPrice (estimated: cost + 30% markup)
    1546.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAS4910', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Tin snip', -- Name
    N'„ﬁ’ ’«Ã „ÊœÌ· √„—ÌﬂÌ 10 »Ê’…', -- NameAr
    N'Size:10 inch/250mm; Max cutting thickness: Carbon steel <=0.8mm, Stainless steel <=0.6mm; Drop-forged steel; Double dipped grip handle | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    189.8, -- SellingPrice (estimated: cost + 30% markup)
    146.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHW7G12', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Pruning saw', -- Name
    N'„‰‘«— ‘—‘—… 12 »Ê’… Ìœ ﬂ«Ê ‘', -- NameAr
    N'Size:12 inch/300mm; Material:65Mn; 6TPI; Triple teeth(Precision ground teeth 3 edges); Fast cut | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    166.4, -- SellingPrice (estimated: cost + 30% markup)
    128.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWK2134', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Garden rake', -- Name
    N'‘Êﬂ… “—«⁄… Ìœ ﬂ«Ê ‘ 3 ”‰Ê‰ 10 »Ê’…', -- NameAr
    N'Length:240mm; Ancho:100mm; Blade with powder coating; PP handle | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    97.5, -- SellingPrice (estimated: cost + 30% markup)
    75.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPR2301', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Hedge shear', -- Name
    N'„ﬁ’ √”Ê«— Ìœ „⁄œ‰ „ﬁ«” 22 »Ê’…', -- NameAr
    N'Length:550mm/22 inch; Material:55# carbon steel; Straight blade for exact shaping and precise cutting results; Heat treatment blade | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    296.4, -- SellingPrice (estimated: cost + 30% markup)
    228.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDRS1820', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Pressure sprayer', -- Name
    N'»Œ«Œ… „Ì«Â ”⁄… 2 · —', -- NameAr
    N'2L; Metal pump lever; Pressure:2.5BAR; Press and release function; Adjustable nozzle with straight jet and mist spray | Unit: PCS | Catalog Qty/Carton: 20', -- Description
    166.4, -- SellingPrice (estimated: cost + 30% markup)
    128.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDKS1520', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Knapsack sprayer', -- Name
    N'»Œ«Œ… „Ì«Â ”⁄… 20 · —', -- NameAr
    N'20L; Fiber glass lance; Pressure:4.5BAR; Adjustable nozzle from jet to mist; Large container designed for spraying larger areas | Unit: PCS | Catalog Qty/Carton: 1', -- Description
    994.5, -- SellingPrice (estimated: cost + 30% markup)
    765.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSN1E03', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Plastic trigger nozzle', -- Name
    N'„”œ” „Ì«Â ÷€ÿ ⁄«·Ì »Ê“ ’€Ì—', -- NameAr
    N'3-Way Plastic Nozzle; TPR insulated comfortable soft grip; Nylon lever | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    94.9, -- SellingPrice (estimated: cost + 30% markup)
    73.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDNE8E34', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Plastic trigger nozzle', -- Name
    N'„”œ” „Ì«Â ÷€ÿ ⁄«·Ì »Ê“ Ê”ÿ', -- NameAr
    N'Adjustable 3-Way plastic trigger nozzle; Trigger lock helps continuous water flow; Flow control regulates water flow; High impact ABS body with anti-slip TPR handle | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    101.4, -- SellingPrice (estimated: cost + 30% markup)
    78.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSN1E07', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Plastic trigger nozzle', -- Name
    N'„”œ” „Ì«Â ÷€ÿ ⁄«·Ì »Ê“ ﬂ»Ì—', -- NameAr
    N'7-spray patterns; TPR insulated comfortable soft grip; Nylon Lever | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    123.5, -- SellingPrice (estimated: cost + 30% markup)
    95.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCE1401', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Wire cup brush', -- Name
    N'›—‘… ﬂ»«Ì… Õ—… „ﬁ«” 3 »Ê’… M14', -- NameAr
    N'Diameter:75mm(3 inch); Thread:M14X2; Wire dia:0.3mm; Wire length:23mm; RPM:12500r/min; Apply on: planing, deburring, edge honing, descaling/paint stripping, weld seams | Unit: PCS | Catalog Qty/Carton: 80', -- Description
    63.7, -- SellingPrice (estimated: cost + 30% markup)
    49.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAC1351', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Abrasive metal cutting disc', -- Name
    N'ÕÃ— »”ﬂÊ Â ›·«  5 »Ê’… *1 ÕœÌœ/” «‰·”', -- NameAr
    N'125mm(5 inch)*1.0mm*22.2mm; Flat centre; Cutting disc for metal and inox | Unit: PCS | Catalog Qty/Carton: 400', -- Description
    14.3, -- SellingPrice (estimated: cost + 30% markup)
    11.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAC1371', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Abrasive metal cutting disc', -- Name
    N'ÕÃ— »”ﬂÊ Â ›·«  7 »Ê’… *1.6 ÕœÌœ- ” «‰·”', -- NameAr
    N'180mm(7 inch)*1.6mm*22.2mm; Flat centre; Cutting disc for metal and inox | Unit: PCS | Catalog Qty/Carton: 50', -- Description
    36.4, -- SellingPrice (estimated: cost + 30% markup)
    28.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDAC1391', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Abrasive metal cutting disc', -- Name
    N'ÕÃ— »”ﬂÊ Â 9 »Ê’… *1.9„„ ÕœÌœ- ‰Õ«”', -- NameAr
    N'230mm(9 inch)*1.9mm*22.2mm; Flat centre; Cutting disc for metal and inox | Unit: PCS | Catalog Qty/Carton: 50', -- Description
    49.4, -- SellingPrice (estimated: cost + 30% markup)
    38.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTD6B02', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'9 Pcs HSS twist drill bits set', -- Name
    N'ÿﬁ„ »‰ÿ Õœ«œÌ 9 ﬁÿ⁄ 2:10 „„', -- NameAr
    N'HSS drill bit; 9 Pcs HSS twist drill bits set: 2mm,3mm,4mm,5mm,6mm,7mm,8mm,9mm,10mm; Suitable for metal drilling operations | Unit: SET | Catalog Qty/Carton: 24', -- Description
    236.6, -- SellingPrice (estimated: cost + 30% markup)
    182.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSV0K12', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Screwdriver bits', -- Name
    N'ÿﬁ„ ”‰ „›ﬂ 2 ﬁÿ⁄… ’·Ì»…/’·Ì»…', -- NameAr
    N'PH2+PH2,65mm,Double end bits,2pcs/set; CR-V,hardened and tempered; Sandblasted surface | Unit: SET | Catalog Qty/Carton: 300', -- Description
    19.5, -- SellingPrice (estimated: cost + 30% markup)
    15.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSV4K64', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Impact screwdriver bits', -- Name
    N'ÿﬁ„ ”‰ „›ﬂ 2 ﬁÿ⁄… ’·Ì»… ÿÊÌ·', -- NameAr
    N'Impact Screwdriver bit PH2 150mm; 2pcs/set; With high visibility sleeve; S2 industrial steel; Black surface with magnet | Unit: SET | Catalog Qty/Carton: 200', -- Description
    59.8, -- SellingPrice (estimated: cost + 30% markup)
    46.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTP2903', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Test pencil', -- Name
    N'„›ﬂ  Ì”  ⁄«œ… 3*140 „„', -- NameAr
    N'Test Voltage:AC 100-250V; Slotted size:3x140mm; Packed by plastic bag | Unit: PCS | Catalog Qty/Carton: 600', -- Description
    23.4, -- SellingPrice (estimated: cost + 30% markup)
    18.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTP2904', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Test pencil', -- Name
    N'„›ﬂ  Ì”  ⁄«œ… 4*190 „„', -- NameAr
    N'Test Voltage:AC 100-250V; Slotted size:4x190mm; Packed by plastic bag | Unit: PCS | Catalog Qty/Carton: 480', -- Description
    28.6, -- SellingPrice (estimated: cost + 30% markup)
    22.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSD4921', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Phillips screwdriver', -- Name
    N'„›ﬂ ’·Ì»… ﬂ—»Ì—« Ì— ⁄ÃÊ“ PH2*38mm', -- NameAr
    N'40CR,PH2,Round shank; Diameter:6.0mm; Length:38mm; Packed by plastic hanger | Unit: PCS | Catalog Qty/Carton: 240', -- Description
    15.6, -- SellingPrice (estimated: cost + 30% markup)
    12.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSDB221', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Phillips screwdriver', -- Name
    N'„›ﬂ ’·Ì»… ﬂ—»Ì—« Ì— ⁄ÃÊ“ PH2*38mm 2 ·Ê‰ „€‰«ÿÌ”Ì', -- NameAr
    N'Blade material:S2; Tip type:phillips PH2(magnetic); Blade length:38mm; Blade diameter:6.0mm; Handle material:two-color ergonomic; With strong magnetic | Unit: PCS | Catalog Qty/Carton: 240', -- Description
    33.8, -- SellingPrice (estimated: cost + 30% markup)
    26.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSD2225', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Phillips screwdriver', -- Name
    N'„›ﬂ ’·Ì»… 2 ·Ê‰ „„€‰ÿ 125 „·Ì', -- NameAr
    N'Tip type:phillips; Tip size:PH2; Diameter:6.0mm; Length:125mm; Material:Cr-V steel; Blade finish:black tip; Magnetised tip; Comfortable soft grip handle | Unit: PCS | Catalog Qty/Carton: 144', -- Description
    31.2, -- SellingPrice (estimated: cost + 30% markup)
    24.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSD2224', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Phillips screwdriver', -- Name
    N'„›ﬂ ’·Ì»Â 100x6 „„', -- NameAr
    N'Tip type:phillips; Tip size:PH2; Diameter:6.0mm; Length:100mm; Material:Cr-V steel; Blade finish:black tip; Magnetised tip | Unit: PCS | Catalog Qty/Carton: 144', -- Description
    28.6, -- SellingPrice (estimated: cost + 30% markup)
    22.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSD2226', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Phillips screwdriver', -- Name
    N'„›ﬂ ’·Ì»Â 150x6 „„', -- NameAr
    N'Tip type:phillips; Tip size:PH2; Diameter:6.0mm; Length:150mm; Material:Cr-V steel; Blade finish:black tip; Comfortable soft grip handle | Unit: PCS | Catalog Qty/Carton: 144', -- Description
    32.5, -- SellingPrice (estimated: cost + 30% markup)
    25.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS8608', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'8 in 1 stubby ratchet screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ „ ⁄œœ 8◊1', -- NameAr
    N'8 in 1 stubby ratchet screwdriver set; Material:Cr-V; Includes 1 Pcs ratchet handle, 7 Pcs 1/4 inch*25mm screwdriver bits: SL5,SL6,PH0,PH1,PH2,T10,T20 | Unit: SET | Catalog Qty/Carton: 80', -- Description
    93.6, -- SellingPrice (estimated: cost + 30% markup)
    72.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1B10', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'10 Pcs interchangeable screwdriver set', -- Name
    N'„›ﬂ ﬁ·«» 9*1 Ìœ ﬂ«Ê ‘ ⁄«œ… Ê ’·Ì»… Ê ﬂ·»”«  Ìœ 2 ·Ê‰', -- NameAr
    N'10 Pcs interchangeable screwdriver set; Material:Cr-V; Includes SL3,SL5,SL6,PH0,PH1,PH2 x65mm screwdrivers, tack remover, scratch awl | Unit: SET | Catalog Qty/Carton: 44', -- Description
    157.3, -- SellingPrice (estimated: cost + 30% markup)
    121.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1202', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'2 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  ﬂ—»Ì—« Ì— 2 ﬁ PH2*38mm SL6.5*38mm', -- NameAr
    N'2 Pcs screwdriver set; Material:CR-V Round shank; SL6.5*38mm, PH2*38mm; Packed by plastic hanger | Unit: SET | Catalog Qty/Carton: 80', -- Description
    44.2, -- SellingPrice (estimated: cost + 30% markup)
    34.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1204', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'4 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  4 ﬁÿ⁄ Ìœ ﬂ«Ê ‘ 2 ·Ê‰ ⁄«œ… + ’·Ì»…', -- NameAr
    N'4 Pcs screwdriver set; Material:CR-V Round shank; SL5.5*75,SL6.5*100,PH1*75,PH2*100 | Unit: SET | Catalog Qty/Carton: 40', -- Description
    110.5, -- SellingPrice (estimated: cost + 30% markup)
    85.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1206', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'6 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  6 ﬁÿ⁄ Ìœ ﬂ«Ê ‘ 2 ·Ê‰ ⁄«œ… + ’·Ì»…', -- NameAr
    N'6 Pcs screwdriver set; Material:CR-V Round shank; SL3*75,SL5.5*100,SL6.5*100,PH0*75,PH1*100,PH2*100 | Unit: SET | Catalog Qty/Carton: 36', -- Description
    149.5, -- SellingPrice (estimated: cost + 30% markup)
    115.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS2408', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'8 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  8 ﬁÿ⁄ Ìœ »·«” Ìﬂ ⁄«œ… + ’·Ì»…', -- NameAr
    N'8 Pcs screwdriver set; Material:40cr Round shank; SL6.5*38,PH2*38,SL3*75,SL5.5*75,SL6.5*100,PH0*75,PH1*75,PH2*100 | Unit: SET | Catalog Qty/Carton: 36', -- Description
    133.9, -- SellingPrice (estimated: cost + 30% markup)
    103.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS2410', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'10 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  10 ﬁÿ⁄ ⁄«œ… - ’·Ì»… Ìœ »·«” Ìﬂ', -- NameAr
    N'10 Pcs screwdriver set; Material:40cr Round shank; SL3*75,SL5.5*100,SL6.5*100,SL8*150,SL6.5*38,PH0*75,PH1*100,PH2*100,PH3*150,PH2*38 | Unit: SET | Catalog Qty/Carton: 24', -- Description
    218.4, -- SellingPrice (estimated: cost + 30% markup)
    168.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1416', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'16 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  16 ﬁÿ⁄… Ìœ 2 ·Ê‰ „€‰«ÿÌ”Ì ⁄«œ… + ’·Ì»…', -- NameAr
    N'16 Pcs Screwdriver set; New design handle; Material:Cr-V; Includes slotted and phillips screwdrivers in various sizes | Unit: SET | Catalog Qty/Carton: 10', -- Description
    284.7, -- SellingPrice (estimated: cost + 30% markup)
    219.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1J31', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'31-In-1 precision screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ „ ⁄œœ 31 ﬁÿ⁄…', -- NameAr
    N'31-In-1 Screwdriver set; Material:Cr-V; Includes 1pcs two color soft handle, 30pcs 4X28mm bits (slotted, phillips, hex, torx, tri-wing, y-type, five star, scratch awl) | Unit: SET | Catalog Qty/Carton: 36', -- Description
    135.2, -- SellingPrice (estimated: cost + 30% markup)
    104.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS1430', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'30 Pcs screwdriver set', -- Name
    N'ÿﬁ„ „›ﬂ«  30 ﬁÿ⁄… ⁄«œ… ’·Ì»… + ”‰Ê‰ „›ﬂ', -- NameAr
    N'30 Pcs screwdriver set; Material:CR-V; With 1pcs 1/4 inch*100mm screwdriver magnetic shank, 20pcs c-rv 4*25mm screwdriver bits, plus 6 individual screwdrivers | Unit: SET | Catalog Qty/Carton: 20', -- Description
    254.8, -- SellingPrice (estimated: cost + 30% markup)
    196.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS8B43', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'43 Pcs Screwdriver bits set', -- Name
    N'ÿﬁ„ Ìœ „›ﬂ ”Ì” Ì„ 43 ﬁÿ⁄… ·ﬁ„ 1/4 »Ê’… + ”‰Ê‰ „›ﬂ ⁄·»… »·«” Ìﬂ', -- NameAr
    N'43 Pcs screwdriver set; Material:CR-V; 18Pcs 1/4 inch X25mm bits, 12Pcs 4X28mm precision bits, bits adaptor, 9Pcs 1/4 inch X20.5mm sockets, bits holder, ratchet handle | Unit: SET | Catalog Qty/Carton: 24', -- Description
    358.8, -- SellingPrice (estimated: cost + 30% markup)
    276.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS8B45', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'45 Pcs T-handle wrench screwdriver set', -- Name
    N'ÿﬁ„ Ìœ Õ—› T „›ﬂ ”Ì” Ì„ 45 ﬁÿ⁄… ·ﬁ„ 1/4 »Ê’… + ”‰Ê‰ „›ﬂ ⁄·»… »·«” Ìﬂ', -- NameAr
    N'45 Pcs T-handle wrench screwdriver set; Material:CR-V; 36Pcs 1/4 inch X25mm screwdriver bits, 6Pcs 1/4 inch X20.5mm socket, adaptor, bits holder, T-ratchet handle | Unit: SET | Catalog Qty/Carton: 24', -- Description
    349.7, -- SellingPrice (estimated: cost + 30% markup)
    269.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL0954', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Mini long nose pliers', -- Name
    N'»‰”… »Ê“ ⁄œ·… ≈·ﬂ —Ê‰Ì 4.5 »Ê’…', -- NameAr
    N'Size:4.5 inch/115mm; Polish and anti-rust oil; With plastic handle | Unit: PCS | Catalog Qty/Carton: 120', -- Description
    66.3, -- SellingPrice (estimated: cost + 30% markup)
    51.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL2926', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Long nose pliers', -- Name
    N'»‰”… »Ê“ ÿÊÌ· ⁄œ·… 6 »Ê’… Ìœ 1 ·Ê‰', -- NameAr
    N'Size:6 inch/160mm; Material:carbon steel; Surface treatment:polish and anti-rust oil; One color handle | Unit: PCS | Catalog Qty/Carton: 60', -- Description
    89.7, -- SellingPrice (estimated: cost + 30% markup)
    69.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL2C08', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Long nose pliers', -- Name
    N'»‰”… »Ê“ ÿÊÌ· ⁄œ·… 8 »Ê’… Ìœ 2 ·Ê‰', -- NameAr
    N'Size:8 inch/200mm; Material:carbon steel; Surface treatment:polish and anti-rust oil; Two color handle | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    132.6, -- SellingPrice (estimated: cost + 30% markup)
    102.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL2718', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High leverage long nose pliers', -- Name
    N'»‰”… »Ê“ ÿÊÌ· ⁄œ·… 8 »Ê’… Ìœ 2 ·Ê‰ Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:8 inch/200mm; Energy saving 30% than normal pliers; Material:Cr-V; With nail holder function; With spanner function | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    157.3, -- SellingPrice (estimated: cost + 30% markup)
    121.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL3927', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Diagonal cutting pliers', -- Name
    N'ﬁ’«›… Ã«‰»Ì… 7 »Ê’… Ìœ 1 ·Ê‰', -- NameAr
    N'Size:7 inch/180mm; Material:carbon steel; Surface treatment:polish and anti-rust oil; One color handle | Unit: PCS | Catalog Qty/Carton: 60', -- Description
    109.2, -- SellingPrice (estimated: cost + 30% markup)
    84.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL7C07', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Heavy-duty diagonal cutting pliers', -- Name
    N'ﬁ’«›… 7 »Ê’… Ìœ 2 ·Ê‰-Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:7 inch/180mm; Material:carbon steel; Surface treatment:polish and anti-rust oil; Two color handle | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    135.2, -- SellingPrice (estimated: cost + 30% markup)
    104.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL3717', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High leverage diagonal cutting pliers', -- Name
    N'ﬁ’«›… Ã«‰»Ì… Œœ„… ‘«ﬁ… 7 »Ê’… Ìœ 2 ·Ê‰', -- NameAr
    N'Size:7 inch/180mm; Energy saving 30% than normal pliers; Material:Cr-V; With deburring function | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    159.9, -- SellingPrice (estimated: cost + 30% markup)
    123.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL3716', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High leverage diagonal cutting pliers', -- Name
    N'ﬁ’«›Â 6 »Ê’Â Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:6 inch/160mm; Energy saving 30% than normal pliers; Surface treatment:black finish and polish; With deburring function | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    128.7, -- SellingPrice (estimated: cost + 30% markup)
    99.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL7717', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High leverage heavy-duty diagonal cutting pliers', -- Name
    N'ﬁ’«›… Ã«‰»Ì… ›ﬂ ⁄—Ì÷ Œœ„… ‘«ﬁ… 7 »Ê’… Ìœ 2 ·Ê‰', -- NameAr
    N'Size:7 inch/180mm; Energy saving 30% than normal pliers; Material:Cr-V; With deburring function | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    169.0, -- SellingPrice (estimated: cost + 30% markup)
    130.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL7718', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High leverage heavy-duty diagonal cutting pliers', -- Name
    N'ﬁ’«›… Ã«‰»Ì… Œœ„… ‘«ﬁ… 8 »Ê’… Ìœ 2 ·Ê‰', -- NameAr
    N'Size:8 inch/200mm; Energy saving 30% than normal pliers; Material:Cr-V; With deburring function | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    176.8, -- SellingPrice (estimated: cost + 30% markup)
    136.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL1928', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Combination pliers', -- Name
    N'–—«œÌ… 8 »Ê’… Ìœ 1 ·Ê‰', -- NameAr
    N'Size:8 inch/200mm; Material:carbon steel; Surface treatment:polish and anti-rust oil; One color handle | Unit: PCS | Catalog Qty/Carton: 60', -- Description
    114.4, -- SellingPrice (estimated: cost + 30% markup)
    88.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL1717', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'High leverage combination pliers', -- Name
    N'–—«œÌ… 7 »Ê’… Ìœ 2 ·Ê‰ Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:7 inch/180mm; Energy saving 30% than normal pliers; With nail holder, spanner, and deburring functions | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    163.8, -- SellingPrice (estimated: cost + 30% markup)
    126.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL2778', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Multi-function long nose pliers', -- Name
    N'»‰”… »Ê“ ÿÊÌ· 8 »Ê’… „ ⁄œœ… «·≈” Œœ«„«  6*1 Ìœ 2 ·Ê‰ Œœ„… ‘«ﬁ…', -- NameAr
    N'6-in-1 Multi-function long nose pliers; Size:8 inch/200mm; Energy saving 30% than normal pliers; Material:Cr-V; Multi-purpose design | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    245.7, -- SellingPrice (estimated: cost + 30% markup)
    189.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL2768', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Compound action long nose pliers', -- Name
    N'»‰”… »Ê“ ÿÊÌ· 8.5 »Ê’… „ ⁄œœ… «·≈” Œœ«„«  3*1 Ìœ 2 ·Ê‰ Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:8.5 inch/215mm; Energy saving 65% than normal pliers; Material:Cr-V; With nail holder function; With spanner function | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    223.6, -- SellingPrice (estimated: cost + 30% markup)
    172.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL1768', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Compound action combination pliers', -- Name
    N'–—«œÌ… 8 »Ê’… „ ⁄œœ… «·≈” Œœ«„«  Ìœ 2 ·Ê‰ Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:8 inch/200mm; Energy saving 65% than normal pliers; Material:Cr-V; With nail holder, spanner and deburring functions | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    230.1, -- SellingPrice (estimated: cost + 30% markup)
    177.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL5685', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Wire stripper', -- Name
    N'»‰”…  —«„· „ ⁄œœ «·«” Œœ«„«  „ﬁ«” 8.5 »Ê’… 5*1', -- NameAr
    N'Size:8.5 inch/215mm; With sharp edge for cutting wire; With push down wire function; With wire stripping function; Can strip seven kinds of wire sizes | Unit: PCS | Catalog Qty/Carton: 72', -- Description
    152.1, -- SellingPrice (estimated: cost + 30% markup)
    117.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPS0623', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'3 Pcs pliers set', -- Name
    N'ÿﬁ„ »‰” 3 ﬁÿ⁄ –—«œÌ… 6 »Ê’… + ﬁ’«›… Ã«‰»Ì… 6 »Ê’… + »‰”… »Ê“ 6 »Ê’… Œœ„… ‘«ﬁ…', -- NameAr
    N'Includes 6 inch Combination pliers, 6 inch Long nose pliers, 6 inch Diagonal cutting pliers; Polish and anti-rust oil; One color handle | Unit: SET | Catalog Qty/Carton: 20', -- Description
    291.2, -- SellingPrice (estimated: cost + 30% markup)
    224.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPS0603', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'3Pcs pliers set', -- Name
    N'ÿﬁ„ »‰” 3 ﬁÿ⁄ –—«œÌ… 7 »Ê’… + ﬁ’«›… Ã«‰»Ì… 6 »Ê’… + »‰”… »Ê“ 6 »Ê’… Ìœ 2 ·Ê‰', -- NameAr
    N'Includes 7 inch Combination pliers, 6 inch Long nose pliers, 6 inch Diagonal cutting pliers; Two color handle | Unit: SET | Catalog Qty/Carton: 20', -- Description
    335.4, -- SellingPrice (estimated: cost + 30% markup)
    258.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPS0605', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'5 Pcs Pliers set', -- Name
    N'ÿﬁ„ »‰” 5 ﬁÿ⁄ –—«œÌ… 6 »Ê’… + ﬁ’«›… Ã«‰»Ì… 6 »Ê’… + »‰”… »Ê“ 6 »Ê’… + –—«œÌ… 4.5 »Ê’… + ﬁ’«›… 4.5 »Ê’… Ìœ »·«” Ìﬂ Ê«Õœ ·Ê‰', -- NameAr
    N'Includes 6 inch Combination, Long nose, Diagonal cutting pliers plus 4.5 inch Mini combination and Mini diagonal cutting pliers | Unit: SET | Catalog Qty/Carton: 20', -- Description
    408.2, -- SellingPrice (estimated: cost + 30% markup)
    314.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPS0413', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Multi-function pliers set', -- Name
    N'ÿﬁ„ »‰” Œœ„… ‘«ﬁ… 3 ﬁÿ⁄ –—«œÌ… 8 »Ê’… „ ⁄œœ… 7*1 / ﬁ’«›… Ã«‰»Ì… 8 »Ê’… „ ⁄œœ… 4*1 / »‰”… »Ê“ 8 »Ê’… „ ⁄œœ… 6*1 Œœ„… ‘«ﬁ… Ìœ 2 ·Ê‰', -- NameAr
    N'3Pcs/set includes: 7-in-1 multi-function combination pliers, 6-in-1 multi-function long nose pliers, 4-in-1 multi-function diagonal cutting pliers; Energy saving 30%; Material:Cr-V | Unit: SET | Catalog Qty/Carton: 20', -- Description
    670.8, -- SellingPrice (estimated: cost + 30% markup)
    516.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL6410', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Slip joint pliers', -- Name
    N'»‰”… Ã«“ ›Ê—œ ⁄œ·… 10 »Ê’… Ìœ 1 ·Ê‰', -- NameAr
    N'Size:10 inch/250mm; Polish and anti-rust oil; Packed by PP hanger | Unit: PCS | Catalog Qty/Carton: 60', -- Description
    152.1, -- SellingPrice (estimated: cost + 30% markup)
    117.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDPL6910', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Pump pliers', -- Name
    N'»‰”… €—«» 10 »Ê’… Ìœ 2 ·Ê‰', -- NameAr
    N'Size:10 inch/250mm; Max clamping open:40mm; Polish and anti-rust oil; Two color handle | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    169.0, -- SellingPrice (estimated: cost + 30% markup)
    130.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDBQ4601', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Automatic wire stripper', -- Name
    N'ﬁ‘«—… ”·ﬂ √Ê Ê„« Ìﬂ 3*1 „ ⁄œœ… «·«” Œœ«„« ', -- NameAr
    N'3 in 1 multi-function; Stripping,cutting and crimping; Stripping wires range:10AWG~24AWG(0.2~6mm2); Crimping function for various terminal sizes; Durable and comfortable handle | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    313.3, -- SellingPrice (estimated: cost + 30% markup)
    241.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLP1110', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Curved jaw locking plier', -- Name
    N'ﬂ·«»Â „ÃÊ›… 10 »Ê’…', -- NameAr
    N'Size:10 inch; Openings range:0-45mm; Carbon steel; HRC38-HRC48; Nickel plated | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    136.5, -- SellingPrice (estimated: cost + 30% markup)
    105.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDLP1C02', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Curved jaw locking plier', -- Name
    N'ﬂ·«»… „ÃÊ›… 10 »Ê’… Œœ„… ‘«ﬁ…', -- NameAr
    N'Size:10 inch; Material:Cr-V; Openings range:0-43mm; Hardened steel:HRC45-HRC60; Nickel plated | Unit: PCS | Catalog Qty/Carton: 30', -- Description
    226.2, -- SellingPrice (estimated: cost + 30% markup)
    174.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDMT4340', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Steel measuring tape', -- Name
    N'„ — ﬁÌ«” 7.5 „ — 25 „„ ‘—Ìÿ ≈ Ã«Â Ê«Õœ ”·«Õ √’›—', -- NameAr
    N'Length & width:7.5mx25mm; Double buttons; With metric and inch | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    115.7, -- SellingPrice (estimated: cost + 30% markup)
    89.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSL2G30', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Spirit level', -- Name
    N'„Ì“«‰ „Ì«… 2 ⁄Ì‰ 30 ”„', -- NameAr
    N'Length:30cm; Aluminum thickness:1mm | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    76.7, -- SellingPrice (estimated: cost + 30% markup)
    59.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSL2G80', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Spirit level', -- Name
    N'„Ì“«‰ „Ì«… 2 ⁄Ì‰ 80 ”„', -- NameAr
    N'Length:80cm; Aluminum thickness:1mm | Unit: PCS | Catalog Qty/Carton: 20', -- Description
    166.4, -- SellingPrice (estimated: cost + 30% markup)
    128.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSL2G100', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Spirit level', -- Name
    N'„Ì“«‰ „Ì«… 2 ⁄Ì‰ 100 ”„', -- NameAr
    N'Length:100cm; Aluminum thickness:1mm | Unit: PCS | Catalog Qty/Carton: 20', -- Description
    197.6, -- SellingPrice (estimated: cost + 30% markup)
    152.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTH6516', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Rapid cross wrench', -- Name
    N'„› «Õ ’·Ì»… ⁄Ã· 16 »Ê’… Ìœ »·«” Ìﬂ 1/2 »Ê’… + 4 ·ﬁ„ „ﬁ«” 17-19-21-23 „„', -- NameAr
    N'Driver size:1/2 inch; Size:16 inch(400mm); Wrench handle:275mm; Extension bar:400mm; With 2 pcs double-ended sockets (17/19mm)&(21/23mm); Material:CR-V | Unit: SET | Catalog Qty/Carton: 10', -- Description
    634.4, -- SellingPrice (estimated: cost + 30% markup)
    488.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSS2604', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'4Pcs Pick and Hook Set', -- Name
    N'ÿﬁ„ 4 ‘ÊﬂÂ „›ﬂ« ', -- NameAr
    N'4Pcs Pick and Hook Set; New design PP handle; Material:45# carbon steel; Full Hook, 45-degree, 90-degree, Straight Pick | Unit: SET | Catalog Qty/Carton: 100', -- Description
    75.4, -- SellingPrice (estimated: cost + 30% markup)
    58.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDHK2281', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Ball point hex key', -- Name
    N'ÿﬁ„ «·‰ﬂÌÂ „ÿÊ«Â »·ÌÂ 1.5-8 „„', -- NameAr
    N'8 Pcs ball point hex key set; Size:1.5mm,2mm,2.5mm,3mm,4mm,5mm,6mm,8mm; Material:Cr-V; Heat treatment and chrome plate | Unit: SET | Catalog Qty/Carton: 96', -- Description
    115.7, -- SellingPrice (estimated: cost + 30% markup)
    89.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDST2L24', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'24 Pcs 1/2 inch socket set', -- Name
    N'ÿﬁ„ ·ﬁ„ 1/2 »Ê’… »„› «Õ ”Ì” Ì„ 24 ﬁÿ⁄… 10:32 „„ ‘‰ÿ… „⁄œ‰', -- NameAr
    N'24 Pcs 1/2 inch socket set; Includes 18 Pcs sockets 10-32mm, extension bars, sliding T-bar, universal joint, spark plug socket, quick ratchet handle; Material:Cr-V steel | Unit: SET | Catalog Qty/Carton: 4', -- Description
    1812.2, -- SellingPrice (estimated: cost + 30% markup)
    1394.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTH1E19', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Open end swivel head socket spanner wrench', -- Name
    N'„› «Õ ‰«ÕÌ… »·œÌ Ê ‰«ÕÌ… ·ﬁ„… „‘—‘—… „ﬁ«” 19 „„ ÿÊ· 25 ”„', -- NameAr
    N'Combination flexible head wrench with 12 points socket; Size:19mm; Length:247mm; Wrench material:45# carbon steel; Chrome plated, matt finish | Unit: PCS | Catalog Qty/Carton: 48', -- Description
    159.9, -- SellingPrice (estimated: cost + 30% markup)
    123.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSP1205', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'5Pcs Combination spanner set', -- Name
    N'ÿﬁ„ „› «Õ »·œÌ - „‘—‘— 5 ﬁÿ⁄ 8/10/12/13/14 „„', -- NameAr
    N'5pcs combination spanner set; Size:8-14mm (8,10,12,13,14mm); Fine polished | Unit: SET | Catalog Qty/Carton: 24', -- Description
    214.5, -- SellingPrice (estimated: cost + 30% markup)
    165.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDSP5205', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'5 Pcs Flexible ratchet spanner set', -- Name
    N'ÿﬁ„ „› «Õ »·œÌ - „‘—‘— ”Ì” Ì„ 5 ﬁÿ⁄ 8-10-12-13-14 „„', -- NameAr
    N'5 Pcs Flexible ratchet spanner set; High quality Cr-Mo ratchet; Size:8-14mm; Drop forged steel; Ratchet action wrenches; 180 degree pivoting head | Unit: SET | Catalog Qty/Carton: 24', -- Description
    561.6, -- SellingPrice (estimated: cost + 30% markup)
    432.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTH4208', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'L-angled socket wrench set', -- Name
    N'ÿﬁ„ „›« ÌÕ »Ì»… 8 ﬁÿ⁄ Õ—› L „‰ 8-19 „„', -- NameAr
    N'L-Angled socket wrench set; 8pcs/set; Size:8,10,11,12,13,14,17,19mm; Material:CR-V; Heat treatment; Surface:chrome-plated | Unit: SET | Catalog Qty/Carton: 12', -- Description
    825.5, -- SellingPrice (estimated: cost + 30% markup)
    635.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCP1104', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'G clamp', -- Name
    N'“—ÃÌ‰Â Õ—› G „ﬁ«” 4 »Ê’…', -- NameAr
    N'Size:4 inch/100mm; Body cast iron; T-shaped thread | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    131.3, -- SellingPrice (estimated: cost + 30% markup)
    101.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCP1106', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'G clamp', -- Name
    N'“—ÃÌ‰Â Õ—› G „ﬁ«” 6 »Ê’…', -- NameAr
    N'Size:6 inch/150mm; Body cast iron; T-shaped thread | Unit: PCS | Catalog Qty/Carton: 16', -- Description
    235.3, -- SellingPrice (estimated: cost + 30% markup)
    181.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCP1108', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'G clamp', -- Name
    N'“—ÃÌ‰Â Õ—› G „ﬁ«” 8 »Ê’…', -- NameAr
    N'Size:8 inch/200mm; Body cast iron; T-shaped thread | Unit: PCS | Catalog Qty/Carton: 12', -- Description
    305.5, -- SellingPrice (estimated: cost + 30% markup)
    235.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDBV1A02', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Bench vice', -- Name
    N'„‰Ã·… ﬁ«⁄œ… „ Õ—ﬂ… “Â— ⁄’›Ê—… „ﬁ«” 2 »Ê’…', -- NameAr
    N'Swivel base Size:50mm; Body cast iron; With anvil | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    455.0, -- SellingPrice (estimated: cost + 30% markup)
    350.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDMB1310', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Machinist hammer', -- Name
    N'‘«ﬂÊ‘ »‰«—ÌÃ 1000 Ã—«„ Ìœ Œ‘»', -- NameAr
    N'Weight:1000g; Drop-forged hammerhead; Heat treatment,45# carbon steel; Hardwood handle | Unit: PCS | Catalog Qty/Carton: 12', -- Description
    193.7, -- SellingPrice (estimated: cost + 30% markup)
    149.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDCC1303', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Concrete chisel', -- Name
    N'√Ã‰… ÌœÊÌ 10 „”„«—', -- NameAr
    N'Size:4mm*16mm*250mm; Material:45# carbon steel; Heat treatment | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    76.7, -- SellingPrice (estimated: cost + 30% markup)
    59.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDWC2204', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'4 Pcs wood chisel set', -- Name
    N'ÿﬁ„ √“«„Ì· 4 ﬁÿ⁄ Ìœ ›«Ì»— 6-12-19-25 „„', -- NameAr
    N'4 pcs Wood Chisel Set; Width:6mm(1/4 inch),12mm(1/2 inch),19mm(3/4 inch),25mm(1 inch); Length:140mm | Unit: SET | Catalog Qty/Carton: 20', -- Description
    373.1, -- SellingPrice (estimated: cost + 30% markup)
    287.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTG3113', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Tools bag', -- Name
    N'‘‰ÿ… ⁄œ… ﬁ„«‘ 13 »Ê’…', -- NameAr
    N'Size:13 inch; Material:600D polyester; Size(L*W*H):33*21*18cm; Max load:8kg; Rigid frame for easy opening; With 3 pockets outside | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    217.1, -- SellingPrice (estimated: cost + 30% markup)
    167.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTG3116', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Tools bag', -- Name
    N'‘‰ÿ… ⁄œ… ﬁ„«‘ 16 »Ê’…', -- NameAr
    N'Size:16 inch; Material:600D polyester; Size(L*W*H):41*29*21cm; Max load:12kg; Rigid frame for easy opening; With 3 pockets outside | Unit: PCS | Catalog Qty/Carton: 20', -- Description
    331.5, -- SellingPrice (estimated: cost + 30% markup)
    255.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTG4100', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Tools backpack', -- Name
    N'‘‰ÿ… ŸÂ— ··⁄œ… 18 »Ê’…', -- NameAr
    N'Size:L34cm*W17cm*H45cm; Material:polyester oxford 600D; Max load:8kg; External pockets for additional storage capacity | Unit: PCS | Catalog Qty/Carton: 10', -- Description
    721.5, -- SellingPrice (estimated: cost + 30% markup)
    555.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDTB1311', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Plastic Organizer', -- Name
    N'‘‰ÿ… „‰Ÿ„ »·«” ﬂ 13.5 »Ê’… 12 ⁄Ì‰', -- NameAr
    N'Material:PP; Size:345mm*280mm*70mm; Built in wide handle; Removable cup compartments in 3 different sizes; With 12pcs removable cups | Unit: PCS | Catalog Qty/Carton: 6', -- Description
    306.8, -- SellingPrice (estimated: cost + 30% markup)
    236.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDKR1G25', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Sucker', -- Name
    N'‘›«ÿ “Ã«Ã 1 ⁄Ì‰ 25 ﬂÃ„', -- NameAr
    N'Handle material:ABS; Max pull:25kg; Cup diameter:115mm | Unit: PCS | Catalog Qty/Carton: 36', -- Description
    101.4, -- SellingPrice (estimated: cost + 30% markup)
    78.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDKR1G50', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Sucker', -- Name
    N'‘›«ÿ “Ã«Ã 2 ⁄Ì‰ 50 ﬂÃ„', -- NameAr
    N'Handle material:ABS; Max pull:50kg; Cup diameter:115mm | Unit: PCS | Catalog Qty/Carton: 18', -- Description
    193.7, -- SellingPrice (estimated: cost + 30% markup)
    149.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

INSERT INTO Products (SKU, Barcode, Name, NameAr, Description, SellingPrice, CurrentCostPrice, QuantityPerCarton, IsActive, TrackInventory, CategoryId, BrandId, CreatedAt, IsDeleted)
VALUES (
    N'JDBP1102', -- SKU
    NULL, -- Barcode (not provided in catalog)
    N'Block plane', -- Name
    N'›«—… Ã»”Ê‰ »Ê—œ 10 »Ê’…', -- NameAr
    N'Length:250mm; Plane body material:ABS; Replaceable high carbon steel blade; The rear screw is fixed to prevent the saw blade from shaking | Unit: PCS | Catalog Qty/Carton: 40', -- Description
    131.3, -- SellingPrice (estimated: cost + 30% markup)
    101.0, -- CurrentCostPrice (from agent price list)
    1, -- QuantityPerCarton (default as requested)
    1, -- IsActive
    1, -- TrackInventory
    1, -- CategoryId
    1, -- BrandId
    GETDATE(), -- CreatedAt
    0  -- IsDeleted
);

COMMIT TRAN;
-- Total products inserted: 184

SELECT * FROM Products;