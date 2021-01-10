using UnityEngine;
using UnityEngine.EventSystems;

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
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(2);
    }
}
