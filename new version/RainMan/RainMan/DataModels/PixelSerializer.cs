using System;
using System.Collections.Generic;
using System.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace RainMan.DataModels
{

    public class RainApiSerializer
    {
        public static String SerializeRequest(APIRequest request)
        {
            var serializer = new DataContractSerializer(typeof(APIRequest));
            var memStream = new MemoryStream();
            serializer.WriteObject(memStream, request);
            var buffer = memStream.ToArray();
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
        public static APIRequest DeserializeRequest(String input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            var memStream = new MemoryStream(buffer);
            var jsonSeriazlier = new DataContractSerializer(typeof(APIRequest));
            return (APIRequest)jsonSeriazlier.ReadObject(memStream);
        }


    }
    [DataContract]

    public class PixelRep
    {
        [DataMember]
        // NOTE: this is the column index
        public int X { get; set; }
        [DataMember]
        // NOTE: this is the row index
        public int Y { get; set; }
        public PixelRep(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    [DataContract]
    public class APIRequest
    {
        [DataMember]
        public List<PixelRep> Pixels { get; set; }
        public APIRequest(List<PixelRep> pixels)
        {
            this.Pixels = pixels;
        }
    }
}

