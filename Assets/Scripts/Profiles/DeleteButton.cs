using Profiles;
using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    public int index;
    void Start()
    {
        var button = GetComponent<Button>();
        var profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        button.onClick.AddListener(() => profileManager.DeleteProfile(index));
    }
}
