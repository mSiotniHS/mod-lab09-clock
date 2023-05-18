using System.Drawing.Drawing2D;
using static System.MathF;

namespace App;

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
	}

	private static readonly Brush HighlightBrush = Brushes.LightCoral;
	private static readonly Brush PrimaryBrush = Brushes.LightGray;

	private static readonly Pen PrimaryPen = new(PrimaryBrush, 2);
	private static readonly Pen ThinPrimaryPen = new(PrimaryBrush, 1);
	private static readonly Pen BoldPrimaryPen = new(PrimaryBrush, 3);
	private static readonly Pen HighlightPen = new(HighlightBrush, 1);

	private static readonly Font BigFont = new("IBM Plex Sans", 14, FontStyle.Bold);
	private static readonly Font RegularFont = new("IBM Plex Sans", 10, FontStyle.Bold);

	private const float Quarter = PI / 2;

	private static readonly StringFormat CenterStringFormat = new()
	{
		LineAlignment = StringAlignment.Center,
		Alignment = StringAlignment.Center
	};

	private void MainForm_Paint(object sender, PaintEventArgs e)
	{
		var moment = DateTime.Now;
		var graphics = e.Graphics;
		graphics.SmoothingMode = SmoothingMode.AntiAlias;

		var width = ClientSize.Width;
		var height = ClientSize.Height;

		var minDimension = Math.Min(width, height);
		var scaleFactor = minDimension / 270f;

		graphics.TranslateTransform(width / 2f, height / 2f);
		graphics.ScaleTransform(scaleFactor, scaleFactor);

		DrawClockBase(graphics);
		DrawClockHandsAndDate(graphics, moment);
	}

	private static void DrawClockBase(Graphics graphics)
	{
		graphics.DrawEllipse(PrimaryPen, -120, -120, 240, 240);

		for (var hour = 1; hour <= 12; hour++)
		{
			var angle = HoursToAngle(hour);

			var font = hour switch
			{
				_ when hour % 3 == 0 => BigFont,
				_ => RegularFont
			};

			graphics.DrawString(
				hour.ToString(),
				font,
				PrimaryBrush,
				AdjustedPolarToCartesian(90, angle),
				CenterStringFormat);

			graphics.DrawLine(
				PrimaryPen,
				AdjustedPolarToCartesian(105, angle),
				AdjustedPolarToCartesian(120, angle));
		}

		for (var minute = 0; minute <= 60; minute++)
		{
			var angle = MinutesToAngle(minute);

			graphics.DrawLine(
				ThinPrimaryPen,
				AdjustedPolarToCartesian(112, angle),
				AdjustedPolarToCartesian(120, angle));
		}
	}

	private static void DrawClockHandsAndDate(Graphics graphics, DateTime moment)
	{
		graphics.DrawString(
			moment.Day.ToString(),
			RegularFont,
			HighlightBrush,
			AdjustedPolarToCartesian(47, Quarter),
			CenterStringFormat);

		graphics.FillEllipse(PrimaryBrush, -3, -3, 6, 6);

		var hour = moment.Hour + moment.Minute / 60f;

		graphics.DrawLine(
			BoldPrimaryPen,
			new PointF(0, 0),
			AdjustedPolarToCartesian(60, HoursToAngle(hour)));

		var minute = moment.Minute + moment.Second / 60f;

		graphics.DrawLine(
			PrimaryPen,
			new PointF(0, 0),
			AdjustedPolarToCartesian(95, MinutesToAngle(minute)));

		graphics.FillEllipse(HighlightBrush, -2, -2, 4, 4);

		graphics.DrawLine(
			HighlightPen,
			AdjustedPolarToCartesian(-25, SecondsToAngle(moment.Second)),
			AdjustedPolarToCartesian(100, SecondsToAngle(moment.Second)));
	}

	private static PointF PolarToCartesian(float distance, float angle) =>
		new(distance * Cos(angle), distance * Sin(angle));

	private static float AdjustAngle(float angle) =>
		angle - PI / 2;

	private static PointF AdjustedPolarToCartesian(float distance, float angle) =>
		PolarToCartesian(distance, AdjustAngle(angle));

	private static float HoursToAngle(float hours) => PI / 6 * hours;
	private static float MinutesToAngle(float minutes) => PI / 30 * minutes;
	private static float SecondsToAngle(int seconds) => MinutesToAngle(seconds);

	private void Timer_Tick(object sender, EventArgs e) => Invalidate();
}
