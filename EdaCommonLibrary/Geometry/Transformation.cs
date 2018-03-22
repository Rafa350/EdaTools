namespace MikroPic.EdaTools.v1.Geometry {

    using System.Windows.Media;

    public struct Transformation {

        private readonly PointInt offset;
        private readonly Angle rotation;

        /// <summary>
        /// Contructor de l'objecte
        /// </summary>
        /// <param name="offset">Translacio.</param>
        /// <param name="rotation">Rotacio</param>
        /// 
        public Transformation(PointInt offset, Angle rotation) {

            this.offset = offset;
            this.rotation = rotation;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// 
        public Transformation(PointInt offset) {

            this.offset = offset;
            this.rotation = Angle.Zero;
        }

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="rotation">Rotacio.</param>
        /// 
        public Transformation(Angle rotation) {

            this.offset = new PointInt();
            this.rotation = rotation;
        }

        public PointInt ApplyTo(PointInt point) {

            if (rotation.IsZero) 
                return new PointInt(point.X + offset.X, point.Y + offset.Y);

            else {
                Matrix m = new Matrix();
                m.Translate(offset.X, offset.Y);
                m.RotateAt(rotation.Degrees / 100.0, offset.X, offset.Y);

                double x = point.X;
                double y = point.Y;
                double xadd = y * m.M21 + m.OffsetX;
                double yadd = x * m.M12 + m.OffsetY;
                x *= m.M11;
                x += xadd;
                y *= m.M22;
                y += yadd;
                return new PointInt((int)x, (int)y);
            }
        }

        public void ApplyTo(PointInt[] points) {

            if (rotation.IsZero) {
                if ((offset.X != 0) || (offset.Y != 0))
                    for (int i = 0; i < points.Length; i++)
                        points[i] = new PointInt(
                            points[i].X + offset.X, 
                            points[i].Y + offset.Y);
            }
            /*else if (rotation.IsOrthogonal) {
                int sin = 0;
                int cos = 0;
            }*/
            else { 
                Matrix m = new Matrix();
                m.Translate(offset.X, offset.Y);
                m.RotateAt(rotation.Degrees / 100.0, offset.X, offset.Y);

                for (int i = 0; i < points.Length; i++) {
                    double x = points[i].X;
                    double y = points[i].Y;
                    double xadd = y * m.M21 + m.OffsetX;
                    double yadd = x * m.M12 + m.OffsetY;
                    x *= m.M11;
                    x += xadd;
                    y *= m.M22;
                    y += yadd;
                    points[i] = new PointInt((int)x, (int)y);
                }
            }
        }

        public PointInt Offset {
            get {
                return offset;
            }
        }

        public Angle Rotation {
            get {
                return rotation;
            }
        }

        public bool IsNull {
            get {
                return (offset.X == 0) && (offset.Y == 0) && (rotation.IsZero);
            }
        }
    }
}
