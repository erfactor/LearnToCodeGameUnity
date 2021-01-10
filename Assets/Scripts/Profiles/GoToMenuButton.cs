using Profiles;
using UnityEngine;
using UnityEngine.UI;

public class GoToMenuButton : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();
        var profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        this.GetComponent<Button>().onClick.AddListener(ProceedToMenu);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ProceedToMenu()
    {
        var profileManager = GameObject.Find("ProfileManager").GetComponent<ProfileManager>();
        profileManager.GoToMainMenu(transform.parent.gameObject);
    }
}
