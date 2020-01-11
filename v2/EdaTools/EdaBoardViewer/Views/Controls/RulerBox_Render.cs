namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    
    public sealed partial class RulerBox: Control {


        public override void Render(DrawingContext context) {

            var brush = new SolidColorBrush(Colors.Yellow);
            context.FillRectangle(brush, new Rect(new Point(0, 0), Bounds.Size));
        }

    }
}
