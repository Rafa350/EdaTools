﻿namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Windows;
    using System.Collections.Generic;

    public sealed class Part: IPosition, IRotation {

        private string name;
        private Point position;
        private double angle;
        private bool isMirror;
        private Component component;
        private Dictionary<string, Parameter> parameters;

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public void AddParameter(Parameter parameter) {

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (parameters == null)
                parameters = new Dictionary<string, Parameter>();

            parameters.Add(parameter.Name, parameter);
        }

        public void RemoveParameter(Parameter parameter) {

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            parameters.Remove(parameter.Name);
            if (parameters.Count == 0)
                parameters = null;
        }

        public Parameter GetParameter(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((parameters != null) && (parameters.ContainsKey(name)))
                return parameters[name];
            else
                return null;
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public double Rotation {
            get {
                return angle;
            }
            set {
                angle = value;
            }
        }

        public bool IsMirror {
            get {
                return isMirror;
            }
            set {
                isMirror = value;
            }
        }

        public Component Component {
            get {
                return component;
            }
            set {
                component = value;
            }
        }

        public IEnumerable<Parameter> Parameters {
            get {
                return parameters == null ? null : parameters.Values;
            }
        }
    }
}
