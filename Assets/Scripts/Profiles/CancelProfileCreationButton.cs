using UnityEngine;
using UnityEngine.UI;

public class CancelProfileCreationButton : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(CancelProfileCreation);
    }

    void CancelProfileCreation()
    {
        GameObject.Find("EditPanel").SetActive(false);
    }
}
