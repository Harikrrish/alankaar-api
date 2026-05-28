namespace alankaar_api.Models;

public static class ClientStatuses
{
    public const string Stage1 = "Stage 1";
    public const string Stage2 = "Stage 2";
    public const string Stage3 = "Stage 3";
    public const string Stage4 = "Stage 4";
    public const string Stage5 = "Stage 5";

    public static readonly string[] All = [Stage1, Stage2, Stage3, Stage4, Stage5];

    public static bool IsValid(string? status)
    {
        return All.Contains(status?.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    public static string Normalize(string status)
    {
        return All.Single(stage => string.Equals(stage, status.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
