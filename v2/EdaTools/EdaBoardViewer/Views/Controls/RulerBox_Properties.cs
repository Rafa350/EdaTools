namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    
    public sealed partial class RulerBox: Control {

        public static readonly StyledProperty<IBrush> TagBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(TagBrush), Brushes.White);
        public static readonly StyledProperty<IBrush> LineBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(LineBrush), Brushes.White);
        
        public static readonly StyledProperty<FontFamily> FontFamilyProperty;
        public static readonly StyledProperty<FontStyle> FontStyleProperty;
        public static readonly StyledProperty<double> FontSizeProperty;
        public static readonly StyledProperty<FontWeight> FontWeightProperty;
        //public static readonly StyledProperty FontStretchProperty;

        public static readonly StyledProperty<double> RegionStartValueProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(RegionStartValue), 0);
        public static readonly StyledProperty<double> RegionEndValueProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(RegionEndValue), 0);
        public static readonly StyledProperty<IBrush> RegionBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(RegionBrush), new SolidColorBrush(Color.FromArgb(0x3F, 0x9A, 0xFB, 0xE1)));
        public static readonly StyledProperty<bool> ShowRegionProperty = AvaloniaProperty.Register<RulerBox, bool>(nameof(ShowRegion), false);
        
        public static readonly StyledProperty<double> PointerValueProperty = AvaloniaProperty.Register<RulerBox, double>(nameof(PointerValue), 0);
        public static readonly StyledProperty<IBrush> PointerBrushProperty = AvaloniaProperty.Register<RulerBox, IBrush>(nameof(PointerBrush), new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)));
        public static readonly StyledProperty<bool> ShowPointerProperty = AvaloniaProperty.Register<RulerBox, bool>(nameof(ShowPointer), false);
        
        public static readonly StyledProperty<Matrix> TransformMatrixProperty;
        //public static readonly StyledProperty RulerAlignmentProperty;
        //public static readonly StyledProperty TransformModeProperty;
        public static readonly StyledProperty<bool> FlipTagsProperty;
        public static readonly StyledProperty<double> ValueDivisorProperty;
        public static readonly StyledProperty<double> MinValueProperty;
        public static readonly StyledProperty<double> MaxValueProperty;

        public IBrush TagBrush {
            get { return GetValue(TagBrushProperty); }
            set { SetValue(TagBrushProperty, value); }
        }

        public IBrush LineBrush {
            get { return GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        public double RegionStartValue {
            get { return GetValue(RegionStartValueProperty); }
            set { SetValue(RegionStartValueProperty, value); }
        }

        public double RegionEndValue {
            get { return GetValue(RegionEndValueProperty); }
            set { SetValue(RegionEndValueProperty, value); }
        }

        public IBrush RegionBrush {
            get { return GetValue(RegionBrushProperty); }
            set { SetValue(RegionBrushProperty, value); }
        }

        public bool ShowRegion {
            get { return GetValue(ShowRegionProperty); }
            set { SetValue(ShowRegionProperty, value); }
        }

        public double PointerValue {
            get { return GetValue(PointerValueProperty); }
            set { SetValue(PointerValueProperty, value); }
        }

        public IBrush PointerBrush {
            get { return GetValue(PointerBrushProperty); }
            set { SetValue(PointerBrushProperty, value); }
        }

        public bool ShowPointer {
            get { return GetValue(ShowPointerProperty); }
            set { SetValue(ShowPointerProperty, value); }
        }
    }
}
