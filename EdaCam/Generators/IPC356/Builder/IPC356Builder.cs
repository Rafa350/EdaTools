﻿using System;
using System.IO;
using MikroPic.EdaTools.v1.Base.Geometry;


namespace MikroPic.EdaTools.v1.Cam.Generators.IPC356.Builder {

    public enum TestAccess {
        None,
        Top,
        Bottom,
        Both
    }

    public enum Units {
        Inches,
        Millimeters
    }

    /// <summary>
    /// Generador de codi IPCD356
    /// </summary>
    public sealed class IPC356Builder {

        private readonly TextWriter _writer;
        private int _offsetX;
        private int _offsetY;
        private EdaAngle _rotation;

        /// <summary>
        /// Constructor del objecte
        /// </summary>
        /// <param name="writer">Escriptor de sortida.</param>
        /// 
        public IPC356Builder(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            this._writer = writer;
        }

        /// <summary>
        /// Escriu un comentari
        /// </summary>
        /// <param name="text">El text del comentari.</param>
        /// 
        public void Comment(string text) {

            _writer.Write(
                "C  {0}",
                text);
            _writer.WriteLine();
        }

        /// <summary>
        /// Declara la versio IPCD.
        /// </summary>
        /// 
        public void SetVersion() {

            _writer.WriteLine("P  VER   IPC-D-356");
        }

        /// <summary>
        /// Declara el tipus d'imatge.
        /// </summary>
        /// 
        public void SetImage() {

            _writer.WriteLine("P  IMAGE PRIMARY");
        }

        /// <summary>
        /// Asigna una transformacio de coordinades.
        /// </summary>
        /// <param name="offset">Desplaçament</param>
        /// <param name="rotation">Rotacio respecte el punt especificat com a desplaxament.</param>
        /// 
        public void SetTransformation(EdaPoint offset, EdaAngle rotation) {

            SetTransformation(offset.X, offset.Y, rotation);
        }

        /// <summary>
        /// Asigna una transformacio de coordinades.
        /// </summary>
        /// <param name="offsetX">Desplaçament X</param>
        /// <param name="offsetY">Desplaçament y</param>
        /// <param name="rotation">Rotacio respecte el punt especificat com a desplaxament.</param>
        /// 
        public void SetTransformation(int offsetX, int offsetY, EdaAngle rotation) {

            _offsetX = offsetX;
            _offsetY = offsetY;
            _rotation = rotation;
        }

        /// <summary>
        /// Desactiva la transformacio de coordinades.
        /// </summary>
        /// 
        public void ResetTransformation() {

            _offsetX = 0;
            _offsetY = 0;
            _rotation = EdaAngle.Zero;
        }

        /// <summary>
        /// Declara les unitats a utilitzar.
        /// </summary>
        /// <param name="units">Les unitatas a seleccionar.</param>
        /// 
        public void SetUnits(Units units) {

            switch (units) {
                case Units.Inches:
                    _writer.WriteLine("P  UNITS CUST 0");
                    break;

                case Units.Millimeters:
                    _writer.WriteLine("P  UNITS CUST 1");
                    break;
            }
        }

        /// <summary>
        /// Declara un alias de senyal
        /// </summary>
        /// <param name="alias">El alias</param>
        /// <param name="name">El nom del senyal.</param>
        /// 
        public void NetAlias(string alias, string name) {

            _writer.Write(
                "P  NNAME{0,-14}{1}",
                alias,
                name);
            _writer.WriteLine();
        }

        /// <summary>
        /// Declara un conductor.
        /// </summary>
        /// <param name="points">Llista les posicions dels nodes del conductor.</param>
        /// <param name="layerNum">Numero de capa.</param>
        /// <param name="thickness">Amplada del conductor.</param>
        /// <param name="netName">Nom de la xarxa del conductor.</param>
        /// 
        public void Conductor(EdaPoint[] points, int layerNum, int thickness, string netName) {

            if (points == null)
                throw new ArgumentNullException(nameof(points));

            if (points.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(points));

            if (layerNum <= 0)
                throw new ArgumentOutOfRangeException(nameof(layerNum));

            if (thickness <= 0)
                throw new ArgumentOutOfRangeException(nameof(thickness));

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException(nameof(netName));

            _writer.Write(
                "378{0,-14} L{1} X{2}Y0000 X{3}Y{4} X{5}Y{6}{7}",
                netName,
                FormatLayerNum(layerNum),
                FormatDiameter(thickness),
                FormatCoordinate(points[0].X + _offsetX),
                FormatCoordinate(points[0].Y + _offsetY),
                FormatCoordinate(points[1].X + _offsetX),
                FormatCoordinate(points[1].Y + _offsetY),
                points.Length > 2 ? ' ' : '*');

            if (points.Length > 2) {
                _writer.WriteLine();
                _writer.Write("078 ");
                for (int i = 2; i < points.Length; i++) {
                    _writer.Write(
                        " X{0}Y{1}",
                        FormatCoordinate(points[i].X + _offsetX),
                        FormatCoordinate(points[i].Y + _offsetY));
                }
                _writer.Write("*");
            }

            _writer.WriteLine();
        }

