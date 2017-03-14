using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Microsoft.Toolkit.Uwp;
using System.Text;




// Dokumentaci k šabloně položky Prázdná stránka najdete na adrese https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x405

namespace UWPDemoPivotNavigation
{
    /// <summary>
    /// Prázdná stránka, která se dá použít samostatně nebo v rámci objektu Frame
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Controls content
        public Double TempMin { get { return Math.Round(this.rangeSelector.RangeMin, 2); } private set { this.rangeSelector.RangeMin = value; } }
        public Double TempMax { get { return Math.Round(this.rangeSelector.RangeMax, 2); } private set { this.rangeSelector.RangeMax = value; } }
        public int DefaultCityID { get { return Convert.ToInt32(this.TextBoxDefaultPoloha.Text); } private set { this.TextBoxDefaultPoloha.Text = value.ToString(); } }

        // Errors properties
        public bool XMLForecastAvailable { get; private set; }        
        public bool NetworkAvailable { get { return ConnectionHelper.IsInternetAvailable; } }
        public bool LocatedByCoords { get; private set; }
        public bool OldXMLForecastUsed { get; private set; }
        public bool IsSomethingToShow { get; private set; }
        public bool XMLDownloaded { get; private set; }
        public DateTime LastSuccessForecastDownload { get; private set; }


        PositionEventArg ee;
        // Forecast object
        public Weatherdata Forecast { get; private set; }
        
        

        public MainPage()
        {
            this.InitializeComponent();
                        

            OpenWeatherMap.OnDownloadComplete += OpenWeatherMap_onDownloadComplete; // nastaveni eventu co se provede po stazeni
            GeoLocation.OnLocate += GeoLocation_OnLocate;

            Init();

            
            GetLocationWeather();

        }

        private void Init()
        {
            if (false)
            {
                // Kdyz existuje soubor s ulozenym nastavenim


            }
            else
            {
                this.TempMin = -30;
                this.TempMax = 20;
                this.DefaultCityID = 3063548; //( id = Usti nad Labem)
            }
        }

        private void GetLocationWeather()
        {
            GeoLocation.GetLocation();
        }
        private async void  OpenWeatherMap_onDownloadComplete(bool status)
        {
            if (status)
                this.LastSuccessForecastDownload = DateTime.Now;
            XMLDownloaded = status;

            /*
             * !!!!  Běží na vedlejším vlákně !!!!
             */


            XMLForecastAvailable = await OpenWeatherMap.IsOfflineXMLAvailable("forecast.xml");

            IsSomethingToShow = false;

            if (XMLForecastAvailable)
            {
                // Get forecast object;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile XMLFile = await localFolder.GetFileAsync("forecast.xml");
                this.Forecast = await OpenWeatherMap.GetForecast(XMLFile);

                if (this.Forecast.ForecasForNowAvailable)
                {
                    IsSomethingToShow = true;
                    OldXMLForecastUsed = !status;
                }
            }


            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        // Toto probehne na hlavnim vlakne
                        ShowForecast(IsSomethingToShow);
                    });
        }
        private void GeoLocation_OnLocate(PositionEventArg e)
        {
            ee = e;
            LocatedByCoords = e.Status;
            if (e.Status)
            {
                // Lokalizovano na zaklade polohy zarizeni
                OpenWeatherMap.DownloadForecastbyCoordinates(e.Pos.Coordinate.Point.Position.Latitude, e.Pos.Coordinate.Point.Position.Longitude);
            }
            else
            {
                // Neznam polohu
                OpenWeatherMap.DownloadForecastbyId(DefaultCityID);
            }
        }
        
        private void ShowForecast(bool status)
        {
            
            // Napsani hlasky + teploty + mista
            if (status)
            {
                var temp = Math.Round(Forecast.AverageTempFromNowToEndOfADay,2);

                this.TextBlockNadHlaska.Text = string.Format("Dneska bude v {0}", Forecast.Location.Name);
                this.TextBlockStupne.Text = temp + " °C";


                if (temp < TempMin)
                {
                    // Pod komfortem
                    TextBlockHlaska.Text = "Zimáááááá";
                }
                else if (temp > TempMax)
                {
                    // Nad komfortem
                    TextBlockHlaska.Text = "Vedro";
                }
                else
                {
                    // V komfortni zone
                    TextBlockHlaska.Text = "Akorát";
                }                
            }
            else
            {
                this.TextBlockHlaska.Text = "Ještě nevím";
                this.TextBlockStupne.Text = "??" + " °C";
                this.TextBlockNadHlaska.Text = "Dneska bude...";

            }
            ShowErrors();
                
        }
        public void ShowErrors()
        {
            var errMsg = new StringBuilder();

            if (!NetworkAvailable)
                errMsg.AppendLine("Bez ppřístupu k internetu!");
            if(!LocatedByCoords)
                errMsg.AppendLine("Požita výchozí poloha!");
            if(OldXMLForecastUsed && XMLForecastAvailable)
                errMsg.AppendLine("Požita stará předpověď!");
            if (!XMLDownloaded)
                errMsg.AppendLine("Nestáhla se nová předpověď");

            errMsg.AppendFormat("Předpověď z {0}.\n ", LastSuccessForecastDownload.ToString("H:mm d.M.y"));

            // DEBUG
            errMsg.AppendLine(DateTime.Now.ToString());
            if (ee != null && ee.Status)
            {
                errMsg.AppendLine(ee.Pos.Coordinate.Point.Position.Latitude.ToString());
            }

            this.TextBlockErr.Text = errMsg.ToString();
        }

        private void RangeSelector_ValueChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RangeChangedEventArgs e)
        {
            ShowForecast(IsSomethingToShow);

        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetLocationWeather();
        }
    }
}
