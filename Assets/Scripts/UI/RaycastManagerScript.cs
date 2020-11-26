﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetRaycastBlockingOnInstructionDragged()
    {
        GameObject.Find("Trash").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("InstructionBank").GetComponent<CanvasGroup>().blocksRaycasts = false;

        GameObject.Find("SolutionPanel").GetComponent<CodePanel>().SetRaycastBlockingForAllInstructions(false);
    }

    public static void SetRaycastBlockingAfterInstructionReleased()
    {
        GameObject.Find("Trash").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("InstructionBank").GetComponent<CanvasGroup>().blocksRaycasts = true;

        GameObject.Find("SolutionPanel").GetComponent<CodePanel>().SetRaycastBlockingForAllInstructions(true);
    }
}