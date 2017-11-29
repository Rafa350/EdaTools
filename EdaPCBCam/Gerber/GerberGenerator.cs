﻿namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    public sealed class GerberGenerator {

        // Definicio del macro per l'apertura rectangle arrodonit
        // $1: Amplada
        // $2: Alçada
        // $3: Radi de corvatura
        // $4: Angle de rotacio
        //
        private const int smdApertureMacroId = 0;
        private const string smdApertureMacro =
            "21,1,$1,$2-$3-$3,0,0,$4*\n" +
            "21,1,$1-$3-$3,$2,0,0,$4*\n" +
            "$5=$1/2*\n" +
            "$6=$2/2*\n" +
            "$7=2x$3*\n" +
            "1,1,$7,$5-$3,$6-$3,$4*\n" +
            "1,1,$7,-$5+$3,$6-$3,$4*\n" +
            "1,1,$7,-$5+$3,-$6+$3,$4*\n" +
            "1,1,$7,$5-$3,-$6+$3,$4*";

        // Definicio del macro per l'apertura rectangle
        // $1: Amplada
        // $2: Alçada
        // $3: Diametre del forat
        // $4: Angle de rotacio
        //
        private const int squareApertureMacroId = 1;
        private const string squareApertureMacro =
            "21,1,$1,$2,0,0,$4*\n" +
            "1,0,$3,0,0,0*";

        private const int oblongApertureMacroId = 2;
        private const string oblongApertureMacro = "";

        private const int octagonApertureMacroId = 3;
        private const string octagonApertureMacro = "";


        public GerberGenerator() {
        }

        /// <summary>
        /// Genera un fitxer gerber.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layers">Llista de capes a procesar.</param>
        /// <param name="fileName">Nom del fitxer de sortida.</param>
        /// 
        public void Generate(Board board, IList<Layer> layers, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if ((layers == null) || (layers.Count == 0))
                throw new ArgumentNullException("layers");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            IVisitor visitor;

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio de capcelera, unitats, format, etc.
                    //
                    gb.Comment("EdaTools v1.0.");
                    gb.Comment("EdaTools CAM processor. Gerber generator.");
                    gb.Comment("BEGIN HEADER");
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.SetOffset(0, 0);
                    gb.SetPolarity(true);
                    gb.SetAperturePolarity(true);
                    gb.SetApertureRotation(0);
                    gb.Comment("END HEADER");

                    // Definicio de macros per les apertures
                    //
                    gb.Comment("BEGIN MACROS");
                    gb.DefineMacro(smdApertureMacroId, smdApertureMacro);
                    gb.DefineMacro(squareApertureMacroId, squareApertureMacro);
                    gb.Comment("END MACROS");

                    // Definicio de les apertures
                    //
                    gb.Comment("BEGIN APERTURES");
                    visitor = new DefineAperturesVisitor(gb, layers);
                    board.AcceptVisitor(visitor);
                    gb.Comment("END APERTURES");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    visitor = new FlashAperturesVisitor(gb, layers);
                    board.AcceptVisitor(visitor);
                    gb.Comment("END IMAGE");

                    // Final
                    //
                    gb.EndFile();
                }
            }
        }

        private sealed class DefineAperturesVisitor : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly Dictionary<string, int> apertures = new Dictionary<string, int>();
            private Part currentPart = null;

            public DefineAperturesVisitor(GerberBuilder gb, IList<Layer> layers) {

                this.gb = gb;
                this.layers = layers;
            }

            public override void Visit(Part part) {

                currentPart = part;
                base.Visit(part);
                currentPart = null;
            }

            public override void Visit(LineElement line) {

                if (layers.Contains(line.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(line);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, gb.DefineCircleAperture(-1, line.Thickness));
                }
            }

            public override void Visit(ViaElement via) {

                if (layers.Contains(via.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(via);
                    if (!apertures.ContainsKey(key)) {
                        switch (via.Shape) {
                            case ViaElement.ViaShape.Circular:
                                apertures.Add(key, gb.DefineCircleAperture(-1, via.Size));
                                break;

                            case ViaElement.ViaShape.Square:
                                apertures.Add(key, gb.DefineRectangleAperture(-1, via.Size, via.Size));
                                break;

                            case ViaElement.ViaShape.Octogonal:
                                apertures.Add(key, gb.DefinePoligonAperture(8, -1, via.Size, 0));
                                break;
                        }
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (layers.Contains(pad.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key)) {
                        double rotate = pad.Rotate + (currentPart != null ? currentPart.Rotate : 0);
                        switch (pad.Shape) {
                            case ThPadElement.ThPadShape.Circular:
                                apertures.Add(key, gb.DefineCircleAperture(-1, pad.Size));
                                break;

                            case ThPadElement.ThPadShape.Square:
                                apertures.Add(key, gb.DefineMacroAperture(-1, squareApertureMacroId, pad.Size, pad.Size, 0, pad.Rotate));
                                break;

                            case ThPadElement.ThPadShape.Octogonal:
                                apertures.Add(key, gb.DefinePoligonAperture(-1, 8, pad.Size, rotate));
                                break;

                            case ThPadElement.ThPadShape.Oval:
                                apertures.Add(key, gb.DefineObroundAperture(-1, pad.Size * 2, pad.Size));
                                break;
                        }
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (layers.Contains(pad.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key)) {
                        double rotate = pad.Rotate + (currentPart != null ? currentPart.Rotate : 0);
                        if (pad.Roundnes > 0) {
                            double radius = (pad.Roundnes - 0.01) * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                            apertures.Add(key, gb.DefineMacroAperture(-1, smdApertureMacroId, pad.Size.Width, pad.Size.Height, radius, rotate));
                        }
                        else
                            apertures.Add(key, gb.DefineMacroAperture(-1, squareApertureMacroId, pad.Size.Width, pad.Size.Height, 0, rotate));
                    }
                }
            }
        }

        private sealed class FlashAperturesVisitor : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly Dictionary<string, int> apertures = new Dictionary<string, int>();
            private int apertureIndex = 10;
            private Part currentPart;

            public FlashAperturesVisitor(GerberBuilder gb, IList<Layer> layers) {

                this.gb = gb;
                this.layers = layers;
            }

            public override void Visit(Part part) {

                currentPart = part;
                base.Visit(part);
                currentPart = null;
            }

            public override void Visit(LineElement line) {

                if (layers.Contains(line.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(line);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(line.StartPosition.X, line.StartPosition.Y, OperationCode.Move);
                    gb.Operation(line.EndPosition.X, line.EndPosition.Y, OperationCode.Interpolate);
                }
            }

            public override void Visit(ViaElement via) {

                if (layers.Contains(via.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(via);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(via.Position.X, via.Position.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Visita un element de tipus ThPadElement
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (layers.Contains(pad.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    Point p = pad.Position;
                    if (currentPart != null)
                        p = RotateTransform(p, currentPart.Position, currentPart.Rotate);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad.
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (layers.Contains(pad.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    Point p = pad.Position;
                    if (currentPart != null)
                        p = RotateTransform(p, currentPart.Position, currentPart.Rotate);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Aplica una rotacio a un punt.
            /// </summary>
            /// <param name="p">El punt a transformar.</param>
            /// <param name="center">Punt del centre de rotacio.</param>
            /// <param name="rotate">Angle de rotacio.</param>
            /// <returns></returns>
            private Point RotateTransform(Point p, Point center, double rotate) {

                Matrix m = new Matrix();
                m.Translate(center.X, center.Y);
                m.RotateAt(rotate, center.X, center.Y);

                return m.Transform(p);
            }
        }
    }
}
