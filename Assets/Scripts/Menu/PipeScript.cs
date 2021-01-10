using UnityEngine;

public class PipeScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int LevelFrom;
    public int LevelTo;

    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Emerge()
    {
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("Emerge");
    }

    public void Extend()
    {
        GetComponent<Animator>().SetTrigger("Extend");
    }
}
