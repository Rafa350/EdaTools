namespace MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure {

    using System;

    public sealed class SLeaf : SNode {

        private readonly int _position;
        private readonly int _length;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="position">Posicio del text.</param>
        /// <param name="length">Longitut.</param>
        /// 
        public SLeaf(int position, int length) :
            base() {

            if (position < 0)
                throw new ArgumentOutOfRangeException(nameof(position));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            _position = position;
            _length = length;
        }

        /// <summary>
        /// Obte la posicio del text.
        /// </summary>
        /// 
        public int Position => _position;

        /// <summary>
        /// Obte la longitut del text.
        /// </summary>
        /// 
        public int Length => _length;
    }

}
