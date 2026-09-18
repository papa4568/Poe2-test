namespace AncientValueOverlay;

public sealed record AscendancyDefinition(string Name, string Theme);

public sealed record ClassDefinition(
    string Name,
    string Attributes,
    string Identity,
    IReadOnlyList<AscendancyDefinition> Ascendancies,
    IReadOnlyList<string> SuggestedWeapons,
    string StarterSkill);

public sealed record SkillDefinition(
    string Name,
    string RequiredWeapon,
    bool GrantedByGearOnly,
    string Tags,
    string Summary);

public sealed record GearDefinition(
    string Name,
    string WeaponType,
    string? GrantedSkill,
    string Effect);

public sealed record BuildEvaluation(
    ClassDefinition Class,
    AscendancyDefinition Ascendancy,
    SkillDefinition Skill,
    GearDefinition WeaponSetOne,
    GearDefinition WeaponSetTwo,
    bool IsSkillUsable,
    string ActiveWeaponSet,
    int SupportSlots,
    string Summary,
    IReadOnlyList<string> GearNotes);

public static class BuildCatalog
{
    public static IReadOnlyList<ClassDefinition> Classes { get; } =
    [
        new(
            "Warrior",
            "STR",
            "Armoured melee foundation with slams, shields, warcries and heavy physical hits.",
            [
                new("Warbringer", "Warcries, ancestral pressure, shields and totems."),
                new("Titan", "Heavy slams, stun pressure and raw physical scaling."),
                new("Smith of Kitava", "Weapon-forging, fire melee and martial-weapon interactions.")
            ],
            ["Melee Martial Weapon", "Armoured Shield"],
            "Resonating Shield"),
        new(
            "Sorceress",
            "INT",
            "Elemental spellcaster that leans into lightning, cold, fire, mana and trigger setups.",
            [
                new("Stormweaver", "Elemental storms and spell scaling."),
                new("Chronomancer", "Cooldown, time and cast-tempo manipulation."),
                new("Disciple of Varashta", "Djinn-backed elemental and minion play.")
            ],
            ["Chiming Staff", "Attuned Wand", "Wrath Sceptre"],
            "Spark"),
        new(
            "Witch",
            "INT",
            "Dark caster and summoner foundation for chaos, minions, blood magic and infernal themes.",
            [
                new("Infernalist", "Infernal pacts, demonform and companion play."),
                new("Blood Mage", "Life-fuelled spellcasting and critical scaling."),
                new("Lich", "Chaos, curses, undeath and necromantic power.")
            ],
            ["Attuned Wand", "Wrath Sceptre"],
            "Essence Drain"),
        new(
            "Ranger",
            "DEX",
            "Fast projectile and ailment specialist with strong bow and poison foundations.",
            [
                new("Deadeye", "Projectiles, ranged pressure and speed."),
                new("Pathfinder", "Flasks, poison, concoctions and sustain.")
            ],
            ["Bow"],
            "Lightning Arrow"),
        new(
            "Monk",
            "DEX / INT",
            "Combo-driven martial caster mixing quarterstaff attacks, elemental strikes and chaos.",
            [
                new("Invoker", "Elemental martial arts and spirit-powered attacks."),
                new("Acolyte of Chayula", "Chaos, darkness and Breach-flavoured melee."),
                new("Martial Artist", "Fast combo attacks, bells and unarmed-style martial play.")
            ],
            ["Quarterstaff", "Melee Martial Weapon"],
            "Tempest Flurry"),
        new(
            "Mercenary",
            "STR / DEX",
            "Crossbow soldier built around ammunition, grenades, tactical utility and flexible gems.",
            [
                new("Witchhunter", "Anti-magic pressure, burst and weapon-set specialisation."),
                new("Gemling Legionnaire", "Gem, attribute and support-system flexibility."),
                new("Tactician", "Control, utility and battlefield setup.")
            ],
            ["Crossbow"],
            "Galvanic Shards"),
        new(
            "Huntress",
            "DEX",
            "Mobile spear fighter with parry, elemental spear attacks, crit and ritual themes.",
            [
                new("Amazon", "Accuracy, critical strikes and elemental spear play."),
                new("Ritualist", "Ritual sacrifice, chaos and sustain."),
                new("Spirit Walker", "Animal spirits, movement and companion-style utility.")
            ],
            ["Spear"],
            "Disengage"),
        new(
            "Druid",
            "STR / INT",
            "Primal hybrid that combines shapeshifting, talismans, elemental spells and beast themes.",
            [
                new("Oracle", "Elemental prophecy and spell-focused primal magic."),
                new("Shaman", "Shapeshifting, storms and primal elemental power.")
            ],
            ["Talisman", "Chiming Staff"],
            "Shapeshift Basic Attack")
    ];

