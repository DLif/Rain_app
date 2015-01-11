using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace App8.DataModel
{
    class ObjectSerializer
    {

            /// Convert a .net object to a byte array.

            public byte[] ObjectToByteArray(Object obj)
            {
                var serializer = new DataContractSerializer(typeof(App8.DataModel.MapUtils.testClass));
                var memStream = new MemoryStream();

                serializer.WriteObject(memStream, obj);

                return memStream.ToArray();
            }

            /// The byte array to convert to a .net object.

            public App8.DataModel.MapUtils.testClass ByteArrayToObject(Byte[] Buffer)
            {
                var memStream = new MemoryStream(Buffer);
                var jsonSeriazlier = new DataContractSerializer(typeof(App8.DataModel.MapUtils.testClass));

                App8.DataModel.MapUtils.testClass result = (App8.DataModel.MapUtils.testClass)jsonSeriazlier.ReadObject(memStream);

                return result;
               
            }
        
    }
}
