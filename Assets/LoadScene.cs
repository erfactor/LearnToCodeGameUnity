using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private void PlayLevel()
    {
        SceneManager.LoadScene("UI Example Scene");
    }
}