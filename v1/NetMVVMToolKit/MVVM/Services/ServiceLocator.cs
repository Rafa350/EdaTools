namespace MikroPic.NetMVVMToolkit.v1.MVVM.Services {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Registra els serveis de l'aplicacio.
    /// </summary>
    public sealed class ServiceLocator {

        private static ServiceLocator instance;
        private readonly Dictionary<Type, object> register = new Dictionary<Type, object>();

        /// <summary>
        /// Constructor de l'objecte. Es privat perque implementa 
        /// el patro 'singleton'.
        /// </summary>
        /// 
        private ServiceLocator() {

        }

        /// <summary>
        /// Registre un servei pel seu interficie
        /// </summary>
        /// <param name="srvType">Tipus de l'interficie.</param>
        /// <param name="srvInstance">Instrancia del servei</param>
        /// 
        public void Register(Type srvType, object srvInstance) {

            if (srvType == null)
                throw new ArgumentNullException("srvType");

            if (srvInstance == null)
                throw new ArgumentNullException("srvInstance");

            if (register.ContainsKey(srvType))
                throw new InvalidOperationException(
                    String.Format("El servicio '{0}' ya ha sido registrado.", srvType));

            if (!srvType.IsAssignableFrom(srvInstance.GetType()))
                throw new InvalidOperationException(
                    String.Format("El servicio a registrar, no implementa el interface '{0}'.", srvType));

            register.Add(srvType, srvInstance);
        }

        /// <summary>
        /// Obte una instancia del servei.
        /// </summary>
        /// <param name="srvType">Interficie del servei</param>
        /// <returns>El servei trobat.</returns>
        /// 
        public object GetService(Type srvType) {

            if (srvType == null)
                throw new ArgumentNullException("srvType");

            if (!register.ContainsKey(srvType))
                throw new InvalidOperationException(
                    String.Format("El servicio '{0}' no esta registrado.", srvType));

            return register[srvType];
        }

        /// <summary>
        /// Obte una instancia del servei
        /// </summary>
        /// <typeparam name="T">Interficie del servei.</typeparam>
        /// <returns>El servei trobat.</returns>
        /// 
        public T GetService<T>() {

            return (T) GetService(typeof(T));
        }

        /// <summary>
        /// Obte la instancia del objecte.
        /// </summary>
        /// 
        public static ServiceLocator Instance {
            get {
                if (instance == null)
                    instance = new ServiceLocator();
                return instance;
            }
        }
    }
}
