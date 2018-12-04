namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        private Dictionary<string, Signal> signals = new Dictionary<string, Signal>();
        private readonly Dictionary<Signal, HashSet<Tuple<IConectable, Part>>> itemsOfSignal = new Dictionary<Signal, HashSet<Tuple<IConectable, Part>>>();
        private readonly Dictionary<Tuple<IConectable, Part>, Signal> signalOfItem = new Dictionary<Tuple<IConectable, Part>, Signal>();

        /// <summary>
        /// Afeigeix una senyal a la placa.
        /// </summary>
        /// <param name="signal">La senyal a afeigir.</param>
        /// 
        public void AddSignal(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if ((signals == null) || signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("Ya existe una señal con el nombre '{0}' en la placa.", signal.Name));

            if (signals == null)
                signals = new Dictionary<string, Signal>();
            signals.Add(signal.Name, signal);
        }

        /// <summary>
        /// Afegeix una col·leccio de senays a la placa.
        /// </summary>
        /// <param name="signals">La col·leccio de senyals.</param>
        /// 
        public void AddSignals(IEnumerable<Signal> signals) {

            if (signals == null)
                throw new ArgumentNullException("signals");

            foreach (var signal in signals)
                AddSignal(signal);
        }

        /// <summary>
        /// Retira una senyal de la placa.
        /// </summary>
        /// <param name="signal">La senyal a retirar.</param>
        /// 
        public void RemoveSignal(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if ((signals == null) || !signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("No se encontro ninguna señal con el nombre '{0}', en la placa.", signal.Name));

            if (itemsOfSignal.ContainsKey(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', esta en uso y no puede ser retirada de la placa.", signal.Name));

            signals.Remove(signal.Name);
            if (signals.Count == 0)
                signals = null;
        }

        /// <summary>
        /// Conecta un element conectable amb una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <param name="element">L'element a conectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Connect(Signal signal, IConectable element, Part part = null) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if (element == null)
                throw new ArgumentNullException("element");

            if ((signals == null) || !signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("La senyal '{0}', no esta asignada a esta placa.", signal.Name));

            Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(element, part);

            if (signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto ya esta conectado.");

            HashSet<Tuple<IConectable, Part>> elementSet;
            if (!itemsOfSignal.TryGetValue(signal, out elementSet)) {
                elementSet = new HashSet<Tuple<IConectable, Part>>();
                itemsOfSignal.Add(signal, elementSet);
            }
            elementSet.Add(item);
            signalOfItem.Add(item, signal);
        }

        /// <summary>
        /// Desconecta un element de la senyal.
        /// </summary>
        /// <param name="element">El element a desconectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Disconnect(IConectable element, Part part = null) {

            if (element == null)
                throw new ArgumentNullException("element");

            Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(element, part);

            if (!signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto no esta conectado a ninguna señal.");
        }

        /// <summary>
        /// Obte la senyal conectada a un objecte.
        /// </summary>
        /// <param name="element">El objecte.</param>
        /// <param name="part">El part al que pertany l'element. Null si pertany a la placa.</param>
        /// <param name="throwOnError">True si cal generar una excepcio en cas d'error.</param>
        /// <returns>La senyal o null si no esta conectat.</returns>
        /// 
        public Signal GetSignal(Element element, Part part = null, bool throwOnError = true) {

            if (element == null)
                throw new ArgumentNullException("element");

            Signal signal = null;

            IConectable conectableElement = element as IConectable;
            if (conectableElement != null) {
                Tuple<IConectable, Part> item = new Tuple<IConectable, Part>(conectableElement, part);
                signalOfItem.TryGetValue(item, out signal);
            }

            if ((signal == null) && throwOnError)
                throw new InvalidOperationException("El item no esta conectado.");

            return signal;
        }

        /// <summary>
        /// Obte una senyal pel seu nom.
        /// </summary>
        /// <param name="name">Nom de la senyal.</param>
        /// <param name="throwOnError">True si cal generar una exepcio en cas d'error.</param>
        /// <returns>La senyal o null si no existeix.</returns>
        /// 
        public Signal GetSignal(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((signals != null) && signals.TryGetValue(name, out var signal))
                return signal;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro la señal '{0}'.", name));

            else 
                return null;
        }

        /// <summary>
        /// Obte els items conectats a una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <returns>Els items conectats. Null si no hi ha cap.</returns>
        /// 
        public IEnumerable<Tuple<IConectable, Part>> GetConnectedItems(Signal signal) {

            if (signal == null)
                throw new ArgumentNullException("signal");

            if ((signals == null) || !signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            HashSet<Tuple<IConectable, Part>> itemSet;
            if (itemsOfSignal.TryGetValue(signal, out itemSet))
                return itemSet;
            else
                return null;
        }

        /// <summary>
        /// Indica si la placa conte senyals
        /// </summary>
        /// 
        public bool HasSignals {
            get {
                return signals != null;
            }
        }

        /// <summary>
        /// Obte un enunerador per les senyals.
        /// </summary>
        /// 
        public IEnumerable<Signal> Signals {
            get {
                return signals?.Values;
            }
        }
    }
}
