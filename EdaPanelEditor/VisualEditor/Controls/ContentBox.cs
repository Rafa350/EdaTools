namespace MikroPic.EdaTools.v1.PanelEditor.VisualEditor.Controls {

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    public sealed class ContentBox: VisualContainer {

        public static readonly DependencyProperty TransformMatrixProperty;
        public static readonly DependencyProperty VisualProperty;

        private readonly VisualItem contentVisual;

        static ContentBox() {

            // Crea la propietat de dependencia 'TransformMatrix'
            //
            TransformMatrixProperty = DependencyProperty.Register(
                "TransformMatrix",
                typeof(Matrix),
                typeof(ContentBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = new Matrix(),
                    PropertyChangedCallback = (d, e) => { ((ContentBox)d).TransformationMatrixPropertyChanged(e); }
                });

            // Crea la propietat de dependencia 'Visual'
            //
            VisualProperty = DependencyProperty.Register(
                "Visual",
                typeof(DrawingVisual),
                typeof(ContentBox),
                new FrameworkPropertyMetadata {
                    DefaultValue = null,
                    PropertyChangedCallback = (d, e) => { ((ContentBox)d).VisualPropertyChanged(e); }
            });
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public ContentBox() {

            ClipToBounds = true;

            contentVisual = new VisualItem();
            AddVisualItem(contentVisual);
        }

        /// <summary>
        /// Accio al canviar el valor de la propietat 'Visual'.
        /// </summary>
        /// <param name="e"></param>
        /// 
        private void VisualPropertyChanged(DependencyPropertyChangedEventArgs e) {

            if (e.OldValue is VisualItem oldVisual)
                contentVisual.Children.Remove(oldVisual);

            if (e.NewValue is VisualItem newVisual)
                contentVisual.Children.Add(newVisual);
        }

        /// <summary>
        /// Acciuo al canviar el valor de la propietat 'TransformMatrix'
        /// </summary>
        /// <param name="e"></param>
        /// 
        private void TransformationMatrixPropertyChanged(DependencyPropertyChangedEventArgs e) {

            contentVisual.Transform = new MatrixTransform((Matrix) e.NewValue);
        }

        /// <summary>
        /// Obte els items en una regio rectangular determinada.
        /// </summary>
        /// <param name="r">Rectangle de la zona.</param>
        /// <returns>Enumerador dels items trobats. Null si no hi ha cap.</returns>
        /// 
        public IEnumerable<VisualItem> GetVisuals(Rect r) {

            if (contentVisual.Children.Count == 1) {

                List<VisualItem> visuals = new List<VisualItem>();

                var visual = contentVisual.Children[0];

                var hitTestParameters = new GeometryHitTestParameters(new RectangleGeometry(r));

                var resultCallback = new HitTestResultCallback(
                    result => HitTestResultBehavior.Continue);

                var filterCallback = new HitTestFilterCallback(
                    element => {
                        visuals.Add(element as VisualItem);
                        return HitTestFilterBehavior.Continue;
                    });

                VisualTreeHelper.HitTest(visual, filterCallback, resultCallback, hitTestParameters);

                return visuals;
            }
            else
                return null;
        }

        /// <summary>
        /// Obte o asigna el visual del contingut.
        /// </summary>
        /// 
        [Bindable(true)]
        [Category("Content")]
        public DrawingVisual Visual {
            get {
                return (DrawingVisual)GetValue(VisualProperty);
            }
            set {
                SetValue(VisualProperty, value);
            }
        }

        /// <summary>
        /// Obte o asigna la matrix de transformacio.
        /// </summary>
        [Bindable(true)]
        [Category("Design")]
        public Matrix TransformMatrix {
            get {
                return (Matrix)GetValue(TransformMatrixProperty);
            }
            set {
                SetValue(TransformMatrixProperty, value);
            }
        }
    }
}
