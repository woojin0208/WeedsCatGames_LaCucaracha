using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] vfxClips;
    [SerializeField] private AudioSource audioSource;

    public void StartVFX(int count)
    {
        audioSource.Stop();

        audioSource.clip = vfxClips[count];
        audioSource.Play();
    }
}
