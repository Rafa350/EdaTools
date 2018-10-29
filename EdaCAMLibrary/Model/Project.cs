namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;

    public sealed class Project {

        private List<Target> targets;

        public Project() {

        }

        public void AddTarget(Target target) {

            if (target == null)
                throw new ArgumentNullException("target");

            if (targets == null)
                targets = new List<Target>();

            targets.Add(target);
        }

        public bool HasTargets {
            get {
                return targets != null;
            }
        }

        public IEnumerable<Target> Targets {
            get {
                return targets;
            }
        }
    }
}
