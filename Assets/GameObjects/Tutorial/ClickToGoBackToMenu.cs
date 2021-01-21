using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorial
{
    public class ClickToGoBackToMenu : MonoBehaviour, IPointerClickHandler, INarrative
    {
        public INarrative Next => throw new System.ArgumentException();

        public void OnFinish()
        {
            
        }        

        public void OnStart()
        {
            GetComponent<FadeText>().Show();
        }

        public IEnumerator OnWaiting()
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject.Find("AnimationPanel").GetComponent<AnimationPanel>().ChangeScene(2);
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

