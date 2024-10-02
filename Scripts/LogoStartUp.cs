using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoStartUp : MonoBehaviour
{
   public void PlayLogo()
    {
      MenuAudioManager.instance.PlayLogo(); 
    }

    public void FinishPlaying()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        Login.instance.WaitForIntro = false;
    }
}
