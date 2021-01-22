using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class ToggleBoard : MonoBehaviour, INarrative
    {
        public GameObject VisiblePosition;
        public GameObject HiddenPosition;

        public GameObject nextGameObject;

        public static bool IsHidden = true;

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
            StartCoroutine(OnWaiting());
        }

        public IEnumerator OnWaiting()
        {
            var targetLocation = IsHidden ? VisiblePosition.transform.position : HiddenPosition.transform.position;
            IsHidden = !IsHidden;
            var movedObject = GameObject.Find("TileLevel");

            for(int i=0; i<40; i++)
            {
                var newPosition = Vector3.Lerp(movedObject.transform.position, targetLocation, 0.12f);
                movedObject.transform.position = newPosition;
                yield return new WaitForFixedUpdate();
            }
            OnFinish();
            yield break;
        }

        // Start is called before the first frame update


        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

