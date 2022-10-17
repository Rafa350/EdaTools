namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed partial class Net {

        private readonly Dictionary<string, NetSignal> _signals = new Dictionary<string, NetSignal>();

        private void InitializeSignals(IEnumerable<NetSignal> signals) {

            foreach (NetSignal signal in signals)
                this._signals.Add(signal.Name, signal);
        }

        public NetSignal GetSignal(string name, bool throwOnError = true) {

            if (_signals.TryGetValue(name, out var signal))
                return signal;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la señal '{0}'.", name));
            else
                return null;
        }


        /// <summary>
        /// Indica si contre senyals.
        /// </summary>
        /// 
        public bool HasSignals {
            get {
                return _signals.Count > 0;
            }
        }

        /// <summary>
        /// Enumera els noms de les senyals.
        /// </summary>
        /// 
        public IEnumerable<string> SignalNames =>
            _signals.Keys;

        /// <summary>
        /// Enumera les senyals.
        /// </summary>
        /// 
        public IEnumerable<NetSignal> Signals =>
            _signals.Values;
    }
}
