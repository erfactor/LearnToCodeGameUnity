using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupManagerScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
