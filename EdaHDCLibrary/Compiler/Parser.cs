namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using MikroPic.EdaTools.v1.Hdc.Ast;
    using System.Collections.Generic;
    using System.IO;

    public sealed class Parser: ParserBase {

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
        public IEnumerable<EntityNode> Parse(TextReader reader) {

            StartTokenizer(reader);

            List<EntityNode> entities = new List<EntityNode>();

            NextToken();
            while (!IsToken(TokenId.EOF)) {
                switch (GetTokenId()) {
                    case TokenId.Device:
                    case TokenId.Module:
                        entities.Add(ParseEntity());
                        break;

                    default:
                        ThrowSyntaxError("Se esperaba 'device' o 'module'.");
                        break;
                }
                NextToken();
            }

            return entities;
        }

        private EntityNode ParseEntity() {

            if (!IsToken(TokenId.Device) && 
                !IsToken(TokenId.Module))
                ThrowSyntaxError("Se esperaba 'device' o 'device'.");

            string prefix = GetToken();

            NextToken();
            if (!IsToken(TokenId.Identifier))
                ThrowSyntaxError("Se esperaba un identificador.");
            string name = GetToken();

            NextToken();
            if (!IsToken(TokenId.LeftBracked))
                ThrowSyntaxError("Se esperaba '{'.");

            List<MemberNode> members = new List<MemberNode>();
            NextToken();
            while (!IsToken(TokenId.RightBracked)) {
                switch (GetTokenId()) {
                    case TokenId.Pin:
                    case TokenId.Port:
                    case TokenId.Net:
                    case TokenId.Integer:
                    case TokenId.Real:
                    case TokenId.String:
                    case TokenId.Identifier:
                        members.Add(ParseMember());
                        break;

                    default:
                        ThrowSyntaxError("Se esperaba 'pin', 'port', 'net', 'string', 'real', 'integer', o '}'.");
                        break;
                }
                NextToken();
            }

            return new EntityNode(prefix, name, members);
        }

        private MemberNode ParseMember() {

            if (!IsToken(TokenId.Port) &&
                !IsToken(TokenId.Pin) &&
                !IsToken(TokenId.Net) &&
                !IsToken(TokenId.Integer) &&
                !IsToken(TokenId.Real) &&
                !IsToken(TokenId.String) &&
                !IsToken(TokenId.Identifier))
                ThrowSyntaxError("Se esperaba 'port', 'pin', 'real', 'integer', 'string', 'identifier'.");

            string prefix = GetToken();

            NextToken();
            if (!IsToken(TokenId.Identifier))
                ThrowSyntaxError("Se esperaba un identificador.");

            string name = GetToken();

            NextToken();
            if (!IsToken(TokenId.LeftBracked) && 
                !IsToken(TokenId.SemiColon))
                ThrowSyntaxError("Se esperaba ';' o'{'.");

            List<OptionNode> options = new List<OptionNode>();
            if (IsToken(TokenId.LeftBracked)) {
                NextToken();
                while (!IsToken(TokenId.RightBracked)) {
                    switch (GetTokenId()) {
                        case TokenId.Identifier:
                            options.Add(ParseOption());
                            break;

                        default:
                            ThrowSyntaxError("Se esperaba un identificador.");
                            break;
                    }
                    NextToken();
                }
            }
        
            return new MemberNode(prefix, name, options);
        }

        private OptionNode ParseOption() {

            if (!IsToken(TokenId.Identifier))
                ThrowSyntaxError("Se esperaba un identificador.");

            string name = GetToken();

            NextToken();
            if (!IsToken(TokenId.Equal))
                ThrowSyntaxError("Se esperaba '='.");

            NextToken();
            if (!IsToken(TokenId.IntegerLiteral) &&
                !IsToken(TokenId.RealLiteral) &&
                !IsToken(TokenId.StringLiteral) &&
                !IsToken(TokenId.Identifier))
                ThrowSyntaxError("Se esperaba un literal o un identificador.");
            string value = GetToken();

            NextToken();
            if (!IsToken(TokenId.SemiColon))
                ThrowSyntaxError("Se esperaba ';'.");

            return new OptionNode(name, value);
        }
    }
}
