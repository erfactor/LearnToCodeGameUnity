using UnityEngine;


public class CogRotateScript : MonoBehaviour
{
    public const float normalRotationSpeed = 0.5f;
    public const float fastRotationSpeed = 1.5f;

    public float rotationSpeed = normalRotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, rotationSpeed);
    }

    public void SpeedUp()
    {
        rotationSpeed = fastRotationSpeed;
    }

    public void SlowDown()
    {
        rotationSpeed = normalRotationSpeed;
    }
}
