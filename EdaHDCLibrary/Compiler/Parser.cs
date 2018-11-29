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
        /// <returns>Llista de nodes.</returns>
        /// 
        public IEnumerable<Node> Parse(TextReader reader) {

            Tokenizer tokenizer = new Tokenizer(reader);

            List<EntityNode> entities = new List<EntityNode>();

            while (tokenizer.NextToken()) {
                switch (tokenizer.TokenId) {
                    case TokenId.Device:
                    case TokenId.Module:
                        entities.Add(ParseEntity(tokenizer));
                        break;

                    default:
                        ThrowSyntaxError(tokenizer, "Se esperaba 'device' o 'module'.");
                        break;
                }
            }

            return entities;
        }

        private EntityNode ParseEntity(Tokenizer tokenizer) {

            if ((tokenizer.TokenId != TokenId.Device) && 
                (tokenizer.TokenId != TokenId.Module))
                ThrowSyntaxError(tokenizer, "Se esperaba 'device' o 'device'.");

            string prefix = tokenizer.Token;

            tokenizer.NextToken();
            if (tokenizer.TokenId != TokenId.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
            string name = tokenizer.Token;

            tokenizer.NextToken();
            if (tokenizer.TokenId != TokenId.LeftBracked)
                ThrowSyntaxError(tokenizer, "Se esperaba '{'.");

            List<MemberNode> members = new List<MemberNode>();
            while (tokenizer.NextToken() && (tokenizer.TokenId != TokenId.RightBracked)) {
                switch (tokenizer.TokenId) {
                    case TokenId.Pin:
                    case TokenId.Port:
                    case TokenId.Net:
                    case TokenId.Integer:
                    case TokenId.Real:
                    case TokenId.String:
                    case TokenId.Identifier:
                        members.Add(ParseMember(tokenizer));
                        break;

                    default:
                        ThrowSyntaxError(tokenizer, "Se esperaba 'pin', 'port', 'net', 'string', 'real', 'integer', o '}'.");
                        break;
                }
            }

            return new EntityNode(prefix, name, members);
        }

        private MemberNode ParseMember(Tokenizer tokenizer) {

            if ((tokenizer.TokenId != TokenId.Port) &&
                (tokenizer.TokenId != TokenId.Pin) &&
                (tokenizer.TokenId != TokenId.Net) &&
                (tokenizer.TokenId != TokenId.Integer) &&
                (tokenizer.TokenId != TokenId.Real) &&
                (tokenizer.TokenId != TokenId.String) &&
                (tokenizer.TokenId != TokenId.Identifier))
                ThrowSyntaxError(tokenizer, "Se esperaba 'port', 'pin', 'real', 'integer', 'string', 'identifier'.");

            string prefix = tokenizer.Token;

            tokenizer.NextToken();
            if (tokenizer.TokenId != TokenId.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");

            string name = tokenizer.Token;

            tokenizer.NextToken();
            if ((tokenizer.TokenId != TokenId.LeftBracked) && (tokenizer.TokenId != TokenId.SemiColon))
                ThrowSyntaxError(tokenizer, "Se esperaba ';' o'{'.");

            List<OptionNode> options = new List<OptionNode>();
            if (tokenizer.TokenId == TokenId.LeftBracked)
                while (tokenizer.NextToken() && (tokenizer.TokenId != TokenId.RightBracked)) {
                    switch (tokenizer.TokenId) {
                        case TokenId.Identifier:
                            options.Add(ParseOption(tokenizer));
                            break;

                        default:
                            ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");
                            break;
                    }
                }
        
            return new MemberNode(prefix, name, options);
        }

        private OptionNode ParseOption(Tokenizer tokenizer) {

            if (tokenizer.TokenId != TokenId.Identifier)
                ThrowSyntaxError(tokenizer, "Se esperaba un identificador.");

            string name = tokenizer.Token;

            tokenizer.NextToken();
            if (tokenizer.TokenId != TokenId.Equal)
                ThrowSyntaxError(tokenizer, "Se esperaba '='.");

            tokenizer.NextToken();
            if ((tokenizer.TokenId != TokenId.IntegerLiteral) &&
                (tokenizer.TokenId != TokenId.RealLiteral) &&
                (tokenizer.TokenId != TokenId.StringLiteral) &&
                (tokenizer.TokenId != TokenId.Identifier))
                ThrowSyntaxError(tokenizer, "Se esperaba un literal o un identificador.");
            string value = tokenizer.Token;

            tokenizer.NextToken();
            if (tokenizer.TokenId != TokenId.SemiColon)
                ThrowSyntaxError(tokenizer, "Se esperaba ';'.");

            return new OptionNode(name, tokenizer.Token);
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
