namespace MikroPic.EdaTools.v1.Base.Geometry {

    using System.Windows.Media;

    public struct Transformation {

        //private Matrix m;
        private readonly Point offset;
        private readonly Angle rotation;

        /// <summary>
        /// Contructor de l'objecte
        /// </summary>
        /// <param name="offset">Translacio.</param>
        /// <param name="rotation">Rotacio</param>
        /// 
        public Transformation(Point offset, Angle rotation) {

            /*m = new Matrix();
            m.Translate(offset.X, offset.Y);
            m.RotateAt(rotation.Degrees / 100, offset.X, offset.Y);*/

            this.offset = offset;
            this.rotation = rotation;
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// 
        public Transformation(Point offset) {

            /*m = new Matrix();
            m.Translate(offset.X, offset.Y);*/

            this.offset = offset;
            this.rotation = Angle.Zero;
        }

        /// <summary>
        /// Contructor del objecte.
        /// </summary>
        /// <param name="rotation">Rotacio.</param>
        /// 
        public Transformation(Angle rotation) {

            /*m = new Matrix();
            m.Rotate(rotation.Degrees / 100);*/

            this.offset = new Point();
            this.rotation = rotation;
        }

        public Point ApplyTo(Point point) {

            if (rotation.IsZero) 
                return point.Offset(offset.X, offset.Y);

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
                return new Point((int)x, (int)y);
            }
        }

        public void ApplyTo(Point[] points) {

            if (rotation.IsZero) {
                if ((offset.X != 0) || (offset.Y != 0))
                    for (int i = 0; i < points.Length; i++)
                        points[i] = points[i].Offset(offset.X, offset.Y);
            }
            /*else if (rotation.IsOrthogonal) {
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
                    points[i] = new Point((int)x, (int)y);
                }
            }
        }

/*        public Point Offset {
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
        }*/
    }
}
