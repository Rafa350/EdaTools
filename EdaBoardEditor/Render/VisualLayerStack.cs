﻿namespace MikroPic.EdaTools.v1.BoardEditor.Render {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System;
    using System.Collections.Generic;

    public sealed class VisualLayerStack {

        private readonly List<VisualLayer> layers = new List<VisualLayer>();

        public void Add(VisualLayer layer) {

            if (layer == null)
                throw new ArgumentNullException("layer");

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

            VisualLayerStack vls = new VisualLayerStack();

            vls.Add(new VisualLayer("Bottom", 
                new string[] { "Bottom.Copper" },
                new ElementType[] { ElementType.Line, ElementType.Arc, ElementType.Region, ElementType.SmdPad },
                true,
                VisualMode.Element,
                new Color(204, 0, 0, 255)));

            vls.Add(new VisualLayer("Top",
                new string[] { "Top.Copper" },
                new ElementType[] { ElementType.Line, ElementType.Arc, ElementType.Region, ElementType.SmdPad },
                true,
                VisualMode.Element,
                new Color(153, 255, 0, 0)));

            vls.Add(new VisualLayer("TopPlace", 
                new string[] { "Top.Place" }, 
                null,
                true,
                VisualMode.Element,
                new Color(204, 211, 211, 211)));

            vls.Add(new VisualLayer("TopDocument", 
                new string[] { "Top.Document" }, 
                null,
                true,
                VisualMode.Element,
                new Color(204, 160, 160, 160)));

            vls.Add(new VisualLayer("TopNames", 
                new string[] { "Top.Names" }, 
                null,
                true,
                VisualMode.Element,
                new Color(255, 211, 211, 211)));
                
            vls.Add(new VisualLayer("Pads", 
                new string[] { "Top.Copper" }, 
                new ElementType[] { ElementType.ThPad },
                true,
                VisualMode.Element,
                new Color(255, 234, 161, 64)));

            vls.Add(new VisualLayer("Vias",
                new string[] { "Top.Copper" },
                new ElementType[] { ElementType.Via },
                true,
                VisualMode.Element,
                new Color(180, 0, 128, 0)));
            
            //vls.Add(new VisualLayer("Drills", new LayerSet(Layer.DrillsId), true, new Color(255, 255, 255, 255)));
            vls.Add(new VisualLayer("Holes", 
                new string[] { "Holes" }, 
                null,
                true,
                VisualMode.Element,
                new Color(255, 240, 128, 128)));

            vls.Add(new VisualLayer("Milling", 
                new string[] { "Milling" },
                null,
                true,
                VisualMode.Element,
                new Color(255, 32, 216, 200)));

            vls.Add(new VisualLayer("Profiles", 
                new string[] { "Profile" },
                null,
                true,
                VisualMode.Element,
                new Color(255, 255, 255, 255)));

            return vls;
        }
    }
}