namespace EdaBoardViewer.Views.Controls {

    using System;
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

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) {

            if ((e.Property == ScaleProperty) ||
                (e.Property == OriginProperty) ||
                (e.Property == RegionStartValueProperty) ||
                (e.Property == RegionEndValueProperty) ||
                (e.Property == ValueDivisorProperty) ||
                (e.Property == OrientationProperty) ||
                (e.Property == BoundsProperty)) {

                regionGeometryCache = null;
            }

            if ((e.Property == ScaleProperty) ||
                (e.Property == OriginProperty) ||
                (e.Property == PointerValueProperty) ||
                (e.Property == ValueDivisorProperty) ||
                (e.Property == OrientationProperty) ||
                (e.Property == BoundsProperty)) {

                pointerGeometryCache = null;
            }

            base.OnPropertyChanged(e);
        }

    }
}
