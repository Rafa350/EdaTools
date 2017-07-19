namespace MikroPic.EdaTools.v1.Providers.OctoPart {

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using RestSharp;
    using RestSharp.Deserializers;
    using RestSharp.Serializers;
    using MikroPic.EdaTools.v1.JSon;
    using MikroPic.EdaTools.v1.JSon.Model;

    public sealed class OctopartServiceProvider: EdaServiceProviderBase {

        private const string apiKey = "0820b5bf";
        private const string urlBase = "http://octopart.com/api/v3";
        private const string endpoint = "parts/search";

        public override string GetName() {

            return "Octopart REST API v3";
        }

        public override void FindPart(Stream stream, string query) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (String.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");

            IRestRequest request = new RestRequest(endpoint, Method.GET)
                .AddParameter("apikey", apiKey)
                .AddParameter("q", query)
                .AddParameter("start", "0")
                .AddParameter("include", "specs");
                //.AddParameter("filter[fields][brand.name][]", "Vishay")
                //.AddParameter("filter[fields][offers.seller.name][]", "Mouser");

            IRestClient client = new RestClient(urlBase);
            IRestResponse response = client.Execute(request);

            JSonParser jsonParser = new JSonParser();
            JSonObject jsonObject = jsonParser.Parse(response.Content);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = false;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "    ";

            XmlWriter writer = XmlWriter.Create(stream, settings);
            try {
                JSonConverter.ConverToXml(writer, jsonObject);
            }
            finally {
                writer.Close();
            }
        }
    }
}
