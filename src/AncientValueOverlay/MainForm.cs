using System;
using System.Drawing;
using System.Windows.Forms;

namespace AncientValueOverlay;

public sealed class MainForm : Form
{
    private readonly TextBox _logBox = new();
    private readonly Label _statusLabel = new();

    public MainForm()
    {
        Text = "Ancient Value Overlay";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(760, 520);
        Size = new Size(860, 560);
        BackColor = Color.FromArgb(18, 20, 24);
        ForeColor = Color.White;

        var title = new Label
        {
            Text = "Ancient Value Overlay",
            AutoSize = true,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            Location = new Point(24, 22)
        };

        var subtitle = new Label
        {
            Text = "Simple starter build: executable first, overlay features second.",
            AutoSize = true,
            ForeColor = Color.Gainsboro,
            Font = new Font("Segoe UI", 10),
            Location = new Point(28, 68)
        };

        _statusLabel.Text = "Status: app opened successfully";
        _statusLabel.AutoSize = true;
        _statusLabel.ForeColor = Color.LightGreen;
        _statusLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _statusLabel.Location = new Point(28, 105);

        var testButton = new Button
        {
            Text = "Run Demo Value Check",
            Width = 190,
            Height = 36,
            Location = new Point(28, 145)
        };
        testButton.Click += (_, _) => RunDemoValueCheck();

        var nextButton = new Button
        {
            Text = "Show Next Steps",
            Width = 150,
            Height = 36,
            Location = new Point(232, 145)
        };
        nextButton.Click += (_, _) => ShowNextSteps();

        _logBox.Multiline = true;
        _logBox.ReadOnly = true;
        _logBox.ScrollBars = ScrollBars.Vertical;
        _logBox.BackColor = Color.FromArgb(28, 32, 38);
        _logBox.ForeColor = Color.White;
        _logBox.Font = new Font("Consolas", 10);
        _logBox.Location = new Point(28, 205);
        _logBox.Size = new Size(790, 295);
        _logBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _logBox.Text = "Ready. This is the simplified executable shell.\r\n\r\n" +
                       "Goal 1: produce a working Windows exe.\r\n" +
                       "Goal 2: add one feature at a time only after builds are green.\r\n";

        Controls.Add(title);
        Controls.Add(subtitle);
        Controls.Add(_statusLabel);
        Controls.Add(testButton);
        Controls.Add(nextButton);
        Controls.Add(_logBox);
    }

    private void RunDemoValueCheck()
    {
        var rows = new[]
        {
            new RewardRow("Divine Orb", 1.00m, 1),
            new RewardRow("Exalted Orb", 0.05m, 3),
            new RewardRow("Unknown Reward", 0m, 1)
        };

        var best = ValueAnalyzer.FindBest(rows);
        var total = ValueAnalyzer.TotalValue(rows);
        var unknown = ValueAnalyzer.UnknownCount(rows);

        _statusLabel.Text = "Status: demo value check completed";
        _logBox.Text =
            "Demo Value Check\r\n" +
            "----------------\r\n" +
            $"Best reward: {best.Name} ({best.TotalDivines:0.##} divine)\r\n" +
            $"Total visible value: {total:0.##} divine\r\n" +
            $"Unknown rows: {unknown}\r\n\r\n" +
            "This proves the executable can run app logic. Next we add real price loading, then calibration, then overlay display.\r\n";
    }

    private void ShowNextSteps()
    {
        _logBox.Text =
            "Next Development Steps\r\n" +
            "----------------------\r\n" +
            "1. Keep this executable green in GitHub Actions.\r\n" +
            "2. Add config save/load.\r\n" +
            "3. Add manual price file import.\r\n" +
            "4. Add poe.ninja live prices after the local app is stable.\r\n" +
            "5. Add calibration.\r\n" +
            "6. Add the overlay window last.\r\n\r\n" +
            "We are not adding OCR, networking, or transparent overlay code until the basic executable build succeeds.\r\n";
    }
}
