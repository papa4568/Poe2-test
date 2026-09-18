using System;
using System.Drawing;
using System.Windows.Forms;

namespace AncientValueOverlay;

public sealed class MainForm : Form
{
    private static readonly Color Background = Color.FromArgb(15, 17, 21);
    private static readonly Color Surface = Color.FromArgb(25, 29, 35);
    private static readonly Color SurfaceAlt = Color.FromArgb(32, 37, 45);
    private static readonly Color Border = Color.FromArgb(53, 60, 70);
    private static readonly Color Primary = Color.FromArgb(94, 129, 244);
    private static readonly Color Success = Color.FromArgb(103, 211, 149);
    private static readonly Color Muted = Color.FromArgb(169, 177, 190);
    private static readonly Color Warning = Color.FromArgb(245, 189, 93);

    private readonly SessionStats _sessionStats = new();
    private readonly DecisionAdvisor _advisor = new();
    private readonly Label _statusDot = new();
    private readonly Label _statusLabel = new();
    private readonly Label _bestValueLabel = new();
    private readonly Label _totalValueLabel = new();
    private readonly Label _rowCountLabel = new();
    private readonly Label _unknownCountLabel = new();
    private readonly RichTextBox _activityBox = new();
    private readonly ToolTip _toolTip = new();
    private readonly TabControl _tabs = new();

    private string _latestSummary = "Run a demo value check to create a summary.";

    public MainForm()
    {
        Text = "Ancient Value Overlay";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(960, 680);
        Size = new Size(1160, 780);
        BackColor = Background;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        AutoScaleMode = AutoScaleMode.Dpi;
        KeyPreview = true;

        Controls.Add(BuildRootLayout());
        KeyDown += OnMainFormKeyDown;

        UpdateStats(0);
        SetStatus("Ready · Value Lab + Build Lab", Success);
        AppendActivity("Ready", "Value checks and the PoE2-inspired build/class planner are available.");
    }

