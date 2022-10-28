﻿using System;
using System.Collections.Generic;
using Avalonia.Media;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace EdaBoardViewer.Render {

    public sealed class VisualLayerStack {

        private static Color _padColor = Color.FromArgb(255, 234, 161, 64);
        private static Color _viaColor = Color.FromArgb(180, 0, 128, 0);
        private static Color _holeColor = Color.FromArgb(255, 50, 50, 50);
        private static Color _topCopperColor = Color.FromArgb(100, 150, 0, 0);
        private static Color _bottomCopperColor = Color.FromArgb(100, 0, 0, 150);

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
                new Type[] { typeof(EdaLineElement), typeof(EdaArcElement), typeof(EdaRectangleElement), typeof(EdaCircleElement), typeof(EdaRegionElement), typeof(EdaTextElement) },
                true,
                VisualMode.Element,
                _bottomCopperColor));

            /*vls.Add(new VisualLayer("Inner2",
                new EdaLayerId[] { EdaLayerId.InnerCopper2 },
                new Type[] { typeof(EdaLineElement), typeof(EdaArcElement), typeof(EdaRectangleElement), typeof(EdaCircleElement), typeof(EdaRegionElement), typeof(EdaTextElement) },
                true,
                VisualMode.Element,
                Color.FromArgb(100, 0, 0, 75)));

            vls.Add(new VisualLayer("Inner1",
                new EdaLayerId[] { EdaLayerId.InnerCopper1 },
                new Type[] { typeof(EdaLineElement), typeof(EdaArcElement), typeof(EdaRectangleElement), typeof(EdaCircleElement), typeof(EdaRegionElement), typeof(EdaTextElement) },
                true,
                VisualMode.Element,
                Color.FromArgb(100, 75, 0, 0)));*/

            vls.Add(new VisualLayer("Top",
                new EdaLayerId[] { EdaLayerId.TopCopper },
                new Type[] { typeof(EdaLineElement), typeof(EdaArcElement), typeof(EdaRectangleElement), typeof(EdaCircleElement), typeof(EdaRegionElement), typeof(EdaTextElement) },
                true,
                VisualMode.Element,
                _topCopperColor));

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
                new Type[] { typeof(EdaThtPadElement), typeof(EdaSmtPadElement) },
                true,
                VisualMode.Element,
                _padColor));

            vls.Add(new VisualLayer("TopCream",
                new EdaLayerId[] { EdaLayerId.TopCream },
                null,
                true,
                VisualMode.Element,
                Color.FromArgb(204, 64, 64, 64)));

            vls.Add(new VisualLayer("Vias",
                new EdaLayerId[] { EdaLayerId.TopCopper },
                new Type[] { typeof(EdaViaElement) },
                true,
                VisualMode.Element,
                _viaColor));

            vls.Add(new VisualLayer("Platted",
                new EdaLayerId[] { EdaLayerId.Platted },
                new Type[] { typeof(EdaViaElement), typeof(EdaThtPadElement), typeof(EdaSmtPadElement), typeof(EdaCircularHoleElement) },
                true,
                VisualMode.Drill,
                _holeColor));

            vls.Add(new VisualLayer("Unplatted",
                new EdaLayerId[] { EdaLayerId.Unplatted },
                new Type[] { typeof(EdaCircleElement), typeof(EdaCircularHoleElement) },
                true,
                VisualMode.Element,
                _holeColor));

            vls.Add(new VisualLayer("Milling",
                new EdaLayerId[] { EdaLayerId.Milling },
                null,
                true,
                VisualMode.Element,
                _holeColor));

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
