﻿namespace MikroPic.EdaTools.v1.Pcb.Geometry.Polygons {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class PolygonBuilder {

        /// <summary>
        /// Crea un poligon en forma de linia ample amb finals arrodonits
        /// </summary>
        /// <param name="start">Posicio inicial.</param>
        /// <param name="end">Posicio final.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildLine(Point start, Point end, double thickness) {

            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double length = Math.Sqrt((dx * dx) + (dy * dy));
            Point[] points = PointsFromLine(length, thickness);

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(start.X, start.Y);
            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            m.RotateAt(angle, start.X, start.Y);
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon en forma de cercle.
        /// </summary>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">Radi.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildCircle(Point position, double radius) {

            Point[] points = PointsFromCircle(position, radius);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon en forma de rectangle.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildRectangle(Point position, Size size, double radius, double rotation) {

            Point[] points;
            if (radius == 0)
                points = PointsFromRectangle(position, size, rotation);
            else
                points = PointsFromRoundRectangle(position, size, radius, rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon regular.
        /// </summary>
        /// <param name="sides">Nombre de cares.</param>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="radius">Radi del cercle maxim.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon BuildRegularPolygon(int sides, Point position, double radius, double rotation) {

            Point[] points = PointsFromRegularPolygon(sides, position, radius, rotation);
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="line">L'element</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(LineElement line, Part part, double clearance) {

            double length =
                Math.Sqrt(
                    Math.Pow(line.EndPosition.X - line.StartPosition.X, 2) +
                    Math.Pow(line.EndPosition.Y - line.StartPosition.Y, 2));
            Point[] points = PointsFromLine(length, line.Thickness + (clearance * 2));

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(line.StartPosition.X, line.StartPosition.Y);
            double angle = Math.Atan2(line.EndPosition.Y - line.StartPosition.Y , line.EndPosition.X - line.StartPosition.X) * 180.0 / Math.PI;
            m.RotateAt(angle, line.StartPosition.X, line.StartPosition.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="hole">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(HoleElement hole, Part part, Double clearance) {

            // Crea els punts per un cercle centrat l'origen.
            //
            Point[] points = PointsFromCircle(hole.Position, (hole.Drill / 2) + clearance);

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
                m.Transform(points);
            }

            // Crea el poligon amb la llista de punts.
            //
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="circle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(CircleElement circle, Part part, double clearance) {

            // Crea els punts per un cercle centrat l'origen.
            //
            Point[] points = PointsFromCircle(circle.Position, circle.Radius + clearance);

            // Realitza la transformacio dels punts, a la posicio i orientacio finals.
            //
            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
                m.Transform(points);
            }

            // Crea el poligon amb la llista de punts.
            //
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="rectangle">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Espai de separacio.</param>
        /// <returns>El poligon creat.</returns>
        /// 
        public static Polygon Build(RectangleElement rectangle, Part part, double clearance) {

            // Crea els punts d'un rectangle
            //
            Point[] points = PointsFromRectangle(rectangle.Position, rectangle.Size, rectangle.Rotation);

            // Realitza la transformacio dels punts, a la posicio i orientacio finals.
            //
            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
                m.Transform(points);
            }

            // Crea el poligon amb la llista de punts.
            //
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="via">El element.</param>
        /// <param name="part">El conmponent al que pertany.</param>
        /// <param name="clearance">Increment de tamany.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon Build(ViaElement via, Part part, double clearance) {

            Point[] points;

            double size = via.OuterSize;
            double radius = size / 2;

            switch (via.Shape) {
                default:
                case ViaElement.ViaShape.Circular:
                    points = PointsFromCircle(via.Position, radius + clearance);
                    break;

                case ViaElement.ViaShape.Square:
                    points = PointsFromRectangle(via.Position, new Size(size + (clearance * 2), size + (clearance * 2)), 0);
                    break;

                case ViaElement.ViaShape.Octogonal:
                    points = PointsFromRegularPolygon(8, via.Position, radius + clearance, 0);
                    break;
            }

            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
                m.Transform(points);
            }

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="pad">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">Increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon Build(ThPadElement pad, Part part, double clearance) {

            Point[] points;

            double size = pad.Size;
            double radius = size / 2;

            switch (pad.Shape) {
                default:
                case ThPadElement.ThPadShape.Circular:
                    points = PointsFromCircle(pad.Position, radius + clearance);
                    break;

                case ThPadElement.ThPadShape.Square: {
                        Rect rect = new Rect(0, 0, pad.Size, pad.Size);
                        if (clearance == 0)
                            points = PointsFromRectangle(new Point(0, 0), rect.Size, 0);
                        else {
                            rect.Inflate(clearance, clearance);
                            points = PointsFromRoundRectangle(new Point(0, 0), rect.Size, clearance, 0);
                        }
                    }
                    break;

                case ThPadElement.ThPadShape.Octogonal:
                    points = PointsFromRegularPolygon(8, pad.Position, radius + clearance, 0);
                    break;
            }

            // Realitza la transformacio dels punts, a la posicio i 
            // orientacio finals.
            //
            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Rotation, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            // Crea el poligon amb la llista de punts.
            //
            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="pad">El element.</param>
        /// <param name="part">El component al que pertany.</param>
        /// <param name="clearance">increment de tamany.</param>
        /// <returns>El poligon.</returns>
        /// 
        public static Polygon Build(SmdPadElement pad, Part part, double clearance) {

            Point[] points;

            double radius = Math.Min(pad.Size.Width, pad.Size.Height) * pad.Roundnes / 2;
            Rect rect = new Rect(0, 0, pad.Size.Width, pad.Size.Height);

            if ((radius == 0) && (clearance == 0))
                points = PointsFromRectangle(new Point(0, 0), rect.Size, 0);
            else {
                rect.Inflate(clearance, clearance);
                points = PointsFromRoundRectangle(new Point(0, 0), rect.Size, clearance + radius, 0);
            }

            Matrix m = new Matrix();
            m.Translate(pad.Position.X, pad.Position.Y);
            m.RotateAt(pad.Rotation, pad.Position.X, pad.Position.Y);
            if (part != null) {
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotation, part.Position.X, part.Position.Y);
            }
            m.Transform(points);

            return new Polygon(points);
        }

        /// <summary>
        /// Crea un poligon a partir d'un element.
        /// </summary>
        /// <param name="region">El element</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public static Polygon Build(RegionElement region) {

            Polygon polygon = new Polygon();

            foreach (RegionElement.Segment segment in region.Segments)
                polygon.Add(segment.Position);

            return polygon;
        }

        /// <summary>
        /// Genera una sequencia de puns per un poligon d'una traç amb origen 0,0,
        /// i extrems de linia arrodonits. 
        /// </summary>
        /// <param name="length">Longitut de la linia.</param>
        /// <param name="thickness">Amplada de linia.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromLine(double length, double thickness) {

            double radius = thickness / 2;

            Point[] q0Points = PointsFromQuadrant(new Point(length, 0), radius, 0);
            Point[] q1Points = PointsFromQuadrant(new Point(0, 0), radius, 1);
            Point[] q2Points = PointsFromQuadrant(new Point(0, 0), radius, 2);
            Point[] q3Points = PointsFromQuadrant(new Point(length, 0), radius, 3);

            Point[] points = new Point[4 * 9];
            q0Points.CopyTo(points, 0);
            q1Points.CopyTo(points, 9);
            q2Points.CopyTo(points, 18);
            q3Points.CopyTo(points, 27);

            return points;
        }

        /// <summary>
        /// Genera una sequencia de punts per un poligon en forma de rectangle, centrat
        /// en l'origen de coordinades.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromRectangle(Point position, Size size, double rotation) {

            // Crea el rectangle unitari
            //
            Point[] points = new Point[4];
            points[0].X = -0.5;
            points[0].Y = 0.5;
            points[1].X = 0.5;
            points[1].Y = 0.5;
            points[2].X = 0.5;
            points[2].Y = -0.5;
            points[3].X = -0.5;
            points[3].Y = -0.5;

            // El transforma al rectangle a la posicio, tamany i orientacio final.
            //
            Matrix m = new Matrix();
            m.Scale(size.Width, size.Height);
            m.Rotate(rotation);
            m.Translate(position.X, position.Y);
            m.Transform(points);

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un rectangle arrodonit.
        /// </summary>
        /// <param name="position">Posicio del centroid.</param>
        /// <param name="size">Tamany del rectangle.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La kkista de punts.</returns>
        /// 
        private static Point[] PointsFromRoundRectangle(Point position, Size size, double radius, double rotation) {

            double cx = (size.Width / 2) - radius;
            double cy = (size.Height / 2) - radius;

            Point[] q0Points = PointsFromQuadrant(new Point(cx, cy), radius, 0);
            Point[] q1Points = PointsFromQuadrant(new Point(-cx, cy), radius, 1);
            Point[] q2Points = PointsFromQuadrant(new Point(-cx, -cy), radius, 2);
            Point[] q3Points = PointsFromQuadrant(new Point(cx, -cy), radius, 3);

            Point[] points = new Point[4 * 9];
            q0Points.CopyTo(points, 0);
            q1Points.CopyTo(points, 9);
            q2Points.CopyTo(points, 18);
            q3Points.CopyTo(points, 27);

            // Transforma a la posicio i orientacio final
            //
            Matrix m = new Matrix();
            m.Rotate(rotation);
            m.Translate(position.X, position.Y);
            m.Transform(points);

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon en forma de cercle, format
        /// per 64 segments de recta i centrat en l'origen de coordinades.
        /// </summary>
        /// <param name="radius">Radi del cercle.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromCircle(Point position, double radius) {

            return PointsFromRegularPolygon(32, position, radius, 0);
        }

        /// <summary>
        /// Crea una sequencia de punts per un poligon regular, on la base es una aresta.
        /// </summary>
        /// <param name="sides">Numero de arestes.</param>
        /// <param name="position">Posicio del centre.</param>
        /// <param name="radius">El radi del cercle exterior.</param>
        /// <param name="rotation">Angle de rotacio.</param>
        /// <returns>La sequencia de punts.</returns>
        /// 
        private static Point[] PointsFromRegularPolygon(int sides, Point position, double radius, double rotation) {

            Point[] points = new Point[sides];

            double x = position.X;
            double y = position.Y;
            double delta = (360.0 * Math.PI / 180.0) / sides;
            double angle = (rotation * Math.PI / 180.0) + (delta / 2);

            for (int i = 0; i < sides; i++) {
                points[i] = new Point(x + (radius * Math.Cos(angle)), y + (radius * Math.Sin(angle)));
                angle += delta;
            }

            return points;
        }

        /// <summary>
        /// Crea una sequencia de punts per un quadrant. 
        /// </summary>
        /// <param name="center">Centre del quandrant.</param>
        /// <param name="radius">Radi de curvatura.</param>
        /// <param name="quadrant">Numero de quadrant (0..3)</param>
        /// <returns>La llista de punts.</returns>
        /// 
        private static Point[] PointsFromQuadrant(Point center, double radius, int quadrant) {

            Point[] points = new Point[9];

            double delta = (360.0 * Math.PI / 180.0) / 32;
            double angle = quadrant * 90 * Math.PI / 180;
            for (int i = 0; i < 9; i++) {
                points[i] = new Point(center.X + (radius * Math.Cos(angle)), center.Y + (radius * Math.Sin(angle)));
                angle += delta;
            }

            return points;
        }
    }
}
