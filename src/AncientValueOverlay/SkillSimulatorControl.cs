using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AncientValueOverlay;

public sealed class SkillSimulatorControl : UserControl
{
    private static readonly Color Background = Color.FromArgb(15, 17, 21);
    private static readonly Color Surface = Color.FromArgb(25, 29, 35);
    private static readonly Color SurfaceAlt = Color.FromArgb(32, 37, 45);
    private static readonly Color Border = Color.FromArgb(53, 60, 70);
    private static readonly Color Primary = Color.FromArgb(94, 129, 244);
    private static readonly Color Success = Color.FromArgb(103, 211, 149);
    private static readonly Color Muted = Color.FromArgb(169, 177, 190);
    private static readonly Color Warning = Color.FromArgb(245, 189, 93);

    private readonly BuildPlanner _planner = new();
    private readonly SkillSimulator _simulator = new();

    private readonly ComboBox _classCombo = CreateComboBox();
    private readonly ComboBox _ascendancyCombo = CreateComboBox();
    private readonly ComboBox _skillCombo = CreateComboBox();
    private readonly ComboBox _weaponSetOneCombo = CreateComboBox();
    private readonly ComboBox _weaponSetOneProfileCombo = CreateComboBox();
    private readonly ComboBox _weaponSetTwoCombo = CreateComboBox();
    private readonly ComboBox _weaponSetTwoProfileCombo = CreateComboBox();
    private readonly NumericUpDown _supportSlots = new();
    private readonly CheckedListBox _supportList = new();

    private readonly Label _statusLabel = new();
    private readonly Label _activeSetValue = new();
    private readonly Label _damageValue = new();
    private readonly Label _speedValue = new();
    private readonly Label _projectilesValue = new();
    private readonly Label _areaValue = new();
    private readonly Label _costValue = new();
    private readonly Label _spiritValue = new();
    private readonly RichTextBox _readout = new();

    private string _latestSummary = string.Empty;

    public SkillSimulatorControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Background;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        Padding = new Padding(4);

        Controls.Add(BuildLayout());
        PopulateCatalog();
        WireEvents();

