namespace AncientValueOverlay;

public sealed record GearRollProfile(
    string Name,
    decimal WeaponDamageIndex,
    decimal AttackSpeedIndex,
    decimal CastSpeedIndex,
    int ExtraSpirit,
    string Note);

public sealed record SupportDefinition(
    string Name,
    string Category,
    string Requirement,
    string Effect,
    decimal DamageMultiplier = 1m,
    decimal SpeedMultiplier = 1m,
    int AdditionalProjectiles = 0,
    decimal AreaMultiplier = 1m,
    decimal CostMultiplier = 1m,
    bool DisablesCriticalHits = false,
    bool DisablesElementalAilments = false);

public sealed record AppliedSupport(string Name, string Effect);

public sealed record RejectedSupport(string Name, string Reason);

public sealed record SkillSimulationResult(
    BuildEvaluation Build,
    GearRollProfile ActiveGearProfile,
    decimal DamageIndex,
    decimal SpeedIndex,
    int ProjectileCount,
    decimal AreaIndex,
    decimal CostIndex,
    int SpiritAvailable,
    bool CanCriticalHit,
    bool CanInflictElementalAilments,
    IReadOnlyList<AppliedSupport> AppliedSupports,
    IReadOnlyList<RejectedSupport> RejectedSupports,
    IReadOnlyList<string> Notes);

public static class SkillSimulationCatalog
{
    public static IReadOnlyList<GearRollProfile> GearProfiles { get; } =
    [
        new("Baseline", 100m, 100m, 100m, 0, "Neutral normalized gear profile."),
        new("Heavy weapon", 125m, 90m, 100m, 0, "Illustrative higher weapon-damage / slower-attack profile."),
        new("Swift weapon", 90m, 120m, 100m, 0, "Illustrative lower weapon-damage / faster-attack profile."),
        new("Caster focus", 100m, 100m, 115m, 0, "Illustrative cast-speed focused caster profile."),
        new("Spirit focus", 100m, 100m, 100m, 50, "Illustrative utility profile adding Spirit."),
        new("Hybrid utility", 105m, 105m, 105m, 25, "Illustrative balanced offensive + utility profile.")
    ];

    public static IReadOnlyList<SupportDefinition> Supports { get; } =
    [
        new(
            "Rapid Attacks I",
            "Rapid Attacks",
            "Attack",
            "15% increased Attack Speed.",
            SpeedMultiplier: 1.15m),
        new(
            "Rapid Casting I",
            "Rapid Casting",
            "Spell",
            "15% increased Cast Speed.",
            SpeedMultiplier: 1.15m),
        new(
            "Multishot I",
            "Additional Projectiles",
            "Projectile",
            "35% less Damage, 2 additional Projectiles, 20% less Skill Speed.",
            DamageMultiplier: 0.65m,
            SpeedMultiplier: 0.80m,
            AdditionalProjectiles: 2),
        new(
            "Magnified Area I",
            "Increased Area of Effect",
            "AoE",
            "35% increased Area of Effect with a 130% cost multiplier.",
            AreaMultiplier: 1.35m,
            CostMultiplier: 1.30m),
        new(
            "Concentrated Area",
            "Concentrated Area",
            "AoE",
            "50% less Area of Effect and 30% more Area Damage.",
            DamageMultiplier: 1.30m,
            AreaMultiplier: 0.50m),
        new(
            "Elemental Armament I",
            "Elemental Armament",
            "Elemental Attack",
            "20% more Elemental Attack Damage.",
            DamageMultiplier: 1.20m),
        new(
            "Controlled Destruction",
            "Controlled Destruction",
            "Hit Spell",
            "25% more Spell Hit Damage; supported Skill cannot deal Critical Hits.",
            DamageMultiplier: 1.25m,
            DisablesCriticalHits: true),
        new(
            "Elemental Focus",
            "Elemental Focus",
            "Elemental Hit",
            "25% more Elemental Damage; supported Skill cannot inflict Elemental Ailments.",
            DamageMultiplier: 1.25m,
            DisablesElementalAilments: true)
    ];

    public static GearRollProfile FindGearProfile(string name) =>
        GearProfiles.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? GearProfiles[0];

    public static SupportDefinition FindSupport(string name) =>
        Supports.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? Supports[0];
}

public sealed class SkillSimulator
{
    private readonly BuildPlanner _planner = new();

