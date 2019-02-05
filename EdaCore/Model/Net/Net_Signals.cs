namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed partial class Net {

        private Dictionary<string, NetSignal> signals;

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="signal">L'element a afeigit.</param>
        /// 
        public void AddSignal(NetSignal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (signals == null)
                signals = new Dictionary<string, NetSignal>();
            signals.Add(signal.Name, signal);
        }

        public bool HasSignals {
            get {
                return signals != null;
            }
        }

        public IEnumerable<string> SignalNames {
            get {
                return signals?.Keys;
            }
        }

        public IEnumerable<NetSignal> Signals {
            get {
                return signals?.Values;
            }
        }
    }
}
