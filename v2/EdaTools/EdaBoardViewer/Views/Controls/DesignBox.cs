namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;

    public sealed partial class DesignBox: Control {

        /// <summary>
        /// Constructor estatic.
        /// </summary>
        /// 
        static DesignBox() {

            AffectsRender<DesignBox>(
                PointerPositionProperty,

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

        /// <summary>
        /// Notoifica que ha canviat una propietat.
        /// </summary>
        /// <param name="e">Arguments de la notificacio.</param>
        /// 
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) {

            base.OnPropertyChanged(e);
        }
    }
}
