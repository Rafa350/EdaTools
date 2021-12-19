﻿using System;
using System.IO;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.IPCD356.Builder;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPCD356 {

    /// <summary>
    /// Generador de codi en format IPCD356
    /// </summary>
    public sealed class IPCD356Generator : Generator {

        public IPCD356Generator(Target target) :
            base(target) {

        }

        /// <summary>
        /// Genera el fitxer corresponent a una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="outputFolder">La carpeta de sortida.</param>
        /// <param name="options">Opcions.</param>
        /// 
        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);

            // Crea el fitxer de sortida
            //
            using (TextWriter writer = new StreamWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))) {

                IPCD356Builder builder = new IPCD356Builder(writer);

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
        private void GenerateFileHeader(IPCD356Builder builder) {

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
        private void GenerateFileTail(IPCD356Builder builder) {

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
        private void GenerateVias(IPCD356Builder builder, EdaBoard board) {

            builder.Comment("BEGIN VIAS");

            IEdaBoardVisitor visitor = new ViasVisitor(builder);
            board.AcceptVisitor(visitor);

            builder.Comment("END VIAS");
        }

        /// <summary>
        /// Genera la definicio de pads
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// <param name="board">La placa a procesar.</param>
        /// 
        private void GeneratePads(IPCD356Builder builder, EdaBoard board) {

            builder.Comment("BEGIN PADS");

            IEdaBoardVisitor visitor = new PadsVisitor(builder);
            board.AcceptVisitor(visitor);

            builder.Comment("END PADS");
        }

        /// <summary>
        /// Genera la definicio de senyals
        /// </summary>
        /// <param name="builder">El generador de codi.</param>
        /// <param name="board">La placa a procesar.</param>
        /// 
        private void GenerateNets(IPCD356Builder builder, EdaBoard board) {

            builder.Comment("BEGIN NETS");

            IEdaBoardVisitor visitor = new NetsVisitor(builder);
            board.AcceptVisitor(visitor);

            builder.Comment("END NETS");
        }

        /// <summary>
        /// Visitador per generar els senyals
        /// </summary>
        private sealed class NetsVisitor : EdaSignalVisitor {

            private readonly IPCD356Builder _builder;

            public NetsVisitor(IPCD356Builder builder) {

                _builder = builder;
            }

            public override void Visit(EdaLineElement line) {

                int layerNum = 3;
                if (line.IsOnLayer(EdaLayerId.TopCopper))
                    layerNum = 1;
                else if (line.IsOnLayer(EdaLayerId.BottomCopper))
                    layerNum = 2;

                if (layerNum > 0) {
                    EdaPoint[] points = new EdaPoint[2];
                    points[0] = line.StartPosition;
                    points[1] = line.EndPosition;
                    _builder.Conductor(points, layerNum, line.Thickness, Signal.Name);
                }
            }

            public override void Visit(EdaArcElement arc) {

                int layerNum = 3;
                if (arc.IsOnLayer(EdaLayerId.TopCopper))
                    layerNum = 1;
                else if (arc.IsOnLayer(EdaLayerId.BottomCopper))
                    layerNum = 2;

                if (layerNum > 0) {
                    EdaPoint[] points = new EdaPoint[2];
                    points[0] = arc.StartPosition;
                    points[1] = arc.EndPosition;
                    _builder.Conductor(points, layerNum, arc.Thickness, Signal.Name);
                }
            }
        }

        /// <summary>
        /// Visitador per generar les definicions de vias
        /// </summary>
        private sealed class ViasVisitor : EdaElementVisitor {

            private readonly IPCD356Builder builder;

            public ViasVisitor(IPCD356Builder builder) {

                this.builder = builder;
            }

            public override void Visit(EdaViaElement via) {

                var signal = Board.GetSignal(via, null, false);
                if (signal != null)
                    builder.Via(via.Position, via.Drill, signal.Name);
            }
        }

        /// <summary>
        /// Visitador per generar les definicions de pads
        /// </summary>
        private sealed class PadsVisitor : EdaElementVisitor {

            private readonly IPCD356Builder _builder;

            public PadsVisitor(IPCD356Builder builder) {

                _builder = builder;
            }

            public override void Visit(EdaSmdPadElement pad) {

                EdaSignal signal = Board.GetSignal(pad, Part, false);
                if (signal != null) {

                    Transformation t = Part.GetLocalTransformation();
                    EdaPoint position = t.ApplyTo(pad.Position);

                    _builder.SmdPad(position, TestAccess.Top, Part.Name, pad.Name, signal.Name);
                }
            }

            public override void Visit(EdaThPadElement pad) {

                EdaSignal signal = Board.GetSignal(pad, Part, false);
                if (signal != null) {

                    Transformation t = Part.GetLocalTransformation();
                    EdaPoint position = t.ApplyTo(pad.Position);

                    _builder.ThPad(position, pad.Drill, Part.Name, pad.Name, signal.Name);
                }
            }
        }
    }
}
