using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : Singleton<MenuAudioManager>
{
    public AudioSource VFX;
    private AudioSource Ambient;
    public AudioClip AmbientMusic,click,sweep,error,Logo;
    
    
    private void Start()
    {
        Ambient = GetComponent<AudioSource>();
    }

    public void PlayLogo()
    {
        VFX.PlayOneShot(Logo);
    }

    public void enableMusic()
    {
        Ambient.clip = AmbientMusic;
        Ambient.loop = true;
        Ambient.Play();
    }
    

    public void DisableMusic()
    {
        Ambient.Stop();
    }

    public void PlaySwoosh()
    {
        if (VFX != null)
            VFX.PlayOneShot(sweep);
    }
    public void EnableVfx()
    {
        VFX.volume = 1;
        PlayerPrefs.SetInt("sfx",1); 
    }

    public void DisableVfx()
    {
        VFX.volume = 0;
        PlayerPrefs.SetInt("sfx",0);
    }

    public void playClick()
    {
        VFX.PlayOneShot(click);
    }
    
    
    public void PlayError()
    {
        VFX.PlayOneShot(error);
    }
    
    
    
}
