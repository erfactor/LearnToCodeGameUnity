using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShooter : MonoBehaviour
{
    ParticleSystem _particleSystem;
    public float DelayInSeconds => _particleSystem.main.duration;
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        _particleSystem.Play();
        GameObject.Find("SFXManager").GetComponent<SFXManagerScript>().PlayConfettiSound();
    }
}
