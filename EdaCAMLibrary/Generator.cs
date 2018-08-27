﻿namespace MikroPic.EdaTools.v1.Cam {

    using System;
    using MikroPic.EdaTools.v1.Cam.Model;

    public abstract class Generator {

        private readonly Target target;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public Generator(Target target) {

            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        /// <summary>
        /// Genera el fitxer de contingut.
        /// </summary>
        /// 
        public abstract void GenerateContent(Panel panel);

        /// <summary>
        /// Obte el target.
        /// </summary>
        /// 
        public Target Target {
            get {
                return target;
            }
        }
    }
}
