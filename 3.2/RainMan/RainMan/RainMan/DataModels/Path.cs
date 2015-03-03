using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainMan.DataModels
{
    public class Path
    {
        /* note that an id for the entry will be created authmaticly */

        public string groupId { get; set; }  /* will be used to "connect" this table with PathGroup */

        public string PathName { get; set; }

        /* the path itself, serialized into a byte[] */
        public byte[] PathClass { get; set; }

        public string UserId { get; set; }

        public string Id { get; set; }


    }
}
