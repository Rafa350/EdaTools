using System;
using System.Collections.Generic;
using System.Globalization;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Primitives;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal class PrimitiveDictionary {

        private readonly Dictionary<int, Primitive> _primitives = new Dictionary<int, Primitive>();
        private int _primitiveId = 0;

        /// <summary>
        /// Obte un identificador unic per una primitiva de tipus 'Circle'.
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
        /// Obte un identificador unic per una primitiva de tipus 'RectRound.
        /// </summary>
        /// <param name="width">L'amplada.</param>
        /// <param name="height">L'alçada.</param>
        /// <param name="radius">El radi de curvatura.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'identificador unic.</returns>
        /// 
        private static int GetRectRoundId(int width, int height, int radius, string tag) {

            string s = String.Format(CultureInfo.InvariantCulture, "RectRound;{0};{1};{2};{3}", width, height, radius, tag);
            return s.GetHashCode();
        }

        /// <summary>
        /// Defineix una primitiva de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">El diametre.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>La primitiva.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public Primitive DefineCirclePrimitive(int diameter, string tag = null) {

            if (diameter <= 0)
                throw new ArgumentOutOfRangeException(nameof(diameter));

            int key = GetCircleId(diameter, tag);
            if (!_primitives.TryGetValue(key, out Primitive primitive)) {
                primitive = new CirclePrimitive(_primitiveId++, tag, diameter);
                _primitives.Add(key, primitive);
            }

            return primitive;
        }

        /// <summary>
        /// Defineix una primitiva de tipus 'RectRound'.
        /// </summary>
        /// <param name="tag">El tag.</param>
        /// <param name="width">L'amplada.</param>
        /// <param name="height">L'alçada.</param>
        /// <param name="radius">El radi de curvatura.</param>
        /// <returns>La primitiva.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public Primitive DefineRoundRectPrimitive(int width, int height, int radius, string tag = null) {

            if (width <= 0)
                throw new ArgumentNullException(nameof(width));

            if (height <= 0)
                throw new ArgumentNullException(nameof(height));

            if (radius < 0)
                throw new ArgumentNullException(nameof(radius));

            int key = GetRectRoundId(width, height, radius, tag);
            if (!_primitives.TryGetValue(key, out Primitive primitive)) {
                primitive = new RectRoundPrimitive(_primitiveId++, tag, width, height, radius);
                _primitives.Add(key, primitive);
            }

            return primitive;
        }

        /// <summary>
        /// Obte una primitiva de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La primitiva.</returns>
        /// 
        public Primitive GetCirclePrimitive(int diameter, string tag = null) {

            int id = GetCircleId(diameter, tag);
            return _primitives[id];
        }

        /// <summary>
        /// Obte una primitiva de tipus 'RectRound'.
        /// </summary>
        /// <param name="width">L'amplada.</param>
        /// <param name="height">L'alçada.</param>
        /// <param name="radius">El radi de curvatura.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>La primitiva.</returns>
        /// 
        public Primitive GetRectRoundPrimitive(int width, int height, int radius, string tag = null) {

            int id = GetRectRoundId(width, height, radius, tag);
            return _primitives[id];
        }

        /// <summary>
        /// Obte les primitives definides.
        /// </summary>
        /// 
        public IEnumerable<Primitive> Primitives =>
            _primitives.Values;
    }
}
