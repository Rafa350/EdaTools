﻿namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public static class ElementExtensions {

        /// <summary>
        /// Comprova si l'element es en una capa conpreta
        /// </summary>
        /// <param name="id">El identificador de la capa.</param>
        /// <returns>True si es en la capa especificada.</returns>
        /// 
        public static bool IsOnLayer(this Element element, LayerId id) =>
            element.LayerSet.Contains(id);

        /// <summary>
        /// Comprova si l'element es en una capa conpreta
        /// </summary>
        /// <param name="id">La capa.</param>
        /// <returns>True si es en la capa especificada.</returns>
        /// 
        public static bool IsOnLayer(this Element element, Layer layer) =>
            element.LayerSet.Contains(layer.Id);
    }
}
