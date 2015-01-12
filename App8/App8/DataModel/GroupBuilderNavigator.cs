using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace App8.DataModel
{


    // this class is used to enable navigation to/from GroupBuilder page

    class GroupBuilderNavigator
    {

        // holds true if we navigated from group creation, i.e. specific to a user
        public Boolean IsUserGroup { set; get; }

        // name of  route if entered
        public String Name { set; get; }

        public Geopoint StartLocation { set; get; }

        public Geopoint EndLocation { set; get; }

    }
}
