using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private Dictionary<string, EdaSignal> _signals = new Dictionary<string, EdaSignal>();
        private readonly Dictionary<EdaSignal, HashSet<Tuple<IConectable, EdaPart>>> _itemsOfSignal = new Dictionary<EdaSignal, HashSet<Tuple<IConectable, EdaPart>>>();
        private readonly Dictionary<Tuple<IConectable, EdaPart>, EdaSignal> _signalOfItem = new Dictionary<Tuple<IConectable, EdaPart>, EdaSignal>();

        /// <summary>
        /// Afeigeix una senyal a la placa.
        /// </summary>
        /// <param name="signal">La senyal a afeigir.</param>
        /// 
        public void AddSignal(EdaSignal signal) {

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if ((_signals == null) || _signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("Ya existe una señal con el nombre '{0}' en la placa.", signal.Name));

            if (_signals == null)
                _signals = new Dictionary<string, EdaSignal>();
            _signals.Add(signal.Name, signal);
        }

        /// <summary>
        /// Afegeix una col·leccio de senays a la placa.
        /// </summary>
        /// <param name="signals">La col·leccio de senyals.</param>
        /// 
        public void AddSignals(IEnumerable<EdaSignal> signals) {

            if (signals == null)
                throw new ArgumentNullException(nameof(signals));

            foreach (var signal in signals)
                AddSignal(signal);
        }

        /// <summary>
        /// Retira una senyal de la placa.
        /// </summary>
        /// <param name="signal">La senyal a retirar.</param>
        /// 
        public void RemoveSignal(EdaSignal signal) {

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if ((_signals == null) || !_signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("No se encontro ninguna señal con el nombre '{0}', en la placa.", signal.Name));

            if (_itemsOfSignal.ContainsKey(signal))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', esta en uso y no puede ser retirada de la placa.", signal.Name));

            _signals.Remove(signal.Name);
            if (_signals.Count == 0)
                _signals = null;
        }

        /// <summary>
        /// Conecta un element conectable amb una senyal.
        /// </summary>
        /// <param name="signal">La senyal.</param>
        /// <param name="element">L'element a conectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Connect(EdaSignal signal, IConectable element, EdaPart part = null) {

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if ((_signals == null) || !_signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("La senyal '{0}', no esta asignada a esta placa.", signal.Name));

            Tuple<IConectable, EdaPart> item = new Tuple<IConectable, EdaPart>(element, part);

            if (_signalOfItem.ContainsKey(item))
                throw new InvalidOperationException("El objeto ya esta conectado.");

            HashSet<Tuple<IConectable, EdaPart>> elementSet;
            if (!_itemsOfSignal.TryGetValue(signal, out elementSet)) {
                elementSet = new HashSet<Tuple<IConectable, EdaPart>>();
                _itemsOfSignal.Add(signal, elementSet);
            }
            elementSet.Add(item);
            _signalOfItem.Add(item, signal);
        }

        /// <summary>
        /// Desconecta un element de la senyal.
        /// </summary>
        /// <param name="element">El element a desconectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Disconnect(IConectable element, EdaPart part = null) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            Tuple<IConectable, EdaPart> item = new Tuple<IConectable, EdaPart>(element, part);

            if (!_signalOfItem.ContainsKey(item))
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
        public EdaSignal GetSignal(EdaElement element, EdaPart part = null, bool throwOnError = true) {

            if (element == null)
                throw new ArgumentNullException(nameof(element));

            EdaSignal signal = null;

            if (element is IConectable conectableElement) {
                Tuple<IConectable, EdaPart> item = new Tuple<IConectable, EdaPart>(conectableElement, part);
                _signalOfItem.TryGetValue(item, out signal);
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
        public EdaSignal GetSignal(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((_signals != null) && _signals.TryGetValue(name, out var signal))
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
        public IEnumerable<Tuple<IConectable, EdaPart>> GetConnectedItems(EdaSignal signal) {

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if ((_signals == null) || !_signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            HashSet<Tuple<IConectable, EdaPart>> itemSet;
            if (_itemsOfSignal.TryGetValue(signal, out itemSet))
                return itemSet;
            else
                return null;
        }

        /// <summary>
        /// Indica si la placa conte senyals
        /// </summary>
        /// 
        public bool HasSignals =>
            _signals != null;

        /// <summary>
        /// Obte un enunerador per les senyals.
        /// </summary>
        /// 
        public IEnumerable<EdaSignal> Signals =>
            _signals == null ? Enumerable.Empty<EdaSignal>() : _signals.Values;
    }
}
