namespace MikroPic.EdaTools.v1.Core.Import.KiCad.Infrastructure {

    using System;
    using System.IO;

    public enum TokenType {
        Unknown,
        StartBlock,
        EndBlock,
        Word,
        EndOfFile
    }

    public sealed class STokenizer {

        private readonly TextReader _reader;
        private int _index;

        /// <summary>
        /// Conmstructor del objecte.
        /// </summary>
        /// <param name="reader">Lector de text.</param>
        /// 
        public STokenizer(TextReader reader) {

            _reader = reader;
            _index = 0;
        }

        /// <summary>
        /// Obte el caracter actual.
        /// </summary>
        /// <returns>El caracter.</returns>
        /// 
        private char Peek() {

            return (char)_reader.Peek();
        }

        /// <summary>
        /// Avança al seguent caracter.
        /// </summary>
        /// 
        private void Next() {

            _reader.Read();
            _index += 1;
        }

        /// <summary>
        /// Obte un token
        /// </summary>
        /// <param name="position">La posicio del token.</param>
        /// <param name="length">La longitut del token.</param>
        /// <returns>El tipus de token.</returns>
        /// <remarks>El valor de 'position' i 'length', nomes son valids en cas d'un token de tipus 'Word'.</remarks>
        /// 
        public TokenType GetToken(out int position, out int length) {

            int state = 0;

            position = 0;
            length = 0;
            TokenType type = TokenType.Unknown;

            while (type == TokenType.Unknown) {
                char ch = Peek();
                if (ch == '\uFFFF')
                    type = TokenType.EndOfFile;
                else
                    switch (state) {
                        case 0:
                            if (ch == '(') {
                                position = _index;
                                length = 1;
                                Next();
                                type = TokenType.StartBlock;
                            }
                            else if (ch == ')') {
                                position = _index;
                                length = 1;
                                Next();
                                type = TokenType.EndBlock;
                            }
                            else if (Char.IsWhiteSpace((char)ch))
                                Next();

                            else if (ch == '"') {
                                Next();
                                position = _index;
                                state = 2;
                            }
                            else {
                                position = _index;
                                Next();
                                state = 1;
                            }
                            break;

                        case 1:
                            if (Char.IsWhiteSpace(ch) || ch == '(' || ch == ')') {
                                length = _index - position;
                                type = TokenType.Word;
                            }
                            else
                                Next();
                            break;

                        case 2:
                            if (ch == '"') {
                                length = _index - position;
                                Next();
                                type = TokenType.Word;
                            }
                            else 
                                Next();
                            break;
                    }
            }

            return type;
        }
    }
}
