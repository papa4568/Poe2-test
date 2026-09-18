using AncientValueOverlay;
using Xunit;

namespace AncientValueOverlay.Tests;

public sealed class SkillSimulatorTests
{
    [Fact]
    public void HeavyWeaponProfile_ChangesAttackDamageAndSpeedIndexes()
    {
        var result = Simulate(
            skill: "Lightning Arrow",
            setOne: "Bow",
            setOneProfile: "Heavy weapon");

        Assert.True(result.Build.IsSkillUsable);
        Assert.Equal(125m, result.DamageIndex);
        Assert.Equal(90m, result.SpeedIndex);
        Assert.Equal(1, result.ProjectileCount);
    }

    [Fact]
    public void Spell_IgnoresWeaponDamageIndex()
    {
        var result = Simulate(
            skill: "Spark",
            setOne: "Empty / spell set",
            setOneProfile: "Heavy weapon");

        Assert.Equal(100m, result.DamageIndex);
        Assert.Equal(100m, result.SpeedIndex);
    }

    [Fact]
    public void Multishot_ChangesProjectileDamageSpeedAndCount()
    {
        var result = Simulate(
            skill: "Lightning Arrow",
            setOne: "Bow",
            supports: ["Multishot I"]);

        Assert.Equal(65m, result.DamageIndex);
        Assert.Equal(80m, result.SpeedIndex);
        Assert.Equal(3, result.ProjectileCount);
        Assert.Single(result.AppliedSupports);
    }

    [Fact]
    public void CurrentSupportRules_RejectIncompatibleSupport()
    {
        var result = Simulate(
            skill: "Lightning Arrow",
            setOne: "Bow",
            supports: ["Controlled Destruction"]);

        Assert.Empty(result.AppliedSupports);
        Assert.Single(result.RejectedSupports);
        Assert.Contains("Hit Spell", result.RejectedSupports[0].Reason);
    }

    [Fact]
    public void ElementalFocus_DisablesAilmentsAndBoostsElementalHit()
    {
        var result = Simulate(
            skill: "Spark",
            setOne: "Empty / spell set",
            supports: ["Elemental Focus"]);

        Assert.Equal(125m, result.DamageIndex);
        Assert.False(result.CanInflictElementalAilments);
    }

    [Fact]
    public void ControlledDestruction_DisablesCritForSpell()
    {
        var result = Simulate(
            skill: "Spark",
            setOne: "Empty / spell set",
            supports: ["Controlled Destruction"]);

        Assert.Equal(125m, result.DamageIndex);
        Assert.False(result.CanCriticalHit);
    }

    [Fact]
    public void SupportSocketPlan_RejectsSelectionsPastLimit()
    {
        var simulator = new SkillSimulator();

        var result = simulator.Simulate(
            "Sorceress",
            "Stormweaver",
            "Spark",
            "Empty / spell set",
            "Empty / spell set",
            2,
            "Baseline",
            "Baseline",
            ["Rapid Casting I", "Controlled Destruction", "Elemental Focus"]);

        Assert.Equal(2, result.AppliedSupports.Count);
        Assert.Single(result.RejectedSupports);
        Assert.Contains("No free support socket", result.RejectedSupports[0].Reason);
    }

    [Fact]
    public void AutoSwap_UsesSecondSetGearProfile()
    {
        var simulator = new SkillSimulator();

        var result = simulator.Simulate(
            "Monk",
            "Invoker",
            "Lightning Arrow",
            "Quarterstaff",
            "Bow",
            3,
            "Heavy weapon",
            "Swift weapon",
            []);

        Assert.Equal("Weapon Set 2", result.Build.ActiveWeaponSet);
        Assert.Equal("Swift weapon", result.ActiveGearProfile.Name);
        Assert.Equal(90m, result.DamageIndex);
        Assert.Equal(120m, result.SpeedIndex);
    }

    [Fact]
    public void SceptreAndSpiritProfile_ExposeSpiritOnActiveSet()
    {
        var result = Simulate(
            skill: "Fulmination",
            setOne: "Wrath Sceptre",
            setOneProfile: "Spirit focus");

        Assert.True(result.Build.IsSkillUsable);
        Assert.Equal(150, result.SpiritAvailable);
    }

    [Fact]
    public void AreaSupports_ChangeAreaDamageAndCostProfile()
    {
        var result = Simulate(
            skill: "Resonating Shield",
            setOne: "Armoured Shield",
            supports: ["Magnified Area I", "Concentrated Area"]);

        Assert.Equal(130m, result.DamageIndex);
        Assert.Equal(67.5m, result.AreaIndex);
        Assert.Equal(130m, result.CostIndex);
    }

    private static SkillSimulationResult Simulate(
        string skill,
        string setOne,
        string setOneProfile = "Baseline",
        string[]? supports = null)
    {
        var simulator = new SkillSimulator();

        return simulator.Simulate(
            "Sorceress",
            "Stormweaver",
            skill,
            setOne,
            "Empty / spell set",
            5,
            setOneProfile,
            "Baseline",
            supports ?? []);
    }
}
