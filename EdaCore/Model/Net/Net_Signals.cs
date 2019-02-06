namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System.Collections.Generic;

    public sealed partial class Net {

        private Dictionary<string, NetSignal> signals;

        private void InitializeSignals(IEnumerable<NetSignal> signals) {

            this.signals = new Dictionary<string, NetSignal>();
            foreach (NetSignal signal in signals)
                this.signals.Add(signal.Name, signal);
        }

        /// <summary>
        /// Indica si contre senyals.
        /// </summary>
        /// 
        public bool HasSignals {
            get {
                return signals != null;
            }
        }

        /// <summary>
        /// Enumera els noms de les senyals.
        /// </summary>
        /// 
        public IEnumerable<string> SignalNames {
            get {
                return signals?.Keys;
            }
        }

        /// <summary>
        /// Enumera les senyals.
        /// </summary>
        /// 
        public IEnumerable<NetSignal> Signals {
            get {
                return signals?.Values;
            }
        }
    }
}
