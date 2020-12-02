using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdateScript : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.Find("MousePosition").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"Mouse x: {Input.mousePosition.x} y: {Input.mousePosition.y}";
    }
}
