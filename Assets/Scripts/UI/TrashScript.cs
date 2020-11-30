using UnityEngine;
using UnityEngine.EventSystems;

public class TrashScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop - usuwanie obiektu");
        if (eventData.pointerDrag != null)
        {
            //eventData.pointerDrag.transform.position = transform.position;            

            Delete(eventData.pointerDrag);
            //CodePanel.Remove(eventData.pointerDrag);

            //GameObject.Find("RaycastManager").GetComponent<RaycastManagerScript>().SetRaycastBlockingAfterInstructionReleased();
            RaycastManagerScript.SetRaycastBlockingAfterInstructionReleased();
        }
    }

    public void Delete(GameObject go)
    {
        GameObject.Find("SolutionPanel").GetComponent<CodePanel>().Remove(go);
    }
}
