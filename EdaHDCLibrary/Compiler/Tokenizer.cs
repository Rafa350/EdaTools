namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public enum TokenId {
        Comma,
        Device,
        Equal,
        Integer,
        IntegerLiteral,
        LeftBracked,
        LeftParenthesis,
        Module,
        Net,
        Pin,
        Port,
        Real,
        RealLiteral,
        RightBracked,
        RightParenthesis,
        SemiColon,
        String,
        StringLiteral,        
        Identifier,
        EOF
    }

    public sealed class Tokenizer {

        private const char EOF = (char)0xFFFF;
        private const char LF = (char)0x000A;
        private const char CR = (char)0x000D;

        private readonly Dictionary<string, TokenId> words = new Dictionary<string, TokenId>();
        private readonly TextReader reader;
        private readonly StringBuilder sb;
        private char urBuffer = EOF;
        private int line;
        private int column;
        private TokenId tokenId;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="reader">Lector de caracters.</param>
        /// 
        public Tokenizer(TextReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            words.Add("device", TokenId.Device);
            words.Add("integer", TokenId.Integer);
            words.Add("module", TokenId.Module);
            words.Add("net", TokenId.Net);
            words.Add("pin", TokenId.Pin);
            words.Add("port", TokenId.Port);
            words.Add("real", TokenId.Integer);
            words.Add("string", TokenId.String);

            this.reader = reader;
            sb = new StringBuilder();
            line = 1;
            column = 1;
        }

        private void ThrowUnexpectedChar(char ch) {

            throw new Exception(String.Format("{1}:{2} - Caracter '{0}' inesperado.", ch, line, column));
        }

        private void ThrowUnexpectedEOF() {

            throw new Exception("Final de archivo inesperado.");
        }

        private char Get() {

            char ch;
            if (urBuffer == EOF)
                ch = (char)reader.Read();
            else {
                ch = urBuffer;
                urBuffer = EOF;
            }

            if ((ch == '\r') || (ch == '\n'))
                column = 1;
            else
                column += 1;

            if (ch == '\n')
                line += 1;

            return ch;
        }

        private void Unget(char ch) {

            column -= 1;

            urBuffer = ch;
        }

        /// <summary>
        /// Obte el seguent token.
        /// </summary>
        /// <returns>True si ha recuperat un token, false en cas contrari.</returns>
        /// 
        public bool NextToken() {

            sb.Clear();

            tokenId = TokenId.EOF;

            int state = 0;
            while (state != -1) {
                char ch = Get();
                switch (state) {
                    case 0:
                        if (ch == 0xFFFF)
                            state = -1;
                        else if (Char.IsDigit(ch)) {
                            sb.Append(ch);
                            state = 100;
                        }
                        else if (Char.IsLetter(ch) || (ch == '_')) {
                            sb.Append(ch);
                            state = 200;
                        }
                        else if (ch == '{') {
                            sb.Append(ch);
                            tokenId = TokenId.LeftBracked;
                            state = -1;
                        }
                        else if (ch == '}') {
                            sb.Append(ch);
                            tokenId = TokenId.RightBracked;
                            state = -1;
                        }
                        else if (ch == '(') {
                            sb.Append(ch);
                            tokenId = TokenId.LeftParenthesis;
                            state = -1;
                        }
                        else if (ch == ')') {
                            sb.Append(ch);
                            tokenId = TokenId.RightParenthesis;
                            state = -1;
                        }
                        else if (ch == '=') {
                            sb.Append(ch);
                            tokenId = TokenId.Equal;
                            state = -1;
                        }
                        else if (ch == ';') {
                            sb.Append(ch);
                            tokenId = TokenId.SemiColon;
                            state = -1;
                        }
                        else if (ch == ',') {
                            sb.Append(ch);
                            tokenId = TokenId.Comma;
                            state = -1;
                        }
                        else if (ch == '"')
                            state = 300;
                        else if (!Char.IsWhiteSpace(ch))
                            ThrowUnexpectedChar(ch);
                        break;

                    case 100:
                        if (Char.IsDigit(ch))
                            sb.Append(ch);
                        else {
                            Unget(ch);
                            tokenId = TokenId.IntegerLiteral;
                            state = -1;
                        }
                        break;

                    case 200:
                        if (Char.IsLetterOrDigit(ch) || (ch == '_') || (ch == '.'))
                            sb.Append(ch);
                        else {
                            Unget(ch);
                            if (!words.TryGetValue(sb.ToString(), out tokenId))
                                tokenId = TokenId.Identifier;
                            state = -1;
                        }
                        break;

                    case 300:
                        if (ch == '"') {
                            tokenId = TokenId.StringLiteral;
                            state = -1;
                        }
                        else
                            sb.Append(ch);
                        break;
                }
            }

            return tokenId != TokenId.EOF;
        }

        /// <summary>
        /// Obte el token actual.
        /// </summary>
        /// 
        public string Token {
            get {
                return sb.ToString();
            }
        }

        /// <summary>
        /// Obte el identificador del token actual.
        /// </summary>
        /// 
        public TokenId TokenId {
            get {
                return tokenId;
            }
        }

        public int Column {
            get {
                return column;
            }
        }

        public int Line {
            get {
                return line;
            }
        }
    }
}
