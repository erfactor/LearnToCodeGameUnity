using System.Collections;
using System.Collections.Generic;
using Tutorial;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(Spelling))]
    public class TextSpeller : MonoBehaviour, INarrative
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
            Next.OnStart();
        }

        public void OnStart()
        {
            Debug.Log("OnStart in TextSpeller...");
            GetComponent<Spelling>().StartSpelling();
        }

        public IEnumerator OnWaiting()
        {
            //Text speller does not need any waiting
            yield break;
        }
    }

}

