using System;
using CardZen.Models;

namespace CardZen.Utilities;

public class DateStatus
{
    public int? DaysLeft { get; set; }
    public string Status { get; set; } = "none"; // overdue, danger, warning, info, safe
    public string Text { get; set; } = string.Empty;
    public string ColorClass { get; set; } = string.Empty;
}

public static class CardLogic
{
    public static DateStatus GetDateStatus(DateTime? targetDate, string type = "default", bool flag = false)
    {
        if (!targetDate.HasValue) return new DateStatus { Status = "none", Text = "---", ColorClass = "text-slate-400" };

        // Logic xử lý trạng thái đặc biệt từ code cũ[cite: 1]
        if (type == "due" && flag) // flag ở đây là isPaid
            return new DateStatus { Text = "Đã thanh toán", ColorClass = "text-emerald-500 font-bold" };
        if (type == "fee" && flag) // flag ở đây là hasOffer
            return new DateStatus { Text = "Đã có Offer", ColorClass = "text-purple-500 font-bold" };

        var now = DateTime.Today;
        var diff = (targetDate.Value.Date - now).Days;
        
        var status = diff switch
        {
            < 0 => "overdue",
            <= 3 => "danger",
            <= 5 => "warning",
            <= 10 => "info",
            _ => "safe"
        };

        var color = status switch
        {
            "overdue" => "text-red-700 font-black",
            "danger" => "text-red-500 font-black",
            "warning" => "text-orange-500 font-bold",
            "info" => "text-yellow-600",
            "none" => "text-slate-400",
            _ => "text-slate-500"
        };

        var text = diff == 0 ? "Hôm nay" : (diff < 0 ? $"Quá {Math.Abs(diff)} ngày" : $"{diff} ngày");

        return new DateStatus { DaysLeft = diff, Status = status, Text = text, ColorClass = color };
    }
    public static void CalculateNextDates(CardModel card)
    {
        if (card.StatementDate <= 0) return;

        DateTime now = DateTime.Today;
        
        // 1. Tính toán ngày chốt sao kê cho tháng hiện tại
        // Đảm bảo không bị Out of Range (Ví dụ: Tháng 4 chỉ có 30 ngày)
        int daysInThisMonth = DateTime.DaysInMonth(now.Year, now.Month);
        int safeDayThisMonth = Math.Min(card.StatementDate, daysInThisMonth);
        DateTime statementThisMonth = new DateTime(now.Year, now.Month, safeDayThisMonth);

        // 2. Xác định NextStatementDate
        if (now > statementThisMonth)
        {
            // Nếu đã qua ngày chốt tháng này, tính cho tháng sau
            DateTime nextMonth = now.AddMonths(1);
            int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
            int safeDayNextMonth = Math.Min(card.StatementDate, daysInNextMonth);
            card.NextStatementDate = new DateTime(nextMonth.Year, nextMonth.Month, safeDayNextMonth);
        }
        else
        {
            card.NextStatementDate = statementThisMonth;
        }

        // 3. Tính NextDueDate (Dựa trên NextStatementDate + GracePeriod)[cite: 1]
        if (card.NextStatementDate.HasValue)
        {
            card.NextDueDate = card.NextStatementDate.Value.AddDays(card.GracePeriod);
        }
    }
}
