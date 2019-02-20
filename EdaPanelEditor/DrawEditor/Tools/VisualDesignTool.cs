namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor.Tools {

    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    public delegate void DesignToolDrawVisualEventHandler(object sender, DrawingContext dc, Point startPosition, Point endPosition);

    public class VisualDesignTool : DesignTool {

        public event DesignToolDrawVisualEventHandler DrawVisual;

        private const double tickInterval = 100;

        private readonly DispatcherTimer dispatcher;
        private readonly IVisualContainer visualContainer;
        private VisualItem visualItem;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="visualContainer">El contenidor visual.</param>
        /// 
        public VisualDesignTool(IVisualContainer visualContainer) {

            if (visualContainer == null)
                throw new ArgumentNullException("visualContainer");

            this.visualContainer = visualContainer;

            dispatcher = new DispatcherTimer();
            dispatcher.Interval = TimeSpan.FromMilliseconds(tickInterval);
            dispatcher.Tick += Dispatcher_Tick;
        }

        /// <summary>
        /// Procesa l'event 'Tick' del objecte 'dispatcher'
        /// </summary>
        /// <param name="sender">L'objecte que ha generat l'event.</param>
        /// <param name="e">Parametres de l'event.</param>
        /// 
        private void Dispatcher_Tick(object sender, EventArgs e) {

            OnTick();
            if (visualItem != null)
                visualItem.Refresh();
        }

        /// <summary>
        /// Mostra la visual de l'eina.
        /// </summary>
        /// 
        private void ShowVisual() {

            if (visualItem == null)
                visualItem = new VisualItem();

            visualContainer.AddVisualItem(visualItem);
            using (DrawingContext dc = visualItem.RenderOpen())
                OnDrawVisual(dc, StartPosition, EndPosition);
        }

        /// <summary>
        /// Oculta la visual de l'eina.
        /// </summary>
        /// 
        private void HideVisual() {

            visualContainer.RemoveVisualItem(visualItem);
        }

        /// <summary>
        /// Dibuixa la visual de l'eina.
        /// </summary>
        /// <param name="dc">Contex de dibuix.</param>
        /// <param name="startPosition">Posicio inicial.</param>
        /// <param name="endPosition">Posicio final.</param>
        /// 
        protected virtual void OnDrawVisual(DrawingContext dc, Point startPosition, Point endPosition) {

            DrawVisual?.Invoke(this, dc, startPosition, endPosition);
        }

        /// <summary>
        /// Executa l'accio corresponent al tick del temporitzador.
        /// </summary>
        /// 
        protected virtual void OnTick() {
        }
        
        /// <summary>
        /// Executa l'accio corresponent a moure el mouse.
        /// </summary>
        /// 
        protected override void OnMouseMove() {

            base.OnMouseMove();
            ShowVisual();
        }

        protected override void OnMouseDown() {

            base.OnMouseDown();
            ShowVisual();
        }

        protected override void OnMouseUp() {

            base.OnMouseUp();
            HideVisual();
        }

        /// <summary>
        /// Obte la visual de l'eina.
        /// </summary>
        /// 
        public VisualItem Visual {
            get {
                return visualItem;
            }
        }
    }
}
