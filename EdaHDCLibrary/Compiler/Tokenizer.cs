namespace MikroPic.EdaTools.v1.Hdc.Compiler {

    using System;
    using System.IO;

    public enum TokenType {
        Attribute,
        Begin,
        Colon,
        Device,
        End,
        Equal,
        Integer,
        Link,
        Literal,
        Module,
        Port,
        Real,
        SemiColon,
        String
    }

    public sealed class Tokenizer {

        private readonly TextReader reader;
        private int state;

        public Tokenizer(TextReader reader) {

            if (reader == null)
                throw new ArgumentNullException("reader");

            this.reader = reader;
        }

        public string Get() {

            return null;
        }

        public void Unget() {

        }
    }
}
