using MongoExample.Business;

namespace Point_of_Sales_System.Tests;

public class EmployeeRoleRulesTests
{
    [Theory]
    [InlineData("M001", true)]
    [InlineData("m999", true)]
    [InlineData(" M123 ", true)]
    [InlineData("E001", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void PidIndicatesManager_follows_M_prefix_rule(string? pid, bool expectedManager)
    {
        Assert.Equal(expectedManager, EmployeeRoleRules.PidIndicatesManager(pid));
    }

    [Theory]
    [InlineData("MGR-1", EmployeeRoleRules.ManagerRole)]
    [InlineData("e001", EmployeeRoleRules.EmployeeRole)]
    [InlineData(null, EmployeeRoleRules.EmployeeRole)]
    public void DetermineSessionRoleFromPid_matches_login_behavior(string? pid, string expectedRole)
    {
        Assert.Equal(expectedRole, EmployeeRoleRules.DetermineSessionRoleFromPid(pid));
    }
}
