using Services;
using System.Collections;
using UI.PanelButtons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour, IPointerClickHandler
{
    private void Awake()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        StopShooting();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(GoToMainMenu());
    }

    private IEnumerator GoToMainMenu()
    {
        GetComponent<Animator>().SetTrigger("Hide");
        yield return new WaitForSeconds(Config.Timing.WinWindowSceneChangeDelay);
        GameObject.Find("ReturnButton").GetComponent<ReturnButtonScript>().ReturnToMainMenu();
    }

    public void Show()
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        var text = transform.Find("Number").GetComponent<Text>().text = LevelLoader.Level.Number.ToString();
        // TODO change win text
        GetComponent<Animator>().SetTrigger("Show");
    }

    public void StopShooting()
    {
        var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        foreach(var v in particleSystems)
        {
            v.Stop();
        }
        StopAllCoroutines();
    }

    public void StartShooting()
    {
        var shooters = transform.GetComponentsInChildren<ParticleShooter>();
        foreach (var shooter in shooters)
        {
            StartCoroutine(CoroutineStartShooting(shooter));
        }
    }

    public IEnumerator CoroutineStartShooting(ParticleShooter shooter)
    {
        while (true)
        {
            shooter.Shoot();
            yield return new WaitForSeconds(shooter.DelayInSeconds);
        }
        
    }
}
