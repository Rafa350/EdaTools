namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using MikroPic.EdaTools.v1.Hdc.Model;
    using MikroPic.EdaTools.v1.Hdc.Ast;
    using System;
    using System.Collections.Generic;

    public sealed class Compiler {

        public void Compile(IEnumerable<EntityNode> entities) {

            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entityNode in entities) {
                switch (entityNode.Prefix) {
                    case "device": {
                        Device device = CompileDevice(entityNode);
                        break;
                    }

                    case "module": {
                        break;
                    }

                    default:
                        throw new InvalidOperationException(
                            String.Format("No se reconoce el elemento '{0}'.", entityNode.Prefix));
                }
            }
        }

        private Device CompileDevice(EntityNode entityNode) {

            Device device = new Device(entityNode.Name);
            if (entityNode.Members != null) {
                foreach (var memberNode in entityNode.Members) {
                    switch (memberNode.Prefix) {
                        case "pin": {
                            DevicePin pin = CompileDevicePin(memberNode);
                            device.AddPin(pin);
                            break;
                        }

                        case "integer":
                            device.DefineAttribute(memberNode.Name);
                            break;

                        case "real":
                            device.DefineAttribute(memberNode.Name);
                            break;

                        case "string":
                            device.DefineAttribute(memberNode.Name);
                            break;

                        default:
                            throw new InvalidOperationException(
                                String.Format("No se puede declarar una propiedad '{0}' en un elemento 'device'.", memberNode.Prefix));
                    }
                }
            }
            return device;
        }

        private DevicePin CompileDevicePin(MemberNode memberNode) {

            DevicePin pin = new DevicePin(memberNode.Name);
            if (memberNode.Options != null) {
                foreach (var optionNode in memberNode.Options) {
                    switch (optionNode.Name) {
                        case "name":
                            pin.PinName = optionNode.Value as string;
                            break;

                        case "type":
                            break;

                        default:
                            throw new InvalidOperationException(
                                String.Format("El objeto 'pin' no posee la propiedad '{0}'.", optionNode.Name));
                    }
                }
            }

            return pin;
        }
    }
}
