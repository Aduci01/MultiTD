using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class LatencyHandler {
    public static IpApiData ipData;

    [System.Serializable]
    public class IpApiData {
        public string country_name;
        public string continent_code;

        public static IpApiData CreateFromJSON(string jsonString) {
            return JsonUtility.FromJson<IpApiData>(jsonString);
        }
    }

    public static IEnumerator SetCountry() {
        string ip = new System.Net.WebClient().DownloadString("https://api.ipify.org");
        string uri = $"https://ipapi.co/{ip}/json/";


        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            ipData = IpApiData.CreateFromJSON(webRequest.downloadHandler.text);
        }
    }

    public static object[] GetLatencies() {
        object[] result = new object[]
                            {
                                new {
                                    region = "EastUs",
                                    latency = ipData.continent_code == "NA" ? 100 : 999
                                },
                                new {
                                    region = "NorthEurope",
                                    latency = ipData.continent_code == "EU" ? 100 : 999
                                }
               };

        return result;
    }
}
