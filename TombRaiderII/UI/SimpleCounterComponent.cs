using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class SimpleCounterComponent : IComponent
    {
        public SimpleCounterComponent(MultiCounterComponentSettings settings, string counterName)
        {
            Settings = settings;
            Settings.Name = counterName;
        }

        public MultiCounterComponentSettings Settings { get; set; } = new MultiCounterComponentSettings();

        public ICounter Counter { get; set; } = new Counter();

        public GraphicsCache Cache { get; set; } = new GraphicsCache();

        public float VerticalHeight { get; set; } = 10;

        public float MinimumHeight { get; set; }

        public float MinimumWidth => NameLabel.X + ValueLabel.ActualWidth;

        public float HorizontalWidth { get; set; }

        public IDictionary<string, Action> ContextMenuControls => null;

        public float PaddingTop { get; set; }
        public float PaddingLeft => 7f;
        public float PaddingBottom { get; set; }
        public float PaddingRight => 7f;

        protected SimpleLabel NameLabel = new SimpleLabel();
        protected SimpleLabel ValueLabel = new SimpleLabel();

        public void Increment() => Counter.Increment();

        protected Font TextFont { get; set; }

        private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // Set background coloration.
            bool bgColor1IsNotTransparent = Settings.BackgroundColor1.A > 0;
            bool bgColor2IsNotTransparent = Settings.BackgroundColor2.A > 0;
            if (bgColor1IsNotTransparent || Settings.BackgroundGradient != GradientType.Plain && bgColor2IsNotTransparent)
            {
                var gradientBrush = new LinearGradientBrush(
                    new PointF(0, 0),
                    Settings.BackgroundGradient == GradientType.Horizontal ? new PointF(width, 0) : new PointF(0, height),
                    Settings.BackgroundColor1,
                    Settings.BackgroundGradient == GradientType.Plain ? Settings.BackgroundColor1 : Settings.BackgroundColor2
                );

                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }

            // Set font.
            TextFont = Settings.OverrideFont ? Settings.TextFont : state.LayoutSettings.TextFont;

            // Calculate Height from Font.
            var textHeight = g.MeasureString("A", TextFont).Height;
            VerticalHeight = 1.2f * textHeight;
            MinimumHeight = MinimumHeight;

            PaddingTop = Math.Max(0, ((VerticalHeight - 0.75f * textHeight) / 2f));
            PaddingBottom = PaddingTop;

            // Assume most users won't count past four digits (will cause a layout resize in Horizontal Mode).
            float fourCharWidth = g.MeasureString("1000", TextFont).Width;
            HorizontalWidth = NameLabel.X + NameLabel.ActualWidth + (fourCharWidth > ValueLabel.ActualWidth ? fourCharWidth : ValueLabel.ActualWidth) + 5; 

            // Set Counter Name Label
            NameLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Near : StringAlignment.Near;
            NameLabel.VerticalAlignment = StringAlignment.Center;
            NameLabel.X = 5;
            NameLabel.Y = 0;
            NameLabel.Width = (width - fourCharWidth - 5);
            NameLabel.Height = height;
            NameLabel.Font = TextFont;
            NameLabel.Brush = new SolidBrush(Settings.OverrideTextColor ? Settings.NameColor : state.LayoutSettings.TextColor);
            NameLabel.HasShadow = state.LayoutSettings.DropShadows;
            NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            NameLabel.Draw(g);

            // Set Counter Value Label.
            ValueLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Far : StringAlignment.Far;
            ValueLabel.VerticalAlignment = StringAlignment.Center;
            ValueLabel.X = 5;
            ValueLabel.Y = 0;
            ValueLabel.Width = (width - 10);
            ValueLabel.Height = height;
            ValueLabel.Font = TextFont;
            ValueLabel.Brush = new SolidBrush(Settings.OverrideTextColor ? Settings.ValueColor : state.LayoutSettings.TextColor);
            ValueLabel.HasShadow = state.LayoutSettings.DropShadows;
            ValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            ValueLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            ValueLabel.Draw(g);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
        }

        public string ComponentName => "Counter";

        public Control GetSettingsControl(LayoutMode mode) => Settings;

        public XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

        public void SetSettings(XmlNode settings) => Settings.SetSettings(settings);
        
        public int GetSettingsHashCode => Settings.GetSettingsHashCode;

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            ValueLabel.Text = Counter.Count.ToString();

            Cache.Restart();
            Cache["CounterNameLabel"] = NameLabel.Text;
            Cache["CounterValueLabel"] = ValueLabel.Text;

            if (invalidator != null && Cache.HasChanged)
                invalidator.Invalidate(0, 0, width, height);
        }

        public void Dispose()
        {
        }
    }
}
