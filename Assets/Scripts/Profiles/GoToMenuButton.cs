using System.Collections.Generic;
using Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToMenuButton : MonoBehaviour
{
    public List<int> unlockedLevels;
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
        SceneManager.LoadScene(1);
    }
}
