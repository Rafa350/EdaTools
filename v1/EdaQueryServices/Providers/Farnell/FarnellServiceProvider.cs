namespace MikroPic.EdaTools.v1.Providers.Farnell {

    using System;
    using System.IO;
    using RestSharp;
    using MikroPic.EdaTools.v1.XmlUtils;
 
    public sealed class FarnellServiceProvider: EdaServiceProviderBase {

        private const string transformationResourceName = "MikroPic.EdaTools.v1.Providers.Farnell.farnell.xsl";

        private const string apiKey = "w6ezrquvwd9mxket3z2f8a8y";
        private const string urlBase = "https://api.element14.com";
        private const string endPoint = "catalog/products";

        public override string GetName() {

            return "Element14 Product Search API (REST)";
        }

        public override void FindPart(Stream stream, string query) {

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (String.IsNullOrEmpty(query))
                throw new ArgumentNullException("query");

            IRestRequest request = new RestRequest(endPoint, Method.GET)
                .AddParameter("term", "any:" + query)
                .AddParameter("storeInfo.id", "es.farnell.com")
                .AddParameter("resultsSettings.offset", "0")
                .AddParameter("resultsSettings.numberOfResults", "10")
                .AddParameter("resultsSettings.refinements.filters", "inStock")
                .AddParameter("resultsSettings.responseGroup", "large")
                .AddParameter("callInfo.omitXmlSchema", "true")
                .AddParameter("callInfo.responseDataFormat", "xml")
                .AddParameter("callInfo.callback", "")
                .AddParameter("callInfo.apikey", apiKey);

            IRestClient client = new RestClient(urlBase);
            IRestResponse response = client.Execute(request);

            StringReader input = new StringReader(response.Content);
            StreamWriter output = new StreamWriter(stream);

            XmlProcessor xmlProcessor = new XmlProcessor();
            xmlProcessor.Transform(
                input, 
                XmlTransformationLoader.FromResource(transformationResourceName), 
                null, 
                output);
        }
    }
}
