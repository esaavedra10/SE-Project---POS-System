namespace MongoExample.Business;

/// <summary>
/// PID-based manager detection (same rule as session role at login and manager credential checks).
/// </summary>
public static class EmployeeRoleRules
{
    public const string ManagerRole = "Manager";
    public const string EmployeeRole = "Employee";

    /// <summary>
    /// Returns true if the trimmed PID starts with "M" (case-insensitive).
    /// </summary>
    public static bool PidIndicatesManager(string? pid)
    {
        var trimmed = pid?.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return false;

        return trimmed.ToUpperInvariant().StartsWith("M", StringComparison.Ordinal);
    }

    /// <summary>
    /// Session role string stored after login: Manager or Employee.
    /// </summary>
    public static string DetermineSessionRoleFromPid(string? pid) =>
        PidIndicatesManager(pid) ? ManagerRole : EmployeeRole;
}
