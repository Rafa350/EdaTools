namespace MikroPic.EdaTools.v1.PanelEditor.Render.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model;
    using System;
    using System.Windows.Media;

    public sealed class ProjectVisual: VisualBase {

        private readonly Color panelColor = Color.FromRgb(44, 115, 13);
        private readonly Project project;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">Visual pare.</param>
        /// <param name="project">El projecte.</param>
        /// 
        public ProjectVisual(DrawingVisual parent, Project project):
            base(parent) {

            if (project == null)
                throw new ArgumentNullException("project");

            this.project = project;
        }

        /// <summary>
        /// Renderitzat.
        /// </summary>
        /// <param name="dc">Context de renderitzat.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            Brush brush = dc.GetBrush(panelColor);

            Base.Geometry.Size size = Project.Size;
            Base.Geometry.Point position = new Base.Geometry.Point(size.Width / 2, size.Height / 2);
            dc.DrawRectangle(brush, null, position, size);
        }

        /// <summary>
        /// Obte el projecte associat.
        /// </summary>
        /// 
        public Project Project {
            get {
                return project;
            }
        }
    }
}
