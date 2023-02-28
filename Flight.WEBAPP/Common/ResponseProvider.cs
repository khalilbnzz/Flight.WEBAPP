using Flight.WEBAPP.Common.Interfaces;
using Flight.WEBAPP.Common.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Flight.WEBAPP.Common
{
    public class ResponseProvider : IResponseProvider
    {
        private readonly ConfigurationModel _ConfigurationModel;
        private readonly string _urlFlight;
        private readonly string _valueFlight;
        

        public ResponseProvider()
        {
            _ConfigurationModel = new ConfigurationModel();
            _urlFlight = _ConfigurationModel.AppConfiguration.flightAPIKEY.link;
            _valueFlight = _ConfigurationModel.AppConfiguration.flightAPIKEY.value;
        }
        public HttpResponseMessage GetAPIResponse(out string pResponse)
        {
            string _completeLink = string.Concat(_urlFlight, _valueFlight);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(_completeLink).Result;
            pResponse = response.Content.ReadAsStringAsync().Result;
            return response;
        }
    }
}
