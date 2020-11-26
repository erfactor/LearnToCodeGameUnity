using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpInstructionArrow : MonoBehaviour
{
    GameObject jumpInstruction;
    GameObject jumpInstructionLabel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(GameObject jumpInstruction)
    {
        this.jumpInstruction = jumpInstruction;
        this.jumpInstructionLabel = jumpInstruction.GetComponent<JumpInstructionScript>().bindedInstruction;
        this.transform.SetParent(jumpInstruction.transform);
        InitializeImageCurve();
    }

    public void UpdateCurve()
    {
        //LineRenderer renderer = GetComponent<LineRenderer>();

        //var newPoints = CalculatePoints(jumpInstruction.transform.position, jumpInstructionLabel.transform.position);

        //for(int i=0; i<newPoints.Length; i++)
        //{
        //    newPoints[i].z = 10;
        //}

        //renderer.positionCount = 2;
        //renderer.SetPositions(newPoints);
        //renderer.widthMultiplier = 10;

        UpdateImageCurve();
    }

    int CurveLength = 50;

    public void InitializeImageCurve()
    {
        System.Random r = new System.Random();

        for(int i=0; i<transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }

        for(int i=0; i<CurveLength; i++)
        {
            var curveElement = GameObject.Instantiate(GameObject.Find("ArrowPart"));
            curveElement.transform.SetParent(this.transform);
            Color randomColor = new Color((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            curveElement.GetComponent<Image>().color = randomColor;
        }
    }

    public void UpdateImageCurve()
    {
        float instructionWidth = jumpInstruction.GetComponent<RectTransform>().rect.width;
        Vector2 arrowStart = new Vector2(jumpInstruction.transform.position.x + instructionWidth / 2, jumpInstruction.transform.position.y);
        Vector2 arrowEnd = new Vector2(jumpInstructionLabel.transform.position.x + instructionWidth / 2, jumpInstructionLabel.transform.position.y);
        CalculateImagePoints(arrowStart, arrowEnd);
    }

    public void CalculateImagePoints(Vector2 posStart, Vector2 posEnd)
    {
        float ratio = 0;

        Vector3 point1 = posStart;
        Vector3 point2 = new Vector3((Mathf.Max(posStart.x, posEnd.x)) + 100, (posStart.y + posEnd.y)/2, 0);
        Vector3 point3 = posEnd;

        for(int i=0; i<CurveLength; i++)
        {
            ratio = (float)i / CurveLength;
            var image = transform.GetChild(i);

            var tangent1 = Vector3.Lerp(point1, point2, ratio);
            var tangent2 = Vector3.Lerp(point2, point3, ratio);
            var bezier = Vector3.Lerp(tangent1, tangent2, ratio);

            image.position = bezier;
        }
    }
}
