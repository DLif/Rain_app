using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App8.DataModel
{
    class Path
    {
        /* note that an id for the entry will be created authmaticly */
        public int groupId { get; set; }  /* will be used to "connect" this table with PathGroup */

        public string PathName { get; set; }

        public byte[] PathClass { get; set; }

       
    }
}
