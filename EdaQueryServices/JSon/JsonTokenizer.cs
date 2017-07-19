namespace MikroPic.EdaTools.v1.JSon {

    using System.Text;
    using System;
    using System.IO;

    public sealed class JSonTokenizer {

        public enum TokenType {
            Unknown,
            Eof,
            BeginObject,
            EndObject,
            BeginArray,
            EndArray,
            String,
            Number,
            Colon,
            Comma,
            Null,
            Boolean
        };

        private readonly TextReader reader;
        private int position = 0;
        private TokenType tokenType = TokenType.Unknown;
        private string token = null;
        private char ungetCh = '\uFFFF';
        private bool ungetting = false;

        public JSonTokenizer(TextReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            this.reader = reader;
        }

        public TokenType GetToken() {

            if (ungetting)
                ungetting = false;

            else {
                StringBuilder tokenBuilder = new StringBuilder();
                tokenType = TokenType.Unknown;
                int state = 0;
                while (state != -1) {
                    char ch;
                    if (ungetCh == '\uFFFF') {
                        ch = Convert.ToChar(reader.Read());
                        position += 1;
                    }
                    else {
                        ch = ungetCh;
                        ungetCh = '\uFFFF';
                    }
                    if (ch == '\uFFFF')
                        tokenType = TokenType.Eof;
                    else {
                        switch (state) {
                            case 0:
                                if (ch == '{') {
                                    tokenBuilder.Append(ch);
                                    tokenType = TokenType.BeginObject;
                                    state = -1;
                                }
                                else if (ch == '}') {
                                    tokenBuilder.Append(ch);
                                    tokenType = TokenType.EndObject;
                                    state = -1;
                                }
                                else if (ch == ':') {
                                    tokenBuilder.Append(ch);
                                    tokenType = TokenType.Colon;
                                    state = -1;
                                }
                                else if (ch == '[') {
                                    tokenBuilder.Append(ch);
                                    tokenType = TokenType.BeginArray;
                                    state = -1;
                                }
                                else if (ch == ']') {
                                    tokenBuilder.Append(ch);
                                    tokenType = TokenType.EndArray;
                                    state = -1;
                                }
                                else if (ch == ',') {
                                    tokenBuilder.Append(ch);
                                    tokenType = TokenType.Comma;
                                    state = -1;
                                }
                                else if (ch == '"')
                                    state = 1;
                                else if (Char.IsDigit(ch) || (ch == '-')) {
                                    tokenBuilder.Append(ch);
                                    state = 2;
                                }
                                else if (!Char.IsWhiteSpace(ch)) {
                                    tokenBuilder.Append(ch);
                                    state = 3;
                                }
                                break;

                            case 1:
                                if (ch == '"') {
                                    tokenType = TokenType.String;
                                    state = -1;
                                }
                                else
                                    tokenBuilder.Append(ch);
                                break;

                            case 2:
                                if (Char.IsDigit(ch) || ch == '.')
                                    tokenBuilder.Append(ch);
                                else {
                                    ungetCh = ch;
                                    tokenType = TokenType.Number;
                                    state = -1;
                                }
                                break;

                            case 3:
                                if (Char.IsLetterOrDigit(ch))
                                    tokenBuilder.Append(ch);
                                else {
                                    ungetCh = ch;
                                    state = -1;
                                }
                                break;
                        }
                    }
                }
                token = tokenBuilder.ToString();
                if (tokenType == TokenType.Unknown) {
                    if (token == "null")
                        tokenType = TokenType.Null;
                    else if (token == "true")
                        tokenType = TokenType.Boolean;
                    else if (token == "false")
                        tokenType = TokenType.Boolean;
                }
            }
            return tokenType;
        }

        public void UngetToken() {

            ungetting = true;
        }

        public string Token {
            get {
                return token;
            }
        }

        public TokenType TypenType {
            get {
                return tokenType;
            }
        }

        public int Position {
            get {
                return position;
            }
        }
    }
}
