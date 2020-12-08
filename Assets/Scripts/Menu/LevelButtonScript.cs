using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonScript : MonoBehaviour
{
    public string filename;
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(PlayLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayLevel()
    {
        Debug.Log($"Loading level with name: [{filename}]");
        LevelLoader.LevelFileName = filename;
        SceneManager.LoadScene("UI Example Scene");
    }
}
