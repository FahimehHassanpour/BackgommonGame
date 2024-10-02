using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Dice : Singleton<Dice>
{
   public Transform DiceApos, Dicea2pos, DiceBpos,DiceBpos2;
   public int _A, _B;
   public DiceParent DiceA, DiceB;
   public int lastMove;
   public List<int> DiceMoves;
   public bool ManualDice;

   public GameObject DicePanelA, DicePanelB;


    public int testA, TestB;
   [ContextMenu("roll dice")] 
   public void TestDice()
    {
        GameStateManager.instance.playerA = true;
        DiceResultFromServer(testA,TestB);
    }
   public void RollDice()
   { 
      //print("about to request server for dice dz");
       DiceMoves.Clear();
       if (GameStateManager.instance.OnlineGame)
       {
          if (ServerConnector.instance.host )
          {
             if (GameStateManager.instance.gameState == GameStateManager.GameState.GameInitDice)
             {
                RequestDiceFromServer();
                print("server dice request dz");
             }
                
          }
          
          if (GameStateManager.instance.gameState != GameStateManager.GameState.GameInitDice)
          {
             print("server dice request dz-");
             RequestDiceFromServer();
          }
          
         
          return;
       }
       print("dize error");
       var a = Random.Range(1, 7);
       var b = Random.Range(1, 7);
      
       if (ManualDice)
       {
          a = _A;
          b = _B; 
       }
       
       if (a == b)
       {
          for (int i = 0; i < 4; i++)
          {
             DiceMoves.Add(a);
          }
       }
       else
       {
          DiceMoves.Add(a);
          DiceMoves.Add(b);
       }
      
       SetDice(a,b);
      
   }


   public void RequestDiceFromServer()
   {
      print("requesting for dice");
      ServerConnector.instance.RequestDice();
   }
    public ParticleSystem DiceADouble, DiceBDouble;
   public void DiceResultFromServer(int a, int b)
   {

      DiceMoves.Clear();
      Dice.instance.DiceA.dice.Shadow.SetActive(false);
        Dice.instance.DiceA.dice.Shadow.SetActive(false);

        Dice.instance.DiceB.dice.Shadow2.SetActive(false);
        Dice.instance.DiceB.dice.Shadow2.SetActive(false);
        if (a == b)
      {
            
         for (int i = 0; i < 4; i++)
         {
            DiceMoves.Add(a);
         }
      }
      else
      {
         DiceMoves.Add(a);
         DiceMoves.Add(b);
      }
      print($"a:{a} b:{b} vizzz ");
      SetDice(a,b);
      print("vizzz"+DiceMoves.Count);
      GameStateManager.instance.DiceCallBack();
   }

   public void SetDice(int a, int b)
   {
      BackGamonBoard.instance.A = a;
      BackGamonBoard.instance.B = b;
        //ss
   }

   
}
