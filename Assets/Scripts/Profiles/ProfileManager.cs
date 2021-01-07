using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Profiles
{
    public class ProfileManager
    {
        public static Profile[] Profiles = new Profile[3];
        const string filename = "profiles.xml";

        public static void SerializeProfiles()
        {
            var profile = new Profile
            {
                Name = "profilo uno",
                UnlockedLevels = new List<int> {2, 3, 4}
            };
            Profiles[1] = profile;
            var serializer = new XmlSerializer(Profiles.GetType());
            var textWriter = new XmlTextWriter(filename, Encoding.Default);
            serializer.Serialize(textWriter, Profiles);
        }
        
        public static void DeserializeProfiles()
        {
            var serializer = new XmlSerializer(Profiles.GetType());
            var textReader = new XmlTextReader(filename);
            Profiles = (Profile[])serializer.Deserialize(textReader);
        }
    }
}