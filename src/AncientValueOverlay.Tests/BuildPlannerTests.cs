using AncientValueOverlay;
using Xunit;

namespace AncientValueOverlay.Tests;

public sealed class BuildPlannerTests
{
    [Fact]
    public void Catalog_ContainsCurrentEightClassRosterAndTwentyTwoAscendancies()
    {
        Assert.Equal(8, BuildCatalog.Classes.Count);
        Assert.Equal(22, BuildCatalog.Classes.Sum(x => x.Ascendancies.Count));
        Assert.Contains(BuildCatalog.Classes, x => x.Name == "Monk" && x.Ascendancies.Any(a => a.Name == "Martial Artist"));
        Assert.Contains(BuildCatalog.Classes, x => x.Name == "Huntress" && x.Ascendancies.Any(a => a.Name == "Spirit Walker"));
        Assert.Contains(BuildCatalog.Classes, x => x.Name == "Sorceress" && x.Ascendancies.Any(a => a.Name == "Disciple of Varashta"));
    }

    [Fact]
    public void ClassDoesNotHardLockWeaponSkill_WhenGearRequirementIsMet()
    {
        var result = new BuildPlanner().Evaluate(
            "Warrior",
            "Titan",
            "Lightning Arrow",
            "Bow",
            "Empty / spell set",
            3);

        Assert.True(result.IsSkillUsable);
        Assert.Equal("Weapon Set 1", result.ActiveWeaponSet);
        Assert.Contains("Warrior", result.Summary);
    }

    [Fact]
    public void Planner_UsesSecondWeaponSet_WhenPrimarySetDoesNotMatch()
    {
        var result = new BuildPlanner().Evaluate(
            "Monk",
            "Invoker",
            "Lightning Arrow",
            "Quarterstaff",
            "Bow",
            4);

        Assert.True(result.IsSkillUsable);
        Assert.Equal("Weapon Set 2", result.ActiveWeaponSet);
    }

    [Fact]
    public void Planner_BlocksWeaponSpecificSkill_WhenNeitherSetMatches()
    {
        var result = new BuildPlanner().Evaluate(
            "Ranger",
            "Deadeye",
            "Galvanic Shards",
            "Bow",
            "Quarterstaff",
            3);

        Assert.False(result.IsSkillUsable);
        Assert.Equal("Blocked by current gear", result.ActiveWeaponSet);
        Assert.Contains("requires Crossbow", result.Summary);
    }

    [Fact]
    public void GearGrantedSkill_RequiresTheItemThatGrantsIt()
    {
        var planner = new BuildPlanner();

        var enabled = planner.Evaluate(
            "Sorceress",
            "Stormweaver",
            "Mana Drain",
            "Attuned Wand",
            "Empty / spell set",
            3);

        var disabled = planner.Evaluate(
            "Sorceress",
            "Stormweaver",
            "Mana Drain",
            "Chiming Staff",
            "Empty / spell set",
            3);

        Assert.True(enabled.IsSkillUsable);
        Assert.False(disabled.IsSkillUsable);
        Assert.Contains(enabled.GearNotes, x => x.Contains("Mana Drain"));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(3, 3)]
    [InlineData(9, 5)]
    public void Planner_ClampsSupportSocketsToPoe2Range(int requested, int expected)
    {
        var result = new BuildPlanner().Evaluate(
            "Sorceress",
            "Stormweaver",
            "Spark",
            "Empty / spell set",
            "Empty / spell set",
            requested);

        Assert.Equal(expected, result.SupportSlots);
    }

    [Fact]
    public void DruidStarter_UsesTalismanGrantedSkill()
    {
        var planner = new BuildPlanner();
        var starter = planner.StarterFor("Druid");

        Assert.Equal("Shapeshift Basic Attack", starter.Skill);
        Assert.Equal("Talisman", starter.WeaponSetOne);

        var evaluation = planner.Evaluate(
            "Druid",
            "Shaman",
            starter.Skill,
            starter.WeaponSetOne,
            starter.WeaponSetTwo,
            3);

        Assert.True(evaluation.IsSkillUsable);
    }
}
