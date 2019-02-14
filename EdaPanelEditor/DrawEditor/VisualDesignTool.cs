namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    using System;
    using System.Windows;
    using System.Windows.Media;

    public delegate void DesignToolDrawVisualEventHandler(object sender, DrawingContext dc, Point startPosition, Point endPosition);

    public class VisualDesignTool : DesignTool {

        public event DesignToolDrawVisualEventHandler OnDrawVisual;

        private readonly DrawingVisual visual;

        public VisualDesignTool(DrawingVisual visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            this.visual = visual;
        }

        /// <summary>
        /// Mostra la visual de l'eina.
        /// </summary>
        /// 
        private void ShowVisual() {

            visual.Opacity = 1;
            using (DrawingContext dc = visual.RenderOpen())
                DoDrawVisual(dc, StartPosition, EndPosition);
        }

        /// <summary>
        /// Oculta la visual de l'eina.
        /// </summary>
        /// 
        private void HideVisual() {

            visual.Opacity = 0;
        }

        /// <summary>
        /// Dibuixa la visual de l'eina.
        /// </summary>
        /// <param name="dc">Contex de dibuix.</param>
        /// <param name="startPosition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// 
        protected virtual void DoDrawVisual(DrawingContext dc, Point startPosition, Point endPosition) {

            OnDrawVisual?.Invoke(this, dc, startPosition, endPosition);
        }

        
        protected override void DoMouseMove() {

            base.DoMouseMove();
            ShowVisual();
        }

        protected override void DoMouseDown() {

            base.DoMouseDown();
            ShowVisual();
        }

        protected override void DoMouseUp() {

            base.DoMouseUp();
            HideVisual();
        }

        /// <summary>
        /// Obte la visual de l'eina.
        /// </summary>
        /// 
        public DrawingVisual Visual {
            get {
                return visual;
            }
        }
    }
}
