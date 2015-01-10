using Microsoft.WindowsAzure.Mobile.Service;

namespace MobileServiceFinal.DataObjects
{
    public class PathGroup : EntityData
    {
        /* note that an id for the entry will be created authmaticly */
        public byte[] SourcePoint { get; set; }

        public byte[] DestinationPoint { get; set; }

        public string GroupName { get; set; }

        public string UserId { get; set; }

    }
}