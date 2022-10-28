using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public struct ConectionItem {
        public IEdaConectable Conectable { get; set; }
        public EdaPart Part { get; set; }
    }

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private Dictionary<string, EdaSignal> _signals = new Dictionary<string, EdaSignal>();

        private readonly Dictionary<EdaSignal, HashSet<ConectionItem>> _signalMap = new Dictionary<EdaSignal, HashSet<ConectionItem>>();
        private readonly Dictionary<ConectionItem, EdaSignal> _connectionMap = new Dictionary<ConectionItem, EdaSignal>();

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

            if (_signalMap.ContainsKey(signal))
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
        /// <param name="conectable">L'element a conectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Connect(EdaSignal signal, IEdaConectable conectable, EdaPart part = null) {

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if (conectable == null)
                throw new ArgumentNullException(nameof(conectable));

            if ((_signals == null) || !_signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            var signalNode = new ConectionItem { Conectable = conectable, Part = part };

            if (_connectionMap.ContainsKey(signalNode))
                throw new InvalidOperationException("El elemento ya esta conectado.");

            if (!_signalMap.TryGetValue(signal, out HashSet<ConectionItem> signalNodes)) {
                signalNodes = new HashSet<ConectionItem>();
                _signalMap.Add(signal, signalNodes);
            }
            signalNodes.Add(signalNode);
            _connectionMap.Add(signalNode, signal);
        }

        /// <summary>
        /// Desconecta un element de la senyal.
        /// </summary>
        /// <param name="conectable">El element a desconectar.</param>
        /// <param name="part">El component del element.</param>
        /// 
        public void Disconnect(IEdaConectable conectable, EdaPart part = null) {

            if (conectable == null)
                throw new ArgumentNullException(nameof(conectable));

            var signalNode = new ConectionItem { Conectable = conectable, Part = part };
            if (!_connectionMap.ContainsKey(signalNode))
                throw new InvalidOperationException("El objeto no esta conectado a ninguna señal.");
        }

        /// <summary>
        /// Obte la senyal conectada a un objecte.
        /// </summary>
        /// <param name="conectable">El objecte.</param>
        /// <param name="part">El part al que pertany l'element. Null si pertany a la placa.</param>
        /// <param name="throwOnError">True si cal generar una excepcio en cas d'error.</param>
        /// <returns>La senyal o null si no esta conectat.</returns>
        /// 
        public EdaSignal GetSignal(IEdaConectable conectable, EdaPart part = null, bool throwOnError = true) {

            if (conectable == null)
                throw new ArgumentNullException(nameof(conectable));

            EdaSignal signal = null;

            if (conectable is IEdaConectable conectableElement) {
                var signalNode = new ConectionItem { Conectable = conectableElement, Part = part};
                _connectionMap.TryGetValue(signalNode, out signal);
            }

            if ((signal == null) && throwOnError)
                throw new InvalidOperationException("El elemento no esta conectado.");

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
        public IEnumerable<ConectionItem> GetConectionItems(EdaSignal signal) {

            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if ((_signals == null) || !_signals.ContainsKey(signal.Name))
                throw new InvalidOperationException(
                    String.Format("La señal '{0}', no esta asignada a esta placa.", signal.Name));

            if (_signalMap.TryGetValue(signal, out HashSet<ConectionItem> signalNodes))
                return signalNodes;
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
