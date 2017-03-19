
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;

namespace UWPDemoPivotNavigation
{
        [XmlRoot(ElementName = "OpenWeatherMapConfig")]
        public class OpenWeatherMapConfig
        {
            [XmlAttribute(AttributeName = "ID")]
            public string ID { get; set; }
            [XmlAttribute(AttributeName = "ForecastbyCoordinates")]
            public string ForecastbyCoordinates { get; set; }
            [XmlAttribute(AttributeName = "ForecastbyId")]
            public string ForecastbyId { get; set; }
            [XmlAttribute(AttributeName = "ForecastbyName")]
            public string ForecastbyName { get; set; }
        }

        [XmlRoot(ElementName = "AppConfig")]
        public class AppConfig
        {
            [XmlAttribute(AttributeName = "TempMax")]
            public double TempMax { get; set; }
            [XmlAttribute(AttributeName = "TempMin")]
            public double TempMin { get; set; }
            [XmlAttribute(AttributeName = "DefaultCityID")]
            public int DefaultCityID { get; set; }
    }

        [XmlRoot(ElementName = "Config")]
        public class Config
        {
            [XmlElement(ElementName = "OpenWeatherMapConfig")]
            public OpenWeatherMapConfig OpenWeatherMapConfig { get; set; }
            [XmlElement(ElementName = "AppConfig")]
            public AppConfig AppConfig { get; set; }
        }
    class ConfigSerializer
    {
        public static async Task<Config> GetConfig(StorageFile file)
        {
            Config configFromXml = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            Stream stream = await file.OpenStreamForReadAsync().ConfigureAwait(false);

            configFromXml = (Config)serializer.Deserialize(stream);

            stream.Dispose();
            return configFromXml;
        }
        public static async void SetConfig(Config config,StorageFile file)
        {


            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            Stream stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false);
            serializer.Serialize(stream, config);
            stream.Dispose();
        }
    }


}
