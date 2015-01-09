using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Mobile.Service;

namespace jesus.DataObjects
{
    public class MyDataObjectClass : EntityData
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}