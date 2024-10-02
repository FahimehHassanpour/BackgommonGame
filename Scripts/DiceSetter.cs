using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSetter : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> DiceSprites;

    public SpriteRenderer top, target;
    public List<SpriteRenderer> RestOfDice;
    public List<GameObject> Dices;
    public int TestVal;
    public GameObject Shadow,Shadow2;
    [ContextMenu("testDice")]
    public void TestDice()
    {
        SetDice(TestVal);
    }

    public void DisAbleShadow()
    {
        Shadow.SetActive(false);
        Shadow2.SetActive(false);
    }

    public void EnableShadow()
    {
        if(GameStateManager.instance.playerA)
        {
            var color = Shadow.GetComponent<SpriteRenderer>().color;
            color.a = 0;
            Shadow.GetComponent<SpriteRenderer>().color = color;
            ActiveDice.DisableShadowCatcher();
            Shadow.SetActive(true);
            LeanTween.alpha(Shadow, 0.5f, 1);

            Shadow2.SetActive(true);
        }
        else
        {
            print("here");
            var color = Shadow2.GetComponent<SpriteRenderer>().color;
            color.a = 0;
            Shadow2.GetComponent<SpriteRenderer>().color = color;
            ActiveDice.DisableShadowCatcher();
            Shadow2.SetActive(true);
            LeanTween.alpha(Shadow2, 0.3f, 0.7f);

            var color2= Shadow.GetComponent<SpriteRenderer>().color;
            color2.a = 0.3f;
            Shadow.GetComponent<SpriteRenderer>().color = color2;

            Shadow.SetActive(true);
        }
        
        

    }
    
    public void SetDice(int val)
    {
        foreach (var VARIABLE in Dices)
        {
            VARIABLE.SetActive(false);
        }
        toActivate = val-1;
        StartCoroutine(ActivateDice(toActivate));
    }

    IEnumerator ActivateDice(int dice)
    {
        yield return null;
        Dices[dice].SetActive(true);
        ActiveDice = Dices[dice].GetComponent<DiceController>();
        ActiveDice.EnableShadowCatcher();
    }
    public DiceController ActiveDice;
    public int toActivate;
    public void Activate()
    {
        Dices[toActivate].SetActive(true);
    }

    public Animator DiceAnim;
    
    public void Roll()
    {
        DiceAnim.SetTrigger("roll");
    }

    public void Deactive()
    {
       // gameObject.SetActive(false);
    }

    private void Update()
    {
        if (ActiveDice != null)
        {
            
            Shadow.transform.position = ActiveDice.transform.position;  
        }
    }

}
