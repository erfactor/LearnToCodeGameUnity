using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDraggedScript : MonoBehaviour
{
    Text text;

    void Start()
    {
        text = GameObject.Find("DraggedPosition").GetComponent<Text>();
        var canvasRT = GameObject.Find("Canvas").GetComponent<RectTransform>();
        Debug.Log($"Canvas width: {canvasRT.rect.width}");
        Debug.Log($"Canvas height: {canvasRT.rect.height}");
        float ratio = canvasRT.rect.width / canvasRT.rect.height;
        Debug.Log($"Ratio: {16} : {16/ratio}");
    }

    // Update is called once per frame
    void Update()
    {
        if (CodePanel.draggedObject == null) return;
        Debug.Log($"drag x: {CodePanel.draggedObject.transform.position.x}");
        Debug.Log($"drag y: {CodePanel.draggedObject.transform.position.y}");
        text.text = $"Drag: x: {CodePanel.draggedObject.transform.position.x} y: {CodePanel.draggedObject.transform.position.y}";


    }
}
