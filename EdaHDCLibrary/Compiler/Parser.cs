namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.Hdc.Ast;

    public sealed class Parser {

        public Parser() {
        }

        public Node Parse(TextReader reader) {

            Tokenizer tokenizer = new Tokenizer(reader);

            Node root = null;

            while (tokenizer.NextToken()) {
                if (tokenizer.Token == "device") {
                    ParseDevice(tokenizer);
                }
            }

            return root;
        }

        private ComplexDeclarationNode ParseDevice(Tokenizer tokenizer) {

            if (tokenizer.Token != "device")
                throw new Exception("Se esperaba 'device'");

            IdentifierNode identifier = ParseIdentifier(tokenizer);

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.LeftBracked))
                throw new Exception("Se esperaba '{'.");

            List<DeclarationNode> declarations = new List<DeclarationNode>();

            while (tokenizer.NextToken()) {
                if (tokenizer.Token == "port")
                    declarations.Add(ParseDevicePort(tokenizer));
                else if (tokenizer.Token == "string" || tokenizer.Token == "real")
                    declarations.Add(ParseDeviceAttribute(tokenizer));
                else if (tokenizer.TokenType == TokenType.RightBracked)
                    break;
            }

            return new ComplexDeclarationNode(
                new TypeNode("device"),
                identifier, 
                declarations);
        }

        private ComplexDeclarationNode ParseDevicePort(Tokenizer tokenizer) {

            if (tokenizer.Token != "port")
                throw new Exception("Se esperaba 'port'.");

            IdentifierNode identifier = ParseIdentifier(tokenizer);

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.LeftBracked))
                throw new Exception("Se esperaba '{'.");

            List<DeclarationNode> declarations = new List<DeclarationNode>();

            while (tokenizer.NextToken()) {
                if (tokenizer.Token == "type")
                    declarations.Add(ParseDevicePortType(tokenizer));
                else if (tokenizer.Token == "pin")
                    declarations.Add(ParseDevicePortPin(tokenizer));
                else if (tokenizer.TokenType == TokenType.RightBracked)
                    break;
                else
                    throw new Exception("Se esperaba 'type', 'pin' o '}'.");
            }

            return new ComplexDeclarationNode(
                new TypeNode("port"),
                identifier,
                declarations);
        }

        private DeclarationNode ParseDeviceAttribute(Tokenizer tokenizer) {

            string typeName = tokenizer.Token;

            TypeNode type = new TypeNode(typeName);
            IdentifierNode identifier = ParseIdentifier(tokenizer);

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Equal))
                throw new Exception("Se esperaba un '='.");

            ValueNode value = null;
            switch (typeName) {
                case "string":
                    value = ParseStringValue(tokenizer);
                    break;

                case "real":
                    value = ParseRealValue(tokenizer);
                    break;

                case "integer":
                    value = ParseIntegerValue(tokenizer);
                    break;

                default:
                    throw new Exception(String.Format("Se esperava un valor '{0}'.", typeName));
            }

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                throw new Exception("Se esperaba un ';'.");

            return new SimpleDeclarationNode(
                type,
                identifier,
                value);
        }

        private DeclarationNode ParseDevicePortType(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Equal))
                throw new Exception("Se esperaba un '='.");

            ValueNode value = ParseValue(tokenizer);

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                throw new Exception("Se esperaba ';'.");

            return new SimpleDeclarationNode(
                new TypeNode("type"),
                new IdentifierNode("type"),
                value);
        }

        private DeclarationNode ParseDevicePortPin(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Equal))
                throw new Exception("Se esperaba '='.");

            ValueNode value = ParseStringValue(tokenizer);

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.SemiColon))
                throw new Exception("Se esperaba ';'.");

            return new SimpleDeclarationNode(
                new TypeNode("pin"),
                new IdentifierNode("pin"),
                value);
        }

        private IdentifierNode ParseIdentifier(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Identifier))
                throw new Exception("Se esperaba un identificador.");

            return new IdentifierNode(tokenizer.Token);
        }

        private TypeNode ParseType(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Identifier))
                throw new Exception("Se esperaba un identificador.");

            return new TypeNode(tokenizer.Token);
        }

        private ValueNode ParseValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken())
                throw new Exception("Final inesperado.");

            return new ValueNode(tokenizer.Token);
        }

        private ValueNode ParseIntegerValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.Integer))
                throw new Exception("Se esperaba un valor entero.");

            return new ValueNode(Convert.ToInt32(tokenizer.Token));
        }

        private ValueNode ParseRealValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || ((tokenizer.TokenType != TokenType.Real) && (tokenizer.TokenType != TokenType.Integer)))
                throw new Exception("Se esperaba un valor real.");

            return new ValueNode(Convert.ToDouble(tokenizer.Token));
        }

        private ValueNode ParseStringValue(Tokenizer tokenizer) {

            if (!tokenizer.NextToken() || (tokenizer.TokenType != TokenType.String))
                throw new Exception("Se esperaba un valor string.");

            return new ValueNode(tokenizer.Token);
        }
    }
}