    public static IReadOnlyList<SkillDefinition> Skills { get; } =
    [
        new("Spark", "Any", false, "Spell · Projectile · Lightning", "A class-agnostic spell example: class choice changes scaling options, but the spell itself does not require a weapon."),
        new("Essence Drain", "Any", false, "Spell · Projectile · Chaos", "A class-agnostic chaos spell example for caster and damage-over-time planning."),
        new("Lightning Arrow", "Bow", false, "Attack · Projectile · Lightning", "Requires a Bow and converts much of its physical attack damage into lightning."),
        new("Galvanic Shards", "Crossbow", false, "Attack · Ammunition · Projectile · Lightning", "Requires a Crossbow and uses ammunition/reload behaviour."),
        new("Tempest Flurry", "Quarterstaff", false, "Attack · Melee · Strike · Lightning", "Requires a Quarterstaff and rewards repeated combo attacks."),
        new("Disengage", "Spear", false, "Attack · Melee · Travel", "Requires a Spear and combines movement with an attack."),
        new("Resonating Shield", "Armoured Shield", false, "Attack · AoE · Channelling", "Requires an Armoured Shield and directly scales part of its damage from shield armour."),
        new("Temper Weapon", "Melee Martial Weapon", false, "Attack · Buff · Fire · Channelling", "Requires a non-talisman melee martial weapon and empowers later melee attacks."),
        new("Mana Drain", "Granted by gear", true, "Spell · Inherent", "An inherent weapon skill example granted by an Attuned Wand."),
        new("Sigil of Power", "Granted by gear", true, "Spell · AoE · Inherent", "An inherent weapon skill example granted by a Chiming Staff."),
        new("Consecrate", "Granted by gear", true, "Spell · AoE · Duration · Inherent", "An inherent weapon skill example granted by a Sanctified Staff."),
        new("Fulmination", "Granted by gear", true, "Buff · Persistent · Aura · Lightning", "An inherent weapon skill example granted by a Wrath Sceptre."),
        new("Shapeshift Basic Attack", "Granted by gear", true, "Attack · Shapeshift · Inherent", "A talisman-style inherent attack: equipping the weapon changes form and grants its basic attack.")
    ];

    public static IReadOnlyList<GearDefinition> Gear { get; } =
    [
        new("Empty / spell set", "Any", null, "No weapon gate. Useful for seeing that many spells are not class-locked."),
        new("Bow", "Bow", null, "Enables Bow attacks such as Lightning Arrow."),
        new("Crossbow", "Crossbow", null, "Enables Crossbow ammunition skills such as Galvanic Shards."),
        new("Quarterstaff", "Quarterstaff", null, "Enables Quarterstaff combo skills such as Tempest Flurry."),
        new("Spear", "Spear", null, "Enables Spear attacks such as Disengage."),
        new("Armoured Shield", "Armoured Shield", null, "Enables shield-specific attacks such as Resonating Shield."),
        new("Melee Martial Weapon", "Melee Martial Weapon", null, "Generic melee martial set for skills such as Temper Weapon."),
        new("Talisman", "Talisman", "Shapeshift Basic Attack", "Equipping a talisman shifts the character into an animal form and grants a form-specific basic attack."),
        new("Attuned Wand", "Wand", "Mana Drain", "Grants the inherent Mana Drain skill."),
        new("Chiming Staff", "Staff", "Sigil of Power", "Grants the inherent Sigil of Power skill."),
        new("Sanctified Staff", "Staff", "Consecrate", "Grants the inherent Consecrate skill."),
        new("Wrath Sceptre", "Sceptre", "Fulmination", "Grants Fulmination and represents how sceptres can also reshape Spirit/reservation planning.")
    ];

    public static ClassDefinition FindClass(string name) =>
        Classes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? Classes[0];

