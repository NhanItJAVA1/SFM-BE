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
        CreateMap<RegisterUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<User, UserResponseDto>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name.ToString()));

        CreateMap<CreateFinancialAccountDto, FinancialAccount>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateFinancialAccountDto, FinancialAccount>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<FinancialAccount, FinancialAccountResponseDto>();

        CreateMap<CreateCategoryDto, Category>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Category, CategoryResponseDto>();

        CreateMap<CreateTransactionDto, Transaction>()
            .ForMember(x => x.AccountId, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>  dest.AccountId = (long)context.Items["AccountId"]);
        CreateMap<UpdateTransactionDto, Transaction>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Transaction, TransactionResponseDto>();

        CreateMap<TransactionAttachment, TransactionAttachmentDto>();
        CreateMap<CreateTransactionItemDto, TransactionItem>();

        CreateMap<CreateTransferDto, Transfer>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<Transfer, TransferResponseDto>();

        CreateMap<CreateBudgetDto, Budget>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateBudgetDto, Budget>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Budget, BudgetResponseDto>();

        CreateMap<BudgetAlert, BudgetAlertResponseDto>();

        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .ForMember(x => x.Status, opt => opt.MapFrom(_ => InvoiceStatus.Pending))
            .AfterMap((src, dest, context) => dest.UserId = (long)context.Items["UserId"]);
        CreateMap<UpdateInvoiceDto, Invoice>()
            .ForMember(x => x.CreatedAt, opt => opt.Ignore())
            .ForMember(x => x.DeletedAt, opt => opt.Ignore())
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Invoice, InvoiceResponseDto>();

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
