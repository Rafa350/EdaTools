namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor.Controls {

    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    public sealed class ContentBox: VisualContainer {

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
                    PropertyChangedCallback = VisualPropertyChanged
                });
        }

        public ContentBox() {

            ClipToBounds = true;
        }

        private static void VisualPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {

            ContentBox sThis = o as ContentBox;
            if (sThis != null) {
                VisualItem visual = e.NewValue as VisualItem;
                if (visual != null) {
                    sThis.RemoveAllVisualItems();
                    sThis.AddVisualItem(visual);
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