    public static SkillDefinition FindSkill(string name) =>
        Skills.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? Skills[0];

    public static GearDefinition FindGear(string name) =>
        Gear.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? Gear[0];
}

public sealed class BuildPlanner
{
    public BuildEvaluation Evaluate(
        string className,
        string ascendancyName,
        string skillName,
        string weaponSetOneName,
        string weaponSetTwoName,
        int supportSlots)
    {
        var classDefinition = BuildCatalog.FindClass(className);
        var ascendancy = classDefinition.Ascendancies
            .FirstOrDefault(x => x.Name.Equals(ascendancyName, StringComparison.OrdinalIgnoreCase))
            ?? classDefinition.Ascendancies[0];

        var skill = BuildCatalog.FindSkill(skillName);
        var setOne = BuildCatalog.FindGear(weaponSetOneName);
        var setTwo = BuildCatalog.FindGear(weaponSetTwoName);
        var clampedSupportSlots = Math.Clamp(supportSlots, 2, 5);

        var setOneMatches = CanUseSkill(skill, setOne);
        var setTwoMatches = CanUseSkill(skill, setTwo);

        var activeSet = setOneMatches
            ? "Weapon Set 1"
            : setTwoMatches
                ? "Weapon Set 2"
                : "Blocked by current gear";

        var usable = setOneMatches || setTwoMatches;
        var gearNotes = BuildGearNotes(setOne, setTwo);

        var summary = usable
            ? $"{classDefinition.Name} → {ascendancy.Name} can use {skill.Name} with {activeSet}. " +
              $"Planned supports: {clampedSupportSlots}/5. Class identity suggests {classDefinition.Identity}"
            : $"{skill.Name} is not usable with the selected weapon sets. It requires {skill.RequiredWeapon}. " +
              "The class itself is not the blocker; change the equipped weapon or choose a compatible skill.";

        return new BuildEvaluation(
            classDefinition,
            ascendancy,
            skill,
            setOne,
            setTwo,
            usable,
            activeSet,
            clampedSupportSlots,
            summary,
            gearNotes);
    }

    public (string Skill, string WeaponSetOne, string WeaponSetTwo) StarterFor(string className)
    {
        var classDefinition = BuildCatalog.FindClass(className);
        var preferred = classDefinition.SuggestedWeapons.FirstOrDefault() ?? "Empty / spell set";
        var gear = BuildCatalog.Gear.FirstOrDefault(x =>
                       x.Name.Equals(preferred, StringComparison.OrdinalIgnoreCase))
                   ?? BuildCatalog.Gear.FirstOrDefault(x =>
                       x.WeaponType.Equals(preferred, StringComparison.OrdinalIgnoreCase))
                   ?? BuildCatalog.Gear[0];

        return (classDefinition.StarterSkill, gear.Name, "Empty / spell set");
    }

    private static bool CanUseSkill(SkillDefinition skill, GearDefinition gear)
    {
        if (skill.GrantedByGearOnly)
        {
            return string.Equals(gear.GrantedSkill, skill.Name, StringComparison.OrdinalIgnoreCase);
        }

        if (skill.RequiredWeapon.Equals("Any", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (skill.RequiredWeapon.Equals("Melee Martial Weapon", StringComparison.OrdinalIgnoreCase))
        {
            return gear.WeaponType is "Melee Martial Weapon" or "Quarterstaff" or "Spear";
        }

        return skill.RequiredWeapon.Equals(gear.WeaponType, StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<string> BuildGearNotes(GearDefinition first, GearDefinition second)
    {
        var notes = new List<string>
        {
            $"Set 1 · {first.Name}: {first.Effect}",
            $"Set 2 · {second.Name}: {second.Effect}",
            "Supports belong to the skill rather than normal equipment sockets; plan up to five support sockets per skill."
        };

        var grantedSkills = new[] { first.GrantedSkill, second.GrantedSkill }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (grantedSkills.Length > 0)
        {
            notes.Add($"Gear-granted skills: {string.Join(", ", grantedSkills)}.");
        }

        if (!first.Name.Equals(second.Name, StringComparison.OrdinalIgnoreCase))
        {
            notes.Add("Different weapon sets can support different skill requirements and specialised passive paths.");
        }

        return notes;
    }
}
