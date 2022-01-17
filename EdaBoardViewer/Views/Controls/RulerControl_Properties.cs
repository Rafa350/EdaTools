namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class RulerControl: Control {

        public static readonly StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<RulerControl, IBrush>(nameof(BackgroundProperty), Brushes.DarkCyan);
        public static readonly StyledProperty<IBrush> TagBrushProperty = AvaloniaProperty.Register<RulerControl, IBrush>(nameof(TagBrush), Brushes.White);

        public static readonly StyledProperty<double> SmallTickLengthProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(SmallTickLength), 0);
        public static readonly StyledProperty<Color> SmallTickColorProperty = AvaloniaProperty.Register<RulerControl, Color>(nameof(SmallTickColor), Colors.White);
        public static readonly StyledProperty<double> SmallTickIntervalProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(SmallTickInterval), 1);

        public static readonly StyledProperty<double> MediumTickLengthProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(MediumTickLength), 0);
        public static readonly StyledProperty<Color> MediumTickColorProperty = AvaloniaProperty.Register<RulerControl, Color>(nameof(MediumTickColor), Colors.White);
        public static readonly StyledProperty<double> MediumTickIntervalProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(MediumTickInterval), 5);

        public static readonly StyledProperty<double> LargeTickLengthProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(LargeTickLength), 0);
        public static readonly StyledProperty<Color> LargeTickColorProperty = AvaloniaProperty.Register<RulerControl, Color>(nameof(LargeTickColor), Colors.White);
        public static readonly StyledProperty<double> LargeTickIntervalProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(LargeTickLength), 10);

        public static readonly StyledProperty<FontFamily> FontFamilyProperty = AvaloniaProperty.Register<RulerControl, FontFamily>(nameof(FontFamily), new FontFamily("Seqoe UI"));
        public static readonly StyledProperty<FontStyle> FontStyleProperty = AvaloniaProperty.Register<RulerControl, FontStyle>(nameof(FontStyle), FontStyle.Normal);
        public static readonly StyledProperty<double> FontSizeProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(FontSize), 10);

        public static readonly StyledProperty<Point> RegionPositionProperty = AvaloniaProperty.Register<RulerControl, Point>(nameof(RegionPosition), new Point(0, 0));
        public static readonly StyledProperty<Size> RegionSizeProperty = AvaloniaProperty.Register<RulerControl, Size>(nameof(RegionSize), new Size(0, 0));
        public static readonly StyledProperty<Color> RegionColorProperty = AvaloniaProperty.Register<RulerControl, Color>(nameof(RegionColor), Color.FromArgb(0x3F, 0x9A, 0xFB, 0xE1));
        public static readonly StyledProperty<bool> ShowRegionProperty = AvaloniaProperty.Register<RulerControl, bool>(nameof(ShowRegion), false);

        public static readonly StyledProperty<Point> PointerPositionProperty = AvaloniaProperty.Register<RulerControl, Point>(nameof(PointerPosition), new Point(0, 0));
        public static readonly StyledProperty<Color> PointerColorProperty = AvaloniaProperty.Register<RulerControl, Color>(nameof(PointerColor), Color.FromArgb(255, 255, 0, 0));
        public static readonly StyledProperty<bool> ShowPointerProperty = AvaloniaProperty.Register<RulerControl, bool>(nameof(ShowPointer), false);

        public static readonly StyledProperty<RulerOrientation> OrientationProperty = AvaloniaProperty.Register<RulerControl, RulerOrientation>(nameof(Orientation), RulerOrientation.Horizontal);
        public static readonly StyledProperty<RulerAlignment> AlignmentProperty = AvaloniaProperty.Register<RulerControl, RulerAlignment>(nameof(Alignment), RulerAlignment.Bottom);
        public static readonly StyledProperty<RulerDirection> DirectionProperty = AvaloniaProperty.Register<RulerControl, RulerDirection>(nameof(Direction), RulerDirection.LeftToRight);
        public static readonly StyledProperty<bool> FlipTagsProperty;

        public static readonly StyledProperty<double> ValueDivisorProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(ValueDivisor), 1);
        public static readonly StyledProperty<Matrix> ValueMatrixProperty = AvaloniaProperty.Register<RulerControl, Matrix>(nameof(ValueMatrix), Matrix.Identity);
        public static readonly StyledProperty<double> MinValueProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(MinValue), 0);
        public static readonly StyledProperty<double> MaxValueProperty = AvaloniaProperty.Register<RulerControl, double>(nameof(MaxValue), 100);

        /// <summary>
        /// Obte o asigna la brotza del fons del control.
        /// </summary>
        /// 
        public IBrush Background {
            get { return GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la brotxa per dibuixar les etiquetes.
        /// </summary>
        /// 
        public IBrush TagBrush {
            get { return GetValue(TagBrushProperty); }
            set { SetValue(TagBrushProperty, value); }
        }

        public double SmallTickLength {
            get { return GetValue(SmallTickLengthProperty); }
            set { SetValue(SmallTickLengthProperty, value); }
        }

        public Color SmallTickColor {
            get { return GetValue(SmallTickColorProperty); }
            set { SetValue(SmallTickColorProperty, value); }
        }

        public double SmallTickInterval {
            get { return GetValue(SmallTickIntervalProperty); }
            set { SetValue(SmallTickIntervalProperty, value); }
        }

        public double MediumTickLength {
            get { return GetValue(MediumTickLengthProperty); }
            set { SetValue(MediumTickLengthProperty, value); }
        }

        public Color MediumTickColor {
            get { return GetValue(MediumTickColorProperty); }
            set { SetValue(MediumTickColorProperty, value); }
        }

        public double MediumTickInterval {
            get { return GetValue(MediumTickIntervalProperty); }
            set { SetValue(MediumTickIntervalProperty, value); }
        }

        public double LargeTickLength {
            get { return GetValue(LargeTickLengthProperty); }
            set { SetValue(LargeTickLengthProperty, value); }
        }

        public Color LargeTickColor {
            get { return GetValue(LargeTickColorProperty); }
            set { SetValue(LargeTickColorProperty, value); }
        }

        public double LargeTickInterval {
            get { return GetValue(LargeTickIntervalProperty); }
            set { SetValue(LargeTickIntervalProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el font de les etiquetes.
        /// </summary>
        /// 
        public FontFamily FontFamily {
            get { return GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el tamany del font de les etiquetes.
        /// </summary>
        /// 
        public double FontSize {
            get { return GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el estil del font de les etiquetes.
        /// </summary>
        /// 
        public FontStyle FontStyle {
            get { return GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Obte o asigna l'orientacio de la regla.
        /// </summary>
        /// 
        public RulerOrientation Orientation {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio de la regla.
        /// </summary>
        /// 
        public RulerAlignment Alignment {
            get { return GetValue(AlignmentProperty); }
            set { SetValue(AlignmentProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la direccio de la regla.
        /// </summary>
        /// 
        public RulerDirection Direction {
            get { return GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la posicio de la regio.
        /// </summary>
        /// 
        public Point RegionPosition {
            get { return GetValue(RegionPositionProperty); }
            set { SetValue(RegionPositionProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el tamany de la regio.
        /// </summary>
        /// 
        public Size RegionSize {
            get { return GetValue(RegionSizeProperty); }
            set { SetValue(RegionSizeProperty, value); }
        }

        /// <summary>
        /// Obte o saigna la brotxa per dibuixar la regio.
        /// </summary>
        /// 
        public Color RegionColor {
            get { return GetValue(RegionColorProperty); }
            set { SetValue(RegionColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el indicador de visibilitat de la regio.
        /// </summary>
        /// 
        public bool ShowRegion {
            get { return GetValue(ShowRegionProperty); }
            set { SetValue(ShowRegionProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la posicio del punter.
        /// </summary>
        /// 
        public Point PointerPosition {
            get { return GetValue(PointerPositionProperty); }
            set { SetValue(PointerPositionProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la britza per dibuixar el punter.
        /// </summary>
        /// 
        public Color PointerColor {
            get { return GetValue(PointerColorProperty); }
            set { SetValue(PointerColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el indicador de visibilitat del punter.
        /// </summary>
        /// 
        public bool ShowPointer {
            get { return GetValue(ShowPointerProperty); }
            set { SetValue(ShowPointerProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el valor minim.
        /// </summary>
        /// 
        public double MinValue {
            get { return GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el valor maxim.
        /// </summary>
        /// 
        public double MaxValue {
            get { return GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el factor de divisio del valor.
        /// </summary>
        /// 
        public double ValueDivisor {
            get { return GetValue(ValueDivisorProperty); }
            set { SetValue(ValueDivisorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la matriu de transformacio del valor.
        /// </summary>
        /// 
        public Matrix ValueMatrix {
            get { return GetValue(ValueMatrixProperty); }
            set { SetValue(ValueMatrixProperty, value); }
        }
    }
}
