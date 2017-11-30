namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Collections.Generic;

    public sealed class GerberApertureTable {

        private readonly List<GerberAperture> apertures = new List<GerberAperture>();

        public GerberApertureTable Add(GerberAperture aperture) {

            if (aperture == null)
                throw new ArgumentNullException("aperture");

            if (apertures.Contains(aperture))
                throw new InvalidOperationException("La apertura ya pertenece a la tabla.");

            apertures.Add(aperture);

            return this;
        }

        public IEnumerable<GerberAperture> Apertures {
            get {
                return apertures;
            }
        }
    }
}
