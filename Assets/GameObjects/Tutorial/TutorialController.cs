using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        public GameObject firstNarrativeObject;

        // Start is called before the first frame update
        void Awake()
        {
            var stages = GameObject.Find("TutorialStages");
            for (int i = 0; i < stages.transform.childCount; i++)
            {
                stages.transform.GetChild(i).gameObject.SetActive(false);
            }

            stages.transform.GetChild(0).gameObject.SetActive(true);            

            StartCoroutine(WaitAndStart());
        }

        IEnumerator WaitAndStart()
        {
            yield return new WaitForSeconds(0.2f);
            GameObject.Find("TileLevel").transform.position = GameObject.Find("TileLevelHideIndicator").transform.position;
            yield return new WaitForSeconds(0.8f);
            firstNarrativeObject.GetComponent<INarrative>().OnStart();
        }
    }
}


