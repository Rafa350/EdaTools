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

    public sealed partial class RulerControl: Control {

        /// <summary>
        /// Constructor estatic.
        /// </summary>
        /// 
        static RulerControl() {

            AffectsRender<RulerControl>(
                PointerPositionProperty,
                RegionPositionProperty,
                RegionSizeProperty,
                MaxValueProperty,
                MinValueProperty,
                ValueMatrixProperty,
                ValueDivisorProperty);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public RulerControl() {

            ClipToBounds = true;
        }
    }
}
