namespace MikroPic.EdaTools.v1.Cam.Generators.Ipcd356 {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Cam.Generators.Ipcd356.Builder;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
    using System;
    using System.IO;

    /// <summary>
    /// Generador de codi en format IPCD356
    /// </summary>
    public sealed class Ipcd356Generator: Generator {

        public Ipcd356Generator(Target target):
            base(target) {

        }

        /// <summary>
        /// Genera el fitxer corresponent a una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="outputFolder">La carpeta de sortida.</param>
        /// <param name="options">Opcions.</param>
        /// 
        public override void Generate(Board board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException("outputFolder");

            string fileName = Path.Combine(outputFolder, Target.FileName);

            // Crea el fitxer de sortida
            //
            using (TextWriter writer = new StreamWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))) {

                Ipcd356Builder builder = new Ipcd356Builder(writer);

                GenerateFileHeader(builder);
                builder.Comment("BEGIN BOARD");
                GenerateVias(builder, board);
                GeneratePads(builder, board);
                GenerateNets(builder, board);
                builder.Comment("END BOARD");
                GenerateFileTail(builder);
            }
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GenerateFileHeader(Ipcd356Builder builder) {

            builder.Comment("BEGIN FILE");
            builder.Comment("EdaTools v1.0.");
            builder.Comment("EdaTools CAM processor. IPC-D-356 generator.");
            builder.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            builder.Comment("BEGIN HEADER");
            builder.SetVersion();
            builder.SetUnits(Units.Millimeters);
            builder.SetImage();
        }

        /// <summary>
        /// Genera el final de fitxer.
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// 
        private void GenerateFileTail(Ipcd356Builder builder) {

            builder.Comment(String.Format("End timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            builder.Comment("END FILE");
            builder.EndFile();
        }

        /// <summary>
        /// Genera la definicio de vias
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// <param name="board">La placa a procesar.</param>
        /// 
        private void GenerateVias(Ipcd356Builder builder, Board board) {

            builder.Comment("BEGIN VIAS");
            
            IVisitor visitor = new ViasVisitor(builder);
            board.AcceptVisitor(visitor);

            builder.Comment("END VIAS");
        }

        /// <summary>
        /// Genera la definicio de pads
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// <param name="board">La placa a procesar.</param>
        /// 
        private void GeneratePads(Ipcd356Builder builder, Board board) {

            builder.Comment("BEGIN PADS");

            IVisitor visitor = new PadsVisitor(builder);
            board.AcceptVisitor(visitor);

            builder.Comment("END PADS");
        }

        /// <summary>
        /// Genera la definicio de senyals
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// <param name="board">La placa a procesar.</param>
        /// 
        private void GenerateNets(Ipcd356Builder builder, Board board) {

            builder.Comment("BEGIN NETS");

            IVisitor visitor = new NetsVisitor(builder);
            board.AcceptVisitor(visitor);

            builder.Comment("END NETS");
        }

        /// <summary>
        /// Visitador per generar els senyals
        /// </summary>
        private sealed class NetsVisitor: SignalVisitor {

            private readonly Ipcd356Builder builder;

            public NetsVisitor(Ipcd356Builder builder) { 

                this.builder = builder;
            }

            public override void Visit(LineElement line) {

                int layerNum = 3;
                if (line.LayerSet.Contains(Layer.TopId))
                    layerNum = 1;
                else if (line.LayerSet.Contains(Layer.BottomId))
                    layerNum = 2;

                if (layerNum > 0) { 
                    Point[] points = new Point[2];
                    points[0] = line.StartPosition;
                    points[1] = line.EndPosition;
                    builder.Conductor(points, layerNum, line.Thickness, Signal.Name);
                }
            }

            public override void Visit(ArcElement arc) {

                int layerNum = 3;
                if (arc.LayerSet.Contains(Layer.TopId))
                    layerNum = 1;
                else if (arc.LayerSet.Contains(Layer.BottomId))
                    layerNum = 2;

                if (layerNum > 0) {
                    Point[] points = new Point[2];
                    points[0] = arc.StartPosition;
                    points[1] = arc.EndPosition;
                    builder.Conductor(points, layerNum, arc.Thickness, Signal.Name);
                }
            }
        }

        /// <summary>
        /// Visitador per generar les definicions de vias
        /// </summary>
        private sealed class ViasVisitor: ElementVisitor {

            private readonly Ipcd356Builder builder;

            public ViasVisitor(Ipcd356Builder builder) {  

                this.builder = builder;
            }

            public override void Visit(ViaElement via) {

                Signal signal = Board.GetSignal(via);
                builder.Via(via.Position, via.Drill, signal.Name);
            }
        }

        /// <summary>
        /// Visitador per generar les definicions de pads
        /// </summary>
        private sealed class PadsVisitor : ElementVisitor {

            private readonly Ipcd356Builder builder;

            public PadsVisitor(Ipcd356Builder builder) { 

                this.builder = builder;
            }

            public override void Visit(SmdPadElement pad) {

                Signal signal = Board.GetSignal(pad, Part, false);
                if (signal != null) {

                    Transformation t = Part.GetLocalTransformation();
                    Point position = t.ApplyTo(pad.Position);

                    builder.SmdPad(position, TestAccess.Top, Part.Name, pad.Name, signal.Name);
                }
            }

            public override void Visit(ThPadElement pad) {

                Signal signal = Board.GetSignal(pad, Part, false);
                if (signal != null) {

                    Transformation t = Part.GetLocalTransformation();
                    Point position = t.ApplyTo(pad.Position);

                    builder.ThPad(position, pad.Drill, Part.Name, pad.Name, signal.Name);
                }
            }
        }
    }
}
