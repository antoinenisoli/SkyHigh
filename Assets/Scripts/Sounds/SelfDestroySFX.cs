using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroySFX : MonoBehaviour
{
    public void Execute(float delay)
    {
        AudioSource source = GetComponent<AudioSource>();
        source.Play();
        if (delay > 0)
            Destroy(gameObject, delay);
        else
            Destroy(gameObject, source.clip.length);
    }
}
