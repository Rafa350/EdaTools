namespace EdaBoardViewer.Views.Controls {

    using Avalonia.Controls;

    public enum RulerOrientation {
        Horizontal,
        Vertical
    }

    public enum RulerAlignment {
        RightOrTop,
        LeftOrBottom
    }
    
    public sealed partial class RulerBox: Control {

        static RulerBox() {

            AffectsRender<RulerBox>(
                PointerValueProperty,
                RegionStartValueProperty,
                RegionEndValueProperty,
                MaxValueProperty,
                MinValueProperty,
                ValueDivisorProperty);
        }

    }
}
