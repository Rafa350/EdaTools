namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public sealed class DesignSurface: Canvas {

        private readonly List<Visual> visuals = new List<Visual>();

        public DesignSurface() {

        }

        protected override Visual GetVisualChild(int index) {

            return visuals[index] as Visual;
        }

        public void ClearVisual() {

            while (visuals.Count > 0)
                RemoveVisual(visuals[0]);
        }

        public void AddVisual(Visual visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            visuals.Add(visual);
            base.AddVisualChild(visual);
            base.AddLogicalChild(visual);
        }

        public void RemoveVisual(Visual visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            visuals.Remove(visual);
            base.RemoveVisualChild(visual);
            base.RemoveLogicalChild(visual);
        }

        public Visual Locate(Point point) {

            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as Visual;
        }

        public Visual[] Locate(Rect rect) {

            return null;
        }

        protected override int VisualChildrenCount {
            get {
                return visuals.Count;
            }
        }

        public IEnumerable<Visual> Visuals {
            get {
                return visuals;
            }
        }
    }
}
