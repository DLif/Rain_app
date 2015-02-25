using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace RainMan.Navigation
{


    public enum RouteKind
    {
        WALK, DRIVE, BIKE
    }

    public class RoutePredictionArgs
    {

        // collection of paths to compute prediction for
        public List<List<Geopoint>> RouteCollection { get; set; }

        // collectin of path names
        public List<String> RouteNames { get; set; }

        // source point 
        public Geopoint StartPoint { get; set; }

        // destination point
        public Geopoint EndPoint { get; set; }

        // this decides what kind of routes we will compute
        public RouteKind Transportation { get; set; }

        // this saves the maximum time we are willing to wait before starting the journey, effects how we calculate the BEST route
        public int MaxStallingTime { get; set; }

        public int NumTimeSlots { get; set; }


        public RoutePredictionArgs(Geopoint startPoint, Geopoint endpoint, List<List<Geopoint>> pathCollection, RouteKind transportation, int maxStallingTime, int NumTimeSlots)
        {

            this.StartPoint = startPoint;
            this.EndPoint = endpoint;
            this.RouteCollection = pathCollection;
            this.Transportation = transportation;
            this.MaxStallingTime = maxStallingTime;
            this.NumTimeSlots = NumTimeSlots;
            this.RouteNames = null;

        }
        public RoutePredictionArgs(Geopoint startPoint, Geopoint endpoint, List<List<Geopoint>> pathCollection, RouteKind transportation, int maxStallingTime, int NumTimeSlots, List<String> routeNames)
        {

            this.StartPoint = startPoint;
            this.EndPoint = endpoint;
            this.RouteCollection = pathCollection;
            this.Transportation = transportation;
            this.MaxStallingTime = maxStallingTime;
            this.NumTimeSlots = NumTimeSlots;
            this.RouteNames = routeNames;

        }


    }
}
