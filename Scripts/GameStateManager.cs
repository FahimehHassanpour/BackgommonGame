using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateManager : Singleton<GameStateManager>
{
    // 1 and 2
    public int PlayerNumber,EnemyNumber;
    public Image GameCounter;
    public float TurnTime;
    private Coroutine TUrningCoroutine;

    public enum Player
    {
        playerAOnServer,
        playerBOnServer
    }

    private void Start()
    {
       
        if (ServerConnector.instance.Resume)
        {
           GameStart();
        }
        else
        {
            print("new");
            Board.SetTrigger("newgame");
        }
    }

    public Animator Board;
    // public Player player;
    [ContextMenu("set board")]
    public void GameStart()
   {
       GetComponent<CounterManager>().CreatePlayerCouterPool();
       GetComponent<CounterManager>().CreateOpponentCouterPool();
       
       GetComponent<CounterManager>().SetBoardStarPoses();

       GetComponent<BackGamonBoard>().SetBoard();
       // GetComponent<BackGamonBoard>().SetBoardForEndStageTest();
       diceInAir = new WaitForSeconds(diceInAirDuration);
       if (ServerConnector.instance != null)
       {
           OnlineGame = true;
           
       }

       if (ServerConnector.instance.Resume)
       {
            //this 2000 is still logicless can be removed
            

            


           if (ServerConnector.instance.PlayersTurn)
           {
               GameStateManager.instance.gameState = GameState.PlayerAMoveturn;
               playerA = true;
               PlayerNumber = 1;
               Playerturn = true;
               EnemyNumber = 2;
           }
           else
           {
               PlayerB = true;
               PlayerNumber = 2;
               EnemyNumber = 1;
               Playerturn = false;
               GameStateManager.instance.gameState = GameState.PlayerBMoveTurn;
           }
           if (ServerConnector.instance.ServerPhase == "move")
           {
              //   print("resume"+ ServerConnector.instance.ServerLastTimer);
               StartTurn((((ServerConnector.instance.getTimestamp()-ServerConnector.instance.ServerLastTimer))/1000)-2);
             //  Dice.instance._A = ServerConnector.instance.DiceA;
             //  Dice.instance._B = ServerConnector.instance.DiceB;

               print(ServerConnector.instance.DiceA);
               print(ServerConnector.instance.DiceB);
                Dice.instance.DiceResultFromServer(ServerConnector.instance.DiceA, ServerConnector.instance.DiceB);
                return;
             //  Dice.instance.DiceMoves.Clear();
             //  Dice.instance.DiceMoves.Add(ServerConnector.instance.DiceB);
             //  Dice.instance.DiceMoves.Add(ServerConnector.instance.DiceA);
             //  BackGamonBoard.instance.Board = ServerConnector.instance.ServerBoard;
             //  DiceVisualizer.instance.VisuaLizeDice(Dice.instance.DiceMoves);
           }else if (ServerConnector.instance.ServerPhase == "dice")
           {
               print("server phase is dice");
               ShowDiceBtnResume();
           }
           
           BackGamonBoard.instance.findPlayAbleCouters();
           
           
       }
       
       

   }

   public bool OnlineGame;
    public void GameStartdice(bool redice=false)
    {
                print(redice);
               if (!ServerConnector.instance.host && !redice)
               {
                   return;
               }
               if (DiceInAir != null && redice)
               {
                   StopCoroutine(DiceInAir); 
                   Dice.instance.DiceA.gameObject.SetActive(false);
                   Dice.instance.DiceB.gameObject.SetActive(false);
               }
         
               gameState = GameState.GameInitDice;
              
             //  rollDice();  
        
       
    }
    
    

    public bool Playerturn;

    public enum GameState
    {
        
        PlayerATurn,
        PlayerBturn,
        
        
        PlayerAdiceTurn,
        PlayerBDiceTurn,
        
        PlayerAMoveturn,
        PlayerBMoveTurn,
        
        GameInitDice,
        DiceInAir,
        
        GameFinished
    }

    public void GameFinish()
    {
        gameState = GameState.GameFinished;
        StartCoroutine(Co_GameFinish());
        PlayerPrefs.SetString("gameid","");
    }

    IEnumerator Co_GameFinish()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }
    
    

    public GameObject DiceBtn;
    public GameState gameState;
    public bool playerA, PlayerB;
    public void ShowDiceBtn()
    {
        if (gameState == GameState.PlayerATurn)
        {
            DiceBtn.SetActive(true);
            waitForRolling = StartCoroutine(DiceWait()); 
        }
        
        
        if (playerA)
        {
            gameState = GameState.PlayerAdiceTurn;
        }
        if (PlayerB)
        {
            gameState = GameState.PlayerBDiceTurn;
        }
    }

    public void ShowDiceBtnResume()
    {
        print("st1");
        if (gameState == GameState.PlayerAMoveturn)
        {
            print("st2");
            DiceBtn.SetActive(true);
            waitForRolling = StartCoroutine(DiceWait((ServerConnector.instance.getTimestamp()- ServerConnector.instance.ServerLastTimer)/1000)); 
        }
        
        
        if (playerA)
        {
            gameState = GameState.PlayerAdiceTurn;
        }
        if (PlayerB)
        {
            gameState = GameState.PlayerBDiceTurn;
        }
    }

    public void rollDice()
    {
        if (waitForRolling != null)
        {
            StopCoroutine(waitForRolling);    
        }
        
        //print("rolling dice dz");
        print("Dice btn");
        
        
        DiceBtn.SetActive(false);
        Dice.instance.RollDice();
        if (!OnlineGame)
        {
            DiceCallBack();
        }
        
        
    }


    public void DiceCallBack()
    {
        Dice.instance.DiceA.dice.DisAbleShadow();
        Dice.instance.DiceB.dice.DisAbleShadow();

        DiceBtn.SetActive(false);
        gameState = GameState.DiceInAir;
        DiceInAir = StartCoroutine(diceInair());   
    }

    public void MoveFinish()
    {
        if (GameStateManager.instance.gameState == GameState.PlayerAMoveturn)
        {
            MoveConfirm.instance.ConfirmBtn.SetActive(true);
        }
       
       
    }

    public void MoveFinishConfirm(bool fromserver=false)
    {
        SFX.instance.PlayTurnFinish();
        Dice.instance.DicePanelA.SetActive(false);
        Dice.instance.DicePanelB.SetActive(false);
        GameUI.instance.playerIcon.GetComponent<UserIcon>().StopExtraCoroutine();
        Dice.instance.DiceA.gameObject.SetActive(false);
        Dice.instance.DiceB.gameObject.SetActive(false);
        if (gameState == GameState.PlayerAMoveturn && !fromserver)
        {
            ServerConnector.instance.SendToServerTurnFinish();
        }
        
        
        MoveConfirm.instance.moves.Clear();
        MoveConfirm.instance.MoveAndHitDictionary.Clear();
        MoveConfirm.instance.UndoBtn.SetActive(false);
        playerA=!playerA;
        PlayerB = !PlayerB;
        gameState = playerA ? GameState.PlayerATurn : GameState.PlayerBturn;
        ShowDiceBtn();
        MoveConfirm.instance.ConfirmBtn.SetActive(false);
        StopTurn();
        DiceVisualizer.instance.HideDicePanels();
    }
    
    
    public float DiceRollWait,diceInAirDuration;
    public Image DiceBtnFillImage;
    private bool rollingTimeAllowed;
    private Coroutine waitForRolling,DiceInAir;

    private WaitForSeconds diceInAir;
    
    IEnumerator diceInair()
    {

        SFX.instance.PlayDiceRoll();
       
        
        print("dzz dice in air");
        
        
        if (!playerA && !PlayerB)
        {
            print("dzz !A!b");
            Dice.instance.DiceB.transform.position = Dice.instance.DiceBpos.transform.position;
            Dice.instance.DiceB.transform.rotation = Dice.instance.DiceBpos.transform.rotation;
        
        //.
            Dice.instance.DiceA.transform.position = Dice.instance.DiceApos.transform.position;
            Dice.instance.DiceA.transform.rotation = Dice.instance.DiceApos.transform.rotation;
        
            Dice.instance.DiceA.gameObject.SetActive(true);
            Dice.instance.DiceB.gameObject.SetActive(true);
        
            Dice.instance.DiceA.dice.SetDice(BackGamonBoard.instance.A);
            Dice.instance.DiceB.dice.SetDice(BackGamonBoard.instance.B);
        
        
            Dice.instance.DiceA.dice.Roll();
            Dice.instance.DiceB.dice.Roll();
            //print("activating dice");
           
        }
        else if(playerA)
        {
            print("dzz a");
            Dice.instance.DiceB.transform.position = Dice.instance.Dicea2pos.transform.position;
            Dice.instance.DiceB.transform.rotation = Dice.instance.Dicea2pos.transform.rotation;
        
        
            Dice.instance.DiceA.transform.position = Dice.instance.DiceApos.transform.position;
            Dice.instance.DiceA.transform.rotation = Dice.instance.DiceApos.transform.rotation;
        
            Dice.instance.DiceA.gameObject.SetActive(true);
            Dice.instance.DiceB.gameObject.SetActive(true);
        
            Dice.instance.DiceA.dice.SetDice(BackGamonBoard.instance.A);
            Dice.instance.DiceB.dice.SetDice(BackGamonBoard.instance.B);
        
        
            Dice.instance.DiceA.dice.Roll();
            Dice.instance.DiceB.dice.Roll();
            
            
        }else if (PlayerB)
        {
            print("dzz b");
            Dice.instance.DiceB.transform.position = Dice.instance.DiceBpos.transform.position;
            Dice.instance.DiceB.transform.rotation = Dice.instance.DiceBpos.transform.rotation;
        
        
            Dice.instance.DiceA.transform.position = Dice.instance.DiceBpos2.transform.position;
            Dice.instance.DiceA.transform.rotation = Dice.instance.DiceBpos2.transform.rotation;
        
            Dice.instance.DiceA.gameObject.SetActive(true);
            Dice.instance.DiceB.gameObject.SetActive(true);
        
            Dice.instance.DiceA.dice.SetDice(BackGamonBoard.instance.A);
            Dice.instance.DiceB.dice.SetDice(BackGamonBoard.instance.B);
        
        
            Dice.instance.DiceA.dice.Roll();
            Dice.instance.DiceB.dice.Roll();
            
            
        }
        yield return new WaitForSeconds(1.4f);
        if (BackGamonBoard.instance.A == BackGamonBoard.instance.B)
        {
            Dice.instance.DiceADouble.Play();
            Dice.instance.DiceBDouble.Play();
            SFX.instance.PlayDouble();
        }
        //print("before landing");
        // //print(diceInAir==null);
        yield return new WaitForSeconds(0.6f);
        print("dzz dice landed");
        //print("finding players");
        
            if (!playerA && !PlayerB)
        {
          //  Dice.instance.DicePanelA.SetActive(true);
          //  Dice.instance.DicePanelB.SetActive(true);
            //print("finding the player to star");
            if (BackGamonBoard.instance.A > BackGamonBoard.instance.B)
            {
                print("dzz player A starts");
                playerA = true;
                
            }else if (BackGamonBoard.instance.A == BackGamonBoard.instance.B)
            {
                MessageBoard.instance.DiceTie();
                yield break;
                
            }
            else
            {
                print("dzz player B starts");
                PlayerB = true;
                
            }
        }
        
        StartTurn();
        DiceVisualizer.instance.VisuaLizeDice(Dice.instance.DiceMoves);
        print("dcbug1");
        if (playerA)
        {
            print("dcbug2");
            gameState = GameState.PlayerAMoveturn;
            Dice.instance.DicePanelA.SetActive(true);
            Dice.instance.DicePanelB.SetActive(false);
            PlayerNumber = 1;
            EnemyNumber = 2;
            GameUI.instance.playerIcon.GetComponent<UserIcon>().Activate();
            GameUI.instance.OppIcon.GetComponent<UserIcon>().DeActivate();
            BackGamonBoard.instance.findPlayAbleCouters();
        }

        if (PlayerB)
        {
            GameUI.instance.playerIcon.GetComponent<UserIcon>().DeActivate();
            GameUI.instance.OppIcon.GetComponent<UserIcon>().Activate();
            gameState = GameState.PlayerBMoveTurn;
            Dice.instance.DicePanelB.SetActive(true);
            Dice.instance.DicePanelA.SetActive(false);
            PlayerNumber = 2;
            EnemyNumber = 1;
            print("dcbug");
            // BackGamonBoard.instance.findPlayAbleCouters();
        }
        
        
    }


    
    
    IEnumerator DiceWait(double elapsed=0)
    {
        print("elapesd"+elapsed);
        double start = Time.time;
        rollingTimeAllowed = true;
        while (rollingTimeAllowed)
        {
            double lerp = ((Time.time - start)+elapsed) / DiceRollWait;
            if (lerp>1)
            {
                rollingTimeAllowed = false;
            }

            DiceBtnFillImage.fillAmount =(float) lerp;
            yield return null;
        }
        rollDice();
        //dice turn time finished
    }

    public void StartTurn(double elapsed=0)
    {
        print("resume" + elapsed);
       if(GameCounter.gameObject.activeInHierarchy)
        { return; }
       TUrningCoroutine= StartCoroutine(Co_Turn(elapsed));
       GameCounter.gameObject.SetActive(true);
    }

    

    public void StopTurn()
    {
        StopCoroutine(TUrningCoroutine);
        GameCounter.gameObject.SetActive(false);
    }
    
    
    IEnumerator Co_Turn(double elapsedTime=0)
    {
        print("Turn start:+"+elapsedTime);
        double start = Time.time;
       // print(start);
        bool canTurn = true;
        while (canTurn)
        {
            double t = ((Time.time - start)+elapsedTime) / TurnTime;
            if (t > 1)
            {
                canTurn = false;
            }
            float WaitingLeft = Mathf.Lerp(1, 0, (float)t);
            GameCounter.fillAmount = WaitingLeft;
            yield return null;
        }
        // TimeOut();
    }

   public void TimeOut(bool won,int point)
    {
        if(GameStateManager.instance.gameState==GameStateManager.GameState.PlayerAMoveturn)
        {
            if(BackGamonBoard.instance.PlayerAFinalCouter.Count==15)
            {
                StartCoroutine(waitForFinalCameraAnimation(won,point));
                return;
            }
        }
        gameState = GameState.GameFinished;
        GameMaterialSource.instance.ResetPlayerMaterials();
        GameUI.instance.FinalResult(won, point);
        
        
    }


    IEnumerator waitForFinalCameraAnimation(bool won,int point)
    {
        GameUI.instance.hideUi();
        yield return new WaitForSeconds(4);
        gameState = GameState.GameFinished;
        GameMaterialSource.instance.ResetPlayerMaterials();
        GameUI.instance.FinalResult(won, point);
    }
    
}
