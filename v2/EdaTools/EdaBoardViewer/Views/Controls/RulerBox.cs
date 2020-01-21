namespace EdaBoardViewer.Views.Controls {

    using Avalonia.Controls;

    public enum RulerOrientation {
        Horizontal,
        Vertical
    }

    public enum RulerAlignment {
        Top,
        Bottom
    }

    public enum RulerDirection {
        LeftToRight,
        RightToLeft
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
    }
}
