using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WallPaper.Utility
{
    internal class Serializer<T>
    {
        public void Serialize(T a,String DataFile)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(DataFile, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, a);
            }
        }
        public T Deserialize(String DataFile)
        {
            var formatter = new BinaryFormatter();
            T a;
            using (var stream = new FileStream(DataFile, FileMode.Open, FileAccess.Read))
            {
                a = (T)formatter.Deserialize(stream);
            }
            return a;

        }
    }
}
