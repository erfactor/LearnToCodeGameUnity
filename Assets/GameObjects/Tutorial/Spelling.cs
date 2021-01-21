using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    [RequireComponent(typeof(Text))]
    public class Spelling : MonoBehaviour
    {
        private Text textToDisplay;
        private string contentToDisplay;

        // Start is called before the first frame update
        void Start()
        {
            textToDisplay = GetComponent<Text>();
            contentToDisplay = textToDisplay.text;
            textToDisplay.text = "";
        }

        public void StartSpelling()
        {
            StopAllCoroutines();
            StartCoroutine(CoroutineSpelling());
        }

        private IEnumerator CoroutineSpelling()
        {
            Debug.Log("starting to spell this:" + contentToDisplay);
            for(int i=0; i<contentToDisplay.Length; i++)
            {
                char currentChar = contentToDisplay[i];
                textToDisplay.text += currentChar;
                yield return new WaitForSeconds(0.001f);
            }
            GetComponent<TextSpeller>().OnFinish();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