        _classCombo.SelectedIndex = 0;
        LoadStarter();
    }

    private Control BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Background,
            Margin = Padding.Empty
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));

        root.Controls.Add(BuildInputPanel(), 0, 0);
        root.Controls.Add(BuildOutputPanel(), 1, 0);
        return root;
    }

    private Control BuildInputPanel()
    {
        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(18),
            Margin = new Padding(0, 0, 10, 0)
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));

        var heading = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        heading.Controls.Add(new Label
        {
            Text = "Skill Change Simulator",
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            Location = new Point(0, 0)
        });
        heading.Controls.Add(new Label
        {
            Text = "See how weapon sets, gear rolls and supports reshape a skill",
            AutoSize = true,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 9F),
            Location = new Point(2, 33)
        });

        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Surface,
            Margin = Padding.Empty
        };

        var fields = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            ColumnCount = 1,
            RowCount = 9,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        fields.Controls.Add(CreateField("CLASS", _classCombo), 0, 0);
        fields.Controls.Add(CreateField("ASCENDANCY", _ascendancyCombo), 0, 1);
        fields.Controls.Add(CreateField("SKILL", _skillCombo), 0, 2);
        fields.Controls.Add(CreateField("WEAPON SET 1", _weaponSetOneCombo), 0, 3);
        fields.Controls.Add(CreateField("SET 1 GEAR PROFILE", _weaponSetOneProfileCombo), 0, 4);
        fields.Controls.Add(CreateField("WEAPON SET 2", _weaponSetTwoCombo), 0, 5);
        fields.Controls.Add(CreateField("SET 2 GEAR PROFILE", _weaponSetTwoProfileCombo), 0, 6);

        _supportSlots.Minimum = 2;
        _supportSlots.Maximum = 5;
        _supportSlots.Value = 3;
        _supportSlots.BackColor = SurfaceAlt;
        _supportSlots.ForeColor = Color.White;
        _supportSlots.BorderStyle = BorderStyle.FixedSingle;
        _supportSlots.Font = new Font("Segoe UI", 10F);
        fields.Controls.Add(CreateField("AVAILABLE SUPPORT SOCKETS", _supportSlots), 0, 7);

        _supportList.Height = 170;
        _supportList.CheckOnClick = true;
        _supportList.BackColor = SurfaceAlt;
        _supportList.ForeColor = Color.White;
        _supportList.BorderStyle = BorderStyle.FixedSingle;
        _supportList.Font = new Font("Segoe UI", 9.5F);
        _supportList.IntegralHeight = false;
        fields.Controls.Add(CreateField("SUPPORTS · overflow/incompatible choices are explained", _supportList, 205), 0, 8);

        scroll.Controls.Add(fields);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0)
        };

        var starter = CreateButton("Load starter", true, 115);
        starter.Click += (_, _) => LoadStarter();

        var swap = CreateButton("Swap sets", false, 100);
        swap.Click += (_, _) => SwapSets();

        var clear = CreateButton("Clear supports", false, 120);
        clear.Click += (_, _) => ClearSupports();

        var copy = CreateButton("Copy sim", false, 95);
        copy.Click += (_, _) => CopySimulation();

        actions.Controls.Add(starter);
        actions.Controls.Add(swap);
        actions.Controls.Add(clear);
        actions.Controls.Add(copy);

        shell.Controls.Add(heading, 0, 0);
        shell.Controls.Add(scroll, 0, 1);
        shell.Controls.Add(actions, 0, 2);
        return shell;
    }

    private Control BuildOutputPanel()
    {
        var output = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty
        };
        output.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
        output.RowStyles.Add(new RowStyle(SizeType.Absolute, 176F));
        output.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        output.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 0, 10)
        };

        var title = new Label
        {
            Text = "NORMALIZED SKILL PROFILE",
            Dock = DockStyle.Top,
            Height = 22,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
        };

        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;

        header.Controls.Add(_statusLabel);
        header.Controls.Add(title);

        output.Controls.Add(header, 0, 0);
        output.Controls.Add(BuildMetricGrid(), 0, 1);
        output.Controls.Add(BuildDisclaimer(), 0, 2);
        output.Controls.Add(BuildReadoutPanel(), 0, 3);
        return output;
    }

    private Control BuildMetricGrid()
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            ColumnCount = 3,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 10)
        };

        for (var i = 0; i < 3; i++)
        {
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        }

        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        grid.Controls.Add(CreateMetricCard("ACTIVE SET", _activeSetValue), 0, 0);
        grid.Controls.Add(CreateMetricCard("DAMAGE INDEX", _damageValue), 1, 0);
        grid.Controls.Add(CreateMetricCard("SPEED INDEX", _speedValue), 2, 0);
        grid.Controls.Add(CreateMetricCard("PROJECTILES", _projectilesValue), 0, 1);
        grid.Controls.Add(CreateMetricCard("AREA INDEX", _areaValue), 1, 1);
        grid.Controls.Add(CreateMetricCard("SPIRIT", _spiritValue), 2, 1);

        return grid;
    }

    private Control BuildDisclaimer()
    {
        return new Label
        {
            Text = "Indexes compare changes from a 100 baseline. They are not exact character DPS or a replacement for full game-data calculation.",
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ForeColor = Warning,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 12, 0),
            Margin = new Padding(0, 0, 0, 10)
        };
    }

    private Control BuildReadoutPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(16),
            Margin = Padding.Empty
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        panel.Controls.Add(new Label
        {
            Text = "WHAT CHANGED",
            Dock = DockStyle.Fill,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        _readout.Dock = DockStyle.Fill;
        _readout.ReadOnly = true;
        _readout.BorderStyle = BorderStyle.None;
        _readout.BackColor = SurfaceAlt;
        _readout.ForeColor = Color.FromArgb(229, 232, 237);
        _readout.Font = new Font("Cascadia Mono", 9.25F);
        _readout.DetectUrls = false;
        _readout.WordWrap = true;
        _readout.Margin = new Padding(0, 4, 0, 0);
        panel.Controls.Add(_readout, 0, 1);

        return panel;
    }

    private void PopulateCatalog()
    {
        _classCombo.Items.AddRange(BuildCatalog.Classes.Select(x => x.Name).Cast<object>().ToArray());
        _skillCombo.Items.AddRange(BuildCatalog.Skills.Select(x => x.Name).Cast<object>().ToArray());
        _weaponSetOneCombo.Items.AddRange(BuildCatalog.Gear.Select(x => x.Name).Cast<object>().ToArray());
        _weaponSetTwoCombo.Items.AddRange(BuildCatalog.Gear.Select(x => x.Name).Cast<object>().ToArray());

        var profiles = SkillSimulationCatalog.GearProfiles.Select(x => x.Name).Cast<object>().ToArray();
        _weaponSetOneProfileCombo.Items.AddRange(profiles);
        _weaponSetTwoProfileCombo.Items.AddRange(profiles);

        _supportList.Items.AddRange(
            SkillSimulationCatalog.Supports
                .Select(x => $"{x.Name} — {x.Requirement}")
                .Cast<object>()
                .ToArray());
    }

    private void WireEvents()
    {
        _classCombo.SelectedIndexChanged += (_, _) =>
        {
            PopulateAscendancies();
            RefreshSimulation();
        };

        _ascendancyCombo.SelectedIndexChanged += (_, _) => RefreshSimulation();
        _skillCombo.SelectedIndexChanged += (_, _) => RefreshSimulation();
        _weaponSetOneCombo.SelectedIndexChanged += (_, _) => RefreshSimulation();
        _weaponSetTwoCombo.SelectedIndexChanged += (_, _) => RefreshSimulation();
        _weaponSetOneProfileCombo.SelectedIndexChanged += (_, _) => RefreshSimulation();
        _weaponSetTwoProfileCombo.SelectedIndexChanged += (_, _) => RefreshSimulation();
        _supportSlots.ValueChanged += (_, _) => RefreshSimulation();
        _supportList.ItemCheck += (_, _) => BeginInvoke(new Action(RefreshSimulation));
    }

    private void PopulateAscendancies()
    {
        if (_classCombo.SelectedIndex < 0)
        {
            return;
        }

        var classDefinition = BuildCatalog.FindClass(Selected(_classCombo));
        var previous = Selected(_ascendancyCombo);

        _ascendancyCombo.BeginUpdate();
        _ascendancyCombo.Items.Clear();
        _ascendancyCombo.Items.AddRange(classDefinition.Ascendancies.Select(x => x.Name).Cast<object>().ToArray());
        _ascendancyCombo.EndUpdate();

        var previousIndex = _ascendancyCombo.Items.IndexOf(previous);
        _ascendancyCombo.SelectedIndex = previousIndex >= 0 ? previousIndex : 0;
    }

    private void LoadStarter()
    {
        if (_classCombo.SelectedIndex < 0)
        {
            return;
        }

        var starter = _planner.StarterFor(Selected(_classCombo));
        SelectItem(_skillCombo, starter.Skill);
        SelectItem(_weaponSetOneCombo, starter.WeaponSetOne);
        SelectItem(_weaponSetTwoCombo, starter.WeaponSetTwo);
        SelectItem(_weaponSetOneProfileCombo, "Baseline");
        SelectItem(_weaponSetTwoProfileCombo, "Baseline");
        _supportSlots.Value = 3;
        ClearSupports(refresh: false);
        RefreshSimulation();
    }

    private void SwapSets()
    {
        var firstGear = Selected(_weaponSetOneCombo);
        var secondGear = Selected(_weaponSetTwoCombo);
        var firstProfile = Selected(_weaponSetOneProfileCombo);
        var secondProfile = Selected(_weaponSetTwoProfileCombo);

        SelectItem(_weaponSetOneCombo, secondGear);
        SelectItem(_weaponSetTwoCombo, firstGear);
        SelectItem(_weaponSetOneProfileCombo, secondProfile);
        SelectItem(_weaponSetTwoProfileCombo, firstProfile);
        RefreshSimulation();
    }

    private void ClearSupports(bool refresh = true)
    {
        for (var i = 0; i < _supportList.Items.Count; i++)
        {
            _supportList.SetItemChecked(i, false);
        }

        if (refresh)
        {
            RefreshSimulation();
        }
    }

    private void CopySimulation()
    {
        if (string.IsNullOrWhiteSpace(_latestSummary))
        {
            RefreshSimulation();
        }

        try
        {
            Clipboard.SetText(_latestSummary);
            _statusLabel.Text = "COPIED · simulation readout is on the clipboard";
            _statusLabel.ForeColor = Success;
        }
        catch
        {
            _statusLabel.Text = "CLIPBOARD UNAVAILABLE";
            _statusLabel.ForeColor = Warning;
        }
    }

    private void RefreshSimulation()
    {
        if (_classCombo.SelectedIndex < 0 ||
            _ascendancyCombo.SelectedIndex < 0 ||
            _skillCombo.SelectedIndex < 0 ||
            _weaponSetOneCombo.SelectedIndex < 0 ||
            _weaponSetTwoCombo.SelectedIndex < 0 ||
            _weaponSetOneProfileCombo.SelectedIndex < 0 ||
            _weaponSetTwoProfileCombo.SelectedIndex < 0)
        {
            return;
        }

        var supportNames = _supportList.CheckedItems
            .Cast<object>()
            .Select(x => x.ToString() ?? string.Empty)
            .Select(x => x.Split(" — ", StringSplitOptions.TrimEntries)[0])
            .ToArray();

        var result = _simulator.Simulate(
            Selected(_classCombo),
            Selected(_ascendancyCombo),
            Selected(_skillCombo),
            Selected(_weaponSetOneCombo),
            Selected(_weaponSetTwoCombo),
            (int)_supportSlots.Value,
            Selected(_weaponSetOneProfileCombo),
            Selected(_weaponSetTwoProfileCombo),
            supportNames);

        _statusLabel.Text = result.Build.IsSkillUsable
            ? $"READY · {result.Build.Skill.Name} · {result.Build.ActiveWeaponSet}"
            : $"BLOCKED · requires {result.Build.Skill.RequiredWeapon}";
        _statusLabel.ForeColor = result.Build.IsSkillUsable ? Success : Warning;

        _activeSetValue.Text = result.Build.ActiveWeaponSet
            .Replace("Weapon ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("Blocked by current gear", "BLOCKED", StringComparison.OrdinalIgnoreCase);
        _damageValue.Text = result.DamageIndex.ToString("0.#");
        _speedValue.Text = result.SpeedIndex.ToString("0.#");
        _projectilesValue.Text = result.ProjectileCount > 0 ? result.ProjectileCount.ToString() : "—";
        _areaValue.Text = result.AreaIndex > 0 ? result.AreaIndex.ToString("0.#") : "—";
        _costValue.Text = result.CostIndex.ToString("0.#");
        _spiritValue.Text = result.SpiritAvailable.ToString();

        var builder = new StringBuilder();
        builder.AppendLine($"{result.Build.Class.Name} → {result.Build.Ascendancy.Name}");
        builder.AppendLine($"{result.Build.Skill.Name} · {result.Build.Skill.Tags}");
        builder.AppendLine($"Set 1: {result.Build.WeaponSetOne.Name} · {Selected(_weaponSetOneProfileCombo)}");
        builder.AppendLine($"Set 2: {result.Build.WeaponSetTwo.Name} · {Selected(_weaponSetTwoProfileCombo)}");
        builder.AppendLine();
        builder.AppendLine($"Damage {result.DamageIndex:0.#} · Speed {result.SpeedIndex:0.#} · Cost {result.CostIndex:0.#}");
        builder.AppendLine($"Crit: {(result.CanCriticalHit ? "enabled" : "disabled")} · Elemental ailments: {(result.CanInflictElementalAilments ? "enabled" : "disabled")}");

        builder.AppendLine();
        builder.AppendLine($"Applied supports ({result.AppliedSupports.Count}/{result.Build.SupportSlots}):");
        if (result.AppliedSupports.Count == 0)
        {
            builder.AppendLine("• none");
        }
        else
        {
            foreach (var support in result.AppliedSupports)
            {
                builder.AppendLine($"• {support.Name}: {support.Effect}");
            }
        }

        if (result.RejectedSupports.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("Rejected supports:");
            foreach (var support in result.RejectedSupports)
            {
                builder.AppendLine($"• {support.Name}: {support.Reason}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("Model notes:");
        foreach (var note in result.Notes)
        {
            builder.AppendLine($"• {note}");
        }

        _latestSummary = builder.ToString().TrimEnd();
        _readout.Text = _latestSummary;
    }

    private static Control CreateField(string title, Control control, int height = 62)
    {
        var field = new TableLayoutPanel
        {
            Height = height,
            Dock = DockStyle.Top,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0, 0, 0, 8)
        };
        field.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
        field.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        field.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.BottomLeft
        }, 0, 0);

        control.Dock = DockStyle.Fill;
        field.Controls.Add(control, 0, 1);
        return field;
    }

    private static Panel CreateMetricCard(string title, Label value)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            Margin = new Padding(0, 0, 8, 8),
            Padding = new Padding(12)
        };

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 20,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold)
        };

        value.Dock = DockStyle.Fill;
        value.ForeColor = Color.White;
        value.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        value.TextAlign = ContentAlignment.MiddleLeft;

        card.Controls.Add(value);
        card.Controls.Add(titleLabel);
        return card;
    }

    private static ComboBox CreateComboBox()
    {
        return new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = SurfaceAlt,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F),
            IntegralHeight = false,
            DropDownHeight = 260
        };
    }

    private static Button CreateButton(string text, bool primary, int width)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = 38,
            BackColor = primary ? Primary : SurfaceAlt,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, primary ? FontStyle.Bold : FontStyle.Regular),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 6),
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderColor = primary ? Primary : Border;
        button.FlatAppearance.BorderSize = 1;
        return button;
    }

    private static string Selected(ComboBox comboBox) =>
        comboBox.SelectedItem?.ToString() ?? string.Empty;

    private static void SelectItem(ComboBox comboBox, string value)
    {
        var index = comboBox.Items.IndexOf(value);
        if (index >= 0)
        {
            comboBox.SelectedIndex = index;
        }
    }
}
