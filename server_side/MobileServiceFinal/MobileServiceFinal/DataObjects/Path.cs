using Microsoft.WindowsAzure.Mobile.Service;

namespace MobileServiceFinal.DataObjects
{
    public class Path : EntityData
    {
        /* note that an id for the entry will be created authmaticly */
        public int groupId { get; set; }  /* will be used to "connect" this table with PathGroup */

        public string PathName { get; set; }

        public byte[] PathClass { get; set; }

        public string UserId { get; set; }
    }
}