namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Project {

        private Panel panel;
        private List<Target> targets;

        public Project() {

        }

        public void Addtarget(Target target) {

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

        public Panel Panel {
            get {
                return panel;
            }
            set {
                panel = value;
            }
        }
    }
}
