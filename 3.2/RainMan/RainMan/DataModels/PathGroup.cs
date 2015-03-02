using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace RainMan.DataModels
{
    public class PathGroup
    {

        /* note that an id for the entry will be created authmaticly */
        public byte[] SourcePoint { get; set; }

        public byte[] DestinationPoint { get; set; }

        public string GroupName { get; set; }

        public string Id { get; set; }

        public string UserId { get; set; }

        public string StartName { get; set; }

        public string FinishName { get; set; }

        public PathGroupWrapper toListViewItem()
        {
            var res = new PathGroupWrapper();
            res.DestinationPoint = DestinationPoint;
            res.SourcePoint = SourcePoint;
            res.GroupName = GroupName;
            res.Id = Id;
            res.UserId = UserId;
            res.StartName = StartName;
            res.FinishName = FinishName;
            res.Selected = false;


            return res;
        }

        public static List<List<Geopoint>> toGeopathList(List<Path> pathList)
        {
            List<List<Geopoint>> res = new List<List<Geopoint>>();

            foreach(Path pt in pathList)
            {
                res.Add(PathSerializer.ByteArrayToObject(pt.PathClass));
            }

            return res;

        }

        public static List<String> toNameList(List<Path> pathList)
        {
            List<String> names = new List<string>();
            foreach(Path pt in pathList)
            {
                names.Add(pt.PathName);
            }
            return names;
        }
    }

    // for list view or other lists
    public class PathGroupWrapper : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
   
        public byte[] SourcePoint { get; set; }

        public byte[] DestinationPoint { get; set; }

        public string GroupName { get; set; }

        public string Id { get; set; }

        public string UserId { get; set; }

        public string StartName { get; set; }

        public string FinishName { get; set; }

        private Boolean selected;

        public Boolean Selected { 

            get
            {
                return selected;
            }

            set
            {
                selected = value;
                NotifyPropertyChanged("Selected");

            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public PathGroup toPathGroup()
        {

            var res = new PathGroup();
            res.DestinationPoint = DestinationPoint;
            res.SourcePoint = SourcePoint;
            res.GroupName = GroupName;
            res.Id = Id;
            res.UserId = UserId;
            res.StartName = StartName;
            res.FinishName = FinishName;

            return res;
        }

    }
}
