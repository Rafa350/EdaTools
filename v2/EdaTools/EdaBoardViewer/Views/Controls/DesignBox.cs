namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;

    public sealed partial class DesignBox : Control {

        /// <summary>
        /// Constructor estatic.
        /// </summary>
        /// 
        static DesignBox() {

            AffectsRender<DesignBox>(
                ShowPointerProperty,
                PointerPositionProperty,

                ShowRegionProperty,
                RegionPositionProperty,
                RegionSizeProperty,
                RegionBackgroundProperty,
                RegionBorderColorProperty,
                RegionTagBackgroundProperty,
                RegionTagBorderColorProperty);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public DesignBox() {

            ClipToBounds = true;
        }
    }
}
