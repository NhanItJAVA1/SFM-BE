using AutoMapper;
using SFM_BE.DTOs.Accounts;
using SFM_BE.DTOs.Attachments;
using SFM_BE.DTOs.Budgets;
using SFM_BE.DTOs.Categories;
using SFM_BE.DTOs.Invoices;
using SFM_BE.DTOs.RecurringTransactions;
using SFM_BE.DTOs.Transactions;
using SFM_BE.DTOs.Transfers;
using SFM_BE.DTOs.Users;
using SFM_BE.Entities;
using SFM_BE.Enums;

namespace SFM_BE.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //USERS
        CreateMap<RegisterUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<User, UserResponseDto>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name.ToString()));

        //FINANCIAL ACCOUNTS
        CreateMap<CreateFinancialAccountDto, FinancialAccount>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateFinancialAccountDto, FinancialAccount>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<FinancialAccount, FinancialAccountResponseDto>()
            .ForMember(x => x.CurrentBalance, opt => opt.MapFrom(x => x.InitialBalance + x.Transactions
            .Where(t => t.DeletedAt == null && !t.IsExcluded)
            .Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount)));

        //CATEGORIES
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Category, CategoryResponseDto>();

        //TRANSACTIONS
        CreateMap<CreateTransactionDto, Transaction>()
            .ForMember(x => x.AccountId, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>  dest.AccountId = (long)context.Items["AccountId"]);
        CreateMap<UpdateTransactionDto, Transaction>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Transaction, TransactionResponseDto>();

        // TRANSACTION ATTACHMENTS
        CreateMap<TransactionAttachment, TransactionAttachmentDto>();
        CreateMap<CreateTransactionItemDto, TransactionItem>();

        // TRANSFERS
        CreateMap<CreateTransferDto, Transfer>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<Transfer, TransferResponseDto>();

        // BUDGETS
        CreateMap<CreateBudgetDto, Budget>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateBudgetDto, Budget>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Budget, BudgetResponseDto>()
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.Name : null));
        CreateMap<Budget, BudgetProgressDto>()
                .ForMember(x => x.BudgetId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.Name : null))
                .ForMember(x => x.SpentAmount, opt => opt.Ignore())
                .ForMember(x => x.RemainingAmount, opt => opt.Ignore())
                .ForMember(x => x.UsedPercentage, opt => opt.Ignore())
                .ForMember(x => x.IsAlert, opt => opt.Ignore());
        CreateMap<Budget, BudgetProgressDetailDto>()
                .ForMember(x => x.BudgetId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.Name : null))
                .ForMember(x => x.SpentAmount, opt => opt.Ignore())
                .ForMember(x => x.RemainingAmount, opt => opt.Ignore())
                .ForMember(x => x.UsedPercentage, opt => opt.Ignore())
                .ForMember(x => x.IsAlert, opt => opt.Ignore())
                .ForMember(x => x.DailySpendings, opt => opt.Ignore());

        // BUDGET ALERTS
        CreateMap<BudgetAlert, BudgetAlertResponseDto>();

        //INVOICES
        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .ForMember(x => x.Status, opt => opt.MapFrom(_ => InvoiceStatus.Pending))
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateInvoiceDto, Invoice>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Invoice, InvoiceResponseDto>();

        //RECURRING TRANSACTIONS
        CreateMap<CreateRecurringTransactionDto, RecurringTransaction>()
            .ForMember(x => x.AccountId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.AccountId = (long)context.Items["AccountId"]);
        CreateMap<UpdateRecurringTransactionDto, RecurringTransaction>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<RecurringTransaction, RecurringTransactionResponseDto>();
    }
}
