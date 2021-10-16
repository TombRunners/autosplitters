using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class NamedTargetCounterComponent : IComponent
    {
        #region IComponent implementations

        public string ComponentName => "TargetCounter";
        public IDictionary<string, Action> ContextMenuControls => null;
        public float PaddingTop { get; set; }
        public float PaddingLeft => 7f;
        public float PaddingBottom { get; set; }
        public float PaddingRight => 7f;
        public float VerticalHeight { get; set; } = 10;
        public float MinimumHeight { get; set; }
        public float MinimumWidth => NameLabel.X + ValueLabel.ActualWidth;
        public float HorizontalWidth { get; set; }
        public Control GetSettingsControl(LayoutMode mode) => throw new NotSupportedException();
        public XmlNode GetSettings(XmlDocument document) => throw new NotSupportedException();
        public void SetSettings(XmlNode settings) => throw new NotSupportedException();
        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) => DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) => DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);

        protected void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // Set background coloration.
            bool bgColor1IsNotTransparent = Settings.BackgroundColor1.A > 0;
            bool bgColor2IsNotTransparent = Settings.BackgroundColor2.A > 0;
            if (bgColor1IsNotTransparent || Settings.BackgroundGradient != ExtendedGradientType.Plain && bgColor2IsNotTransparent)
            {
                if (Settings.BackgroundGradient == ExtendedGradientType.Alternating)
                {
                    var gradientBrush = new SolidBrush(Index % 2 == 1 ? Settings.BackgroundColor2 : Settings.BackgroundColor1);
                    g.FillRectangle(gradientBrush, 0, 0, width, height);
                }
                else
                {
                    var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.BackgroundGradient == ExtendedGradientType.Horizontal ? new PointF(width, 0) : new PointF(0, height),
                        Settings.BackgroundColor1,
                        Settings.BackgroundGradient == ExtendedGradientType.Plain ? Settings.BackgroundColor1 : Settings.BackgroundColor2
                    );
                    g.FillRectangle(gradientBrush, 0, 0, width, height);
                }
            }

            // Set font.
            TextFont = Settings.OverrideFont ? Settings.TextFont : state.LayoutSettings.TextFont;

            // Calculate Height and Padding from Font.
            var textHeight = g.MeasureString("A", TextFont).Height;
            VerticalHeight = 1.2f * textHeight;
            MinimumHeight = MinimumHeight;
            PaddingBottom = PaddingTop = Math.Max(0, ((VerticalHeight - 0.75f * textHeight) / 2f));

            // Assume most users won't count past four digits (will cause a layout resize in Horizontal Mode).
            float fourCharWidth = g.MeasureString("1000", TextFont).Width;
            HorizontalWidth = NameLabel.X + NameLabel.ActualWidth + (fourCharWidth > ValueLabel.ActualWidth ? fourCharWidth : ValueLabel.ActualWidth) + 5;

            // Set Counter Name Label.
            NameLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Near : StringAlignment.Near;
            NameLabel.VerticalAlignment = StringAlignment.Center;
            NameLabel.X = 5;
            NameLabel.Y = 0;
            NameLabel.Width = (width - fourCharWidth - 5);
            NameLabel.Height = height;
            NameLabel.Font = TextFont;
            Color nameBrushColor;
            if (!Settings.OverrideTextColor)
                nameBrushColor = state.LayoutSettings.TextColor;
            else
                nameBrushColor = Counter.TargetReached && Settings.ExpandTargetColor ? Settings.TargetColor : Settings.NameColor;
            NameLabel.Brush = new SolidBrush(nameBrushColor);
            NameLabel.HasShadow = state.LayoutSettings.DropShadows;
            NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            NameLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            NameLabel.Draw(g);

            // Set Counter Count Label.
            ValueLabel.HorizontalAlignment = mode == LayoutMode.Horizontal ? StringAlignment.Far : StringAlignment.Far;
            ValueLabel.VerticalAlignment = StringAlignment.Center;
            ValueLabel.X = 5;
            ValueLabel.Y = 0;
            ValueLabel.Width = (width - 10);
            ValueLabel.Height = height;
            ValueLabel.Font = TextFont;
            Color valueBrushColor;
            if (!Settings.OverrideTextColor)
                valueBrushColor = Counter.TargetReached ? state.LayoutSettings.AheadGainingTimeColor : state.LayoutSettings.TextColor;
            else
                valueBrushColor = Counter.TargetReached ? Settings.TargetColor : Settings.ValueColor;
            ValueLabel.Brush = new SolidBrush(valueBrushColor);
            ValueLabel.HasShadow = state.LayoutSettings.DropShadows;
            ValueLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            ValueLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
            ValueLabel.Draw(g);
        }
        
        protected readonly GraphicsCache Cache = new GraphicsCache();

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            NameLabel.Text = $"{Counter.Name}";
            ValueLabel.Text = $"{Counter}";

            Cache.Restart();
            Cache["NameLabel"] = NameLabel.Text;
            Cache["ValueLabel"] = ValueLabel.Text;

            if (invalidator != null && Cache.HasChanged)
                invalidator.Invalidate(0, 0, width, height);
        }

        #endregion IComponent implementations

        public MultiCounterComponentSettings Settings { get; set; }

        public readonly INamedTargetCounter Counter;

        /// <summary>
        ///     If within a parent component, this can be set to match counter's index within a list.
        ///     Useful for determining <see cref="ExtendedGradientType.Alternating"/> row coloration.
        /// </summary>
        public int Index { get; set; }

        private Font TextFont { get; set; }
        protected readonly SimpleLabel NameLabel = new SimpleLabel();
        protected readonly SimpleLabel ValueLabel = new SimpleLabel();

        /// <summary>Initializes the <see cref="NamedTargetCounterComponent"/>.</summary>
        /// <param name="settings">Settings passed by <see cref="MultiCounterComponent"/></param>
        /// <param name="counterSettings">Values to apply to the <see cref="Counter"/></param>
        /// <param name="index">Determines row color if <see cref="MultiCounterComponentSettings.BackgroundGradient"/> is <see cref="ExtendedGradientType.Alternating"/></param>
        public NamedTargetCounterComponent(MultiCounterComponentSettings settings, NamedTargetCounterSettings counterSettings, int index = 0)
        {
            Settings = settings;
            Counter = new NamedTargetCounter(counterSettings);
            Index = index;
        }

        public void Dispose() {}
    }
}
