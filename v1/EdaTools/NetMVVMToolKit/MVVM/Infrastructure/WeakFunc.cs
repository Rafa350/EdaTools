﻿// ****************************************************************************
// <copyright file="WeakFunc.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2012-2015
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>15.1.2012</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.mvvmlight.net</web>
// <license>
// See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MikroPic.NetMVVMToolkit.v1.MVVM.Infrastructure
{
    /// <summary>
    /// Stores a Func&lt;T&gt; without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the Func that will be stored
    /// by this weak reference.</typeparam>
    ////[ClassInfo(typeof(WeakAction)]
    public class WeakFunc<TResult>
    {
#if SILVERLIGHT
        private Func<TResult> _func;
#endif
        private Func<TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakFunc's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Get a value indicating whether the WeakFunc is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
#if SILVERLIGHT
                return (_func != null && _func.Target == null)
                    || _staticFunc != null;
#else
                return _staticFunc != null;
#endif
            }
        }

        /// <summary>
        /// Gets the name of the method that this WeakFunc represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
#if NETFX_CORE
                    return _staticFunc.GetMethodInfo().Name;
#else
                    return _staticFunc.Method.Name;
#endif
                }

#if SILVERLIGHT
                if (_func != null)
                {
                    return _func.Method.Name;
                }

                if (Method != null)
                {
                    return Method.Name;
                }

                return string.Empty;
#else
                return Method.Name;
#endif
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakFunc's action's target.
        /// This is not necessarily the same as
        /// <see cref="Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference FuncReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakFunc. This is not necessarily the same as
        /// <see cref="FuncReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes an empty instance of the WeakFunc class.
        /// </summary>
        protected WeakFunc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<TResult> func)
            : this(func == null ? null : func.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if func is null.")]
        public WeakFunc(object target, Func<TResult> func)
        {
#if NETFX_CORE
            if (func.GetMethodInfo().IsStatic)
#else
            if (func.Method.IsStatic)
#endif
            {
                _staticFunc = func;

                if (target != null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    Reference = new WeakReference(target);
                }

                return;
            }

#if SILVERLIGHT
            if (!func.Method.IsPublic
                || (target != null
                    && !target.GetType().IsPublic
                    && !target.GetType().IsNestedPublic))
            {
                _func = func;
            }
            else
            {
                var name = func.Method.Name;

                if (name.Contains("<")
                    && name.Contains(">"))
                {
                    // Anonymous method
                    _func = func;
                }
                else
                {
                    Method = func.Method;
                    FuncReference = new WeakReference(func.Target);
                }
            }
#else
#if NETFX_CORE
            Method = func.GetMethodInfo();
#else
            Method = func.Method;
#endif
            FuncReference = new WeakReference(func.Target);
#endif

            Reference = new WeakReference(target);
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc == null
                    && Reference == null)
                {
                    return false;
                }

                if (_staticFunc != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Func's owner. This object is stored as a 
        /// <see cref="WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }

                return Reference.Target;
            }
        }

        /// <summary>
        /// Gets the owner of the Func that was passed as parameter.
        /// This is not necessarily the same as
        /// <see cref="Target" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected object FuncTarget
        {
            get
            {
                if (FuncReference == null)
                {
                    return null;
                }

                return FuncReference.Target;
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (Method != null
                    && FuncReference != null
                    && funcTarget != null)
                {
                    return (TResult)Method.Invoke(funcTarget, null);
                }

#if SILVERLIGHT
                if (_func != null)
                {
                    return _func();
                }
#endif
            }

            return default(TResult);
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            Method = null;
            _staticFunc = null;

#if SILVERLIGHT
            _func = null;
#endif
        }
    }
}