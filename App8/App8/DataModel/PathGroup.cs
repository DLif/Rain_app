using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App8.DataModel
{
    class PathGroup
    {

        /* note that an id for the entry will be created authmaticly */
        public byte[] SourcePoint { get; set; }

        public byte[] DestinationPoint { get; set; }

        public string GroupName { get; set; }

        public string Id { get; set; }

        public string UserId { get; set; }
    }
}
