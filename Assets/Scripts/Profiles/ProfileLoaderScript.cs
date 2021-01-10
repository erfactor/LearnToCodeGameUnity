using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Profiles
{
    public class ProfileLoaderScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GameObject.Find("ProfileManager").GetComponent<ProfileManager>().LoadProfiles();
        }

        // Update is called once per frame
        void Update()
        {
            
            gameObject.SetActive(false);
        }
    }
}


