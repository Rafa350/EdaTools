namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using System;
    using System.IO;

    public abstract class ParserBase {

        private Tokenizer tokenizer;

        /// <summary>
        /// Genera una excepcio per error sintactic.
        /// </summary>
        /// <param name="text">El text de l'excepcio.</param>
        /// 
        protected void ThrowSyntaxError(string text) {

            throw new Exception(String.Format("Error - {0}:{1} - {2}", tokenizer.Line, tokenizer.Column, text));
        }

        protected void StartTokenizer(TextReader reader) {

            tokenizer = new Tokenizer(reader);
        }

        protected void NextToken() {

            tokenizer.NextToken();
        }

        protected bool IsToken(TokenId tokenId) {

            return tokenizer.TokenId == tokenId;
        }

        protected bool IsToken(string token) {

            return tokenizer.Token == token;
        }

        protected string GetToken() {

            return tokenizer.Token;
        }

        protected TokenId GetTokenId() {

            return tokenizer.TokenId;
        }
    }
}
