using Profiles;
using UnityEngine;
using UnityEngine.UI;

public class AcceptProfileDeletionButton : MonoBehaviour
{
    public int index;
    private ProfileManager profileManager;

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(DeleteProfile);
        profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
    }

    void DeleteProfile()
    {
        profileManager.DeleteProfile(index);
        GameObject.Find("DeletePanel").GetComponent<FadingPanel>().Hide();// SetActive(false);
    }
}
