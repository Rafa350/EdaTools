namespace MikroPic.EdaTools.v1.Cam.Ipcd365 {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Cam.Ipcd365.Builder;

    /// <summary>
    /// Generador de codi en format IPCD365
    /// </summary>
    public sealed class Icpd365Generator {

        private readonly Board board;

        /// <summary>
        /// Contructor del objecte
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        public Icpd365Generator(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        /// <summary>
        /// Genera el nom del fitxer
        /// </summary>
        /// <param name="prefix">Prefix del nom.</param>
        /// <returns>El nom del fitxer.</returns>
        /// 
        public string GenerateFileName(string prefix) {

            return prefix + ".ipc";
        }

        /// <summary>
        /// Genera el contingut del fitxer
        /// </summary>
        /// <param name="writer">Esciptor del text de sortida.</param>
        /// 
        public void GenerateContent(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            Ipcd365Builder builder = new Ipcd365Builder(writer);

            GenerateFileHeader(builder);
            GenerateVias(builder);
            GeneratePads(builder);
            GenerateNets(builder);
            GenerateFileTail(builder);
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GenerateFileHeader(Ipcd365Builder builder) {

            builder.Comment("BEGIN FILE");
            builder.Comment("EdaTools v1.0.");
            builder.Comment("EdaTools CAM processor. IPC-D-365 generator.");
            builder.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            builder.Comment("BEGIN HEADER");
            builder.SetVersion();
            builder.SetUnits();
        }

        /// <summary>
        /// Genera el final de fitxer.
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GenerateFileTail(Ipcd365Builder builder) {

            builder.Comment(String.Format("End timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            builder.Comment("END FILE");
            builder.EndFile();
        }

        /// <summary>
        /// Genera la definicio de vias
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GenerateVias(Ipcd365Builder builder) {

            builder.Comment("BEGIN VIAS");
            
            IVisitor visitor = new ViasVisitor(builder, board);
            visitor.Run();

            builder.Comment("END VIAS");
        }

        /// <summary>
        /// Genera la definicio de pads
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GeneratePads(Ipcd365Builder builder) {

            builder.Comment("BEGIN PADS");

            IVisitor visitor = new PadsVisitor(builder, board);
            visitor.Run();

            builder.Comment("END PADS");
        }

        /// <summary>
        /// Genera la definicio de senyals
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GenerateNets(Ipcd365Builder builder) {

            builder.Comment("BEGIN NETS");

            foreach (Signal signal in board.Signals) {
                IEnumerable<Tuple<IConectable, Part>> items = board.GetConnectedItems(signal);
                if (items != null)
                    foreach (Tuple<IConectable, Part> item in items) {
                        if (item.Item2 is Part) {

                        }
                    }
            }

            builder.Comment("END NETS");
        }

        private sealed class ViasVisitor: ElementVisitor {

            private readonly Ipcd365Builder builder;

            public ViasVisitor(Ipcd365Builder builder, Board board) 
                : base(board, null) {

                this.builder = builder;
            }

            public override void Visit(ViaElement via) {

                Signal signal = Board.GetSignal(via);
                builder.Via(via.Position, via.Drill, signal.Name);
            }
        }

        private sealed class PadsVisitor : ElementVisitor {

            private readonly Ipcd365Builder builder;

            public PadsVisitor(Ipcd365Builder builder, Board board)
                : base(board, null) {

                this.builder = builder;
            }

            public override void Visit(SmdPadElement pad) {

                Signal signal = Board.GetSignal(pad, Part, false);
                if (signal != null) {

                    Transformation t = Part.GetLocalTransformation();
                    PointInt position = t.ApplyTo(pad.Position);

                    builder.SmdPad(position, TestAccess.Top, Part.Name, pad.Name, signal.Name);
                }
            }

            public override void Visit(ThPadElement pad) {

                Signal signal = Board.GetSignal(pad, Part, false);
                if (signal != null) {

                    Transformation t = Part.GetLocalTransformation();
                    PointInt position = t.ApplyTo(pad.Position);

                    builder.ThPad(position, pad.Drill, Part.Name, pad.Name, signal.Name);
                }
            }
        }
    }
}
