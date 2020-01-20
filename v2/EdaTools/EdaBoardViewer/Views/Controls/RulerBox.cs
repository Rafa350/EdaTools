namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;

    public enum RulerOrientation {
        Horizontal,
        Vertical
    }

    public enum RulerAlignment {
        RightOrTop,
        LeftOrBottom
    }

    public sealed partial class RulerBox : Control {

        /// <summary>
        /// Constructor estatic.
        /// </summary>
        /// 
        static RulerBox() {

            AffectsRender<RulerBox>(
                PointerPositionProperty,
                RegionPositionProperty,
                RegionSizeProperty,
                MaxValueProperty,
                MinValueProperty,
                ValueDivisorProperty,
                OriginProperty,
                ScaleProperty);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public RulerBox() {

            ClipToBounds = true;
        }

        /// <summary>
        /// Notifica que una propietat ha canviat el seu valor.
        /// </summary>
        /// <param name="e">Arguments de la notificacio.</param>
        /// 
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) {

            if ((e.Property == ScaleProperty) ||
                (e.Property == OriginProperty) ||
                (e.Property == ValueDivisorProperty) ||
                (e.Property == OrientationProperty) ||
                (e.Property == BoundsProperty)) {

                rulerGeometryCache = null;
            }

            base.OnPropertyChanged(e);
        }

    }
}
