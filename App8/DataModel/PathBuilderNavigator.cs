using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace App8.DataModel
{
    class PathBuilderNavigator
    {

        public Geopoint Source { set; get; }

        public Geopoint Dest { set; get; }

        public MapRoute route { set; get; }

        public PathBuilderNavigator(Geopoint source, Geopoint dest, MapRoute route)
        {
            this.Source = source;
            this.Dest = dest;
            this.route = route;
        }

    }
}
