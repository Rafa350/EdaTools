namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.Hdc.Ast;

    public sealed class Parser {

        /// <summary>
        /// \brief Constructor de l'objecte.
        /// </summary>
        public Parser() {
        }

        /// <summary>
        /// Genera un arbre AST a partir de les dades d'entrada.
        /// </summary>
        /// <param name="reader">Lector de dades d'entrada.</param>
        /// <returns>El node arrel del arbre AST.</returns>
        /// 
        public Node Parse(TextReader reader) {

            Tokenizer tokenizer = new Tokenizer(reader);

            List<Node> declarations = new List<Node>();

            while (tokenizer.NextToken()) {
                switch (tokenizer.Token) {
                    case "device":
                        declarations.Add(ParseDeviceDeclaration(tokenizer));
                        break;

                    case "module":
                        declarations.Add(ParseModuleDeclaration(tokenizer));
                        break;

                    default:
                        ThrowSyntaxError(tokenizer, "Se esperaba 'device' o 'module'.");
                        break;
                }
            }

            return null;
        }

        private DeviceDeclarationNode ParseDeviceDeclaration(Tokenizer tokenizer) {

            if (tokenizer.Token != "device")
                ThrowSyntaxError(tokenizer, "Se esperaba 'device'.");

            tokenizer.NextToken();
            if (tokenizer.TokenType != TokenType.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string deviceName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.LeftBracked))
                ThrowSyntaxError(tokenizer, "Se esperaba '{'.");

            List<PinDefinitionNode> pins = new List<PinDefinitionNode>();
            List<AttributeDefinitionNode> attributes = new List<AttributeDefinitionNode>();

            while (tokenizer.NextToken() && (tokenizer.TokenType != TokenType.RightBracked)) {
                if (tokenizer.Token == "pin")
                    pins.Add(ParsePinDeclaration(tokenizer));

                else if (tokenizer.Token == "string" || tokenizer.Token == "real" || tokenizer.Token == "integer")
                    attributes.Add(ParseAttributeDeclaration(tokenizer));

                else
                    ThrowSyntaxError(tokenizer, "Se esperaba 'pin', 'string', 'real', 'integer', o '}'.");
            }

            return new DeviceDeclarationNode(deviceName, pins, attributes);
        }

        private PinDefinitionNode ParsePinDeclaration(Tokenizer tokenizer) {

            if (tokenizer.Token != "pin")
                ThrowSyntaxError(tokenizer, "Se esperaba 'pin'.");

            tokenizer.NextToken();
            if (tokenizer.TokenType != TokenType.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string pinName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.LeftBracked))
                ThrowSyntaxError(tokenizer, "Se esperaba '{'.");

            while (tokenizer.NextToken() && (tokenizer.TokenType != TokenType.RightBracked)) {

                if (tokenizer.TokenType != TokenType.Identifier)
                    ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");

                string propName = tokenizer.Token;

                tokenizer.NextToken();
                if (tokenizer.TokenType != TokenType.Equal)
                    ThrowSyntaxError(tokenizer, "Se esperaba '='.");

                tokenizer.NextToken();
                if (tokenizer.TokenType != TokenType.Integer && tokenizer.TokenType != TokenType.Real && tokenizer.TokenType != TokenType.String)
                    ThrowSyntaxError(tokenizer, "Se esperaba un valor.");
                string propValue = tokenizer.Token;

                if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                    ThrowSyntaxError(tokenizer, "Se esperaba ';'.");
            }

            return new PinDefinitionNode(pinName, "");
        }

        private AttributeDefinitionNode ParseAttributeDeclaration(Tokenizer tokenizer) {

            if (tokenizer.Token != "string" && tokenizer.Token != "real" && tokenizer.Token != "integer")
                ThrowSyntaxError(tokenizer, "Se esperaba 'integer', 'real' o 'string'.");

            Type type = null;
            switch (tokenizer.Token) {
                case "integer":
                    type = typeof(int);
                    break;

                case "real":
                    type = typeof(double);
                    break;

                case "string":
                    type = typeof(string);
                    break;
            }

            if (!tokenizer.NextToken() || tokenizer.TokenType != TokenType.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string name = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                ThrowSyntaxError(tokenizer, "Se esperaba un ';'.");

            return new AttributeDefinitionNode(name, type);
        }

        private ModuleDeclarationNode ParseModuleDeclaration(Tokenizer tokenizer) {

            if (tokenizer.Token != "module")
                ThrowSyntaxError(tokenizer, "Se esperaba 'module'");

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Identifier))
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string moduleName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.LeftBracked))
                ThrowSyntaxError(tokenizer, "Se esperaba '{'.");

            List<PortDefinitionNode> ports = new List<PortDefinitionNode>();
            List<NetDefinitionNode> nets = new List<NetDefinitionNode>();
            List<InstanceDefinitionNode> instances = new List<InstanceDefinitionNode>();

            while (tokenizer.NextToken() && (tokenizer.TokenType != TokenType.RightBracked)) {
                if (tokenizer.Token == "port")
                    ports.Add(ParsePort(tokenizer));

                else if (tokenizer.Token == "net")
                    nets.Add(ParseNet(tokenizer));

                else if (tokenizer.TokenType == TokenType.Identifier)
                    instances.Add(ParseInstance(tokenizer));

                else
                    ThrowSyntaxError(tokenizer, "Se esperaba 'port', 'net' o un identificador.");
            }

            return new ModuleDeclarationNode(moduleName);
        }

        private PortDefinitionNode ParsePort(Tokenizer tokenizer) {

            if (tokenizer.Token != "port")
                ThrowSyntaxError(tokenizer, "Se esperaba 'port'.");

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Identifier))
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string portName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                ThrowSyntaxError(tokenizer, "Se esperaba ';'.");

            return new PortDefinitionNode(portName);
        }

        private NetDefinitionNode ParseNet(Tokenizer tokenizer) {

            if (tokenizer.Token != "net")
                ThrowSyntaxError(tokenizer, "Se esperaba 'net';");

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Identifier))
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string netName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                ThrowSyntaxError(tokenizer, "Se esperaba ';'.");

            return new NetDefinitionNode(netName);
        }

        private InstanceDefinitionNode ParseInstance(Tokenizer tokenizer) {

            if (tokenizer.TokenType != TokenType.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");

            string instanceTypeName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Identifier))
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");

            string instanceName = tokenizer.Token;

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.LeftBracked))
                ThrowSyntaxError(tokenizer, "Se esperaba '{'.");

            while (tokenizer.NextToken() && (tokenizer.TokenType != TokenType.RightBracked)) {

                if (tokenizer.TokenType != TokenType.Identifier)
                    ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");

                if (!tokenizer.NextToken() || tokenizer.TokenType != TokenType.Equal)
                    ThrowSyntaxError(tokenizer, "Se esperaba '='.");

                if (!tokenizer.NextToken() || ((tokenizer.TokenType != TokenType.String) && (tokenizer.TokenType != TokenType.Identifier)))
                    ThrowSyntaxError(tokenizer, "Se esperaba un identificador o un valor integer, real o string.");

                if (!tokenizer.NextToken() || tokenizer.TokenType != TokenType.SemiColon)
                    ThrowSyntaxError(tokenizer, "Se esperaba ';'.");
            }

            return new InstanceDefinitionNode(instanceName);
        }

        private ValueNode ParseIntegerValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Integer))
                ThrowSyntaxError(tokenizer, "Se esperaba un valor entero.");

            return new ValueNode(Convert.ToInt32(tokenizer.Token), typeof(int));
        }

        private ValueNode ParseRealValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || ((tokenizer.TokenType != TokenType.Real) && (tokenizer.TokenType != TokenType.Integer)))
                ThrowSyntaxError(tokenizer, "Se esperaba un valor real.");

            return new ValueNode(Convert.ToDouble(tokenizer.Token), typeof(Double));
        }

        private ValueNode ParseStringValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.String))
                ThrowSyntaxError(tokenizer, "Se esperaba un valor string.");

            return new ValueNode(tokenizer.Token, typeof(string));
        }

        /// <summary>
        /// Genera una excepcio per error sintactic.
        /// </summary>
        /// <param name="tokenizer">El tokenizador.</param>
        /// <param name="text">El text de l'excepcio.</param>
        /// 
        private void ThrowSyntaxError(Tokenizer tokenizer, string text) {

            throw new Exception(String.Format("Error - {0}:{1} - {2}", tokenizer.Line, tokenizer.Column, text));
        }
    }
}
