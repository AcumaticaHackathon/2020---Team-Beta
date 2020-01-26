using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGToggl
{
    public class GPSTracker
    {
        public AcumaticaQueries queries = new AcumaticaQueries();

        private string ClockIntoGPSTask(JToken jToken)
        {
            return InvokeLink(jToken["TogglTimerLink"].ToString());
        }

        private string InvokeLink(string link)
        {
            var client = new RestClient(link) { Timeout = -1 };
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }

        public void TrackGPS(string employee, string lat, string lon)
        {
            var Tasks = queries.GetTogglTasksFromAcumatica(employee);

            //todo: Get this into some configurable place in acumatica.
            //      if we want a sample where whe walk around to the various gambling
            //      places we will need to throttle back the threshold.
            var distanceThresholdInMiles = 1.0;
            var TasksInRange = Tasks.Where(t => isInRange(distanceThresholdInMiles, t, new LatLng(Double.Parse(lat), Double.Parse(lon))) == true);
            if (!TasksInRange.Any()) ClockIntoInTransitTask();
        }

        private bool isInRange(double distanceThresholdInMiles, JToken t, LatLng latLng1)
        {
            if (String.IsNullOrEmpty(t["latitude"].ToString()) || String.IsNullOrEmpty(t["longitude"].ToString()) == null) return false;
            var latLng2 = new LatLng(Double.Parse(t["latitude"].ToString()), Double.Parse(t["longitude"].ToString()));
            var distance = HaversineDistance(latLng1, latLng2, DistanceUnit.Miles);
            return distanceThresholdInMiles >= distance;
        }

        private string ClockIntoInTransitTask()
        {
            //todo: this needs to be refined for now im just going to clock out.
            //      this will need to be refactored so that we do not have the bar url and API key hard coded.
            var link = "http://localhost/Toggl20_092_0043/Webhooks/Company/26ac3e17-6d41-4b0c-b6de-856883a05e9b?Method=ClockOut&TogglAPIKey=a4f41289cd78eb6a62627fbb9332af40";
            return InvokeLink(link);
        }

        public double HaversineDistance(LatLng pos1, LatLng pos2, DistanceUnit unit)
        {
            double R = (unit == DistanceUnit.Miles) ? 3960 : 6371;
            var lat = (pos2.Latitude - pos1.Latitude).ToRadians();
            var lng = (pos2.Longitude - pos1.Longitude).ToRadians();
            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                     Math.Cos(pos1.Latitude.ToRadians()) * Math.Cos(pos2.Latitude.ToRadians()) *
                     Math.Sin(lng / 2) * Math.Sin(lng / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
            return R * h2;
        }

        public enum DistanceUnit { Miles, Kilometers };

        public class LatLng
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public LatLng()
            {
            }

            public LatLng(double lat, double lng)
            {
                this.Latitude = lat;
                this.Longitude = lng;
            }
        }

    }

    /// <summary>
    /// Convert to Radians.
    /// </summary>
    /// <param name="val">The value to convert to radians</param>
    /// <returns>The value in radians</returns>
    public static class NumericExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }

    public class AcumaticaQueries
    {
        public AcumaticaQueries()
        {
            //todo: these need to get into a configuration somewhere likely a PXSetup Page.
            this.AcumaticaBaseUri = "http://localhost/Toggl20_092_0043"; //Settings.Default.AcumaticaBaseUri;
            this.UserName = "Admin"; //Settings.Default.UserName;
            this.Password = "123"; //Settings.Default.Password;
        }

        public AcumaticaQueries(string acumaticaBaseUri, string userName, string password)
        {
            this.AcumaticaBaseUri = acumaticaBaseUri;
            this.UserName = userName;
            this.Password = password;
        }

        private string Password { get; }

        private string UserName { get; }

        private string AcumaticaBaseUri { get; set; }

        public JArray GetTogglTasksFromAcumatica(string employeeName)
        {
            const string uriTemplate = @"{0}/OData/CR-Activity?$format=json&filter=Owner eq '{1}' ";
            var requestUri = string.Format(uriTemplate, AcumaticaBaseUri, employeeName);
            var client = new RestClient(requestUri) { Timeout = -1 };
            var request = new RestRequest(Method.GET);
            var authHeader = GetBasicAuthHeader();
            request.AddHeader("Authorization", authHeader);
            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            return (JArray)JObject.Parse(response.Content)["value"];
        }

        public string InvokeTogglLink(string link)
        {
            return InvokeBasicGetRequest(link);
        }

        public string InvokeBasicGetRequest(string link)
        {
            if (link == null) return null;
            var client = new RestClient(link);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }

        private string GetBasicAuthHeader()
        {
            //todo: circle back and parse this as it needs to be so its not hard coded.
            var user = UserName;
            var password = Password;
            return "Basic QWRtaW46MTIz";
        }


    }
}
