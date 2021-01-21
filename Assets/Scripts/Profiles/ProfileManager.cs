using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        const string fileName = "profiles.xml";
        public GameObject profilePrefab;
        public GameObject noProfilePrefab;
        private static GameObject _editPanel;
        private int _createdIndex;

        public Profile selectedProfile;

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void LoadProfiles()
        {
            _editPanel = GameObject.Find("EditPanel");
            //_editPanel.SetActive(false);
            //GameObject.Find("DeletePanel").SetActive(false);
            DeserializeProfiles();
        }

        public void SerializeProfiles()
        {
            var serializer = new XmlSerializer(Profiles.GetType());
            var textWriter = new XmlTextWriter(fileName, Encoding.Default);
            serializer.Serialize(textWriter, Profiles);
            textWriter.Close();
        }
        
        public void DeserializeProfiles()
        {
            if (!File.Exists(fileName))
            {
                SerializeProfiles();
            }
            var serializer = new XmlSerializer(Profiles.GetType());
            var textReader = new XmlTextReader(fileName);
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
            _editPanel.GetComponentInChildren<InputField>().text = "";
            _editPanel.GetComponent<FadingPanel>().Show();
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
            _editPanel.GetComponent<FadingPanel>().Hide();
        }
        
        public void DeleteProfile(int index)
        {
            Profiles[index] = null;
            SerializeProfiles();
            DeserializeProfiles();
        }

        private int GetProfileIndex(GameObject profileGameObject)
        {
            for(int i=0; i<ProfileGameObjects.Length; i++)
            {
                if (profileGameObject == ProfileGameObjects[i]) return i;
            }

            throw new Exception("Could not find a desired profile");
        }

        private void SetSelectedProfile(int selectedProfileIndex)
        {
            selectedProfile = Profiles[selectedProfileIndex];
        }

        public void GoToMainMenu(GameObject clickedGameObject)
        {
            int selectedProfileIndex = GetProfileIndex(clickedGameObject);
            SetSelectedProfile(selectedProfileIndex);
            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(2);
        }

        public void UnlockLevel(int levelToUnlock)
        {
            if (selectedProfile.UnlockedLevels.Contains(levelToUnlock)) return;
            selectedProfile.UnlockedLevels.Add(levelToUnlock);
            SerializeProfiles();
            // DeserializeProfiles();
        }

        public void UnlockNextLevel()
        {
            UnlockLevel(selectedProfile.UnlockedLevels.Max() + 1);
        }

        public bool ShouldPlayTutorial()
        {
            return !selectedProfile.TutorialCompleted;
        }

        public void MarkTutorialAsCompleted()
        {
            selectedProfile.TutorialCompleted = true;
            SerializeProfiles();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                UnlockNextLevel();
            }
        }
    }
}