using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal class DataCache {

        private readonly Dictionary<int, DataCacheEntry> _entries = new Dictionary<int, DataCacheEntry>();
        private int _entryId = 0;

        /// <summary>
        /// Afegeig una entrada de tipus de  tipus 'LineDesc'.
        /// </summary>
        /// <param name="width">L'amplada de linia.</param>
        /// <param name="capStyle">Estil del extrem de linia.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'entrada.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public LineDescEntry AddLineDescEntry(int width, EdaLineCap capStyle, string tag = null) {

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            int key = LineDescEntry.GetId(width, capStyle, tag);
            if (!_entries.TryGetValue(key, out DataCacheEntry entry)) {
                entry = new LineDescEntry(_entryId++, tag, width, capStyle);
                _entries.Add(key, entry);
            }

            return (LineDescEntry)entry;
        }

        /// <summary>
        /// Afegeig una entrada de tipus de tipus 'FillDesc'.
        /// </summary>
        /// <param name="fiil">El tipus de farciment.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'entrada.</returns>
        /// 
        public FillDescEntry AddFillDescEntry(bool fill, string tag = null) {

            int key = FillDescEntry.GetId(fill, tag);
            if (!_entries.TryGetValue(key, out DataCacheEntry entry)) {
                entry = new FillDescEntry(_entryId++, tag, fill);
                _entries.Add(key, entry);
            }

            return (FillDescEntry)entry;
        }

        /// <summary>
        /// Afegeix una entrada de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">El diametre.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'entrada.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public CircleEntry AddCircleEntry(int diameter, string tag = null) {

            if (diameter <= 0)
                throw new ArgumentOutOfRangeException(nameof(diameter));

            int key = CircleEntry.GetId(diameter, tag);
            if (!_entries.TryGetValue(key, out DataCacheEntry entry)) {
                entry = new CircleEntry(_entryId++, tag, diameter);
                _entries.Add(key, entry);
            }

            return (CircleEntry)entry;
        }

        /// <summary>
        /// Afegeix una entrada de tipus 'RectRound'.
        /// </summary>
        /// <param name="size">El tamany.</param>
        /// <param name="ratio">El ratio de curvatura de les cantonades.</param>
        /// <param name="tag">El tag.</param>
        /// <returns>L'entrada.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// 
        public RectRoundEntry AddRectRoundEntry(EdaSize size, EdaRatio ratio, string tag = null) {

            int key = RectRoundEntry.GetId(size, ratio, tag);
            if (!_entries.TryGetValue(key, out DataCacheEntry entry)) {
                entry = new RectRoundEntry(_entryId++, tag, size, ratio);
                _entries.Add(key, entry);
            }

            return (RectRoundEntry)entry;
        }

        /// <summary>
        /// Obte una entrada de tipus 'LineDesc'.
        /// </summary>
        /// <param name="width">L'amplada de linia.</param
        /// <param name="capStyle">Estil del extrem de linia.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'entrada.</returns>
        /// 
        public LineDescEntry GetLineDescEntry(int width, EdaLineCap capStyle, string tag = null) {

            int id = LineDescEntry.GetId(width, capStyle, tag);
            return (LineDescEntry)_entries[id];
        }

        /// <summary>
        /// Obte una entrada de tipus 'FillDesc'.
        /// </summary>
        /// <param name="fill">El tipus de farciment.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'entrada.</returns>
        /// 
        public FillDescEntry GetFillDescEntry(bool fill, string tag = null) {

            int id = FillDescEntry.GetId(fill, tag);
            return (FillDescEntry)_entries[id];
        }

        /// <summary>
        /// Obte una entrada de tipus 'Circle'.
        /// </summary>
        /// <param name="diameter">Diametre.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'entrada.</returns>
        /// 
        public CircleEntry GetCircleEntry(int diameter, string tag = null) {

            int id = CircleEntry.GetId(diameter, tag);
            return (CircleEntry)_entries[id];
        }

        /// <summary>
        /// Obte una entrada de tipus 'RectRound'.
        /// </summary>
        /// <param name="size">El tamany</param>
        /// <param name="ratio">El ratio de curvatura.</param>
        /// <param name="tag">Etiqueta.</param>
        /// <returns>L'entrada.</returns>
        /// 
        public RectRoundEntry GetRectRoundEntry(EdaSize size, EdaRatio ratio, string tag = null) {

            int id = RectRoundEntry.GetId(size, ratio, tag);
            return (RectRoundEntry)_entries[id];
        }

        /// <summary>
        /// Obte les primitives definides.
        /// </summary>
        /// 
        public IEnumerable<DataCacheEntry> Entries =>
            _entries.Values;
    }
}
