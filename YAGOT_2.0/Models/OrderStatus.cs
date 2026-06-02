namespace YAGOT_2._0.Models;

/// <summary>
/// حالات الطلب المطابقة لقيمة الحقل status في قاعدة البيانات.
/// </summary>
public enum OrderStatus
{
    Pending,
    Processed,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}

public static class OrderStatusFilters
{
    public const string All = "All";

    /// <summary>
    /// يحوّل معامل الاستعلام إلى قيمة مخزنة في قاعدة البيانات، أو null لعرض جميع الطلبات.
    /// يقبل أيضاً "Processing" كمرادف لـ Processed حسب التوثيق.
    /// </summary>
    public static string? NormalizeQuery(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status.Equals(All, StringComparison.OrdinalIgnoreCase))
            return null;

        if (status.Equals("Processing", StringComparison.OrdinalIgnoreCase))
            return nameof(OrderStatus.Processed);

        foreach (var name in Enum.GetNames<OrderStatus>())
        {
            if (name.Equals(status, StringComparison.OrdinalIgnoreCase))
                return name;
        }

        return null;
    }
}
