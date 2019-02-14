namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    public enum RulerAlign {
        Top,
        Bottom
    }

    public enum RulerAxis {
        XAxis,
        YAxis
    }

    public sealed class RulerBox: VisualContainer {

        public static readonly DependencyProperty TagBrushProperty;
        public static readonly DependencyProperty LineBrushProperty;
        public static readonly DependencyProperty BorderBrushProperty;
        public static readonly DependencyProperty FontFamilyProperty;
        public static readonly DependencyProperty FontStyleProperty;
        public static readonly DependencyProperty FontSizeProperty;
        public static readonly DependencyProperty FontWeightProperty;
        public static readonly DependencyProperty FontStretchProperty;
        public static readonly DependencyProperty RegionPositionProperty;
        public static readonly DependencyProperty RegionSizeProperty;
        public static readonly DependencyProperty RegionBrushProperty;
        public static readonly DependencyProperty ShowRegionProperty;
        public static readonly DependencyProperty PointerPositionProperty;
        public static readonly DependencyProperty PointerBrushProperty;
        public static readonly DependencyProperty ShowPointerProperty;
        public static readonly DependencyProperty ViewTransformProperty;
        public static readonly DependencyProperty ViewAreaProperty;
        public static readonly DependencyProperty RulerAlignmentProperty;
        public static readonly DependencyProperty RulerAxisProperty;
        public static readonly DependencyProperty FlipTagsProperty;
        public static readonly DependencyProperty ScaleFactorProperty;

        private readonly VisualItem rulerVisual;
        private readonly VisualItem pointerVisual;
        private readonly VisualItem regionVisual;

        /// <summary>
        /// Constructor estatic. Crea les propietats de dependencia i les inicialitza.
        /// </summary>
        /// 
        static RulerBox() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RulerBox),
                new FrameworkPropertyMetadata(typeof(RulerBox)));

            // Crea la propietat de dependencia 'TagBrush'
            //
            TagBrushProperty = DependencyProperty.Register(
                "TagBrush",
                typeof(Brush),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = Brushes.White,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'LineBrush'
            //
            LineBrushProperty = DependencyProperty.Register(
                "LineBrush",
                typeof(Brush),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = Brushes.White,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'BorderBrush'
            //
            BorderBrushProperty = DependencyProperty.Register(
                "BorderBrush",
                typeof(Brush),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = Brushes.Black,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'FontFamily'
            //
            FontFamilyProperty = DependencyProperty.Register(
                "FontFamily",
                typeof(FontFamily),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new FontFamily("Seqoe UI"),
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'FontStyle'
            //
            FontStyleProperty = DependencyProperty.Register(
                "FontStyle",
                typeof(FontStyle),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new FontStyle(),
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'FontSize'
            //
            FontSizeProperty = DependencyProperty.Register(
                "FontSize",
                typeof(double),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = 10.0,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'FontWeight'
            //
            FontWeightProperty = DependencyProperty.Register(
                "FontWeight",
                typeof(FontWeight),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new FontWeight(),
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'FontStretch'
            //
            FontStretchProperty = DependencyProperty.Register(
                "FontStretch",
                typeof(FontStretch),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new FontStretch(),
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'PointerPosition'
            //
            PointerPositionProperty = DependencyProperty.Register(
                "PointerPosition",
                typeof(Point),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new Point(0, 0),
                    PropertyChangedCallback = PointerChanged
                });

            // Crea la propietat de dependencia 'PointerBrush'
            //
            PointerBrushProperty = DependencyProperty.Register(
                "PointerBrush",
                typeof(Brush),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    PropertyChangedCallback = PointerChanged
                });

            // Crea la propietat de dependencia 'PointerMark'
            //
            ShowPointerProperty = DependencyProperty.Register(
                "ShowPointer",
                typeof(bool),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = false,
                    PropertyChangedCallback = PointerChanged
                });

            // Crea la propietat de dependencia 'RegionPosition'
            //
            RegionPositionProperty = DependencyProperty.Register(
                "RegionPosition",
                typeof(Point),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new Point(0, 0),
                    PropertyChangedCallback = RegionChanged
                });

            // Crea la propietat de dependencia 'RegionSize'
            //
            RegionSizeProperty = DependencyProperty.Register(
                "RegionSize",
                typeof(Size),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new Size(0, 0),
                    PropertyChangedCallback = RegionChanged
                });

            // Crea la propietat de dependencia 'RegionBrush'
            //
            RegionBrushProperty = DependencyProperty.Register(
                "RegionBrush",
                typeof(Brush),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new SolidColorBrush(Color.FromArgb(0x3F, 0x9A, 0xFB, 0xE1)),
                    PropertyChangedCallback = RegionChanged
                });

            // Crea la propietat de dependencia 'ShowRegion'
            //
            ShowRegionProperty = DependencyProperty.Register(
                "ShowRegion",
                typeof(bool),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = false,
                    PropertyChangedCallback = RegionChanged
                });

            // Crea la propietat de dependencia 'ViewTransform'
            //
            ViewTransformProperty = DependencyProperty.Register(
                "ViewTransform",
                typeof(Transform),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new MatrixTransform(new Matrix()),
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'ViewArea'
            //
            ViewAreaProperty = DependencyProperty.Register(
                "ViewArea",
                typeof(Rect),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new Rect(),
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'RulerAlignment'
            //
            RulerAlignmentProperty = DependencyProperty.Register(
                "RulerAlignment",
                typeof(RulerAlign),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = RulerAlign.Bottom,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'RulerAxis'
            //
            RulerAxisProperty = DependencyProperty.Register(
                "RulerAxis",
                typeof(RulerAxis),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = RulerAxis.XAxis,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'FlipTags'
            //
            FlipTagsProperty = DependencyProperty.Register(
                "FlipTags",
                typeof(bool),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = false,
                    PropertyChangedCallback = VisualAspectChanged
                });

            // Crea la propietat de dependencia 'ScaleFactor'
            //
            ScaleFactorProperty = DependencyProperty.Register(
                "ScaleFactor",
                typeof(double),
                typeof(RulerBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = 1.0,
                    PropertyChangedCallback = VisualAspectChanged
                });
        }

        /// <summary>
        /// Contructor del control.
        /// </summary>
        /// 
        public RulerBox() {

            // Inicialitza lñes propietats heredades
            //
            ClipToBounds = true;

            // Crea la visual del la regla
            //
            rulerVisual = new VisualItem();
            rulerVisual.Render += OnRenderRuler;
            AddVisualItem(rulerVisual);

            // Crea la visual del punter
            //
            pointerVisual = new VisualItem();
            pointerVisual.Render += OnRenderPointer;
            AddVisualItem(pointerVisual);

            // Crea la visual de la regio
            //
            regionVisual = new VisualItem();
            regionVisual.Render += OnRenderRegion;
            AddVisualItem(regionVisual);

            // Inicialitza els events
            //
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Notifica els canvis en l'aspecte visual.
        /// </summary>
        /// <param name="d">Propietat modificada.</param>
        /// <param name="e">Arguments del event.</param>
        /// 
        private static void VisualAspectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            RulerBox sThis = (RulerBox) d;
            sThis.rulerVisual.Refresh();
            sThis.pointerVisual.Refresh();
            sThis.regionVisual.Refresh();
        }

        /// <summary>
        /// Notifica els canvis en la posicio del punter.
        /// </summary>
        /// <param name="d">Propietat modificada.</param>
        /// <param name="e">Arguments del event.</param>
        /// 
        private static void PointerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            RulerBox sThis = (RulerBox) d;
            sThis.pointerVisual.Refresh();
        }

        /// <summary>
        /// Notifica els canvis en la regio.
        /// </summary>
        /// <param name="d">Propietat modificada.</param>
        /// <param name="e">Arguments del event.</param>
        /// 
        private static void RegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            RulerBox sThis = (RulerBox) d;
            sThis.regionVisual.Refresh();
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e) {

            rulerVisual.Refresh();
        }

        /// <summary>
        /// Procesa el event 'Render' del 'backgroundVisual'.
        /// </summary>
        /// <param name="sender">El que genera el event.</param>
        /// <param name="e">Els argument del event.</param>
        /// 
        private void OnRenderRuler(object sender, VisualItemRenderEventArgs e) {

            DrawingContext dc = e.Dc;

            Typeface typeface = new Typeface(
                FontFamily,
                FontStyle,
                FontWeight,
                FontStretch);

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
            FormattedText ft = new FormattedText(
                "0123456789.",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface, 
                FontSize,
                TagBrush);
#pragma warning restore CS0618 // El tipo o el miembro están obsoleto

            double length1 = ActualHeight - ft.Height / 2;
            double length2 = length1 * 0.66;
            double length3 = length1 * 0.33;

            // Dibuixa les linies de la regla
            //
            Pen linePen = new Pen(LineBrush, 0.5);
            double vx = RulerAxis == RulerAxis.XAxis ? ViewArea.X : ViewArea.Y;
            double vw = RulerAxis == RulerAxis.XAxis ? ViewArea.Width : ViewArea.Height;

            double sf = ScaleFactor;

            double u = 1 * sf;
            double u5x = u * 5;
            double u10x = u * 10;

            for (double m = vx - (vx % sf); m <= vx + vw; m += u) {

                Point p = ViewTransform.Transform(new Point(m, m));
                double x = RulerAxis == RulerAxis.XAxis ? p.X : p.Y;

                // Linia 10 unitats
                //
                if ((m % u10x) == 0) {
                    double y1 = RulerAlignment == RulerAlign.Top ? 0 : ActualHeight - length1;
                    double y2 = RulerAlignment == RulerAlign.Top ? length1 : ActualHeight;
                    DrawLine(dc, linePen, x, y1, x, y2);

                    if ((m < ActualWidth) || ((m % 50) == 0)) {
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                        FormattedText ft2 = new FormattedText(
                            (m / sf).ToString(),
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            typeface,
                            FontSize,
                            TagBrush);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                        ft2.TextAlignment = TextAlignment.Left;
                        double y = RulerAlignment == RulerAlign.Bottom ? 0 : ActualHeight - ft2.Height;
                        if (FlipTags) 
                            dc.PushTransform(new RotateTransform(180, x, y + ft.Height / 2));
                        dc.DrawText(ft2, new Point(x + 2, y));
                        if (FlipTags)
                            dc.Pop();
                    }
                }

                // Linia de 5 d'unitats
                //
                else if ((m % u5x) == 0) {
                    double y1 = RulerAlignment == RulerAlign.Top ? 0 : ActualHeight - length2;
                    double y2 = RulerAlignment == RulerAlign.Top ? length2 : ActualHeight;
                    DrawLine(dc, linePen, x, y1, x, y2);
                }

                // Linia d'unitat
                //
                else {
                    double y1 = RulerAlignment == RulerAlign.Top ? 0 : ActualHeight - length3;
                    double y2 = RulerAlignment == RulerAlign.Top ? length3 : ActualHeight;
                    DrawLine(dc, linePen, x, y1, x, y2);
                }
            }
        }

        /// <summary>
        /// Procesa l'event 'Render' del 'pointerVisual'.
        /// </summary>
        /// <param name="sender">El que genera l'event.</param>
        /// <param name="e">Els arguments del event.</param>
        /// 
        private void OnRenderPointer(object sender, VisualItemRenderEventArgs e) {

            if (ShowPointer) {
                DrawingContext dc = e.Dc;
                Point p = ViewTransform.Transform(PointerPosition);
                double x = RulerAxis == RulerAxis.XAxis ? p.X : p.Y;
                DrawLine(dc, new Pen(PointerBrush, 0.5), x, 0, x, ActualHeight);
            }
        }

        /// <summary>
        /// Procesa l'event 'Render' del 'regionVisual'.
        /// </summary>
        /// <param name="sender">El que genera l'event.</param>
        /// <param name="e">Els arguments del event.</param>
        /// 
        private void OnRenderRegion(object sender, VisualItemRenderEventArgs e) {

            if (ShowRegion) {
                DrawingContext dc = e.Dc;
                Rect r = ViewTransform.TransformBounds(new Rect(RegionPosition, RegionSize));
                double x = RulerAxis == RulerAxis.XAxis ? r.X : r.Y;
                double w = RulerAxis == RulerAxis.XAxis ? r.Width : r.Height;
                DrawRectangle(dc, RegionBrush, null, x, 0, w, ActualHeight);
            }
        }

        /// <summary>
        /// Dibuixa una linia.
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="pen">Pincell per dibuixar la linia</param>
        /// <param name="x1">Origen X.</param>
        /// <param name="y1">Origen Y.</param>
        /// <param name="x2">Final X.</param>
        /// <param name="y2">Final Y.</param>
        /// 
        private static void DrawLine(DrawingContext dc, Pen pen, double x1, double y1, double x2, double y2) {

            // Afeigint 0.5 a cada coordinada, eliminem el antialiasing
            //
            dc.DrawLine(pen, new Point(x1 + 0.5, y1 + 0.5), new Point(x2 + 0.5, y2 + 0.5));            
        }

        /// <summary>
        /// Dibuixa un rectangle.
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="brush">Brotxa.</param>
        /// <param name="pen">Pincell</param>
        /// <param name="x">Posicio X.</param>
        /// <param name="y">Posicio Y.</param>
        /// <param name="w">Amplada.</param>
        /// <param name="h">Alçada.</param>
        /// 
        private static void DrawRectangle(DrawingContext dc, Brush brush, Pen pen, double x, double y, double w, double h) {

            // Afeigint 0.5 a cada coordinada, eliminem el antialiasing
            //
            dc.DrawRectangle(brush, pen, new Rect(x + 0.5, y + 0.5, w + 0.5, h + 0.5));
        }

        /// <summary>
        /// Obte o asigna la brotxa de les etiquetes.
        /// </summary>
        [Bindable(true)]
        [Category("Pinceles")]
        [Description("Tag brush")]
        public Brush TagBrush {
            get {
                return (Brush)GetValue(TagBrushProperty);
            }
            set {
                SetValue(TagBrushProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la brotxa de les linies.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Pinceles")]
        [Description("Line brush")]
        public Brush LineBrush {
            get {
                return (Brush)GetValue(LineBrushProperty);
            }
            set {
                SetValue(LineBrushProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la brotxa del marc del control.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Pinceles")]
        [Description("Border brush")]
        public Brush BorderBrush {
            get {
                return (Brush) GetValue(BorderBrushProperty);
            }
            set {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el tipus de lletra de les etiquetes.
        /// </summary>
        /// 
        [Bindable(true)]
        [Localizability(LocalizationCategory.Font)]
        public FontFamily FontFamily {
            get {
                return (FontFamily) GetValue(FontFamilyProperty);
            }
            set {
                SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el estil de lletra de les etiquetes.
        /// </summary>
        /// 
        [Bindable(true)]
        [Localizability(LocalizationCategory.Font)]
        public FontStyle FontStyle {
            get {
                return (FontStyle) GetValue(FontStyleProperty);
            }
            set {
                SetValue(FontStyleProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de lletra de les etiquetes.
        /// </summary>
        /// 
        [Bindable(true)]
        [Localizability(LocalizationCategory.Font)]
        public double FontSize {
            get {
                return (double) GetValue(FontSizeProperty);
            }
            set {
                SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el pes de les lletres de l'etiqueta.
        /// </summary>
        /// 
        [Bindable(true)]
        [Localizability(LocalizationCategory.Font)]
        public FontWeight FontWeight {
            get {
                return (FontWeight) GetValue(FontWeightProperty);
            }
            set {
                SetValue(FontWeightProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la compressio de lletre de les etiquetes.
        /// </summary>
        /// 
        [Bindable(true)]
        [Localizability(LocalizationCategory.Font)]
        public FontStretch FontStretch {
            get {
                return (FontStretch) GetValue(FontStretchProperty);
            }
            set {
                SetValue(FontStretchProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la posicio de la regio.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Region")]
        [Description("Region position")]
        public Point RegionPosition {
            get {
                return (Point) GetValue(RegionPositionProperty);
            }
            set {
                SetValue(RegionPositionProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de la regio.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Region")]
        [Description("Region size")]
        public Size RegionSize {
            get {
                return (Size) GetValue(RegionSizeProperty);
            }
            set {
                SetValue(RegionSizeProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la brotxa de la regio.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Region")]
        [Description("Region brush")]
        public Brush RegionBrush {
            get {
                return (Brush) GetValue(RegionBrushProperty);
            }
            set {
                SetValue(RegionBrushProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la visibilitat de la regio.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Region")]
        [Description("Visibility")]
        public bool ShowRegion {
            get {
                return (bool) GetValue(ShowRegionProperty);
            }
            set {
                SetValue(ShowRegionProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la posicio del punter.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Pointer")]
        [Description("Pointer position")]
        public Point PointerPosition {
            get {
                return (Point) GetValue(PointerPositionProperty);
            }
            set {
                SetValue(PointerPositionProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la brotxa del punter.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Pointer")]
        [Description("Pointer brush")]
        public Brush PointerBrush {
            get {
                return (Brush) GetValue(PointerBrushProperty);
            }
            set {
                SetValue(PointerBrushProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la visibilitat del punter.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Pointer")]
        [Description("Pointer visibility")]
        public bool ShowPointer {
            get {
                return (bool) GetValue(ShowPointerProperty);
            }
            set {
                SetValue(ShowPointerProperty, value);
            }
        }


        [Bindable(true)]
        [Category("Viewpoint")]
        public Transform ViewTransform {
            get {
                return (Transform)GetValue(ViewTransformProperty);
            }
            set {
                SetValue(ViewTransformProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del viewport.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Viewpoint")]
        public Rect ViewArea {
            get {
                return (Rect) GetValue(ViewAreaProperty);
            }
            set {
                SetValue(ViewAreaProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna l'aliniacio de la regle.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Design")]
        public RulerAlign RulerAlignment {
            get {
                return (RulerAlign) GetValue(RulerAlignmentProperty);
            }
            set {
                SetValue(RulerAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna l'eix de la regla.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Design")]
        public RulerAxis RulerAxis {
            get {
                return (RulerAxis)GetValue(RulerAxisProperty);
            }
            set {
                SetValue(RulerAxisProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el voltejat de les etiquetes.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Design")]
        public bool FlipTags {
            get {
                return (bool)GetValue(FlipTagsProperty);
            }
            set {
                SetValue(FlipTagsProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna el factor d'escala de les unitats.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Design")]
        public double ScaleFactor {
            get {
                return (double)GetValue(ScaleFactorProperty);
            }
            set {
                SetValue(ScaleFactorProperty, value);
            }
        }
    }
}
