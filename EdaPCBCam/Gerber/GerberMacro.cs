namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;

    public sealed class GerberMacro {

        private static int __id = 0;
        private readonly int id;
        private readonly string command;

        public GerberMacro(string command) {

            this.id = __id++;
            this.command = command;
        }

        public int Id {
            get {
                return id;
            }
        }

        public string Command {
            get {
                return command;
            }
        }
    }
}
