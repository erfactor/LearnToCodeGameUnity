using UnityEngine;
using UnityEngine.EventSystems;
using UI;

//Passesdragmessagestothe parent
public class PassMouseScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public DragScript selectedScript;
    public Rect panelRect;

    void Start()
    {
        InitializePanel();
    }

    void Awake()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (selectedScript == null) return;
        selectedScript.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selectedScript == null) return;
        selectedScript.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (selectedScript == null) return;
        selectedScript.OnEndDrag(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("clicc");
        for(int i=0; i<transform.childCount; i++)
        {
            if (ChildClicked(i, eventData))
            {
                Debug.Log("OH YEEEE");
                selectedScript = transform.GetChild(i).GetComponent<DragScript>();
            }
        }
    }

    public bool ChildClicked(int i, PointerEventData eventData)
    {
        var child = transform.GetChild(i);
        var rect = GetRect(child);
        return CodePanel.IsInRect(rect, eventData.position);
    }

    void InitializePanel()
    {
        panelRect = GetRect(transform);
    }

    Rect GetRect(Transform t)
    {
        Rect rect = this.gameObject.GetComponent<RectTransform>().rect;
        float left = gameObject.transform.position.x - rect.width / 2;
        float top = gameObject.transform.position.y + rect.height / 2;

        return new Rect(left, top, rect.width, rect.height);
    }
}