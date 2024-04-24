using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] public AudioSource musicSource;

    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip eliminateClip;
    [SerializeField] AudioClip click2Clip;
    [SerializeField] AudioClip moveClip;
    [SerializeField] AudioClip grassClip;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayClickSound()
    {
        musicSource.loop = false;
        musicSource.clip = clickClip;
        musicSource.Play();
    }

    public void PlayElimiSound()
    {
        musicSource.loop = false;
        musicSource.clip = eliminateClip;
        musicSource.Play();
    }

    public void PlayClick2Sound()
    {
        musicSource.loop = false;
        musicSource.clip = click2Clip;
        musicSource.Play();
    }

    public void PlayMoveSound()
    {
        musicSource.loop = true;
        musicSource.clip = moveClip;
        musicSource.Play();
    }
    public void StopMoveSound()
    {
        musicSource.loop = false;
        musicSource.Stop();
    }

    public void PlayGrassSound()
    {
        musicSource.loop = true;
        musicSource.clip = grassClip;
        musicSource.Play();
    }
    public void StopGrassSound()
    {
        musicSource.loop = false;
        musicSource.Stop();
    }
}
