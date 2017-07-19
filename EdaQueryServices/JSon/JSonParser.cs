namespace MikroPic.EdaTools.v1.JSon {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.JSon.Model;

    public sealed class JSonParser {

        public JSonObject Parse(string jsonString) {

            if (String.IsNullOrEmpty(jsonString))
                throw new ArgumentNullException("jsonString");

            return Parse(new StringReader(jsonString));
        }

        public JSonObject Parse(Stream jsonStream) {

            if (jsonStream == null)
                throw new ArgumentNullException("jsonStream");

            return Parse(new StreamReader(jsonStream));
        }

        public JSonObject Parse(TextReader jsonReader) {

            if (jsonReader == null)
                throw new ArgumentNullException("jsonReader");

            JSonTokenizer tokenizer = new JSonTokenizer(jsonReader);
            return ParseObject(tokenizer);
        }

        private JSonObject ParseObject(JSonTokenizer tokenizer) {

            if (tokenizer == null)
                throw new ArgumentNullException("tokenizer");

            string name = null;
            string className = null;

            List<JSonProperty> properties = new List<JSonProperty>();

            int state = 0;
            while (state != -1) {
                JSonTokenizer.TokenType tokenType = tokenizer.GetToken();
                switch (state) {
                    case 0:
                        if (tokenType == JSonTokenizer.TokenType.BeginObject)
                            state = 1;
                        else
                            throw new InvalidDataException("Se esperaba '{'.");
                        break;

                    case 1:
                        if (tokenType == JSonTokenizer.TokenType.EndObject) 
                            state = -1;
                        else if (tokenType == JSonTokenizer.TokenType.String) {
                            name = tokenizer.Token;
                            state = 2;
                        }
                        break;

                    case 2:
                        if (tokenType == JSonTokenizer.TokenType.Colon) {
                            if (name == "__class__") {
                                if (tokenizer.GetToken() != JSonTokenizer.TokenType.String)
                                    throw new InvalidDataException("Se esperaba 'string'.");
                                className = tokenizer.Token;
                            }
                            else {
                                JSonValue jsonValue = ParseValue(tokenizer);
                                properties.Add(new JSonProperty(name, jsonValue));
                            }
                            state = 3;
                        }
                        else
                            throw new InvalidDataException("Se esperaba ':'.");
                        break;

                    case 3:
                        if (tokenType == JSonTokenizer.TokenType.Comma)
                            state = 1;
                        else if (tokenType == JSonTokenizer.TokenType.EndObject) {
                            state = -1;
                        }
                        else
                            throw new InvalidDataException("Se esperaba '}'.");
                        break;
                }
            }

            return new JSonObject(className, properties.ToArray());
        }

        private JSonArray ParseArray(JSonTokenizer tokenizer) {

            List<JSonValue> values = new List<JSonValue>();

            int state = 0;
            while (state != -1) {
                JSonTokenizer.TokenType tokenType = tokenizer.GetToken();
                switch (state) {
                    case 0:
                        if (tokenType == JSonTokenizer.TokenType.BeginArray)
                            state = 1;
                        else
                            throw new InvalidDataException("Se esperaba '['.");
                        break;

                    case 1:
                        if (tokenType == JSonTokenizer.TokenType.EndArray)
                            state = -1;
                        else {
                            tokenizer.UngetToken();
                            JSonValue jsonValue = ParseValue(tokenizer);
                            values.Add(jsonValue);
                            state = 2;
                        }
                        break;

                    case 2:
                        if (tokenType == JSonTokenizer.TokenType.Comma)
                            state = 1;
                        else if (tokenType == JSonTokenizer.TokenType.EndArray)
                            state = -1;
                        else
                            throw new InvalidDataException("Selesperaba ',' o ']'.");
                        break;
                }
            }

            return new JSonArray(values.ToArray());
        }

        private JSonValue ParseValue(JSonTokenizer tokenizer) {

            JSonValue jsonValue = null;

            JSonTokenizer.TokenType tokenType = tokenizer.GetToken();
            if (tokenType == JSonTokenizer.TokenType.String)
                jsonValue = new JSonString(tokenizer.Token);

            else if (tokenType == JSonTokenizer.TokenType.Number) {

                int integerValue;
                double realValue;

                if (Int32.TryParse(tokenizer.Token, out integerValue))
                    jsonValue = new JSonInteger(integerValue);

                else if (!Double.TryParse(tokenizer.Token, out realValue))
                    jsonValue = new JSonReal(realValue);

                else
                    throw new InvalidDataException("Se esperaba un numero.");
            }

            else if (tokenType == JSonTokenizer.TokenType.BeginObject) {
                tokenizer.UngetToken();
                jsonValue = ParseObject(tokenizer);
            }

            else if (tokenType == JSonTokenizer.TokenType.BeginArray) {
                tokenizer.UngetToken();
                jsonValue = ParseArray(tokenizer);
            }

            else if (tokenType == JSonTokenizer.TokenType.Null)
                jsonValue = null;

            else if (tokenType == JSonTokenizer.TokenType.Boolean)
                jsonValue = new JSonBoolean(Boolean.Parse(tokenizer.Token));

            else
                throw new InvalidDataException("Se esperava un valor.");

            return jsonValue;
        }
    }
}
