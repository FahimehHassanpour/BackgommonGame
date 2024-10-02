using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : Singleton<SFX>
{
    public AudioClip diceRoll,couter1,couter2,turn,laugh,smile,cry,surprise,coin,power,finalSwoosh,finalcouter,doubleSfx;
    public AudioSource SfxAudioSource,Sfx2;
    public void PlayDiceRoll()
    {
        SfxAudioSource.PlayOneShot(diceRoll);
    }
    public void PlayDouble()
    {
        if (SfxAudioSource != null)
        {
            SfxAudioSource.PlayOneShot(doubleSfx);
        }
    }
    public void playFinalCouter()
    {
        if (SfxAudioSource != null)
        {
            Sfx2.PlayOneShot(finalcouter);
        }
    }
    [ContextMenu("fswo")]
    public void playFinalSwoosh()
    {
        print("playFinalSwoosh");
        if (SfxAudioSource != null)
        {
            Sfx2.PlayOneShot(finalSwoosh);
        }
    }


    public void PlayCouter()
    {
        var rsn=Random.Range(0, 2);
        if (rsn == 0)
        {
            
            SfxAudioSource.PlayOneShot(couter1);
        }
        else
        {
            SfxAudioSource.PlayOneShot(couter2);
        }

    }


    public void PlayTurnFinish()
    {
        SfxAudioSource.PlayOneShot(turn);
    }
    

    public void PlayLaugh()
    {
        SfxAudioSource.PlayOneShot(laugh);
    }

    public void PlaySmile()
    {
        SfxAudioSource.PlayOneShot(smile);
    }

    public void PlaySurprise()
    {
        SfxAudioSource.PlayOneShot(surprise);
    }

    public void PlayCry()
    {
        SfxAudioSource.PlayOneShot(cry);
    }

    public void Playcoin()
    {
        SfxAudioSource.PlayOneShot(coin);
    }

    public void playPower()
    {
        SfxAudioSource.PlayOneShot(power);
    }


    
}
