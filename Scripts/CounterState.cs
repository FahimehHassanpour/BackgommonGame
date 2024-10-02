using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterState : MonoBehaviour
{
    public bool InGame;
    public int i, j;
    public bool Opponent;
    public bool Hitted, Reached,CanMoveIn;
    public GameObject Flasher;
    public bool active;
    public GameObject Sword;
    public List<int> availableMoves;


    public void playfinalSwoosh()
    {
        SFX.instance.playFinalSwoosh();
    }

    public void playfinalcouter()
    {
        SFX.instance.playFinalCouter();
    }

    public void ShowSword()
    {
        Sword.SetActive(true);
    }

    public void HideSword()
    {
        Sword.SetActive(false);
    }

    private void OnEnable()
    {
        GameTouchManager.OnSwordDeactive += HideSword;
        BackGamonBoard.clear += Clear;
        availableMoves = new List<int>();
    }

    public void Clear()
    {
        availableMoves.Clear();
    }
    private void OnDisable()
    {
        GameTouchManager.OnSwordDeactive -= HideSword;
        BackGamonBoard.clear -= Clear;
    }

    public bool ifCanMoveToWinStageByDrag()
    {
        var res = false;

        if(availableMoves.Contains(24-i))
        {
            res= true;  
        }
        if (i == BackGamonBoard.instance.FindTheSmallestColumnForPlayeA())
        {
            foreach (var move in availableMoves)
            {
                if(move+i>23)
                {
                    res=true;
                }

            }
        }
        

        return res;
    }

    [ContextMenu("startflashing")]
    public void StartFlashing()
    {
        Flasher.SetActive(true);
        StartCoroutine(Co_Flash());
        active = true;
    }

    public void StopFlashing()
    {
        Flasher.SetActive(false);
        StopAllCoroutines();
        active = false;
    }

    private Coroutine FlasherCo;

    IEnumerator Co_Flash()
    {
        float start = Time.time;

        bool CanFlash=true;

        Material mat = Flasher.GetComponent<MeshRenderer>().material;
        
        while (CanFlash)
        {
            var lerp = (Time.time - start) / 0.7f;
            if (lerp > 1)
            {
                CanFlash = false;
            }

            var a = 1 - lerp;
            
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, a);

            yield return null;

        }

        StartCoroutine(CoFlash_On());

    }

    IEnumerator CoFlash_On()
    {
        float start = Time.time;

        bool CanFlash=true;

        Material mat = Flasher.GetComponent<MeshRenderer>().material;
        
        while (CanFlash)
        {
            var lerp = (Time.time - start) / 0.7f;
            if (lerp > 1)
            {
                CanFlash = false;
            }

            
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, lerp);

            yield return null;

        }

        StartCoroutine(Co_Flash());
    }

}
