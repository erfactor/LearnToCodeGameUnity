using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class ClickToContinue : MonoBehaviour, INarrative
    {
        public GameObject nextGameObject;

        public INarrative Next
        {
            get
            {
                return nextGameObject.GetComponent<INarrative>();
            }
        }

        public void OnFinish()
        {
            //StartCoroutine(CoroutineProceedToNextTutorialStage());
        }

        public IEnumerator CoroutineProceedToNextTutorialStage()
        {            
            ActivateNextStage();
            yield return new WaitForFixedUpdate();
            Debug.Log("nextGameObject.name:");
            Debug.Log(nextGameObject.name);
            Next.OnStart();
            yield return new WaitForFixedUpdate();
            DeactivateThisStage();
            yield return new WaitForFixedUpdate();
            Debug.Log("After switching scenes...");
        }

        public void ActivateNextStage()
        {
            int currentStageIndex = transform.parent.GetSiblingIndex();
            var nextStage = transform.parent.parent.GetChild(currentStageIndex + 1);
            nextStage.gameObject.SetActive(true);
        }

        public void DeactivateThisStage()
        {
            transform.parent.gameObject.SetActive(false);
        }

        public void OnStart()
        {
            GetComponent<FadeText>().Show();
            StartCoroutine(OnWaiting());
        }

        public IEnumerator OnWaiting()
        {
            Debug.Log("Click to continue begins waiting...");
            yield return new WaitUntil(() => GetComponent<ClickAwaiter>().hasBeenClicked == true);
            Debug.Log("Click to continue finished waiting...");
            //OnFinish();
            yield return CoroutineProceedToNextTutorialStage();
        }

    }
}

