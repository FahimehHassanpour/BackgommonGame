using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBoard : Singleton<MessageBoard>
{
    public TextMeshProUGUI MessageBoardText;

    [ContextMenu("test")]
    public void test()
    {
        ShowMessage("test");
    }

    public void DiceTie()
    {
        ShowMessage("Dice Tie");
        
        StartCoroutine(ReDice());
    }
    
    public void ShowMessage(string message)
    {
        MessageBoardText.transform.parent.gameObject.SetActive(true);
        MessageBoardText.transform.localScale=Vector3.zero;
        MessageBoardText.text = message;
        LeanTween.scale(MessageBoardText.gameObject, Vector3.one, 2f).setEaseOutElastic().setOnComplete(()=>MessageBoardText.transform.parent.gameObject.SetActive(false));

    }


    public void blocked()
    {
        ShowMessage("Blocked");
        StartCoroutine(Co_NoMoves());
    }

    public void NoMoves()
    {
        ShowMessage("No Moves");
        StartCoroutine(Co_NoMoves());
    }

    IEnumerator Co_NoMoves()
    {
        yield return new WaitForSeconds(2);
        GameStateManager.instance.MoveFinish();
    }
    
    IEnumerator ReDice()
    {
        yield return new WaitForSeconds(2);
        Dice.instance.DiceA.gameObject.SetActive(false);
        Dice.instance.DiceB.gameObject.SetActive(false);
        Dice.instance.RollDice();

    }

    public void GameFinished(string finalMessage)
    {
        ShowMessage(finalMessage);
        GameStateManager.instance.GameFinish();
    }
    
    
}
