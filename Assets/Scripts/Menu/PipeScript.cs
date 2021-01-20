using UnityEngine;

public class PipeScript : MonoBehaviour
{
    public int LevelFrom;
    public int LevelTo;

    void Awake()
    {
        gameObject.SetActive(false);
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
