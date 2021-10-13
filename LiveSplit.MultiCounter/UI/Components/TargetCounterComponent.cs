using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class TargetCounterComponent : IComponent
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

        private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
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

            // Calculate Height from Font.
            var textHeight = g.MeasureString("A", TextFont).Height;
            VerticalHeight = 1.2f * textHeight;
            MinimumHeight = MinimumHeight;

            PaddingTop = Math.Max(0, ((VerticalHeight - 0.75f * textHeight) / 2f));
            PaddingBottom = PaddingTop;

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

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            NameLabel.Text = Name;
            ValueLabel.Text = Counter.Count.ToString() + ValueLabelSuffix;

            Cache.Restart();
            Cache["CounterNameLabel"] = NameLabel.Text;
            Cache["CounterValueLabel"] = ValueLabel.Text;

            if (invalidator != null && Cache.HasChanged)
                invalidator.Invalidate(0, 0, width, height);
        }

        #endregion IComponent implementations

        public ICounter Counter { get; set; }
        public MultiCounterComponentSettings Settings { get; set; }
        public GraphicsCache Cache { get; set; } = new GraphicsCache();

        /// <summary>
        ///     Amount by which to increment/decrement <see cref="Value"/>.
        /// </summary>
        public int IncrementValue { get; }

        /// <summary>
        ///     Amount at which <see cref="Value"/> should start when initialized or reset.
        /// </summary>
        public int InitialValue { get; }

        /// <summary>
        ///     Displayed as the target in text appended to <see cref="Value"/> for <see cref="ValueLabel"/>.
        /// </summary>
        /// <remarks>
        ///     Defaults to <see langword="null"/>, which results in no extraneous text shown after <see cref="Value"/>.
        /// </remarks>
        public int? Target { get; set; }
        
        protected int Index { get; }
        protected Font TextFont { get; set; }
        protected SimpleLabel NameLabel = new SimpleLabel();
        protected SimpleLabel ValueLabel = new SimpleLabel();
        /// <summary>
        ///     The string appended after <see cref="Value"/> for the right-aligned string of the component.
        /// </summary>
        protected string ValueLabelSuffix => Target is null ? string.Empty : $" / {Target}";

        /// <summary>
        ///     The left-aligned string of the counter component.
        /// </summary>
        public string Name
        {
            get => NameLabel.Text;
            set => NameLabel.Text = value;
        }

        /// <summary>
        ///     The value (<see cref="ICounter.Count"/>) used in the right-aligned string of the counter component.
        /// </summary>
        public int Value
        {
            get => Counter.Count;
            set => Counter.SetCount(value);
        }

        public TargetCounterComponent(MultiCounterComponentSettings settings, TargetCounterSettings counterSettings, int index = 0)
        {
            Settings = settings;
            Name = counterSettings.Name;
            Counter = new Counter(counterSettings.Start);
            InitialValue = Counter.Count;
            Counter.SetIncrement(counterSettings.Increment);
            IncrementValue = counterSettings.Increment;
            Target = counterSettings.Target;
            Index = index;
        }

        /// <summary>
        ///     Decreases <see cref="Value"/> by <see cref="IncrementValue"/>.
        /// </summary>
        public void Decrement() => Counter.Decrement();
        /// <summary>
        ///     Increases <see cref="Value"/> by <see cref="IncrementValue"/>.
        /// </summary>
        public void Increment() => Counter.Increment();
        /// <summary>
        ///     Sets <see cref="Value"/> to <see cref="InitialValue"/>.
        /// </summary>
        public void Reset() => Counter.Reset();

        public void Dispose() {}
    }
}
