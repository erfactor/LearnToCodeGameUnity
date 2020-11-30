using UnityEngine;

public class JumpInstructionScript : MonoBehaviour
{
    public GameObject bindedInstruction;
    public GameObject arrow;

    public bool IsLabel => transform.childCount == 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject CreateBindedLabel()
    {
        if (IsLabel) throw new System.InvalidOperationException("Can not create a jump label for a jump label");
        if (bindedInstruction != null) throw new System.InvalidOperationException("Can not create a jump label if a jump label already exists");

        GameObject jumpInstructionLabelPrefab = GameObject.Find("JumpInstructionLabel");
        GameObject jumpInstructionLabel = GameObject.Instantiate(jumpInstructionLabelPrefab);
        BindJumpInstructionAndLabel(this.gameObject, jumpInstructionLabel);

        //this is very important - without setting this, the created label would behave like a instruction in the instruction bank
        jumpInstructionLabel.GetComponent<DragScript>().IsReplicator = false;

        return jumpInstructionLabel;
    }

    public void Bind(GameObject otherInstruction)
    {
        this.bindedInstruction = otherInstruction;
    }

    public void BindJumpInstructionAndLabel(GameObject activeInstruction, GameObject passiveInstruction)
    {
        activeInstruction.GetComponent<JumpInstructionScript>().Bind(passiveInstruction);
        passiveInstruction.GetComponent<JumpInstructionScript>().Bind(activeInstruction);
    }

    public void AttachArrow()
    {
        arrow = CreateArrow();
        bindedInstruction.GetComponent<JumpInstructionScript>().arrow = arrow;
        var temp = arrow.GetComponent<JumpInstructionArrow>();
        temp.Initialize(this.gameObject);
    }

    private GameObject CreateArrow()
    {
        if (IsLabel) throw new System.InvalidOperationException("Can not create an arrow for a jump label");
        if (bindedInstruction == null) throw new System.InvalidOperationException("Can not create an arrow if a jump label does not exist");
        if (arrow != null) throw new System.InvalidOperationException("Can not create an arrow if an arrow already exists");

        GameObject arrowPrefab = GameObject.Find("JumpInstructionArrow");
        GameObject createdArrow = GameObject.Instantiate(arrowPrefab);

        return createdArrow;
    }

    
    
}
