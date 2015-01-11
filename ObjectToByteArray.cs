
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace MPSHouse.Tools
{
    public class ObjectToArray
    {

        /// Convert a .net object to a byte array.

        public byte[] ObjectToByteArray(Object obj)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, obj);
            byte[] rval = fs.ToArray();
            fs.Close();
            
            return rval;
        }

        /// The byte array to convert to a .net object.

        public object ByteArrayToObject(Byte[] Buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(Buffer);
            object rval = formatter.Deserialize(stream);
            stream.Close();
            return rval;
        }
    }
}