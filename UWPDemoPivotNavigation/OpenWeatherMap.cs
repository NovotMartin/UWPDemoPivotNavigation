using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web;
using System.Net.Http;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Toolkit.Uwp;
using System.Globalization;

namespace UWPDemoPivotNavigation
{

    static class OpenWeatherMap
    {
        #pragma warning disable  
        private static string  id = "00c672d6c339146e13a3157368489999";
        private static string defaultId = "00c672d6c339146e13a3157368489999";
        private static string uri = @"api.openweathermap.org/data/2.5/forecast?id=";
        static string f = "524901&appid=00c672d6c339146e13a3157368489999&mode=xml";
        #pragma warning restore   

        // static HttpClient httpClient;
        public static string ID {
            get {
                return id;
            }
            set {
                id = value;
            } }
       
        public delegate void DelegateDownloadCompete(bool status);
        public static event DelegateDownloadCompete OnDownloadComplete;
      
        public static void SetDefaulId()
        {
            id = defaultId;
        }

        public static void DownloadForecastbyCoordinates(double lat,double lon)
        {
            /*
             * Nutne aby se do adresy vkladala cisla se separatorem '.'
             */
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            Uri url = new Uri(string.Format("http://api.openweathermap.org/data/2.5/forecast?lat={0:0.0000}&lon={1:0.0000}&appid={2}&mode=xml&units=metric", lat.ToString(nfi),lon.ToString(nfi), id).Trim());
            Task.Run(() => Download(url));
        }
        public static void DownloadForecastbyId(int cityid)
        {
            Uri url = new Uri(string.Format("http://api.openweathermap.org//data//2.5//forecast?id={0}&appid={1}&mode=xml&units=metric", cityid, id));
            Task.Run(() => Download(url));
            
        }
        public static void DownloadForecastbyName(string name)
        {
            Uri url = new Uri(string.Format("http://api.openweathermap.org//data//2.5//forecast?q={0}&appid={1}&mode=xml&units=metric", name, id));
            Task.Run(() => Download(url));
        }

        private static async Task Download(Uri url)
        {
            string response = null;

            


            bool success = true;
            using (var httpClient = new HttpClient())
            {
                try
                {
                    response = await httpClient.GetStringAsync(url);       // stažení souboru z netu     
                    

                }
                catch
                {
                    success = false;
                }
                if (success)
                {
                    // Saving
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await localFolder.CreateFileAsync("forecast.xml", CreationCollisionOption.ReplaceExisting);

                    
                    using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (DataWriter textWriter = new DataWriter(textStream))
                        {
                            textWriter.WriteString(response);
                            await textWriter.StoreAsync();
                        }
                    }
                }
                
            }
            OnDownloadComplete(success);

        }


        /// <summary>
        /// Deserialize XML from OpenWeatherMap
        /// </summary>
        /// <param name="file">File to deserialize</param>
        /// <returns>The Forecas object </returns>
        public static async Task<Weatherdata> GetForecast(StorageFile file)
        {
            Weatherdata forecastFromXML = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Weatherdata));

            Stream stream = await file.OpenStreamForReadAsync().ConfigureAwait(false);

            forecastFromXML = (Weatherdata)serializer.Deserialize(stream);

            stream.Dispose();
            return forecastFromXML;
        }

        public async static Task<bool> IsOfflineXMLAvailable(string FileName)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            
            // Existuje nejaka predpoved?
            var ForecastAvailable = await localFolder.FileExistsAsync(FileName);
            return ForecastAvailable;
        }
    }





}
