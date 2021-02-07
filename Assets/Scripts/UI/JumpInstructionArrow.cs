using UnityEngine;

public class JumpInstructionArrow : MonoBehaviour
{
    GameObject jumpInstruction;
    GameObject jumpInstructionLabel;
    LineRenderer lineRenderer;
    GameObject lineObject;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Destroy(lineObject);
    }

    public void Initialize(GameObject jumpInstruction)
    {
        this.jumpInstruction = jumpInstruction;
        this.jumpInstructionLabel = jumpInstruction.GetComponent<JumpInstructionScript>().bindedInstruction;
        this.transform.SetParent(jumpInstruction.transform);
        InitializeImageCurve();
    }

    public void FixedUpdate()
    {
        UpdateCurve();
    }

    public void UpdateCurve()
    {
        UpdateImageCurve();
    }


    public void InitializeImageCurve()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }        

        var line = GameObject.Instantiate(GameObject.Find("ArrowLine"));
        line.transform.SetParent(GameObject.Find("SolutionPanel").transform);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = CurveLength * 2;
        lineObject = line;
    }

    public void UpdateImageCurve()
    {
        if (jumpInstruction == null) return;
        float instructionWidth = jumpInstruction.GetComponent<RectTransform>().rect.width;
        Vector2 arrowStart = new Vector2(jumpInstruction.transform.position.x + instructionWidth / 2, jumpInstruction.transform.position.y);
        Vector2 arrowEnd = new Vector2(jumpInstructionLabel.transform.position.x + instructionWidth / 2, jumpInstructionLabel.transform.position.y);
        RecalculateImagePoints(arrowStart, arrowEnd);
    }

    int CurveLength = 50;

    public void RecalculateImagePoints(Vector2 posStart, Vector2 posEnd)
    {
        float ratio = 0;
        
        Vector3 point1 = posStart;
        Vector3 point2 = new Vector3((Mathf.Max(posStart.x, posEnd.x)) + 200, (posStart.y + posEnd.y)/2, 0);
        Vector3 point3 = posEnd;

        for(int i=0; i<CurveLength; i++)
        {
            ratio = (float)i / CurveLength;

            var tangent1 = Vector3.Lerp(point1, point2, ratio);
            var tangent2 = Vector3.Lerp(point2, point3, ratio);
            var bezier = Vector3.Lerp(tangent1, tangent2, ratio);

            lineRenderer.SetPosition(i, bezier);
            lineRenderer.SetPosition(2* CurveLength - 1 - i, bezier);
        }

        CalculateHeadRotationAndPosition(posEnd);
    }

    Vector2 Heading1 => lineRenderer.GetPosition(CurveLength - 1);
    Vector2 Heading2 => lineRenderer.GetPosition(CurveLength - 2);

    private void CalculateHeadRotationAndPosition(Vector2 position)
    {
        Vector2 headingVector = Heading1 - Heading2;
        float angle = Mathf.Atan2(headingVector.y, headingVector.x);
        lineObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
        lineObject.transform.position = Heading1;
    }
}
