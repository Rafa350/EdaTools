namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    public sealed class ContentBox: VisualContainer {

        public static readonly DependencyProperty VisualProperty;
        public static readonly DependencyProperty ViewTransformProperty;

        static ContentBox() {

            // Crea la propietat de dependencia 'Visual'
            //
            VisualProperty = DependencyProperty.Register(
                "Visual",
                typeof(Visual),
                typeof(ContentBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = null,
                    PropertyChangedCallback = Visual_PropertyChanged
                });

            // Crea la propietat de dependencia 'ViewTransform'
            //
            ViewTransformProperty = DependencyProperty.Register(
                "ViewTransform",
                typeof(Transform),
                typeof(ContentBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new MatrixTransform(new Matrix()),
                    PropertyChangedCallback = ViewTransform_PropertyChanged
                });
        }

        public ContentBox() {

            ClipToBounds = true;
        }

        private static void Visual_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ContentBox sThis = o as ContentBox;
            if (sThis != null) {
                VisualItem visual = e.NewValue as VisualItem;
                if (visual != null)
                    sThis.AddItem(visual);
            }
        }

        private static void ViewTransform_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ContentBox sThis = o as ContentBox;
            if (sThis != null) {
            }
        }

        [BindableAttribute(true)]
        [Category("Content")]
        public Visual Visual {
            get {
                return (Visual)GetValue(VisualProperty);
            }
            set {
                SetValue(VisualProperty, value);
            }
        }

        [BindableAttribute(true)]
        [Category("Viewpoint")]
        public Transform ViewTransform {
            get {
                return (Transform)GetValue(ViewTransformProperty);
            }
            set {
                SetValue(ViewTransformProperty, value);
            }
        }
    }
}
