using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReturnToProfileSelectionScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Destroy(GameObject.Find("LevelLoader"));
        GameObject.Destroy(GameObject.Find("TileLevel"));
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
