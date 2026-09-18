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

    private string _latestSummary = "Run a demo value check to create a summary.";

    public MainForm()
    {
        Text = "Ancient Value Overlay";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(860, 620);
        Size = new Size(1020, 720);
        BackColor = Background;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        AutoScaleMode = AutoScaleMode.Dpi;
        KeyPreview = true;

        Controls.Add(BuildRootLayout());
        KeyDown += OnMainFormKeyDown;

        UpdateStats(0);
        SetStatus("Ready for beta testing", Success);
        AppendActivity("Ready", "Use Run Demo Value Check to exercise the current value-analysis flow.");
    }

    private Control BuildRootLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Background,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 6
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildStatusBar(), 0, 1);
        root.Controls.Add(BuildStatsGrid(), 0, 2);
        root.Controls.Add(BuildActionBar(), 0, 3);
        root.Controls.Add(BuildActivityPanel(), 0, 4);
        root.Controls.Add(BuildShortcutLabel(), 0, 5);

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
            Text = "PoE2 reward value helper · fast feedback, clean session view",
            AutoSize = true,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Muted,
            Location = new Point(2, 50)
        };

        copy.Controls.Add(title);
        copy.Controls.Add(subtitle);

        var badge = new Label
        {
            Text = "BETA 0.2",
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

        var nextButton = CreateButton("Next Steps", false, 115);
        nextButton.Click += (_, _) => ShowNextSteps();
        _toolTip.SetToolTip(nextButton, "Show the planned development path (F1)");

        var clearButton = CreateButton("Clear Activity", false, 120);
        clearButton.Click += (_, _) => ClearActivity();
        _toolTip.SetToolTip(clearButton, "Clear the activity panel (Ctrl+L)");

        actions.Controls.Add(runButton);
        actions.Controls.Add(copyButton);
        actions.Controls.Add(resetButton);
        actions.Controls.Add(nextButton);
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
            Text = "Shortcuts: Ctrl+Enter run · Ctrl+Shift+C copy · Ctrl+L clear · F1 next steps",
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
            SetStatus("Summary copied to clipboard", Success);
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
        SetStatus("Session reset", Success);
        AppendActivity("Session reset", "Session totals and best reward were cleared.");
    }

    private void ShowNextSteps()
    {
        const string nextSteps =
            "1. Load a manual price file.  2. Add live price refresh with cache fallback.  " +
            "3. Add calibration.  4. Add the transparent in-game overlay after the data path is stable.";

        SetStatus("Development path shown", Primary);
        AppendActivity("Next steps", nextSteps);
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

    private void OnMainFormKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.Enter)
        {
            RunDemoValueCheck();
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
            return;
        }

        if (e.KeyCode == Keys.F1)
        {
            ShowNextSteps();
            e.SuppressKeyPress = true;
        }
    }
}