    public SkillSimulationResult Simulate(
        string className,
        string ascendancyName,
        string skillName,
        string weaponSetOneName,
        string weaponSetTwoName,
        int supportSlots,
        string weaponSetOneProfileName,
        string weaponSetTwoProfileName,
        IEnumerable<string> selectedSupportNames)
    {
        var build = _planner.Evaluate(
            className,
            ascendancyName,
            skillName,
            weaponSetOneName,
            weaponSetTwoName,
            supportSlots);

        var setOneProfile = SkillSimulationCatalog.FindGearProfile(weaponSetOneProfileName);
        var setTwoProfile = SkillSimulationCatalog.FindGearProfile(weaponSetTwoProfileName);
        var activeProfile = build.ActiveWeaponSet == "Weapon Set 2" ? setTwoProfile : setOneProfile;
        var activeGear = build.ActiveWeaponSet == "Weapon Set 2" ? build.WeaponSetTwo : build.WeaponSetOne;

        var damageIndex = 100m;
        var speedIndex = 100m;
        var projectileCount = HasTag(build.Skill, "Projectile") ? 1 : 0;
        var areaIndex = HasTag(build.Skill, "AoE") ? 100m : 0m;
        var costIndex = 100m;
        var canCrit = true;
        var canInflictElementalAilments = IsElemental(build.Skill);

        if (HasTag(build.Skill, "Attack"))
        {
            damageIndex *= activeProfile.WeaponDamageIndex / 100m;
            speedIndex *= activeProfile.AttackSpeedIndex / 100m;
        }
        else if (HasTag(build.Skill, "Spell"))
        {
            speedIndex *= activeProfile.CastSpeedIndex / 100m;
        }

        if (!build.IsSkillUsable)
        {
            damageIndex = 0m;
            speedIndex = 0m;
        }

        var spirit = Math.Max(0, activeGear.Spirit + activeProfile.ExtraSpirit);
        var applied = new List<AppliedSupport>();
        var rejected = new List<RejectedSupport>();
        var usedCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var supportName in selectedSupportNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var support = SkillSimulationCatalog.FindSupport(supportName);

            if (applied.Count >= build.SupportSlots)
            {
                rejected.Add(new RejectedSupport(
                    support.Name,
                    $"No free support socket ({build.SupportSlots}/5 available in this plan)."));
                continue;
            }

            if (!usedCategories.Add(support.Category))
            {
                rejected.Add(new RejectedSupport(
                    support.Name,
                    $"Another support from category '{support.Category}' is already applied."));
                continue;
            }

            if (!SupportsSkill(support, build.Skill))
            {
                rejected.Add(new RejectedSupport(
                    support.Name,
                    $"Requires {support.Requirement}; {build.Skill.Name} has tags {build.Skill.Tags}."));
                continue;
            }

            damageIndex *= support.DamageMultiplier;
            speedIndex *= support.SpeedMultiplier;
            projectileCount += support.AdditionalProjectiles;

            if (areaIndex > 0m)
            {
                areaIndex *= support.AreaMultiplier;
            }

            costIndex *= support.CostMultiplier;
            canCrit &= !support.DisablesCriticalHits;
            canInflictElementalAilments &= !support.DisablesElementalAilments;
            applied.Add(new AppliedSupport(support.Name, support.Effect));
        }

        var notes = BuildNotes(build, activeGear, activeProfile, spirit);

        return new SkillSimulationResult(
            build,
            activeProfile,
            Math.Round(damageIndex, 1),
            Math.Round(speedIndex, 1),
            projectileCount,
            Math.Round(areaIndex, 1),
            Math.Round(costIndex, 1),
            spirit,
            canCrit,
            canInflictElementalAilments,
            applied,
            rejected,
            notes);
    }

    private static bool SupportsSkill(SupportDefinition support, SkillDefinition skill) =>
        support.Requirement switch
        {
            "Attack" => HasTag(skill, "Attack"),
            "Spell" => HasTag(skill, "Spell") && !HasTag(skill, "Persistent"),
            "Projectile" => HasTag(skill, "Projectile"),
            "AoE" => HasTag(skill, "AoE"),
            "Elemental Attack" => HasTag(skill, "Attack") && IsElemental(skill),
            "Hit Spell" => HasTag(skill, "Spell") && !HasTag(skill, "Persistent"),
            "Elemental Hit" => (HasTag(skill, "Attack") || HasTag(skill, "Spell")) &&
                               !HasTag(skill, "Persistent") &&
                               IsElemental(skill),
            _ => false
        };

    private static bool HasTag(SkillDefinition skill, string tag) =>
        skill.Tags
            .Split('·', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(x => x.Equals(tag, StringComparison.OrdinalIgnoreCase));

    private static bool IsElemental(SkillDefinition skill) =>
        HasTag(skill, "Fire") || HasTag(skill, "Cold") || HasTag(skill, "Lightning");

    private static IReadOnlyList<string> BuildNotes(
        BuildEvaluation build,
        GearDefinition activeGear,
        GearRollProfile activeProfile,
        int spirit)
    {
        var notes = new List<string>
        {
            "Damage, speed, area and cost are normalized comparison indexes, not an exact in-game DPS calculation.",
            $"{build.ActiveWeaponSet}: {activeGear.Name} with '{activeProfile.Name}' profile.",
            activeProfile.Note
        };

        if (HasTag(build.Skill, "Attack"))
        {
            notes.Add("Attack profile uses the active weapon-set damage and attack-speed indexes.");
        }

        if (HasTag(build.Skill, "Spell"))
        {
            notes.Add("Spell profile ignores weapon-damage index and uses the active set's cast-speed index.");
        }

        if (activeGear.Spirit > 0 || activeProfile.ExtraSpirit > 0)
        {
            notes.Add($"Active set exposes {spirit} Spirit in this model.");
        }

        if (build.ActiveWeaponSet == "Weapon Set 2")
        {
            notes.Add("The simulator auto-swapped to Weapon Set 2 because Set 1 could not use the selected skill.");
        }

        return notes;
    }
}
