namespace MikroPic.EdaTools.v1.Panel.Model.Items {

    using System;

    public static class CutItemExtensions {

        /// <summary>
        /// Calcula els punts de referencia.
        /// </summary>
        /// <param name="item">El item.</param>
        /// <returns>La llista de punts de referencia.</returns>
        /// 
        public static int[] GetReferencePoints(this CutItem item) {

            double dx = item.EndPosition.X - item.StartPosition.X;
            double dy = item.EndPosition.Y - item.StartPosition.Y;
            int length = (int)Math.Sqrt((dx * dx) + (dy * dy));

            if (item.Cuts <= 1)
                return new int[] {
                    0,
                    length };

            else if (item.Cuts == 2)
                return new int[] {
                    0,
                    length / 2,
                    length };

            else if (item.Cuts == 3) {
                return new int[] {
                    0,
                    item.Margin,
                    length - item.Margin,
                    length
                };
            }
            else {

                int numPoints = item.Cuts + 1;
                int cutLength = (length - (item.Margin * 2)) / (item.Cuts - 2);

                int[] points = new int[numPoints];

                int i = 0;
                points[i++] = 0;
                points[i++] = item.Margin;
                while (i < numPoints - 2) {
                    points[i] = item.Margin + (cutLength * (i - 1));
                    i++;
                }
                points[i++] = length - item.Margin;
                points[i] = length;

                return points;
            }
        }

        /// <summary>
        /// Obte els punts de referencia dels tall, a partir dels punts de referencia basics.
        /// </summary>
        /// <param name="item">El item.</param>
        /// <param name="refPoints">Punts de referencia.</param>
        /// <returns>Punts del talls.</returns>
        /// 
        public static int[] GetCutReferencePoints(this CutItem item, int[] refPoints) {

            int s = item.CutSpacing / 2;
            int[] points = new int[(refPoints.Length - 1) * 2];

            int r = 0;
            int p = 0;
            points[p++] = refPoints[r++];
            points[p++] = refPoints[r] - s;
            while (r < refPoints.Length - 2) {
                points[p++] = refPoints[r] + s;
                r++;
                points[p++] = refPoints[r] - s;
            }
            points[p++] = refPoints[r++] + s;
            points[p] = refPoints[r];

            return points;
        }
    
        /// <summary>
        /// Obte els punts de referencia dels forats, a partir dels punts de referencia basics.
        /// </summary>
        /// <param name="cut">El item.</param>
        /// <param name="refPoints">Punts de referencia.</param>
        /// <returns>Punts dels forats.</returns>
        /// 
        public static int[] GetHoleReferencePoints(this CutItem cut, int[] refPoints) {

            int[] points = new int[(refPoints.Length - 2) * 5];

            int r = 1;
            int p = 0;
            int s = cut.HoleSpacing;
            while (r < refPoints.Length - 1) {
                points[p++] = refPoints[r] - s - s;
                points[p++] = refPoints[r] - s;
                points[p++] = refPoints[r];
                points[p++] = refPoints[r] + s;
                points[p++] = refPoints[r] + s + s;
                r++;
            }

            return points;
        }
    }
}
