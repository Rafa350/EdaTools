namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

    public class GerberImageGenerator: GerberGenerator { 

        public enum ImageType {
            Top,
            Bottom,
            TopSolderMask,
            BottomSolderMask,
            Profile,
            TopLegend,
            BottomLegend
        }

        // Definicio del macro per l'apertura rectangle arrodonit
        // $1: Amplada
        // $2: Alçada
        // $3: Radi de corvatura
        // $4: Angle de rotacio
        //
        private static readonly Macro smdApertureMacro = new Macro(
            "21,1,$1,$2-$3-$3,0,0,$4*\n" +
            "21,1,$1-$3-$3,$2,0,0,$4*\n" +
            "$5=$1/2*\n" +
            "$6=$2/2*\n" +
            "$7=2x$3*\n" +
            "1,1,$7,$5-$3,$6-$3,$4*\n" +
            "1,1,$7,-$5+$3,$6-$3,$4*\n" +
            "1,1,$7,-$5+$3,-$6+$3,$4*\n" +
            "1,1,$7,$5-$3,-$6+$3,$4*");

        // Definicio del macro per l'apertura rectangle
        // $1: Amplada
        // $2: Alçada
        // $3: Diametre del forat
        // $4: Angle de rotacio
        //
        private static readonly Macro squareApertureMacro = new Macro(
            "21,1,$1,$2,0,0,$4*\n" +
            "1,0,$3,0,0,0*");

        // Definicio del macro per l'apertura ovalada
        // $1: Amplada
        // $2: Alçada
        // $3: Diametre del forat
        // $4: Angle de rotacio
        //
        private const string oblongApertureMacro = "";

        // Definicio del macro per l'apertura octogonal
        // $1: Amplada
        // $2: Alçada
        // $3: Diametre del forat
        // $4: Angle de rotacio
        //
        private const string octagonApertureMacro = "";

        public GerberImageGenerator() {
        }

        /// <summary>
        /// Genera un fitxer gerber.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layers">Llista de capes a procesar.</param>
        /// <param name="fileName">Nom del fitxer de sortida.</param>
        /// 
        public virtual void Generate(Board board, IList<Layer> layers, ImageType imageType, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {

                     // Crea llista de macros
                    //
                    IList<Macro> macros = BuildMacroList();

                    IDictionary<string, Aperture> apertures = new Dictionary<string, Aperture>();
                    board.AcceptVisitor(new DefineAperturesVisitor(layers, apertures));

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio de capcelera, unitats, format, etc.
                    //
                    gb.Comment("EdaTools v1.0.");
                    gb.Comment("EdaTools CAM processor. Gerber generator.");
                    gb.Comment("BEGIN HEADER");
                    switch (imageType) {
                        case ImageType.Top:
                            gb.Attribute(".FileFunction,Copper,L1,Top,Signal");
                            gb.Attribute(".FilePolarity,Positive");
                            break;

                        case ImageType.Bottom:
                            gb.Attribute(".FileFunction,Copper,L2,Bottom,Signal");
                            gb.Attribute(".FilePolarity,Positive");
                            break;

                        case ImageType.TopSolderMask:
                            gb.Attribute(".FileFunction,Soldermask,Top");
                            gb.Attribute(".FilePolarity,Negative");
                            break;

                        case ImageType.BottomSolderMask:
                            gb.Attribute(".FileFunction,Soldermask,Bot");
                            gb.Attribute(".FilePolarity,Negative");
                            break;

                        case ImageType.Profile:
                            gb.Attribute(".FileFunction,Profile,NP");
                            gb.Attribute(".FilePolarity,Positive");
                            break;
                    }
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.SetOffset(0, 0);
                    gb.SetPolarity(true);
                    gb.SetAperturePolarity(AperturePolarity.Dark);
                    gb.SetApertureRotation(0);
                    gb.Comment("END HEADER");

                    // Definicio de macros per les apertures
                    //
                    gb.Comment("BEGIN MACROS");
                    gb.DefineMacros(macros);
                    gb.Comment("END MACROS");

                    // Definicio de les apertures
                    //
                    gb.Comment("BEGIN APERTURES");
                    gb.DefineApertures(apertures.Values);
                    gb.Comment("END APERTURES");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    board.AcceptVisitor(new ImageGeneratorVisitor(gb, layers, apertures));
                    gb.Comment("END IMAGE");

                    // Final
                    //
                    gb.EndFile();
                }
            }
        }

        /// <summary>
        /// Crea la llista de macros.
        /// </summary>
        /// <returns>La llista de macros</returns>
        /// 
        protected virtual IList<Macro> BuildMacroList() {

            List<Macro> macros = new List<Macro>();
            macros.Add(smdApertureMacro);
            macros.Add(squareApertureMacro);

            return macros;
        }

        private sealed class DefineAperturesVisitor : DefaultVisitor {

            private readonly IList<Layer> layers;
            private readonly IDictionary<string, Aperture> apertures;

            public DefineAperturesVisitor(IList<Layer> layers, IDictionary<string, Aperture> apertures) {

                this.layers = layers;
                this.apertures = apertures;
            }

            public override void Visit(LineElement line) {

                if (line.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(line);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, new CircleAperture(line.Thickness));
                }
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.Thickness == 0) {
                    if (rectangle.InAnyLayer(layers)) {
                        string key = ApertureKeyGenerator.GenerateKey(rectangle);
                        if (!apertures.ContainsKey(key))
                            apertures.Add(key, new RectangleAperture(rectangle.Size.Width, rectangle.Size.Height));
                    }
                }
            }

            public override void Visit(ViaElement via) {

                if (via.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(via, false);
                    if (!apertures.ContainsKey(key)) {
                        switch (via.Shape) {
                            case ViaElement.ViaShape.Circular:
                                apertures.Add(key, new CircleAperture(via.Size));
                                break;

                            case ViaElement.ViaShape.Square:
                                apertures.Add(key, new RectangleAperture(via.Size, via.Size));
                                break;

                            case ViaElement.ViaShape.Octogonal:
                                apertures.Add(key, new PoligonAperture(8, via.Size, 22.5));
                                break;
                        }
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad, false);
                    if (!apertures.ContainsKey(key)) {
                        double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                        switch (pad.Shape) {
                            case ThPadElement.ThPadShape.Circular:
                                apertures.Add(key, new CircleAperture(pad.Size));
                                break;

                            case ThPadElement.ThPadShape.Square:
                                apertures.Add(key, new MacroAperture(squareApertureMacro, pad.Size, pad.Size, 0, pad.Rotate));
                                break;

                            case ThPadElement.ThPadShape.Octogonal:
                                apertures.Add(key, new PoligonAperture(8, pad.Size, 22.5 + rotate));
                                break;

                            case ThPadElement.ThPadShape.Oval:
                                apertures.Add(key, new ObroundAperture(pad.Size * 2, pad.Size));
                                break;
                        }
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key)) {
                        double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                        if (pad.Roundnes > 0) {
                            double radius = (pad.Roundnes - 0.01) * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                            apertures.Add(key, new MacroAperture(smdApertureMacro, pad.Size.Width, pad.Size.Height, radius, rotate));
                        }
                        else
                            apertures.Add(key, new MacroAperture(squareApertureMacro, pad.Size.Width, pad.Size.Height, 0, rotate));
                    }
                }
            }
        }

        private sealed class ImageGeneratorVisitor : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly IDictionary<string, Aperture> apertures = new Dictionary<string, Aperture>();

            public ImageGeneratorVisitor(GerberBuilder gb, IList<Layer> layers, IDictionary<string, Aperture> apertures) {

                this.gb = gb;
                this.layers = layers;
                this.apertures = apertures;
            }

            public override void Visit(LineElement line) {

                if (line.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(line);
                    gb.SelectAperture(apertures[key]);
                    gb.Operation(line.StartPosition.X, line.StartPosition.Y, OperationCode.Move);
                    gb.Operation(line.EndPosition.X, line.EndPosition.Y, OperationCode.Interpolate);
                }
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.Thickness == 0) {
                    if (rectangle.InAnyLayer(layers)) {
                        string key = ApertureKeyGenerator.GenerateKey(rectangle);

                        Point p = rectangle.Position;
                        if (VisitingPart != null)
                            p = Transform(p, VisitingPart.Position, VisitingPart.Rotate);

                        gb.SelectAperture(apertures[key]);
                        gb.Operation(p.X, p.Y, OperationCode.Flash);
                    }
                }
            }

            public override void Visit(ViaElement via) {

                if (via.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(via, false);
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

                if (pad.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad, false);

                    Point p = pad.Position;
                    if (VisitingPart != null)
                        p = Transform(p, VisitingPart.Position, VisitingPart.Rotate);

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

                if (pad.InAnyLayer(layers)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad);

                    Point p = pad.Position;
                    if (VisitingPart != null)
                        p = Transform(p, VisitingPart.Position, VisitingPart.Rotate);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Aplica una transformacio a un punt.
            /// </summary>
            /// <param name="p">El punt a transformar.</param>
            /// <param name="center">Punt del centre de rotacio.</param>
            /// <param name="rotate">Angle de rotacio.</param>
            /// <returns>El punt transformat.</returns>
            /// 
            private Point Transform(Point p, Point center, double rotate) {

                Matrix m = new Matrix();
                m.Translate(center.X, center.Y);
                m.RotateAt(rotate, center.X, center.Y);

                return m.Transform(p);
            }
        }
    }
}
