using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public GameObject SettingFrame,AudiouCheck,SFX_Check;
    private Vector3 SettingOriginalPo;
    public ScrollRect settingScroll;

    private void Start()
    {
        SettingOriginalPo = SettingFrame.transform.position;
    }

    public void CloseAudioSetting()
    {
        MenuAudioManager.instance.playClick();
        LeanTween.scale(SettingFrame, Vector3.zero, 0.4f).setOnComplete(() => { SettingFrame.transform.parent.gameObject.SetActive(false); });
        LeanTween.move(SettingFrame,
            new Vector3(SettingOriginalPo.x, SettingOriginalPo.y - 550,
                SettingOriginalPo.z), 0.2f);
        LeanTween.alphaCanvas(SettingFrame.GetComponent<CanvasGroup>(), 0, 0.15f);

    }


    public void OpenAudioSetting()
    {
        MenuAudioManager.instance.playClick();
        
        SettingFrame.transform.parent.gameObject.SetActive(true);
        SettingFrame.transform.localScale=Vector3.zero;
        SettingFrame.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.scale(SettingFrame, Vector3.one, 0.1f).setOnComplete(()=>{SettingFrame.transform.parent.gameObject.SetActive(true);});
        LeanTween.alphaCanvas(SettingFrame.GetComponent<CanvasGroup>(), 1, 0.4f);
        SettingFrame.transform.position = new Vector3(SettingOriginalPo.x,
            SettingOriginalPo.y - 550,
            SettingOriginalPo.z);
        
        LeanTween.move(SettingFrame,
            new Vector3(SettingFrame.transform.position.x, SettingFrame.transform.position.y + 550,
                SettingFrame.transform.position.y), 0.2f).setOnComplete(()=>{settingScroll.verticalNormalizedPosition = 1;});
    }

    public void ClickAudioSetting()
    {
        MenuAudioManager.instance.playClick();
        AudiouCheck.SetActive(!AudiouCheck.activeInHierarchy);
        print(AudiouCheck.activeInHierarchy);
        if (AudiouCheck.activeInHierarchy)
        {
            MenuAudioManager.instance.enableMusic();
        }
        else
        {
            MenuAudioManager.instance.DisableMusic();
        }
    }

    public void ClickVfxSetting()
    {
        MenuAudioManager.instance.playClick();
        SFX_Check.SetActive(!SFX_Check.activeInHierarchy);
        print(SFX_Check.activeInHierarchy);
        if (SFX_Check.activeInHierarchy)
        {
            MenuAudioManager.instance.EnableVfx();
        }
        else
        {
            MenuAudioManager.instance.DisableVfx();
        }
    }
}
