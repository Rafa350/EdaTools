namespace Eda.PCBViewer.DrawEditor {

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class ZoomControl: ContentControl {

        public static readonly DependencyProperty ScaleProperty;
        public static readonly DependencyProperty ScaleCenterXProperty;
        public static readonly DependencyProperty ScaleCenterYProperty;
        public static readonly DependencyProperty OffsetXProperty;
        public static readonly DependencyProperty OffsetYProperty;

        private readonly TransformGroup transform;
        private readonly ScaleTransform scaleTransform;
        private readonly TranslateTransform offsetTransform;

        private FrameworkElement content;

        static ZoomControl() {
        
            ScaleProperty = DependencyProperty.Register(
                "Scale", 
                typeof(double), 
                typeof(ZoomControl),
                new FrameworkPropertyMetadata(
                    1.0, 
                    Scale_PropertyChanged, 
                    Scale_Coerce));

            ScaleCenterXProperty = DependencyProperty.Register(
                "ScaleCenterX",
                typeof(double),
                typeof(ZoomControl),
                new FrameworkPropertyMetadata(
                    0.0,
                    ScaleCenterX_PropertyChanged));

            ScaleCenterYProperty = DependencyProperty.Register(
                "ScaleCenterY",
                typeof(double),
                typeof(ZoomControl),
                new FrameworkPropertyMetadata(
                    0.0,
                    ScaleCenterY_PropertyChanged));

            OffsetXProperty = DependencyProperty.Register(
                "OffsetX",
                typeof(double),
                typeof(ZoomControl),
                new FrameworkPropertyMetadata(
                    0.0,
                    OffsetX_PropertyChanged,
                    OffsetX_Coerce));

            OffsetYProperty = DependencyProperty.Register(
                "OffsetY",
                typeof(double),
                typeof(ZoomControl),
                new FrameworkPropertyMetadata(
                    0.0,
                    OffsetY_PropertyChanged,
                    OffsetY_Coerce));
        }

        public ZoomControl() {

            Background = Brushes.Transparent;

            scaleTransform = new ScaleTransform();
            offsetTransform = new TranslateTransform();

            transform = new TransformGroup();
            transform.Children.Add(offsetTransform);
            transform.Children.Add(scaleTransform);
        }

        public override void OnApplyTemplate() {

            base.OnApplyTemplate();

            content = Content as FrameworkElement;
            if (content != null) {
                
                scaleTransform.ScaleX = Scale;
                scaleTransform.ScaleY = Scale;
                scaleTransform.CenterX = ScaleCenterX;
                scaleTransform.CenterY = ScaleCenterY;
                
                offsetTransform.X = OffsetX;
                offsetTransform.Y = OffsetY;

                content.RenderTransform = transform;
            }
        }

        private static void Scale_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ZoomControl c = (ZoomControl) o;
            c.scaleTransform.ScaleX = (double) e.NewValue;
            c.scaleTransform.ScaleY = (double) e.NewValue;
        }

        private static object Scale_Coerce(DependencyObject o, object baseValue) {

            return Math.Max((double) baseValue, 1.0);
        }

        private static void ScaleCenterX_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ZoomControl c = (ZoomControl) o;
            c.scaleTransform.CenterX = (double) e.NewValue;
        }
        
        private static void ScaleCenterY_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ZoomControl c = (ZoomControl) o;
            c.scaleTransform.CenterY = (double) e.NewValue;
        }

        private static void OffsetX_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ZoomControl c = (ZoomControl) o;
            c.offsetTransform.X = (double) e.NewValue;
        }

        private static object OffsetX_Coerce(DependencyObject o, object baseValue) {

            return baseValue;
        }

        private static void OffsetY_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ZoomControl c = (ZoomControl) o;

            c.offsetTransform.Y = (double) e.NewValue;
        }

        private static object OffsetY_Coerce(DependencyObject o, object baseValue) {

            return baseValue;
        }

        public double Scale {
            get {
                return (double) GetValue(ScaleProperty);
            }
            set {
                SetValue(ScaleProperty, value);
            }
        }

        public double ScaleCenterX {
            get {
                return (double) GetValue(ScaleCenterXProperty);
            }
            set {
                SetValue(ScaleCenterXProperty, value);
            }
        }

        public double ScaleCenterY {
            get {
                return (double) GetValue(ScaleCenterYProperty);
            }
            set {
                SetValue(ScaleCenterYProperty, value);
            }
        }

        public double OffsetX {
            get {
                return (double) GetValue(OffsetXProperty);
            }
            set {
                SetValue(OffsetXProperty, value);
            }
        }

        public double OffsetY {
            get {
                return (double) GetValue(OffsetYProperty);
            }
            set {
                SetValue(OffsetYProperty, value);
            }
        }
    }
}