    private Control BuildRootLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 4
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildStatusBar(), 0, 1);
        root.Controls.Add(BuildTabs(), 0, 2);
        root.Controls.Add(BuildShortcutLabel(), 0, 3);

        return root;
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Margin = Padding.Empty
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var copy = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty
        };

        var title = new Label
        {
            Text = "Ancient Value Overlay",
            AutoSize = true,
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(0, 4)
        };

        var subtitle = new Label
        {
            Text = "PoE2 value helper + class, skill and gear interaction lab",
            AutoSize = true,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Muted,
            Location = new Point(2, 50)
        };

        copy.Controls.Add(title);
        copy.Controls.Add(subtitle);

        var badge = new Label
        {
            Text = "BETA 0.4",
            AutoSize = true,
            BackColor = Primary,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Padding = new Padding(12, 7, 12, 7),
            Margin = new Padding(12, 8, 0, 0),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };

        header.Controls.Add(copy, 0, 0);
        header.Controls.Add(badge, 1, 0);
        return header;
    }

    private Control BuildStatusBar()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 8),
            Padding = new Padding(12, 0, 12, 0)
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24F));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _statusDot.Text = "●";
        _statusDot.Dock = DockStyle.Fill;
        _statusDot.TextAlign = ContentAlignment.MiddleCenter;
        _statusDot.Font = new Font("Segoe UI Symbol", 10F);

        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        panel.Controls.Add(_statusDot, 0, 0);
        panel.Controls.Add(_statusLabel, 1, 0);
        return panel;
    }

    private Control BuildTabs()
    {
        _tabs.Dock = DockStyle.Fill;
        _tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        _tabs.SizeMode = TabSizeMode.Fixed;
        _tabs.ItemSize = new Size(150, 34);
        _tabs.Padding = new Point(16, 5);
        _tabs.Margin = Padding.Empty;
        _tabs.DrawItem += DrawTab;
        _tabs.SelectedIndexChanged += (_, _) =>
        {
            var status = _tabs.SelectedIndex switch
            {
                1 => "Build Lab active",
                2 => "Skill Simulator active",
                _ => "Value Lab active"
            };
            SetStatus(status, Primary);
            _tabs.Invalidate();
        };

        var valuePage = new TabPage("Value Lab")
        {
            BackColor = Background,
            ForeColor = Color.White,
            Padding = new Padding(0, 10, 0, 0)
        };
        valuePage.Controls.Add(BuildValueDashboard());

        var buildPage = new TabPage("Build Lab")
        {
            BackColor = Background,
            ForeColor = Color.White,
            Padding = new Padding(0, 10, 0, 0)
        };
        buildPage.Controls.Add(new BuildLabControl());

        var skillPage = new TabPage("Skill Simulator")
        {
            BackColor = Background,
            ForeColor = Color.White,
            Padding = new Padding(0, 10, 0, 0)
        };
        skillPage.Controls.Add(new SkillSimulatorControl());

        _tabs.TabPages.Add(valuePage);
        _tabs.TabPages.Add(buildPage);
        _tabs.TabPages.Add(skillPage);
        return _tabs;
    }

    private Control BuildValueDashboard()
    {
        var dashboard = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty
        };

        dashboard.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
        dashboard.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        dashboard.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        dashboard.Controls.Add(BuildStatsGrid(), 0, 0);
        dashboard.Controls.Add(BuildActionBar(), 0, 1);
        dashboard.Controls.Add(BuildActivityPanel(), 0, 2);
        return dashboard;
    }

    private Control BuildStatsGrid()
    {
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            Margin = new Padding(0, 0, 0, 8)
        };

        for (var i = 0; i < 4; i++)
        {
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        }

        grid.Controls.Add(CreateStatCard("BEST REWARD", _bestValueLabel), 0, 0);
        grid.Controls.Add(CreateStatCard("SESSION VALUE", _totalValueLabel), 1, 0);
        grid.Controls.Add(CreateStatCard("ROWS CHECKED", _rowCountLabel), 2, 0);
        grid.Controls.Add(CreateStatCard("UNKNOWN", _unknownCountLabel), 3, 0);

        return grid;
    }

    private Control BuildActionBar()
    {
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            Margin = new Padding(0, 2, 0, 8),
            Padding = Padding.Empty
        };

        var runButton = CreateButton("Run Demo Value Check", true, 190);
        runButton.Click += (_, _) => RunDemoValueCheck();
        _toolTip.SetToolTip(runButton, "Run the demo analyzer (Ctrl+Enter)");

        var copyButton = CreateButton("Copy Summary", false, 135);
        copyButton.Click += (_, _) => CopySummary();
        _toolTip.SetToolTip(copyButton, "Copy the latest result (Ctrl+Shift+C)");

        var resetButton = CreateButton("Reset Session", false, 130);
        resetButton.Click += (_, _) => ResetSession();

        var buildButton = CreateButton("Open Build Lab", false, 135);
        buildButton.Click += (_, _) => _tabs.SelectedIndex = 1;
        _toolTip.SetToolTip(buildButton, "Open class/build planner (Ctrl+B)");

        var skillButton = CreateButton("Skill Simulator", false, 130);
        skillButton.Click += (_, _) => _tabs.SelectedIndex = 2;
        _toolTip.SetToolTip(skillButton, "Open normalized gear/support simulator (Ctrl+G)");

        var clearButton = CreateButton("Clear Activity", false, 120);
        clearButton.Click += (_, _) => ClearActivity();
        _toolTip.SetToolTip(clearButton, "Clear the activity panel (Ctrl+L)");

        actions.Controls.Add(runButton);
        actions.Controls.Add(copyButton);
        actions.Controls.Add(resetButton);
        actions.Controls.Add(buildButton);
        actions.Controls.Add(skillButton);
        actions.Controls.Add(clearButton);

        return actions;
    }

    private Control BuildActivityPanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(16)
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var heading = new Label
        {
            Text = "Activity",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        _activityBox.Dock = DockStyle.Fill;
        _activityBox.ReadOnly = true;
        _activityBox.BorderStyle = BorderStyle.None;
        _activityBox.BackColor = SurfaceAlt;
        _activityBox.ForeColor = Color.FromArgb(229, 232, 237);
        _activityBox.Font = new Font("Cascadia Mono", 10F);
        _activityBox.DetectUrls = false;
        _activityBox.WordWrap = true;
        _activityBox.Margin = new Padding(0, 4, 0, 0);

        panel.Controls.Add(heading, 0, 0);
        panel.Controls.Add(_activityBox, 0, 1);
        return panel;
    }

    private Control BuildShortcutLabel()
    {
        return new Label
        {
            Text = "Shortcuts: Ctrl+Enter value check · Ctrl+B Build Lab · Ctrl+G Skill Simulator · Ctrl+Shift+C copy · Ctrl+L clear",
            Dock = DockStyle.Fill,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8.5F),
            TextAlign = ContentAlignment.BottomLeft,
            Margin = Padding.Empty
        };
    }

    private Panel CreateStatCard(string title, Label valueLabel)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Surface,
            Margin = new Padding(0, 0, 8, 0),
            Padding = new Padding(14)
        };

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 22,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
        };

        valueLabel.Dock = DockStyle.Fill;
        valueLabel.ForeColor = Color.White;
        valueLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        valueLabel.TextAlign = ContentAlignment.MiddleLeft;

        card.Controls.Add(valueLabel);
        card.Controls.Add(titleLabel);
        return card;
    }

    private Button CreateButton(string text, bool primary, int width)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = 40,
            BackColor = primary ? Primary : SurfaceAlt,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F, primary ? FontStyle.Bold : FontStyle.Regular),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0),
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderColor = primary ? Primary : Border;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = primary
            ? Color.FromArgb(111, 143, 247)
            : Color.FromArgb(43, 49, 59);

        return button;
    }

    private void RunDemoValueCheck()
    {
        _tabs.SelectedIndex = 0;

        var rows = new[]
        {
            new RewardRow("Divine Orb", 1.00m, 1),
            new RewardRow("Exalted Orb", 0.05m, 3),
            new RewardRow("Chaos Orb", 0.01m, 12),
            new RewardRow("Unknown Reward", 0m, 1)
        };

        _sessionStats.Record(rows);
        _latestSummary = _advisor.BuildSummary(rows);
        var unknown = ValueAnalyzer.UnknownCount(rows);

        UpdateStats(unknown);
        SetStatus("Demo value check completed", Success);
        AppendActivity("Demo check", _latestSummary);
    }

    private void CopySummary()
    {
        try
        {
            Clipboard.SetText(_latestSummary);
            SetStatus("Value summary copied to clipboard", Success);
            AppendActivity("Copied", _latestSummary);
        }
        catch (Exception ex)
        {
            SetStatus("Could not copy summary", Warning);
            AppendActivity("Copy failed", ex.Message);
        }
    }

    private void ResetSession()
    {
        _sessionStats.Reset();
        _latestSummary = "Run a demo value check to create a summary.";
        UpdateStats(0);
        SetStatus("Value session reset", Success);
        AppendActivity("Session reset", "Session totals and best reward were cleared.");
    }

    private void ClearActivity()
    {
        _activityBox.Clear();
        SetStatus("Activity cleared", Primary);
        AppendActivity("Activity", "Log cleared. The app is ready for another test.");
    }

    private void UpdateStats(int currentUnknown)
    {
        var best = _sessionStats.BestSeen;

        _bestValueLabel.Text = best.IsUnknown || best.TotalDivines <= 0m
            ? "—"
            : $"{best.Name}\n{best.TotalDivines:0.##} div";

        _totalValueLabel.Text = $"{_sessionStats.TotalSeenDivines:0.##} div";
        _rowCountLabel.Text = _sessionStats.RowCount.ToString();
        _unknownCountLabel.Text = currentUnknown.ToString();
        _unknownCountLabel.ForeColor = currentUnknown > 0 ? Warning : Color.White;
    }

    private void SetStatus(string message, Color color)
    {
        _statusLabel.Text = message;
        _statusLabel.ForeColor = color;
        _statusDot.ForeColor = color;
    }

    private void AppendActivity(string heading, string message)
    {
        if (_activityBox.TextLength > 0)
        {
            _activityBox.AppendText(Environment.NewLine);
        }

        _activityBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {heading}{Environment.NewLine}");
        _activityBox.AppendText($"{message}{Environment.NewLine}");
        _activityBox.SelectionStart = _activityBox.TextLength;
        _activityBox.ScrollToCaret();
    }

    private void DrawTab(object? sender, DrawItemEventArgs e)
    {
        var selected = e.Index == _tabs.SelectedIndex;
        using var backgroundBrush = new SolidBrush(selected ? SurfaceAlt : Surface);
        using var textBrush = new SolidBrush(selected ? Color.White : Muted);

        e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
        var text = _tabs.TabPages[e.Index].Text;
        TextRenderer.DrawText(
            e.Graphics,
            text,
            new Font("Segoe UI", 9.5F, selected ? FontStyle.Bold : FontStyle.Regular),
            e.Bounds,
            textBrush.Color,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private void OnMainFormKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.Enter)
        {
            RunDemoValueCheck();
            e.SuppressKeyPress = true;
            return;
        }

        if (e.Control && e.KeyCode == Keys.B)
        {
            _tabs.SelectedIndex = 1;
            e.SuppressKeyPress = true;
            return;
        }

        if (e.Control && e.KeyCode == Keys.G)
        {
            _tabs.SelectedIndex = 2;
            e.SuppressKeyPress = true;
            return;
        }

        if (e.Control && e.Shift && e.KeyCode == Keys.C)
        {
            CopySummary();
            e.SuppressKeyPress = true;
            return;
        }

        if (e.Control && e.KeyCode == Keys.L)
        {
            ClearActivity();
            e.SuppressKeyPress = true;
        }
    }
}
