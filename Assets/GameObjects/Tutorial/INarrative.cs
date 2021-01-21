using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public interface INarrative
    {
        //public INarrative Next
        //{
        //    get
        //    {
        //        return nextGameObject.GetComponent<INarrative>();
        //    }
        //}

        INarrative Next { get; }
        void OnFinish();
        IEnumerator OnWaiting();
        void OnStart();
    }
}


