namespace OutsideServer.Utils;

public class Comparator
{
    public static bool IsValidUpdate(string source, string? target)
        => !string.IsNullOrEmpty(target) && !target.Equals(source);

    public static bool IsValidUpdate(double source, double? target)
        => target is not null && !double.IsNaN((double)target) && !target.Equals(source);

    public static bool IsValidUpdate(int source, int? target)
        => target is not null && target >= 0 && !target.Equals(source);

    public static bool IsValidUpdate(DateTime source, DateTime? target)
        => target is not null && !target.Equals(source);
}
