namespace MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure {

    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class SParser {

        public STree Parse(string source) {

            TextReader reader = new StringReader(source);
            STokenizer tokenizer = new STokenizer(reader);
            SNode root = ParseNode(tokenizer);
            
            return new STree(source, root);
        }

        /// <summary>
        /// Procesas un node individual.
        /// </summary>
        /// <param name="tokenizer">El tokenizador.</param>
        /// <returns>El node procesat.</returns>
        /// 
        private SNode ParseNode(STokenizer tokenizer) {

            TokenType tkType = tokenizer.GetToken(out int start, out int length);
            switch (tkType) {
                case TokenType.Word:
                    return new SLeaf(start, length);

                case TokenType.StartBlock:
                    return ParseNodeList(tokenizer);

                case TokenType.EndOfFile:
                    throw new Exception("Unspected end.");

                default:
                    throw new Exception("Unspected token.");
            }
        }

        /// <summary>
        /// Procesa un node compost.
        /// </summary>
        /// <param name="tokenizer">El tokenizador.</param>
        /// <returns>El node procesat.</returns>
        /// 
        private SNode ParseNodeList(STokenizer tokenizer) {

            List<SNode> nodes = new List<SNode>();

            TokenType type;
            do {
                type = tokenizer.GetToken(out int position, out int length);
                switch (type) {
                    case TokenType.Word:
                        nodes.Add(new SLeaf(position, length));
                        break;

                    case TokenType.StartBlock:
                        nodes.Add(ParseNodeList(tokenizer));
                        break;

                    case TokenType.EndBlock:
                        break;

                    case TokenType.EndOfFile:
                        throw new Exception("Unspected end.");

                    default:
                        throw new Exception("Unspected token.");
                }
            } while (type != TokenType.EndBlock);

            return new SBranch(nodes);
        }

    }
}
