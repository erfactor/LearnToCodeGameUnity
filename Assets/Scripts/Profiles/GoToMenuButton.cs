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
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
