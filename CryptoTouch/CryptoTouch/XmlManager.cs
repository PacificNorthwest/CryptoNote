using System;
using System.Xml.Serialization;
using System.IO;

namespace CryptoTouch
{
    class XmlManager
    {
        public static void Save(object obj)
        {
            XmlSerializer xml = new XmlSerializer(obj.GetType());
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, "data.xml");
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(file, obj);
            }
        }

        public static object Load(Type type)
        {
            XmlSerializer xml = new XmlSerializer(type);
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, "data.xml");
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return xml.Deserialize(file);
            }
        }
    }
}
