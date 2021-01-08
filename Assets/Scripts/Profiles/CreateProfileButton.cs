using Profiles;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CreateProfileButton : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(CreateProfile);
    }

    void CreateProfile()
    {
        var profileName = GameObject.Find("CreatedProfileText").GetComponent<Text>().text;
        var profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        profileManager.CreateProfile(profileName);
    }
}
