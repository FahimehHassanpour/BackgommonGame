using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoveConfirm : Singleton<MoveConfirm>
{
    public List<Move> moves;
    public List<Move> HittedMoves;
    public GameObject UndoBtn, ConfirmBtn;
    public Dictionary<Move, Move> MoveAndHitDictionary;

    private void Start()
    {
        MoveAndHitDictionary = new Dictionary<Move, Move>();
        moves = new List<Move>();
        HittedMoves = new List<Move>();
    }

    public void Undo()
    {
        if (GameStateManager.instance.OnlineGame)
        {
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                ServerConnector.MessageTag ms = new ServerConnector.MessageTag();
                ms.tag = "undo";
                ServerConnector.instance.SendWebSocketMessage(JsonUtility.ToJson(ms));
            }
        }
        
        Move lastMove = moves.Last();
        moves.RemoveAt(moves.Count - 1);

        GameObject couterForUndo = null;
        if (lastMove.BackToGame)
        {
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                couterForUndo = BackGamonBoard.instance
                    .GetThecouterByindex(lastMove.Itarget,lastMove.Jtarget);
                
                
            }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                couterForUndo = BackGamonBoard.instance
                    .GetThecouterByindex(lastMove.Itarget,lastMove.Jtarget);
                
            }

            
        }else if (lastMove.Reached)
        {
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                couterForUndo = BackGamonBoard.instance
                    .PlayerAFinalCouter.Last().gameObject;
                BackGamonBoard.instance
                    .PlayerAFinalCouter.RemoveAt(BackGamonBoard.instance
                        .PlayerAFinalCouter.Count-1);

            }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                couterForUndo = BackGamonBoard.instance
                    .PlayerBFinalCouter.Last().gameObject;
                BackGamonBoard.instance
                    .PlayerBFinalCouter.RemoveAt(BackGamonBoard.instance
                        .PlayerBFinalCouter.Count-1);
                
            }
            
            couterForUndo.transform.Rotate(couterForUndo.transform.right,-90);
            /*couterForUndo.transform.position =
                BackGamonBoard.instance.GetPositionBasedOnIndex(lastMove.Istart,lastMove.Jstart);*/
            print("t1");
            StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couterForUndo.transform,
                BackGamonBoard.instance.GetPositionBasedOnIndex(lastMove.Istart, lastMove.Jstart)));
        }
        else
        {
            couterForUndo= BackGamonBoard.instance.GetThecouterByindex(lastMove.Itarget, lastMove.Jtarget);
            StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couterForUndo.transform,
                BackGamonBoard.instance.GetPositionBasedOnIndex(lastMove.Istart, lastMove.Jstart)));
            
        }


      

        couterForUndo.GetComponent<CounterState>().i = lastMove.Istart;
        couterForUndo.GetComponent<CounterState>().j = lastMove.Jstart;

        if (lastMove.BackToGame)
        {
            couterForUndo.GetComponent<CounterState>().Hitted = true;

            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                CouterHitting.instance.PlayerAHittedCouters.Add(couterForUndo.transform);
             //   couterForUndo.transform.position = CouterHitting.instance.playerAGetFirstOUtPosition();
                StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couterForUndo.transform,
                    CouterHitting.instance.playerAGetFirstOUtPosition()));
            }
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                CouterHitting.instance.PlayerBHittedCouters.Add(couterForUndo.transform);
                StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couterForUndo.transform,
                    CouterHitting.instance.playerBGetFirstOUtPosition()));
            }
            
            BackGamonBoard.instance.Board[lastMove.Itarget, lastMove.Jtarget] = 0; 
        }else if (lastMove.Reached)
        {
            BackGamonBoard.instance.Board[lastMove.Istart, lastMove.Jstart] = GameStateManager.instance.PlayerNumber;
        }
        else
        {
            BackGamonBoard.instance.Board[lastMove.Istart, lastMove.Jstart] = GameStateManager.instance.PlayerNumber;
            BackGamonBoard.instance.Board[lastMove.Itarget, lastMove.Jtarget] = 0; 
        }
       
        
        DiceVisualizer.instance.DiceUndo(lastMove.moveStep);

        BackGamonBoard.instance.BoarDictionary[ BackGamonBoard.instance.GetKeyForRowANdCouterIndex( lastMove.Istart, lastMove.Jstart)] =couterForUndo;
        foreach (var VARIABLE in lastMove.moveStep)
        {
            Dice.instance.DiceMoves.Add(VARIABLE); 
        }
        if (MoveAndHitDictionary[lastMove]!=null)
        {
            Move lastMovehit = MoveAndHitDictionary[lastMove];
            MoveAndHitDictionary.Remove(lastMove);
            HittedMoves.RemoveAt(HittedMoves.Count - 1);
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                CouterHitting.instance.PlayerBHittedCouters.RemoveAt(CouterHitting.instance.PlayerBHittedCouters.Count-1);
                
            }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                CouterHitting.instance.PlayerAHittedCouters.RemoveAt(CouterHitting.instance.PlayerAHittedCouters.Count-1);
                
            }

            
            GameObject couterForUndohit = lastMovehit.Couter;
            couterForUndohit.GetComponent<CounterState>().Hitted = false;
            print("t1");
            StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couterForUndohit.transform,
                BackGamonBoard.instance.GetPositionBasedOnIndex(lastMovehit.Istart, lastMovehit.Jstart)));
            
            /*couterForUndohit.transform.position =
                BackGamonBoard.instance.GetPositionBasedOnIndex(lastMovehit.Istart,lastMovehit.Jstart);*/

            couterForUndohit.GetComponent<CounterState>().i = lastMovehit.Istart;
            couterForUndohit.GetComponent<CounterState>().j = lastMovehit.Jstart;

            BackGamonBoard.instance.Board[lastMovehit.Istart, lastMovehit.Jstart] = GameStateManager.instance.EnemyNumber;
            
        
            // nnDiceVisualizer.instance.DiceUndo(lastMovehit.moveSteps);

            BackGamonBoard.instance.BoarDictionary[ BackGamonBoard.instance.GetKeyForRowANdCouterIndex( lastMovehit.Istart, lastMovehit.Jstart)] =couterForUndohit;
            //Dice.instance.DiceMoves.Add(lastMovehit.moveSteps);
            // GameMaterialSource.instance.ResetPlayerMaterials();
        
            //BackGamonBoard.instance.findPlayAbleCouters();
        
            //  RemainingMoves.instance.CalculateRamainingMoves();
           
        }
        if (moves.Count == 0)
        {
            UndoBtn.SetActive(false);
        }

        if (Dice.instance.DiceMoves.Count != 0)
        {
            ConfirmBtn.SetActive(false);
        }
        GameMaterialSource.instance.ResetPlayerMaterials();
        
        BackGamonBoard.instance.findPlayAbleCouters();
        
        RemainingMoves.instance.CalculateRamainingMoves();
    }

    public void SendMoveToServer(int originI,int originJ,int targetI,int targetJ,List<int> step,string tag="")
    {
        ServerConnector.MoveObj move = new ServerConnector.MoveObj();
             move.tag = tag;
        
            int t = 0;
            foreach (var VARIABLE in step)
            {
                t += VARIABLE;
            }
       
           
            move.targetI =23- targetI;
            move.targetJ = targetJ;
            move.OriginI =23- originI;
            move.OriginJ = originJ;
            move.step = t;
            print($"sending {move.OriginI} to server");
        
       
        if (ServerConnector.instance.ws != null)
        {
            ServerConnector.instance.SendWebSocketMessage(JsonUtility.ToJson(move));
        }
    }

    public void recieveMoveToSelectedFromServer(int originI,int originalJ,int targetI,int targetJ,int step)
    {
        BackGamonBoard.instance.MoveToSelectedColumn(BackGamonBoard.instance.GetThecouterByindex(originI,originalJ
        ).GetComponent<CounterState>(),targetI,step);
    }

    public void recieveMoveFromServer(int targetColumn)
    {
        print("mvzv2");
        print("move recieved"+targetColumn);
        BackGamonBoard.instance.MoveCouter(targetColumn);
    }
    
    

    public Move LastHitMoveCach;
    public void CreateMove(int Istar,int jStart,int Itarget,int Jtarget,List<int> steps,bool hitted=false,bool Backtogame=false,bool Reached=false
        ,GameObject couter=null,string tag="")
    {
        if (GameStateManager.instance.OnlineGame)
        {
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                if (tag != "winstage")
                {
                    if(Reached)
                    {
                        ServerConnector.instance.SendWinstageMessageToServer(Istar, jStart);
                    }
                    else
                    {
                        SendMoveToServer(Istar, jStart, Itarget, Jtarget, steps, tag: tag);
                    }
                   
                }
                      
            }
        }
        else
        {
            if (tag != "winstage")
            {
                if (Reached)
                {
                    ServerConnector.instance.SendWinstageMessageToServer(Istar, jStart);
                }
                else
                {
                    SendMoveToServer(Istar, jStart, Itarget, Jtarget, steps, tag: tag);
                }
            }
        }
       
        Move newmove = new Move(Istar, jStart, Itarget, Jtarget, steps,hitted,Backtogame,reached:Reached, couter);
        if (hitted)
        {
           HittedMoves.Add(newmove); 
           LastHitMoveCach = newmove;
        }
        else
        {
            moves.Add(newmove);
            MoveConfirm.instance.MoveAndHitDictionary.Add(newmove,LastHitMoveCach);
            LastHitMoveCach = null;
        }
    }

    
    public class Move
    {
        public int Istart, Jstart, Itarget, Jtarget;
        public List<int> moveStep;
        public bool Hitted,BackToGame,Reached;
        public GameObject Couter;
        public Move(int istart, int jstart, int itarget, int jtarget, List<int> moveSteps,bool Hitted=false,bool backTogame=false,bool reached=false,GameObject couter=null)
        {
            this.Istart = istart;
            this.Jstart = jstart;
            this.Itarget = itarget;
            this.Jtarget = jtarget;
            this.moveStep = moveSteps;
            this.Hitted = Hitted;
            this.Couter = couter;
            this.BackToGame = backTogame;
            this.Reached = reached;

        }
    }
    
}
