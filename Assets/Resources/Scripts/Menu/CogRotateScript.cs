using UnityEngine;


public class CogRotateScript : MonoBehaviour
{
    public float rotationSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, rotationSpeed);
    }
}
