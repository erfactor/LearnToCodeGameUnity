using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Commands;

public class MoveInstructionScript : MonoBehaviour, IGUIInstruction
{
    // Start is called before the first frame update
    void Start()
    {
        var directionIndicator = transform.Find("DirectionIndicator").gameObject;
        var directionIndicatorScript = directionIndicator.GetComponent<DirectionIndicatorScript>();
        directionIndicatorScript.Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    public List<ICommand> GetInstruction()
    {
        throw new System.NotImplementedException();
    }
}

public interface IGUIInstruction
{
    List<ICommand> GetInstruction(); 
}
