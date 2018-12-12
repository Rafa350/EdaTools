namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    public sealed class ContentBox: VisualFrameworkElement {

        public static readonly DependencyProperty VisualProperty;

        static ContentBox() {

            // Crea la propietat de dependencia 'Visual'
            //
            VisualProperty = DependencyProperty.Register(
                "Visual",
                typeof(DrawingVisual),
                typeof(ContentBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = null,
                    PropertyChangedCallback = Visual_PropertyChanged
                });
        }

        public ContentBox() {

            ClipToBounds = true;
        }

        private static void Visual_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ContentBox sThis = o as ContentBox;
            if (sThis != null) {
                DrawingVisual visual = e.NewValue as DrawingVisual;
                if (visual != null) {
                    sThis.RemoveAll();
                    sThis.Add(visual);
                }
            }
        }

        [BindableAttribute(true)]
        [Category("Content")]
        public DrawingVisual Visual {
            get {
                return (DrawingVisual)GetValue(VisualProperty);
            }
            set {
                SetValue(VisualProperty, value);
            }
        }
    }
}
