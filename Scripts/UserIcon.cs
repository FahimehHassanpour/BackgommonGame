using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserIcon : MonoBehaviour
{
  public GameObject Light;
  public Image ExtraTime;
  public void Activate()
  {
    Light.SetActive(true);
  }

  public void DeActivate()
  {
    Light.SetActive(false);
  }

    public bool ExtraTimeerFlag;

    Coroutine ExtraCoRoutine;

    [ContextMenu("testFiller")]
    public void Test()
    {
        StartExtraCounter(300000);
    }
    public void StartExtraCounter(int init)
    {
        ExtraTime.gameObject.SetActive(true);   
        ExtraCoRoutine= StartCoroutine(FillCounter(init));
    }
    //ss
    public void StopExtraCoroutine()
    {
        ExtraTime.gameObject.SetActive(false);  
        ExtraTimeerFlag=false;
        if(ExtraCoRoutine != null)
        {
            StopCoroutine(ExtraCoRoutine);
        }
        
    }

    
    IEnumerator FillCounter(int start)
    {
        ExtraTimeerFlag = true;
        int remainingseconds = start / 1000;
        float startTime=Time.time;
        while(ExtraTimeerFlag)
        {
            yield return null;

            float elapsed = (Time.time) - (startTime);

            float fillAmout =(((float)remainingseconds-(float)elapsed) / (float)300);
            
            ExtraTime.fillAmount = fillAmout;
        }

        
    }
}
