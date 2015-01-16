using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

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


    [DataContract]
    public class SerializeableGeopoint
    {

        [DataMember]
        public Double Lat;

        [DataMember]
        public Double Long;

        public SerializeableGeopoint(Geopoint point)
        {
            Lat = point.Position.Latitude;
            Long = point.Position.Longitude;
        }

        public Geopoint toGeopoint()
        {
            return new Geopoint(new BasicGeoposition() { Latitude = Lat, Longitude = Long });
        }

    }

    public class GeopointSerializer
    {


        public static byte[] ObjectToByteArray(Geopoint point)
        {
            SerializeableGeopoint serializeablePoint = new SerializeableGeopoint(point);
            var serializer = new DataContractSerializer(typeof(SerializeableGeopoint));
            var memStream = new MemoryStream();

            serializer.WriteObject(memStream, serializeablePoint);

            return memStream.ToArray();
        }

        /// The byte array to convert to a .net object.

        public static Geopoint ByteArrayToObject(Byte[] Buffer)
        {
            var memStream = new MemoryStream(Buffer);
            var jsonSeriazlier = new DataContractSerializer(typeof(SerializeableGeopoint));

            SerializeableGeopoint result = (SerializeableGeopoint)jsonSeriazlier.ReadObject(memStream);

            return result.toGeopoint();

        }



    }

}
