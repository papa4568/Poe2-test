using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AncientValueOverlay;

public sealed class BuildLabControl : UserControl
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
    private readonly ComboBox _classCombo = CreateComboBox();
    private readonly ComboBox _ascendancyCombo = CreateComboBox();
    private readonly ComboBox _skillCombo = CreateComboBox();
    private readonly ComboBox _weaponSetOneCombo = CreateComboBox();
    private readonly ComboBox _weaponSetTwoCombo = CreateComboBox();
    private readonly NumericUpDown _supportSlots = new();
    private readonly Label _classIdentityLabel = new();
    private readonly Label _ascendancyThemeLabel = new();
    private readonly Label _skillInfoLabel = new();
    private readonly Label _compatibilityLabel = new();
    private readonly RichTextBox _resultBox = new();
    private string _latestBuildSummary = string.Empty;

    public BuildLabControl()
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

        root.Controls.Add(BuildSelectorPanel(), 0, 0);
        root.Controls.Add(BuildInsightPanel(), 1, 0);

        return root;
    }

    private Control BuildSelectorPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(18),
            Margin = new Padding(0, 0, 10, 0)
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));

        var heading = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        heading.Controls.Add(new Label
        {
            Text = "Build Lab",
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 17F, FontStyle.Bold),
            Location = new Point(0, 0)
        });
        heading.Controls.Add(new Label
        {
            Text = "Class identity + skill requirements + gear interaction",
            AutoSize = true,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 9F),
            Location = new Point(2, 34)
        });

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Margin = new Padding(0, 8, 0, 8)
        };

        for (var row = 0; row < 6; row++)
        {
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6667F));
        }

        fields.Controls.Add(CreateField("CLASS", _classCombo), 0, 0);
        fields.Controls.Add(CreateField("ASCENDANCY", _ascendancyCombo), 0, 1);
        fields.Controls.Add(CreateField("PRIMARY SKILL", _skillCombo), 0, 2);
        fields.Controls.Add(CreateField("WEAPON SET 1", _weaponSetOneCombo), 0, 3);
        fields.Controls.Add(CreateField("WEAPON SET 2", _weaponSetTwoCombo), 0, 4);

        _supportSlots.Minimum = 2;
        _supportSlots.Maximum = 5;
        _supportSlots.Value = 2;
        _supportSlots.Dock = DockStyle.Fill;
        _supportSlots.BackColor = SurfaceAlt;
        _supportSlots.ForeColor = Color.White;
        _supportSlots.BorderStyle = BorderStyle.FixedSingle;
        _supportSlots.Font = new Font("Segoe UI", 10F);
        fields.Controls.Add(CreateField("PLANNED SUPPORT SOCKETS", _supportSlots), 0, 5);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 6, 0, 0),
            Margin = Padding.Empty
        };

        var starterButton = CreateButton("Load class starter", true, 150);
        starterButton.Click += (_, _) => LoadStarter();

        var evaluateButton = CreateButton("Evaluate", false, 100);
        evaluateButton.Click += (_, _) => RefreshEvaluation();

        var swapButton = CreateButton("Swap sets", false, 100);
        swapButton.Click += (_, _) => SwapWeaponSets();

        var copyButton = CreateButton("Copy build", false, 100);
        copyButton.Click += (_, _) => CopyBuild();

        actions.Controls.Add(starterButton);
        actions.Controls.Add(evaluateButton);
        actions.Controls.Add(swapButton);
        actions.Controls.Add(copyButton);

        panel.Controls.Add(heading, 0, 0);
        panel.Controls.Add(fields, 0, 1);
        panel.Controls.Add(actions, 0, 2);

        return panel;
    }

    private Control BuildInsightPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 102F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        panel.Controls.Add(BuildClassCard(), 0, 0);
        panel.Controls.Add(BuildSkillCard(), 0, 1);
        panel.Controls.Add(BuildCompatibilityCard(), 0, 2);
        panel.Controls.Add(BuildResultCard(), 0, 3);

        return panel;
    }

    private Control BuildClassCard()
    {
        var card = CreateCard();
        var title = CreateCardTitle("CLASS / ASCENDANCY IDENTITY");

        _classIdentityLabel.Dock = DockStyle.Top;
        _classIdentityLabel.Height = 42;
        _classIdentityLabel.ForeColor = Color.White;
        _classIdentityLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _classIdentityLabel.Padding = new Padding(0, 4, 0, 0);

        _ascendancyThemeLabel.Dock = DockStyle.Fill;
        _ascendancyThemeLabel.ForeColor = Muted;
        _ascendancyThemeLabel.Font = new Font("Segoe UI", 9F);

        card.Controls.Add(_ascendancyThemeLabel);
        card.Controls.Add(_classIdentityLabel);
        card.Controls.Add(title);
        return card;
    }

    private Control BuildSkillCard()
    {
        var card = CreateCard();
        var title = CreateCardTitle("SKILL + GEAR RULE");

        _skillInfoLabel.Dock = DockStyle.Fill;
        _skillInfoLabel.ForeColor = Color.FromArgb(225, 229, 235);
        _skillInfoLabel.Font = new Font("Segoe UI", 9.5F);
        _skillInfoLabel.Padding = new Padding(0, 6, 0, 0);

        card.Controls.Add(_skillInfoLabel);
        card.Controls.Add(title);
        return card;
    }

    private Control BuildCompatibilityCard()
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            Margin = new Padding(0, 0, 0, 10),
            Padding = new Padding(14, 8, 14, 8)
        };

        _compatibilityLabel.Dock = DockStyle.Fill;
        _compatibilityLabel.TextAlign = ContentAlignment.MiddleLeft;
        _compatibilityLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        card.Controls.Add(_compatibilityLabel);

        return card;
    }

    private Control BuildResultCard()
    {
        var card = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(16),
            Margin = Padding.Empty
        };
        card.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        card.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        card.Controls.Add(CreateCardTitle("BUILD READOUT"), 0, 0);

        _resultBox.Dock = DockStyle.Fill;
        _resultBox.ReadOnly = true;
        _resultBox.BorderStyle = BorderStyle.None;
        _resultBox.BackColor = SurfaceAlt;
        _resultBox.ForeColor = Color.FromArgb(229, 232, 237);
        _resultBox.Font = new Font("Cascadia Mono", 9.5F);
        _resultBox.DetectUrls = false;
        _resultBox.WordWrap = true;
        _resultBox.Margin = new Padding(0, 6, 0, 0);
        card.Controls.Add(_resultBox, 0, 1);

        return card;
    }

    private void PopulateCatalog()
    {
        _classCombo.Items.AddRange(BuildCatalog.Classes.Select(x => x.Name).Cast<object>().ToArray());
        _skillCombo.Items.AddRange(BuildCatalog.Skills.Select(x => x.Name).Cast<object>().ToArray());
        _weaponSetOneCombo.Items.AddRange(BuildCatalog.Gear.Select(x => x.Name).Cast<object>().ToArray());
        _weaponSetTwoCombo.Items.AddRange(BuildCatalog.Gear.Select(x => x.Name).Cast<object>().ToArray());
    }

    private void WireEvents()
    {
        _classCombo.SelectedIndexChanged += (_, _) =>
        {
            PopulateAscendancies();
            RefreshClassInfo();
        };

        _ascendancyCombo.SelectedIndexChanged += (_, _) =>
        {
            RefreshClassInfo();
            RefreshEvaluation();
        };

        _skillCombo.SelectedIndexChanged += (_, _) => RefreshEvaluation();
        _weaponSetOneCombo.SelectedIndexChanged += (_, _) => RefreshEvaluation();
        _weaponSetTwoCombo.SelectedIndexChanged += (_, _) => RefreshEvaluation();
        _supportSlots.ValueChanged += (_, _) => RefreshEvaluation();
    }

    private void PopulateAscendancies()
    {
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
        var className = Selected(_classCombo);
        var starter = _planner.StarterFor(className);

        SelectItem(_skillCombo, starter.Skill);
        SelectItem(_weaponSetOneCombo, starter.WeaponSetOne);
        SelectItem(_weaponSetTwoCombo, starter.WeaponSetTwo);
        _supportSlots.Value = 3;
        RefreshEvaluation();
    }

    private void SwapWeaponSets()
    {
        var first = Selected(_weaponSetOneCombo);
        var second = Selected(_weaponSetTwoCombo);

        SelectItem(_weaponSetOneCombo, second);
        SelectItem(_weaponSetTwoCombo, first);
        RefreshEvaluation();
    }

    private void CopyBuild()
    {
        if (string.IsNullOrWhiteSpace(_latestBuildSummary))
        {
            RefreshEvaluation();
        }

        try
        {
            Clipboard.SetText(_latestBuildSummary);
            _compatibilityLabel.Text = "Copied build readout to clipboard";
            _compatibilityLabel.ForeColor = Success;
        }
        catch
        {
            _compatibilityLabel.Text = "Clipboard unavailable";
            _compatibilityLabel.ForeColor = Warning;
        }
    }

    private void RefreshClassInfo()
    {
        if (_classCombo.SelectedIndex < 0 || _ascendancyCombo.SelectedIndex < 0)
        {
            return;
        }

        var classDefinition = BuildCatalog.FindClass(Selected(_classCombo));
        var ascendancy = classDefinition.Ascendancies.FirstOrDefault(x =>
                             x.Name.Equals(Selected(_ascendancyCombo), StringComparison.OrdinalIgnoreCase))
                         ?? classDefinition.Ascendancies[0];

        _classIdentityLabel.Text = $"{classDefinition.Name} · {classDefinition.Attributes} · {ascendancy.Name}";
        _ascendancyThemeLabel.Text = $"{classDefinition.Identity}\r\nAscendancy: {ascendancy.Theme}";
    }

    private void RefreshEvaluation()
    {
        if (_classCombo.SelectedIndex < 0 ||
            _ascendancyCombo.SelectedIndex < 0 ||
            _skillCombo.SelectedIndex < 0 ||
            _weaponSetOneCombo.SelectedIndex < 0 ||
            _weaponSetTwoCombo.SelectedIndex < 0)
        {
            return;
        }

        var evaluation = _planner.Evaluate(
            Selected(_classCombo),
            Selected(_ascendancyCombo),
            Selected(_skillCombo),
            Selected(_weaponSetOneCombo),
            Selected(_weaponSetTwoCombo),
            (int)_supportSlots.Value);

        _skillInfoLabel.Text =
            $"{evaluation.Skill.Name} · {evaluation.Skill.Tags}\r\n" +
            $"Requirement: {evaluation.Skill.RequiredWeapon}. {evaluation.Skill.Summary}";

        _compatibilityLabel.Text = evaluation.IsSkillUsable
            ? $"READY · {evaluation.ActiveWeaponSet}"
            : $"GEAR MISMATCH · requires {evaluation.Skill.RequiredWeapon}";
        _compatibilityLabel.ForeColor = evaluation.IsSkillUsable ? Success : Warning;

        var builder = new StringBuilder();
        builder.AppendLine(evaluation.Summary);
        builder.AppendLine();
        builder.AppendLine($"Weapon Set 1: {evaluation.WeaponSetOne.Name}");
        builder.AppendLine($"Weapon Set 2: {evaluation.WeaponSetTwo.Name}");
        builder.AppendLine($"Supports: {evaluation.SupportSlots}/5");
        builder.AppendLine();
        builder.AppendLine("Gear influence:");
        foreach (var note in evaluation.GearNotes)
        {
            builder.AppendLine($"• {note}");
        }

        builder.AppendLine();
        builder.AppendLine("Design note:");
        builder.AppendLine("Class/Ascendancy shapes scaling and playstyle, while the skill and equipped weapon decide whether many attacks can actually be used.");

        _latestBuildSummary = builder.ToString().TrimEnd();
        _resultBox.Text = _latestBuildSummary;
    }

    private static Control CreateField(string title, Control control)
    {
        var field = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
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

    private static Panel CreateCard()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            Margin = new Padding(0, 0, 0, 10),
            Padding = new Padding(14)
        };
    }

    private static Label CreateCardTitle(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 22,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
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

    private static string Selected(ComboBox comboBox) => comboBox.SelectedItem?.ToString() ?? string.Empty;

    private static void SelectItem(ComboBox comboBox, string value)
    {
        var index = comboBox.Items.IndexOf(value);
        if (index >= 0)
        {
            comboBox.SelectedIndex = index;
        }
    }
}
