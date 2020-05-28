namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;

    public sealed partial class DesignControl : Control {

        /// <summary>
        /// Constructor estatic.
        /// </summary>
        /// 
        static DesignControl() {

            AffectsRender<DesignControl>(
                ShowPointerProperty,
                ShowPointerTagsProperty,
                PointerPositionProperty,

                ShowRegionProperty,
                ShowRegionTagsProperty,
                ShowRegionHandlesProperty,
                RegionPositionProperty,
                RegionSizeProperty,
                RegionBackgroundProperty,
                RegionBorderColorProperty,
                RegionTagBackgroundProperty,
                RegionTagBorderColorProperty,
                
                ValueDivisorProperty,
                ValueMatrixProperty);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public DesignControl() {

            ClipToBounds = true;
        }

        private Point TransformPoint(Point p) {

            Matrix matrix = ValueMatrix;

            return new Point(
                (p.X * matrix.M11) + (p.Y * matrix.M21) + matrix.M31,
                (p.X * matrix.M12) + (p.Y * matrix.M22) + matrix.M32);
        }

    }
}
