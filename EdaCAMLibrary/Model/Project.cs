namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;
    using System.Collections.Generic;

    public sealed class Project {

        private readonly string boardFileName;
        private List<Target> targets;

        public Project(string boardFileName, IEnumerable<Target> targets = null) {

            if (String.IsNullOrEmpty(boardFileName))
                throw new ArgumentNullException("boardFileName");

            this.boardFileName = boardFileName;

            if (targets != null)
                foreach (var target in targets)
                    AddTarget(target);
        }

        public void AddTarget(Target target) {

            if (target == null)
                throw new ArgumentNullException("target");

            if (targets == null)
                targets = new List<Target>();

            targets.Add(target);
        }

        public string BoardFileName {
            get {
                return boardFileName;
            }
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
