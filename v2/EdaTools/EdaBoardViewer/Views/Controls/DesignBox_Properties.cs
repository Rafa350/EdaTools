namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;

    public sealed partial class DesignBox : Control {

        public static readonly StyledProperty<IBrush> BackgroundProperty = AvaloniaProperty.Register<DesignBox, IBrush>(nameof(BackgroundProperty), Brushes.Transparent);
        public static readonly StyledProperty<IBrush> TagBrushProperty = AvaloniaProperty.Register<DesignBox, IBrush>(nameof(TagBrush), Brushes.White);
        public static readonly StyledProperty<IBrush> LineBrushProperty = AvaloniaProperty.Register<DesignBox, IBrush>(nameof(LineBrush), Brushes.White);

        public static readonly StyledProperty<FontFamily> FontFamilyProperty = AvaloniaProperty.Register<DesignBox, FontFamily>(nameof(FontFamily), new FontFamily("Seqoe UI"));
        public static readonly StyledProperty<FontStyle> FontStyleProperty = AvaloniaProperty.Register<DesignBox, FontStyle>(nameof(FontStyle), FontStyle.Normal);
        public static readonly StyledProperty<double> FontSizeProperty = AvaloniaProperty.Register<DesignBox, double>(nameof(FontSize), 11);

        public static readonly StyledProperty<Point> RegionPositionProperty = AvaloniaProperty.Register<DesignBox, Point>(nameof(RegionPosition), new Point(0, 0));
        public static readonly StyledProperty<Size> RegionSizeProperty = AvaloniaProperty.Register<DesignBox, Size>(nameof(RegionSize), new Size(0, 0));
        public static readonly StyledProperty<IBrush> RegionBackgroundProperty = AvaloniaProperty.Register<DesignBox, IBrush>(nameof(RegionBackground), Brushes.Transparent);
        public static readonly StyledProperty<Color> RegionBorderColorProperty = AvaloniaProperty.Register<DesignBox, Color>(nameof(RegionBorderColor), Color.FromArgb(0x3F, 0x9A, 0xFB, 0xE1));
        public static readonly StyledProperty<Color> RegionTagTextColorProperty = AvaloniaProperty.Register<DesignBox, Color>(nameof(RegionTagTextColor), Colors.WhiteSmoke);
        public static readonly StyledProperty<Color> RegionTagBorderColorProperty = AvaloniaProperty.Register<DesignBox, Color>(nameof(RegionTagBorderColor), Colors.WhiteSmoke);
        public static readonly StyledProperty<IBrush> RegionTagBackgroundProperty = AvaloniaProperty.Register<DesignBox, IBrush>(nameof(RegionTagBackground), new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30)));
        public static readonly StyledProperty<bool> ShowRegionProperty = AvaloniaProperty.Register<DesignBox, bool>(nameof(ShowRegion), false);
        public static readonly StyledProperty<bool> ShowRegionTagsProperty = AvaloniaProperty.Register<DesignBox, bool>(nameof(ShowRegionTags), true);

        public static readonly StyledProperty<Point> PointerPositionProperty = AvaloniaProperty.Register<DesignBox, Point>(nameof(PointerPosition), new Point(0, 0));
        public static readonly StyledProperty<Color> PointerColorProperty = AvaloniaProperty.Register<DesignBox, Color>(nameof(PointerColor), Color.FromArgb(255, 255, 0, 0));
        public static readonly StyledProperty<Color> PointerTagTextColorProperty = AvaloniaProperty.Register<DesignBox, Color>(nameof(PointerTagTextColor), Colors.WhiteSmoke);
        public static readonly StyledProperty<Color> PointerTagBorderColorProperty = AvaloniaProperty.Register<DesignBox, Color>(nameof(PointerTagBorderColor), Colors.WhiteSmoke);
        public static readonly StyledProperty<IBrush> PointerTagBackgroundProperty = AvaloniaProperty.Register<DesignBox, IBrush>(nameof(PointerTagBackground), new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30)));
        public static readonly StyledProperty<bool> ShowPointerProperty = AvaloniaProperty.Register<DesignBox, bool>(nameof(ShowPointer), false);
        public static readonly StyledProperty<bool> ShowPointerTagsProperty = AvaloniaProperty.Register<DesignBox, bool>(nameof(ShowPointerTags), true);

        public static readonly StyledProperty<double> ValueDivisorProperty = AvaloniaProperty.Register<DesignBox, double>(nameof(ValueDivisor), 1);

        public static readonly StyledProperty<Point> OriginProperty = AvaloniaProperty.Register<DesignBox, Point>(nameof(Origin), new Point(0, 0));
        public static readonly StyledProperty<double> ScaleProperty = AvaloniaProperty.Register<DesignBox, double>(nameof(Scale), 1);

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
        /// Obte o asigna el color per dibuixar el punter.
        /// </summary>
        /// 
        public Color PointerColor {
            get { return GetValue(PointerColorProperty); }
            set { SetValue(PointerColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el color per dibuixar el text de l'etiqueta.
        /// </summary>
        /// 
        public Color PointerTagTextColor {
            get { return GetValue(PointerTagTextColorProperty); }
            set { SetValue(PointerTagTextColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el color per dibuixar marc de l'etiqueta.
        /// </summary>
        /// 
        public Color PointerTagBorderColor {
            get { return GetValue(PointerTagBorderColorProperty); }
            set { SetValue(PointerTagBorderColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la brotxa pel fons de l'etiqueta.
        /// </summary>
        /// 
        public IBrush PointerTagBackground {
            get { return GetValue(PointerTagBackgroundProperty); }
            set { SetValue(PointerTagBackgroundProperty, value); }
        }

        public bool ShowPointer {
            get { return GetValue(ShowPointerProperty); }
            set { SetValue(ShowPointerProperty, value); }
        }

        public bool ShowPointerTags {
            get { return GetValue(ShowPointerTagsProperty); }
            set { SetValue(ShowPointerTagsProperty, value); }
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
        /// Obte o asigna la brotxa per dibuixar el fons de la regio.
        /// </summary>
        /// 
        public IBrush RegionBackground {
            get { return GetValue(RegionBackgroundProperty); }
            set { SetValue(RegionBackgroundProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el color del perfil de la regio.
        /// </summary>
        /// 
        public Color RegionBorderColor {
            get { return GetValue(RegionBorderColorProperty); }
            set { SetValue(RegionBorderColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna la brotxa per dibuixar el fons de la etiqueta de la regio.
        /// </summary>
        /// 
        public IBrush RegionTagBackground {
            get { return GetValue(RegionTagBackgroundProperty); }
            set { SetValue(RegionTagBackgroundProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el color per dibuixar el perfil de la etiqueta de la regio.
        /// </summary>
        /// 
        public Color RegionTagTextColor {
            get { return GetValue(RegionTagTextColorProperty); }
            set { SetValue(RegionTagTextColorProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el color per dibuixar el text de la etiqueta de la regio.
        /// </summary>
        /// 
        public Color RegionTagBorderColor {
            get { return GetValue(RegionTagBorderColorProperty); }
            set { SetValue(RegionTagBorderColorProperty, value); }
        }

        public Point RegionPosition {
            get { return GetValue(RegionPositionProperty); }
            set { SetValue(RegionPositionProperty, value); }
        }

        public Size RegionSize {
            get { return GetValue(RegionSizeProperty); }
            set { SetValue(RegionSizeProperty, value); }
        }

        public bool ShowRegion {
            get { return GetValue(ShowRegionProperty); }
            set { SetValue(ShowRegionProperty, value); }
        }

        public bool ShowRegionTags {
            get { return GetValue(ShowRegionTagsProperty); }
            set { SetValue(ShowRegionTagsProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el factor de divisio del valor.
        /// </summary>
        /// 
        public double ValueDivisor {
            get { return GetValue(ValueDivisorProperty); }
            set { SetValue(ValueDivisorProperty, value); }
        }

        public Point Origin {
            get { return GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        public double Scale {
            get { return GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
    }
}
