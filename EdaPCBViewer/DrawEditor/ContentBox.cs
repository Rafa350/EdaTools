namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System.ComponentModel;
    using System.Windows;

    public sealed class ContentBox: VisualContainer {

        public static readonly DependencyProperty VisualProperty;

        static ContentBox() {

            // Crea la propietat de dependencia 'Visual'
            //
            VisualProperty = DependencyProperty.Register(
                "Visual",
                typeof(VisualItem),
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
                VisualItem visual = e.NewValue as VisualItem;
                if (visual != null) {
                    sThis.RemoveAll();
                    sThis.Add(visual);
                }
            }
        }

        [BindableAttribute(true)]
        [Category("Content")]
        public VisualItem Visual {
            get {
                return (VisualItem)GetValue(VisualProperty);
            }
            set {
                SetValue(VisualProperty, value);
            }
        }
    }
}
