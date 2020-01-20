namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class RulerBox : Control {

        public static readonly StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(BackgroundProperty), Brushes.Transparent);
        public static readonly StyledProperty<IBrush> TagBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(TagBrush), Brushes.White);
        public static readonly StyledProperty<IBrush> LineBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(LineBrush), Brushes.White);

        public static readonly StyledProperty<FontFamily> FontFamilyProperty = AvaloniaProperty.Register<RulerBox, FontFamily>(nameof(FontFamily), new FontFamily("Seqoe UI"));
        public static readonly StyledProperty<FontStyle> FontStyleProperty = AvaloniaProperty.Register<RulerBox, FontStyle>(nameof(FontStyle), FontStyle.Normal);
        public static readonly StyledProperty<double> FontSizeProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(FontSize), 10);

        public static readonly StyledProperty<double> RegionPositionProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(RegionPosition), 0);
        public static readonly StyledProperty<double> RegionSizeProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(RegionSize), 0);
        public static readonly StyledProperty<IBrush> RegionBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(RegionBrush), new SolidColorBrush(Color.FromArgb(0x3F, 0x9A, 0xFB, 0xE1)));
        public static readonly StyledProperty<bool> ShowRegionProperty = AvaloniaProperty.Register<RulerBox, bool>(nameof(ShowRegion), false);

        public static readonly StyledProperty<double> PointerPositionProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(PointerPosition), 0);
        public static readonly StyledProperty<IBrush> PointerBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(PointerBrush), new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)));
        public static readonly StyledProperty<bool> ShowPointerProperty = AvaloniaProperty.Register<RulerBox, bool>(nameof(ShowPointer), false);

        public static readonly StyledProperty<RulerOrientation> OrientationProperty = AvaloniaProperty.Register<RulerBox, RulerOrientation>(nameof(Orientation), RulerOrientation.Horizontal);
        public static readonly StyledProperty<RulerAlignment> AlignmentProperty = AvaloniaProperty.Register<RulerBox, RulerAlignment>(nameof(Alignment), RulerAlignment.LeftOrBottom);
        public static readonly StyledProperty<bool> FlipTagsProperty;

        public static readonly StyledProperty<double> ValueDivisorProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(ValueDivisor), 1);
        public static readonly StyledProperty<double> MinValueProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(MinValue), 0);
        public static readonly StyledProperty<double> MaxValueProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(MaxValue), 100);

        public static readonly StyledProperty<double> OriginProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(Origin), 0);
        public static readonly StyledProperty<double> ScaleProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(Scale), 1);

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

        /// <summary>
        /// Obte o asigna la brotxa per dibuixar les linies de mesura.
        /// </summary>
        /// 
        public IBrush LineBrush {
            get { return GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
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
        /// Obte o asigna la posicio de la regio.
        /// </summary>
        /// 
        public double RegionPosition {
            get { return GetValue(RegionPositionProperty); }
            set { SetValue(RegionPositionProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el tamany de la regio.
        /// </summary>
        /// 
        public double RegionSize {
            get { return GetValue(RegionSizeProperty); }
            set { SetValue(RegionSizeProperty, value); }
        }

        /// <summary>
        /// Obte o saigna la brotxa per dibuixar la regio.
        /// </summary>
        public IBrush RegionBrush {
            get { return GetValue(RegionBrushProperty); }
            set { SetValue(RegionBrushProperty, value); }
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
        public double PointerPosition {
            get { return GetValue(PointerPositionProperty); }
            set { SetValue(PointerPositionProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la britza per dibuixar el punter.
        /// </summary>
        /// 
        public IBrush PointerBrush {
            get { return GetValue(PointerBrushProperty); }
            set { SetValue(PointerBrushProperty, value); }
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

        public double Origin {
            get { return GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        public double Scale {
            get { return GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
    }
}
