namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;
    using System.Collections.Generic;

    internal static class ApertureTableGenerator {

        private class Visitor: DefaultVisitor {

            private readonly Dictionary<string, ApertureBase> apertures = new Dictionary<string, ApertureBase>();
            private int apertureId = 10;

            public override void Visit(Board board) {

                if (board.Parts != null)
                    foreach (Part part in board.Parts)
                        part.AcceptVisitor(this);

                if (board.Signals != null)
                    foreach (Signal signal in board.Signals)
                        signal.AcceptVisitor(this);
            }

            public override void Visit(Part part) {

                if (part.Component != null)
                    part.Component.AcceptVisitor(this);
            }

            public override void Visit(Component component) {

                if (component.Elements != null)
                    foreach (ElementBase element in component.Elements)
                        element.AcceptVisitor(this);
            }

            public override void Visit(Signal signal) {

                if (signal.Elements != null)
                    foreach (ElementBase element in signal.Elements)
                        element.AcceptVisitor(this);
            }

            public override void Visit(ViaElement via) {

                string key = ApertureKeyGenerator.GenerateKey(via);
                if (!apertures.ContainsKey(key)) {
                    ApertureBase aperture = null;

                    switch (via.Shape) {
                        default:
                        case ViaElement.ViaShape.Circular:
                            aperture = new CircleAperture(apertureId, via.Size, via.Drill);
                            break;

                        case ViaElement.ViaShape.Square:
                            aperture = new SquareAperture(apertureId, via.Size, via.Drill);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            aperture = new OctagonAperture(apertureId, via.Size, via.Drill);
                            break;
                    }

                    apertures.Add(key, aperture);
                    apertureId++;
                }
            }

            public override void Visit(ThPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad);
                if (!apertures.ContainsKey(key)) {
                    ApertureBase aperture = null;
                    switch (pad.Shape) {
                        default:
                        case ThPadElement.ThPadShape.Circular:
                            aperture = new CircleAperture(apertureId, pad.Size, pad.Drill);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            aperture = new SquareAperture(apertureId, pad.Size, pad.Drill);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            aperture = new OctagonAperture(apertureId, pad.Size, pad.Drill);
                            break;
                    }
                    apertures.Add(key, aperture);
                    apertureId++;
                }
            }

            public override void Visit(SmdPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad);
                if (!apertures.ContainsKey(key)) {
                    ApertureBase aperture = new RoundRectAperture(apertureId, pad.Size.Width, pad.Size.Height, pad.Roundnes);
                    apertures.Add(key, aperture);
                    apertureId++;
                }
            }

            public IDictionary<string, ApertureBase> Apertures {
                get {
                    return apertures;
                }
            }
        }

        public static IDictionary<string, ApertureBase> Generate(Board board) {

            Visitor visitor = new Visitor();
            visitor.Visit(board);

            return visitor.Apertures;
        }
    }
}
