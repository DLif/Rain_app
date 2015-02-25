using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

namespace RainMan.DataModels
{
   

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

    [DataContract]
    public class SerializeablePath
    {
        [DataMember]
        public List<SerializeableGeopoint> SerializedWayPoints;



        public SerializeablePath(List<Geopoint> wayPoints)
        {

            SerializedWayPoints = new List<SerializeableGeopoint>();

            // serialize each point 
            foreach (Geopoint point in wayPoints)
            {
                SerializedWayPoints.Add(new SerializeableGeopoint(point));
            }


        }
        public List<Geopoint> toGeopoints()
        {

            List<Geopoint> res = new List<Geopoint>();

            foreach (SerializeableGeopoint point in this.SerializedWayPoints)
            {
                res.Add(point.toGeopoint());
            }
            return res;
        }

    }


    public class PathSerializer
    {

        public static byte[] ObjetToByteArray(List<Geopoint> wayPoints)
        {
            SerializeablePath seriPath = new SerializeablePath(wayPoints);

            var serializer = new DataContractSerializer(typeof(SerializeablePath));
            var memStream = new MemoryStream();

            serializer.WriteObject(memStream, seriPath);

            return memStream.ToArray();
        }


        public static List<Geopoint> ByteArrayToObject(Byte[] Buffer)
        {
            var memStream = new MemoryStream(Buffer);
            var jsonSeriazlier = new DataContractSerializer(typeof(SerializeablePath));

            SerializeablePath result = (SerializeablePath)jsonSeriazlier.ReadObject(memStream);

            return result.toGeopoints();

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
