namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using System;
    using System.IO;
    using System.Text;

    public enum TokenType {
        Comma,
        Equal,
        Integer,
        LeftBracked,
        Real,
        RightBracked,
        SemiColon,
        String,        
        Identifier,
        EOF
    }

    public sealed class Tokenizer {

        private const char EOF = (char)0xFFFF;
        private const char LF = (char)0x000A;
        private const char CR = (char)0x000D;

        private readonly TextReader reader;
        private readonly StringBuilder sb;
        private char urBuffer = EOF;
        private int line;
        private int column;
        private string token;
        private TokenType tokenType;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="reader">Lector de caracters.</param>
        /// 
        public Tokenizer(TextReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

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

            token = null;
            tokenType = TokenType.EOF;

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
                            token = "{";
                            tokenType = TokenType.LeftBracked;
                            state = -1;
                        }
                        else if (ch == '}') {
                            token = "}";
                            tokenType = TokenType.RightBracked;
                            state = -1;
                        }
                        else if (ch == '=') {
                            token = "=";
                            tokenType = TokenType.Equal;
                            state = -1;
                        }
                        else if (ch == ';') {
                            token = ";";
                            tokenType = TokenType.SemiColon;
                            state = -1;
                        }
                        else if (ch == ',') {
                            token = ",";
                            tokenType = TokenType.Comma;
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
                            token = sb.ToString();
                            tokenType = TokenType.Integer;
                            state = -1;
                        }
                        break;

                    case 200:
                        if (Char.IsLetterOrDigit(ch) || (ch == '_') || (ch == '.'))
                            sb.Append(ch);
                        else {
                            Unget(ch);
                            token = sb.ToString();
                            tokenType = TokenType.Identifier;
                            state = -1;
                        }
                        break;

                    case 300:
                        if (ch == '"') {
                            token = sb.ToString();
                            tokenType = TokenType.String;
                            state = -1;
                        }
                        else
                            sb.Append(ch);
                        break;
                }
            }

            return tokenType != TokenType.EOF;
        }

        /// <summary>
        /// Obte el token actual.
        /// </summary>
        /// 
        public string Token {
            get {
                return token;
            }
        }

        /// <summary>
        /// Obte el tipus de token actual.
        /// </summary>
        /// 
        public TokenType TokenType {
            get {
                return tokenType;
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
