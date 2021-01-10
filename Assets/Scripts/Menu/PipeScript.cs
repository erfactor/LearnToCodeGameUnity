using UnityEngine;

public class PipeScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int LevelFrom;
    public int LevelTo;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EmergeAnimation()
    {
        GetComponent<Animator>().SetTrigger("Emerge");
    }

    public void ExtendAnimation()
    {
        GetComponent<Animator>().SetTrigger("Extend");
    }
}
