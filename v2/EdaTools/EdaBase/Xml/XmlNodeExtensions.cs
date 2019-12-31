namespace MikroPic.EdaTools.v1.Base.Xml {

    using System;
    using System.Xml;

    public static class XmlNodeExtensions {

        public static bool AttributeExists(this XmlNode node, string name) {

            return node.Attributes[name] != null;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom del atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor obtingut.</returns>
        /// 
        public static string AttributeAsString(this XmlNode node, string name, string defValue = null) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? 
                defValue : 
                attribute.Value;
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte..</param>
        /// <returns>El seu valor com a double.</returns>
        /// 
        public static double AttributeDouble(this XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? 
                defValue : 
                XmlConvert.ToDouble(attribute.Value);
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor com integer.</returns>
        /// 
        public static int AttributeAsInteger(this XmlNode node, string name, int defValue = 0) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? 
                defValue : 
                XmlConvert.ToInt32(attribute.Value);
        }

        /// <summary>
        /// Obte el valor d'un atribut.
        /// </summary>
        /// <param name="node">El node.</param>
        /// <param name="name">El nom de l'atribut.</param>
        /// <param name="defValue">El valor per defecte.</param>
        /// <returns>El valor com boolean.</returns>
        /// 
        public static bool AttributeAsBoolean(this XmlNode node, string name, bool defValue = false) {

            XmlAttribute attribute = node.Attributes[name];
            return attribute == null ? 
                defValue : 
                String.Compare(attribute.Value, "yes", true) == 0 || String.Compare(attribute.Value, "true", true) == 0;
        }
    }
}
