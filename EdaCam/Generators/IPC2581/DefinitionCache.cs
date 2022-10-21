using System;
using System.Collections.Generic;
using System.Globalization;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal class DefinitionCache {

        private readonly Dictionary<int, Definition> _definitions = new Dictionary<int, Definition>();
        private int _definitionId = 0;

        /// <summary>
        /// Obte un identificador unic per una definicio de tipus 'Line'.
        /// </summary>
        /// <param name="width">L'amplada de linia.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        private static int GetLineId(int width, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "Line;{0};{1}", width, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte un identificador unic per una definicio de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">El diametre.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        private static int GetCircleId(int diameter, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "Circle;{0};{1}", diameter, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Obte un identificador unic per una definicio de tipus 'RectRound.
        /// </summary>
        /// <param name="size">El tamany</param>
        /// <param name="ratio">El ratio de curvatura.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        private static int GetRectRoundId(EdaSize size, EdaRatio ratio, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "RectRound;{0};{1};{2};{3}", size.Width, size.Height, ratio.AsPercent, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Declara una definicio de  tipus 'Line'.
        /// </summary>
        /// <param name="width">L'amplada de linia.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>La definicio.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public LineDefinition DeclareLineDefinition(int width, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            int key = GetLineId(width, tag);
            if (!_definitions.TryGetValue(key, out Definition definition)) {
                definition = new LineDefinition(_definitionId++, tag, width);
                _definitions.Add(key, definition);
            }

            return (LineDefinition) definition;
        }

        /// <summary>
        /// Declara una definicio de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">El diametre.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>La definicio.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public CircleDefinition DeclareCircleDefinition(int diameter, string tag = null) {

            if (diameter <= 0)
                throw new ArgumentOutOfRangeException(nameof(diameter));

            int key = GetCircleId(diameter, tag);
            if (!_definitions.TryGetValue(key, out Definition definition)) {
                definition = new CircleDefinition(_definitionId++, tag, diameter);
                _definitions.Add(key, definition);
            }

            return (CircleDefinition) definition;
        }

        /// <summary>
        /// Declara una definicio de tipus 'RectRound'.
        /// </summary>
        /// <param name="tag">El tag.</param>
        /// <param name="width">L'amplada.</param>
        /// <param name="height">L'alçada.</param>
        /// <param name="radius">El radi de curvatura.</param>
        /// <returns>La definicio.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public RectRoundDefinition DeclareRectRoundDefinition(EdaSize size, EdaRatio ratio, string tag = null) {

            int key = GetRectRoundId(size, ratio, tag);
            if (!_definitions.TryGetValue(key, out Definition definition)) {
                definition = new RectRoundDefinition(_definitionId++, tag, size, ratio);
                _definitions.Add(key, definition);
            }

            return (RectRoundDefinition) definition;
        }

        /// <summary>
        /// Obte una definicio de tipus 'Line'.
        /// </summary>
        /// <param name="width">L'amplada de linia.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La definicio.</returns>
        /// 
        public LineDefinition GetLineDefinition(int width, string tag = null) {

            int id = GetLineId(width, tag);
            return (LineDefinition) _definitions[id];
        }

        /// <summary>
        /// Obte una definicio de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La definicio.</returns>
        /// 
        public CircleDefinition GetCircleDefinition(int diameter, string tag = null) {

            int id = GetCircleId(diameter, tag);
            return (CircleDefinition) _definitions[id];
        }

        /// <summary>
        /// Obte una definicio de tipus 'RectRound'.
        /// </summary>
        /// <param name="size">El tamany</param>
        /// <param name="ratio">El ratio de curvatura.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La definicio.</returns>
        /// 
        public RectRoundDefinition GetRectRoundDefinition(EdaSize size, EdaRatio ratio, string tag = null) {

            int id = GetRectRoundId(size, ratio, tag);
            return (RectRoundDefinition) _definitions[id];
        }

        /// <summary>
        /// Obte les primitives definides.
        /// </summary>
        /// 
        public IEnumerable<Definition> Definitions =>
            _definitions.Values;
    }
}
