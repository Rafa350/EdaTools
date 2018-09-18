﻿namespace MikroPic.EdaTools.v1.Designer.Render {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Collections;
    using MikroPic.EdaTools.v1.Geometry;

    public sealed class VisualLayer {

        private readonly SetOf<LayerId> layerSet;
        private readonly string name;
        private bool visible;
        private Color color;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="layerSet">Conjunt de capes de la placa.</param>
        /// 
        public VisualLayer(string name, SetOf<LayerId> layerSet):
            this(name, layerSet, true, new Color(255, 128, 128, 128)) {

        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Nom de la capa.</param>
        /// <param name="layerSet">Conjunt de capes de la placa.</param>
        /// <param name="visible">Indica si es visible.</param>
        /// <param name="color">Color</param>
        /// 
        public VisualLayer(string name, SetOf<LayerId> layerSet, bool visible, Color color) {

            this.name = name;
            this.layerSet = layerSet;
            this.visible = visible;
            this.color = color;
        }

        /// <summary>
        /// Obte el nom de la capa
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte el conjunt de capes de la placa que es visualitzen en aquesta capa.
        /// </summary>
        /// 
        public SetOf<LayerId> Layers {
            get {
                return layerSet;
            }
        }

        /// <summary>
        /// Obte o asigna el indicador de visibilitat.
        /// </summary>
        /// 
        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
            }
        }

        /// <summary>
        /// Obte o asigna el color.
        /// </summary>
        /// 
        public Color Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }
    }
}
