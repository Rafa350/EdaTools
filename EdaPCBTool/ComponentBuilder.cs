namespace Eda.PCBTool {

    using MikroPic.EdaTools.v1.Model;

    public static class PackageBuilder {

        public static Component DualInLine(string name, string description, int pinCount, 
            float padWidth, float padHeight, float padRoundness, float pitch, float rowSpace, float packageWidth, float packageHeight,
            double stopClearance = 0, double creamClearance = 0) {

            float x, y;
            float radius = 0.5f;

            x = -((pitch * ((pinCount / 2) - 1)) / 2);
            y = -rowSpace / 2;

            Component component = new Component();

            //Size signalPadSize = new Size(padWidth, padHeight);

            double stopPadWidth = padWidth + stopClearance;
            double stopPadHeight = padHeight + stopClearance;
            //Size creamPadSize = new Size(padWidth + creamClearance, padHeight + creamClearance);

            for (int count = 0; count < pinCount / 2; count++) {

                component.Add(new SmdPad((count + 1).ToString(), x, y, padWidth, padHeight, radius));
                //component.Add(new SmdPad(null, x, y, stopPadWidth, stopPadHeight, radius));
                //creamShapes.Add(new Pad(null, x, y, creamPadSize.Width, creamPadSize.Height, 0, PadStyle.Normal, 0, 0, 0, 0));

                x += pitch;
            }

            x = (pitch * ((pinCount / 2) - 1)) / 2;
            y = rowSpace / 2;
            for (int count = pinCount / 2; count < pinCount; count++) {

                component.Add(new SmdPad((count + 1).ToString(), x, y, padWidth, padHeight, radius));
                //stopShapes.Add(new SmdPad(null, x, y, stopPadWidth, stopPadHeight, 0, radius));
                //creamShapes.Add(new Pad(null, x, y, creamPadSize.Width, creamPadSize.Height, 0, PadStyle.Normal, 0, 0, 0, 0));

                x -= pitch;
            }

            float x1 = -packageWidth / 2;
            float y1 = packageHeight / 2;
            float x2 = packageWidth / 2;
            float y2 = -packageHeight / 2;

            float thickness = 0.15f;
            component.Add(new Line(x1, y1, x1 + 1, y1, thickness));
            component.Add(new Line(x1, y2, x1 + 1, y2, thickness));
            component.Add(new Line(x1, y1, x1, 1, thickness));
            //component.Add(new Arc(x1, 0, 1, 270, 90, thickness));
            //silkShapes.Add(new Circle(x1, 0, 0.25, 0)); 
            //silkShapes.Add(new Line(x1, y2, x1, -1, thickness));

            component.Add(new Line(x2, y1, x2 - 1, y1, thickness));
            component.Add(new Line(x2, y2, x2 - 1, y2, thickness));
            component.Add(new Line(x2, y1, x2, y2, thickness));

            layers.Add(new Layer("signal", LayerType.Signal, signalShapes));
            layers.Add(new Layer("place", LayerType.Place, silkShapes));
            layers.Add(new Layer("stop", LayerType.Stop, stopShapes));
            layers.Add(new Layer("cream", LayerType.Cream, creamShapes));

            return component;
        }
    }
}
