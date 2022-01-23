namespace EdaBoardViewer.Render {

    using Avalonia.Media;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System;
    using System.Collections.Generic;

    public sealed class VisualLayerStack {

        private readonly List<VisualLayer> layers = new List<VisualLayer>();

        public void Add(VisualLayer layer) {

            if (layer == null)
                throw new ArgumentNullException(nameof(layer));

            if (layers.Contains(layer))
                throw new InvalidOperationException("Ya existe esta capa en la lista.");

            layers.Add(layer);
        }

        public IEnumerable<VisualLayer> VisualLayers {
            get {
                return layers;
            }
        }

        public static VisualLayerStack CreateDefault() {

            var vls = new VisualLayerStack();

            vls.Add(new VisualLayer("Bottom",
                new EdaLayerId[] { EdaLayerId.BottomCopper },
                new ElementType[] { ElementType.Line, ElementType.Arc, ElementType.Rectangle, ElementType.Circle, ElementType.Region, ElementType.SmdPad, ElementType.Text },
                true,
                VisualMode.Element,
                Color.FromArgb(100, 0, 0, 150)));

            vls.Add(new VisualLayer("Inner2",
                new EdaLayerId[] { EdaLayerId.InnerCopper2 },
                new ElementType[] { ElementType.Line, ElementType.Arc, ElementType.Rectangle, ElementType.Circle, ElementType.Region, ElementType.SmdPad, ElementType.Text },
                true,
                VisualMode.Element,
                Color.FromArgb(100, 0, 0, 75)));

            vls.Add(new VisualLayer("Inner1",
                new EdaLayerId[] { EdaLayerId.InnerCopper1 },
                new ElementType[] { ElementType.Line, ElementType.Arc, ElementType.Rectangle, ElementType.Circle, ElementType.Region, ElementType.SmdPad, ElementType.Text },
                true,
                VisualMode.Element,
                Color.FromArgb(100, 75, 0, 0)));

            vls.Add(new VisualLayer("Top",
                new EdaLayerId[] { EdaLayerId.TopCopper },
                new ElementType[] { ElementType.Line, ElementType.Arc, ElementType.Rectangle, ElementType.Circle, ElementType.Region, ElementType.SmdPad, ElementType.Text },
                true,
                VisualMode.Element,
                Color.FromArgb(100, 150, 0, 0)));

            vls.Add(new VisualLayer("TopPlace",
                new EdaLayerId[] { EdaLayerId.TopPlace },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(204, 211, 211, 211)));

            vls.Add(new VisualLayer("TopDocument",
                new EdaLayerId[] { EdaLayerId.TopDocument },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(204, 160, 160, 160)));

            vls.Add(new VisualLayer("TopNames",
                new EdaLayerId[] { EdaLayerId.TopNames },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(255, 211, 211, 211)));

            vls.Add(new VisualLayer("Pads",
                new EdaLayerId[] { EdaLayerId.TopCopper },
                new ElementType[] { ElementType.ThPad },
                true,
                VisualMode.Element,
                Color.FromArgb(255, 234, 161, 64)));

            vls.Add(new VisualLayer("Vias",
                new EdaLayerId[] { EdaLayerId.TopCopper },
                new ElementType[] { ElementType.Via },
                true,
                VisualMode.Element,
                Color.FromArgb(180, 0, 128, 0)));

            vls.Add(new VisualLayer("Drils",
                new EdaLayerId[] { EdaLayerId.Drills },
                new ElementType[] { ElementType.Via, ElementType.ThPad },
                true,
                VisualMode.Drill,
                Color.FromArgb(255, 255, 255, 255)));

            vls.Add(new VisualLayer("Holes",
                new EdaLayerId[] { EdaLayerId.Unplatted },
                new ElementType[] { ElementType.Circle },
                true,
                VisualMode.Element,
                Color.FromArgb(255, 240, 128, 128)));

            vls.Add(new VisualLayer("Milling",
                new EdaLayerId[] { EdaLayerId.Milling },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(255, 32, 216, 200)));

            vls.Add(new VisualLayer("Keepout",
                new EdaLayerId[] { EdaLayerId.TopKeepout },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(255, 64, 64, 64)));

            vls.Add(new VisualLayer("Profiles",
                new EdaLayerId[] { EdaLayerId.Profile },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(255, 255, 255, 255)));

            return vls;
        }
    }
}
