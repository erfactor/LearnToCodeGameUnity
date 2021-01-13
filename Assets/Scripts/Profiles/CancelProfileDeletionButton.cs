using UnityEngine;
using UnityEngine.UI;

public class CancelProfileDeletionButton : MonoBehaviour
{
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(CancelProfileDeletion);
    }

    void CancelProfileDeletion()
    {
        GameObject.Find("DeletePanel").SetActive(false);
    }
}
