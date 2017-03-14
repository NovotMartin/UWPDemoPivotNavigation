using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

using Windows.UI.Core;

namespace UWPDemoPivotNavigation
{

    class PositionEventArg
    {
        public string Msg { get; private set; }
        public Geoposition Pos { get; private set; }
        public bool Status { get; private set; }

        public PositionEventArg(string msg)
        {
            this.Msg = msg;
            this.Pos = null;
            this.Status = false;
        }
        public PositionEventArg(string msg, Geoposition pos)
        {
            this.Msg = msg;
            this.Pos = pos;
            this.Status = true;
        }
    }

    static class GeoLocation
    {
        public delegate void DelegateOnLocate(PositionEventArg e);
        public static event DelegateOnLocate OnLocate;        
        public static async void GetLocation()
        {
            // pozadam o pristup k poloze
            var accesStatus = await Geolocator.RequestAccessAsync();

            if (accesStatus != GeolocationAccessStatus.Allowed )
            {
                // Nemam pristup
                OnLocate(new PositionEventArg("Nemam pristup"));

            }
            else
            {
                // Pozadovana presnost neni nastavena, vrati klidne i hodne nepresny vysledek
                var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };
                var position = await geolocator.GetGeopositionAsync();
                OnLocate(new PositionEventArg("", position));
            }
        }       
    }
}
