using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnButtonScript : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RemoveLevelObjects()
    {
        var levelObject = GameObject.Find("LevelLoader");
        Destroy(levelObject);
        var tileLevel = GameObject.Find("TileLevel");
        Destroy(tileLevel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RemoveLevelObjects();
        SceneManager.LoadScene("MainMenu");
    }
}
