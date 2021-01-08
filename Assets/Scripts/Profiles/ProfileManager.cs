using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Profiles
{
    public class ProfileManager : MonoBehaviour
    {
        public static Profile[] Profiles = new Profile[3];
        public GameObject[] ProfileGameObjects = new GameObject[3];
        const string filename = "profiles.xml";
        public GameObject profilePrefab;
        public GameObject noProfilePrefab;
        private static GameObject _editPanel;
        private int _createdIndex;

        private void Start()
        {
            _editPanel = GameObject.Find("EditPanel");
            _editPanel.SetActive(false);
            DeserializeProfiles();
        }

        public void SerializeProfiles()
        {
            var serializer = new XmlSerializer(Profiles.GetType());
            var textWriter = new XmlTextWriter(filename, Encoding.Default);
            serializer.Serialize(textWriter, Profiles);
            textWriter.Close();
        }
        
        public void DeserializeProfiles()
        {
            var serializer = new XmlSerializer(Profiles.GetType());
            var textReader = new XmlTextReader(filename);
            Profiles = (Profile[])serializer.Deserialize(textReader);
            textReader.Close();
            
            var panel = GameObject.Find("ProfilesPanel");
            for (var index = 0; index < Profiles.Length; index++)
            {
                if (ProfileGameObjects[index] != null)
                {
                    Destroy(ProfileGameObjects[index]);
                }
                var profile = Profiles[index];
                if (profile == null)
                {
                    var noProfileGO = Instantiate(noProfilePrefab, panel.transform, true);
                    noProfileGO.GetComponentInChildren<AddButton>().index = index;
                    ProfileGameObjects[index] = noProfileGO;
                }
                else
                {
                    var profileGO = Instantiate(profilePrefab, panel.transform, true);
                    profileGO.GetComponentInChildren<DeleteButton>().index = index;
                    ProfileGameObjects[index] = profileGO;
                    profileGO.transform.Find("ProfileText").GetComponent<Text>().text = profile.Name;
                    profileGO.transform.Find("LevelText").GetComponent<Text>().text =
                        $"Level {profile.UnlockedLevels.Max()}";
                }
            }
        }

        public void AddProfileClick(int index)
        {
            _createdIndex = index;
            _editPanel.SetActive(true);
        }

        public void CreateProfile(string profileName)
        {
            Profiles[_createdIndex] = new Profile
            {
                Name = profileName,
                UnlockedLevels = new List<int> {1}
            };
            SerializeProfiles();
            DeserializeProfiles();
            _editPanel.SetActive(false);
        }
        
        public void DeleteProfile(int index)
        {
            Profiles[index] = null;
            SerializeProfiles();
            DeserializeProfiles();
        }
    }
}