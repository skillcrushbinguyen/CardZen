using System;
using System.Text.Json.Serialization;
using CardZen.Utilities;

namespace CardZen.Models;

// Cấu trúc dùng chung cho các danh mục dropdown
public class SelectOption
{
    public string Value { get; set; } = string.Empty; // Tên danh mục (Ẩm thực, Shopee...)[cite: 2]
    public string Icon { get; set; } = string.Empty;  // Icon hiển thị (🍴, 🛒...)[cite: 2]
    public string Description { get; set; } = string.Empty;
}

public class MasterData
{
    // Cả hai đều dùng chung cấu trúc Value/Icon[cite: 2]
    [JsonPropertyName("mcc_list")]
    public List<SelectOption> MccList { get; set; } = new();
    [JsonPropertyName("spending_categories")]
    public List<SelectOption> SpendingCategories { get; set; } = new();
}

public class BaseResponse<T>
{
    public bool Status { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public string ServerTime { get; set; } = DateTime.UtcNow.ToString("o");
}

public class CardModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty; // Ví dụ: VIB, VPBank...
    public string Status { get; set; } = "Active"; // Active hoặc Locked
    
    // Hạn mức và chi tiêu
    public long Limit { get; set; }
    public long TotalUsed { get; set; }
    
    // Chu kỳ thanh toán
    [JsonPropertyName("statement_date")]
    public int StatementDate { get; set; } // Ngày chốt sao kê
    [JsonPropertyName("grace_period")]
    public int GracePeriod { get; set; } // Hạn mức ưu đãi lãi suất
    
    // Trạng thái thanh toán & Phí
    [JsonPropertyName("is_paid")]
    public bool IsPaid { get; set; } // Tương ứng với card.is_paid === "TRUE"
    [JsonPropertyName("billing_note")]
    public string? BillingNote { get; set; }
    
    // --- CÁC TRƯỜNG BỔ SUNG CHO UI LOGIC ---
    // Các trường này sẽ được Backend (GAS) tính toán hoặc C# tính toán lại
    public DateTime? NextStatementDate { get; set; } 
    public DateTime? NextDueDate { get; set; }

    public long AnnualFeeAmount { get; set; }
    
    [JsonPropertyName("annual_fee_date")]
    [JsonConverter(typeof(FlexibleNullableDateTimeConverter))]
    public DateTime? AnnualFeeDate { get; set; }
    
    [JsonPropertyName("has_offer")]
    public bool HasOffer { get; set; } // Tương ứng với card.has_offer === "TRUE"
    [JsonPropertyName("fee_note")]
    public string? FeeNote { get; set; }

    // Cashback (Được parse từ chuỗi JSON cashback_rules trong code cũ)
    //public CashbackRules CashbackRules { get; set; } = new();
    
    // Dữ liệu Reward tính toán từ Backend
    public RewardSummary? Reward { get; set; }
    
    // Danh sách benefits rút gọn hiển thị trên badge
    [JsonPropertyName("benefits_list")]
    public List<BenefitItem> BenefitsList { get; set; } = new();
    // Mapping ngược: Lấy giá trị từ phần tử đầu tiên nếu có dữ liệu
    public bool IsCashbackActive 
    { 
        get => BenefitsList.Any() && BenefitsList.First().IsActive;
        set { /* Sẽ được xử lý khi lưu */ } 
    }

    public long GlobalLimit 
    { 
        get => BenefitsList.FirstOrDefault()?.GlobalLimit ?? 0;
        set { /* Sẽ được xử lý khi lưu */ }
    }

    public long MinSpendRequest 
    { 
        get => BenefitsList.FirstOrDefault()?.MinSpendReq ?? 0;
        set { /* Sẽ được xử lý khi lưu */ }
    }
}

public class MccCategory
{
    public string Mcc { get; set; } = string.Empty; // Tên danh mục (Ẩm thực, Shopee...)
    public double Rate { get; set; } // % Hoàn tiền
    public long MaxLimit { get; set; } // Giới hạn hoàn tiền riêng cho danh mục này
}

public class RewardSummary
{
    public bool Status { get; set; }
    public long Earned { get; set; }
    public long Limit { get; set; } // Trần hoàn tiền kỳ này
    public double Ratio => Limit > 0 ? Math.Min(100, (double)Earned / Limit * 100) : 0;
}

public class BenefitItem
{
    [JsonConverter(typeof(FlexibleStringConverter))] // Fix lỗi parse number -> string
    [JsonPropertyName("id")] 
    public string Id { get; set; } = "";
    [JsonPropertyName("mcc_group")] 
    public string MccGroup { get; set; } = "";
    [JsonPropertyName("rate")] 
    public double Rate { get; set; }
    [JsonPropertyName("cat_limit")] 
    public long CatLimit { get; set; }
    
    // Các trường sau tạm thời lặp lại theo JSON nhưng sẽ được quản lý tập trung ở CardModel
    [JsonPropertyName("global_limit")] 
    public long GlobalLimit { get; set; }
    [JsonPropertyName("min_spend_req")] 
    public long MinSpendReq { get; set; }
    [JsonPropertyName("is_active")] 
    public bool IsActive { get; set; }
}