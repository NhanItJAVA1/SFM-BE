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

namespace SFM_BE.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserResponseDto>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name.ToString()));

        CreateMap<CreateFinancialAccountDto, FinancialAccount>();
        CreateMap<UpdateFinancialAccountDto, FinancialAccount>();
        CreateMap<FinancialAccount, FinancialAccountResponseDto>();

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        CreateMap<Category, CategoryResponseDto>();

        CreateMap<CreateTransactionDto, Transaction>();
        CreateMap<UpdateTransactionDto, Transaction>();
        CreateMap<Transaction, TransactionResponseDto>();

        CreateMap<TransactionAttachment, TransactionAttachmentDto>();

        CreateMap<CreateTransferDto, Transfer>();
        CreateMap<Transfer, TransferResponseDto>();

        CreateMap<CreateBudgetDto, Budget>();
        CreateMap<UpdateBudgetDto, Budget>();
        CreateMap<Budget, BudgetResponseDto>();
        CreateMap<BudgetAlert, BudgetAlertResponseDto>();

        CreateMap<CreateInvoiceDto, Invoice>();
        CreateMap<UpdateInvoiceDto, Invoice>();
        CreateMap<Invoice, InvoiceResponseDto>();

        CreateMap<CreateRecurringTransactionDto, RecurringTransaction>();
        CreateMap<UpdateRecurringTransactionDto, RecurringTransaction>();
        CreateMap<RecurringTransaction, RecurringTransactionResponseDto>();
    }
}
