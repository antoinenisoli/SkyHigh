using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroySFX : MonoBehaviour
{
    private void Awake()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.Play();
        Destroy(gameObject, source.clip.length);
    }
}
