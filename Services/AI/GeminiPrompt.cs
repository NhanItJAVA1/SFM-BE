using SFM_BE.DTOs.FinancialInsights;
using System.Text.Json;

namespace SFM_BE.Services.AI
{
    public class GeminiPrompt
    {
        public static string BuildBillScanPrompt(string categoryList)
        {
            return $$"""
               Analyze this receipt/invoice and extract its transaction data.

               This is an EXPENSE transaction.

               Available expense categories:
               {{categoryList}}

               Return ONLY valid JSON with this exact structure:
               {
                 "amount": null,
                 "transactionDate": null,
                 "location": null,
                 "description": null,
                 "suggestedCategoryId": null,
                 "items": [
                   {
                     "name": null,
                     "quantity": null,
                     "unitPrice": null,
                     "amount": null
                   }
                 ]
               }

               Extraction rules:
               - amount: final total amount actually paid.
               - transactionDate: date/time printed on the receipt, format yyyy-MM-ddTHH:mm:ss.
               - location: merchant/store/restaurant name and address if visible.
               - description: short description based only on visible receipt information.
               - Extract every readable purchased item.
               - quantity: purchased quantity.
               - unitPrice: price per unit.
               - item amount: line total; use quantity * unitPrice only when clearly determinable.
               - Do not include subtotal, tax, discount, total, cash, change, or payment method as items.
               - Use null for any value that cannot be reliably determined.
               - Use an empty items array if no purchased items can be identified.
               - Never guess unreadable or missing values.

               Category rules:
               - Choose the category that best represents the overall purpose of the transaction.
               - suggestedCategoryId MUST be one of the provided category IDs.
               - Never invent or modify a category ID.
               - Prefer a specific category over "Khác" when clearly applicable.
               - Use "Khác" only when no more specific category fits.
               - Return null if the category cannot be reasonably determined.

               Language rules:
               - Preserve the original language from the receipt.
               - Never translate merchant names, product names, addresses, or extracted text.
               - If the receipt is Vietnamese, keep the extracted text in Vietnamese.

               Output rules:
               - Return JSON only.
               - No markdown, comments, explanations, or additional fields.
               """;
        }

        public static string BuildFinancialInsight(FinancialInsightDataDto data)
        {
            var json = JsonSerializer.Serialize(data);

            return $$"""
            Bạn là trợ lý phân tích tài chính cá nhân.

            Dữ liệu dưới đây đã được backend tính toán chính xác.
            Không tự tạo, thay đổi hoặc suy đoán số liệu không có trong dữ liệu.

            Hãy phân tích:
            - Thu nhập
            - Chi tiêu
            - So sánh với tháng trước
            - Những thay đổi đáng chú ý

            Trả về JSON theo đúng cấu trúc:
            {    
              "summary": "...",
              "insights": [
                {    
                  "type": "...",
                  "title": "...",
                  "message": "..."
                }
              ]
            }

            Dữ liệu:
            {{json}}
            """;
        }
    }
}
