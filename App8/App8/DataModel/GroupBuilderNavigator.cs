using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace App8.DataModel
{


    // this class is used to enable navigation to/from GroupBuilder page

    public class GroupBuilderNavigator
    {

        // holds true if we navigated from group creation, i.e. specific to a user
        // when this flag is set, the group builder page will request a name for the pass
        // if this is a one time calculation, no name is required
        public Boolean IsUserGroup { set; get; }

        // name of  route if entered
        public String Name { set; get; }

        public Geopoint StartLocation { set; get; }

        public Geopoint EndLocation { set; get; }


        public GroupBuilderNavigator(Boolean isUserGroup)
        {
            this.IsUserGroup = isUserGroup;
        }


        // creates the model from the result saved by this GroupBuilderNavigator
        // this instance will hold the built group after returned from GroupBuilder page
        public PathGroup toPathGroup()
        {
            PathGroup pg = new PathGroup();
            pg.DestinationPoint = GeopointSerializer.ObjectToByteArray(this.EndLocation);
            pg.SourcePoint = GeopointSerializer.ObjectToByteArray(this.StartLocation);
            pg.GroupName = this.Name;

            return pg;
        }

    }

    
}