        /// <summary>
        /// Declara un pad throug hole
        /// </summary>
        /// <param name="position">Posicio del pad.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="partId">Identificador del component.</param>
        /// <param name="padId">Identificador del pad.</param>
        /// <param name="netName">Nom de la xarxa a la que pertany el pad.</param>
        /// 
        public void ThPad(EdaPoint position, int drill, string partId, string padId, string netName) {

            if (drill <= 0)
                throw new ArgumentOutOfRangeException(nameof(drill));

            if (String.IsNullOrEmpty(partId))
                throw new ArgumentNullException(nameof(partId));

            if (String.IsNullOrEmpty(padId))
                throw new ArgumentNullException(nameof(padId));

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException(nameof(netName));

            _writer.Write(
                "317{0,-14}   {3,-6}-{4,-4} D{5}PA00X{1}Y{2}",
                netName,
                FormatCoordinate(position.X + _offsetX),
                FormatCoordinate(position.Y + _offsetY),
                partId,
                padId,
                FormatDiameter(drill));
            _writer.WriteLine();
        }

        /// <summary>
        /// Escriu una definicio de pad smd
        /// </summary>
        /// <param name="position">Posicio del pad.</param>
        /// <param name="access">Capa a la que pertany.</param>
        /// <param name="partId">Identificador del component.</param>
        /// <param name="padId">Identificador del pad.</param>
        /// <param name="netName">Nom de la xarxa a la que pertany el pad.</param>
        /// 
        public void SmdPad(EdaPoint position, TestAccess access, string partId, string padId, string netName) {

            if (String.IsNullOrEmpty(partId))
                throw new ArgumentNullException(nameof(partId));

            if (String.IsNullOrEmpty(padId))
                throw new ArgumentNullException(nameof(padId));

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException(nameof(netName));

            _writer.Write(
                "327{0,-14}   {3,-6}-{4,-4}       A{5}X{1}Y{2}",
                netName,
                FormatCoordinate(position.X + _offsetX),
                FormatCoordinate(position.Y + _offsetY),
                partId,
                padId,
                FormatAccess(access));
            _writer.WriteLine();
        }

        /// <summary>
        /// Escriu una definicio de via
        /// </summary>
        /// <param name="position">Posicio de la via.</param>
        /// <param name="drill">Diametre del forat.</param>
        /// <param name="netName">Nom de la arxa a la que pertany la via.</param>
        /// 
        public void Via(EdaPoint position, int drill, string netName) {

            if (drill <= 0)
                throw new ArgumentOutOfRangeException(nameof(drill));

            if (String.IsNullOrEmpty(netName))
                throw new ArgumentNullException(nameof(netName));

            _writer.Write(
                "317{0,-14}   VIA   -     D{3}PA00X{1}Y{2}",
                netName,
                FormatCoordinate(position.X + _offsetX),
                FormatCoordinate(position.Y + _offsetY),
                FormatDiameter(drill));
            _writer.WriteLine();
        }

        /// <summary>
        /// Declara el final de fitxer
        /// </summary>
        /// 
        public void EndFile() {

            _writer.WriteLine("999");
        }

        /// <summary>
        /// Formateja una mesura del diametre
        /// </summary>
        /// <param name="number">Valor a formatejar</param>
        /// <returns>La cadena formatejada</returns>
        /// 
        private static string FormatDiameter(int number) {

            return String.Format("{0:0000}", number / 1000);
        }

        /// <summary>
        /// Formateja una mesura de posicio
        /// </summary>
        /// <param name="number">El valor a formatejat</param>
        /// <returns>La cadena formatejada</returns>
        /// 
        private static string FormatCoordinate(int number) {

            bool sign = number >= 0;
            int value = Math.Abs(number / 1000);

            return String.Format("{0}{1:000000}", sign ? '+' : '-', value);
        }

        /// <summary>
        /// Formateja el numero de capa.
        /// </summary>
        /// <param name="layerNum">El numero de capa.</param>
        /// <returns>La cadena formatejada.</returns>
        /// 
        private string FormatLayerNum(int layerNum) {

            return String.Format("{0:00}", layerNum);
        }

        private string FormatAccess(TestAccess access) {

            switch (access) {
                default:
                case TestAccess.Both:
                    return "00";

                case TestAccess.Top:
                    return "01";

                case TestAccess.Bottom:
                    return "02";
            }
        }
    }
}
