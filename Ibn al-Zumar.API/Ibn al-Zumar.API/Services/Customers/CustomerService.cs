using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Persistence;
using IbnAlZumar.Domain.Entities.Sales;
using IbnAlZumar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IbnAlZumar.API.Services.Customers;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    private static readonly Expression<Func<Customer, DTOs.Customers.CustomerResponseDto>> ProjectToDto = c => new DTOs.Customers.CustomerResponseDto
    {
        Id = c.Id,
        FullName = c.FullName,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        Governorate = c.Governorate,
        IsRegistered = c.IsRegistered,
        CreditLimit = c.CreditLimit,
        CurrentBalance = c.CurrentBalance,
        CreatedAt = c.CreatedAt
    };

    public async Task<PagedResultDto<DTOs.Customers.CustomerResponseDto>> GetAllAsync(DTOs.Customers.CustomerFilterDto filter)
    {
        var query = _context.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.FullName.ToLower().Contains(term) ||
                (c.Phone != null && c.Phone.ToLower().Contains(term)) ||
                (c.Email != null && c.Email.ToLower().Contains(term)));
        }

        query = query.OrderBy(c => c.FullName);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(ProjectToDto)
            .ToListAsync();

        return new PagedResultDto<DTOs.Customers.CustomerResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<DTOs.Customers.CustomerResponseDto> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Include(c => c.Orders)
            .Select(c => new DTOs.Customers.CustomerResponseDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                Governorate = c.Governorate,
                IsRegistered = c.IsRegistered,
                CreditLimit = c.CreditLimit,
                CurrentBalance = c.CurrentBalance,
                CreatedAt = c.CreatedAt,
                Orders = c.Orders.Select(o => new DTOs.Sales.CustomerOrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    CustomerName = c.FullName,
                    CustomerPhone = c.Phone ?? string.Empty,
                    CustomerEmail = c.Email,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCost = 0, // تم تعديلها لتجنب الخطأ إذا لم تكن موجودة في الـ Entity
                    ShippingFee = 0, // استخدام الحقل المتاح في الـ Order
                    Notes = o.Notes,
                    SubTotal = o.SubTotal,
                    DiscountAmount = o.DiscountAmount,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (customer is null)
            throw new NotFoundException($"لم يتم العثور على عميل بالرقم {id}");

        return customer;
    }

    public async Task<DTOs.Customers.CustomerResponseDto> CreateAsync(DTOs.Customers.CreateCustomerDto dto)
    {
        await EnsurePhoneIsUniqueAsync(dto.Phone);

        var customer = new Customer
        {
            FullName = dto.FullName.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim(),
            Address = dto.Address?.Trim(),
            Governorate = dto.Governorate?.Trim(),
            CreditLimit = dto.CreditLimit,
            IsRegistered = dto.IsRegistered,
            CurrentBalance = 0
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(customer.Id);
    }

    public async Task<DTOs.Customers.CustomerResponseDto> UpdateAsync(int id, DTOs.Customers.UpdateSalesCustomerDto dto)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
            throw new NotFoundException($"لم يتم العثور على عميل بالرقم {id}");

        await EnsurePhoneIsUniqueAsync(dto.Phone, excludeCustomerId: id);

        customer.FullName = dto.FullName.Trim();
        customer.Phone = dto.Phone?.Trim();
        customer.Email = dto.Email?.Trim();
        customer.Address = dto.Address?.Trim();
        customer.Governorate = dto.Governorate?.Trim();
        customer.CreditLimit = dto.CreditLimit;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(customer.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
            throw new NotFoundException($"لم يتم العثور على عميل بالرقم {id}");

        if (customer.CurrentBalance != 0)
            throw new BadRequestException("لا يمكن حذف عميل عليه رصيد ديون قائم. قم بتسوية الرصيد أولاً.");

        customer.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<DTOs.Customers.CustomerResponseDto> AdjustDebtAsync(int id, DTOs.Customers.AdjustCustomerDebtDto dto)
    {
        if (dto.Amount == 0)
            throw new BadRequestException("قيمة التسوية يجب ألا تساوي صفر");

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null)
            throw new NotFoundException($"لم يتم العثور على عميل بالرقم {id}");

        var newBalance = customer.CurrentBalance + dto.Amount;
        customer.CurrentBalance = newBalance;

        _context.CustomerLedgerEntries.Add(new CustomerLedgerEntry
        {
            CustomerId = id,
            Amount = Math.Abs(dto.Amount),
            RunningBalance = newBalance,
            TransactionType = LedgerTransactionType.ManualAdjustment,
            TransactionDate = DateTime.UtcNow,
            Notes = dto.Reason
        });

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    private async Task EnsurePhoneIsUniqueAsync(string? phone, int? excludeCustomerId = null)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return;

        var exists = await _context.Customers
            .AnyAsync(c => c.Phone == phone && (!excludeCustomerId.HasValue || c.Id != excludeCustomerId.Value));

        if (exists)
            throw new BadRequestException($"يوجد بالفعل عميل بنفس رقم الهاتف: {phone}");
    }
}