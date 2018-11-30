namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public static class ElementExtensions {

        /// <summary>
        /// Comprova si l'element es en una capa conpreta
        /// </summary>
        /// <param name="layerId">El identificador de la capa.</param>
        /// <returns>True si es en la capa especificada.</returns>
        /// 
        public static bool IsOnLayer(this Element element, LayerId layerId) {

            return element.LayerSet.Contains(layerId);
        }
    }
}
