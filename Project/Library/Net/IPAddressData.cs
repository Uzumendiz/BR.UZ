using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace PointBlank
{
     public class IPDataModel
    {
        public string status { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string timezone { get; set; }
        public string isp { get; set; }
        public string org { get; set; }
        public string @as { get; set; }
        public string query { get; set; }
    }
    public class IPAddressData
    {
        public static string GetLocationIPAPI(string ipaddress)
        {
            try
            {
                IPDataModel ipInfo = new IPDataModel();
                using (WebClient Web = new WebClient())
                {
                    string strResponse = Web.DownloadString("http://ip-api.com/json/" + ipaddress);
                    if (strResponse == null || strResponse == "")
                    {
                        return "NULL";
                    }
                    ipInfo = JsonConvert.DeserializeObject<IPDataModel>(strResponse);
                    if (ipInfo == null || ipInfo.status.ToLower().Trim() == "fail")
                    {
                        return "FAIL";
                    }
                    else
                    {
                        return $"City: {ipInfo.city} RegionName: {ipInfo.regionName} RegionCode: {ipInfo.region} Timezone: {ipInfo.timezone} Country: {ipInfo.country} CountryCode: {ipInfo.countryCode}";
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        
    }
}
