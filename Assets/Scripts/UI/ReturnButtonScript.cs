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
        

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameObject.Destroy(GameObject.Find("LevelLoader"));
        //GameObject.Destroy(GameObject.Find("TileLevel"));
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(2);
    }
}
