using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class BackGamonBoard : Singleton<BackGamonBoard>
{
    public int[,] Board;

    public List<Transform> BoarTransformPoses;
    public Dictionary<string, GameObject> BoarDictionary;
    public int A, B;
    public List<Transform> PlayerAFinalCouter;
    public List<Transform> PlayerBFinalCouter;
    public Transform leftStartPos, RightStartPos;

    public int GetTheIndexOfLastCouterInColumn(int Column)
    {
        int count = 0;
        for (int i = 0; i < 15; i++)
        {
            if (Board[Column, i] != 0)
            {
                count++;
            }
        }

        return (count-1);
    }
    public Dictionary<int, int> PlayAbleCouters;
    public List<int> LandableColumns;
    private void Start()
    {
        LandableColumns = new List<int>();
        PlayerAFinalCouter = new List<Transform>();
        PlayerBFinalCouter = new List<Transform>();
    }

    List<Vector3> PathToMove=new List<Vector3>();

    [ContextMenu("MoveTest")]
    public void MoveTest()
    {
        Time.timeScale = 0.1f;
        StartCoroutine(  Co_MoveCouter(testCouter.transform, target.position));

    }

    

    public Transform testingCouter,target;

    

    Vector3 PointBetweenTwoPoints(Vector3 start, Vector3 end, float percent)
    {
        return (start + percent * (end - start));
    }
    public IEnumerator Co_MoveCouter(Transform couter, Vector3 targetPos)
    {
        /* float Start = Time.time;
         bool CanLerp = true;
         var originalPos = couter.position;
         couter.Rotate(couter.right,35);


         while (CanLerp)
         {
             float t = (Time.time - Start) / 0.5f;
             float Ht = t * 2;
             var HtReverse = 2 - Ht;
             float height = 0;

             if (Ht >= 1)
             {
                 height = Mathf.Lerp(0, 1, HtReverse);
             }
             else
             {
                  height = Mathf.Lerp(0, 1, Ht);
             }

             Vector3 lerp = Vector3.Lerp(originalPos, targetPos, t);
            // //print(height);
             lerp = new Vector3(lerp.x, lerp.y, lerp.z - height);

             couter.position = lerp;
             if (t > 1)
             {
                 CanLerp = false;
             }
             yield return null;

         }
         couter.Rotate(couter.right,-35);*/
        float t = Vector3.Distance(couter.position, targetPos);
        //print(t);
        t /= 10;
        t = Mathf.Clamp(t, 0.6f, 2);
        var pos = PointBetweenTwoPoints(couter.position, targetPos, 0.5f);
        pos += new Vector3(0, 0, -1.5f);



        LTSpline ltSpline = new LTSpline(new Vector3[] { couter.position, couter.position, pos, targetPos, targetPos });

        LeanTween.moveSpline(couter.gameObject, ltSpline, t).setEaseOutCubic().setOnComplete(() =>
        {
            couter.position = targetPos;
            SFX.instance.PlayCouter();
        });
       
        yield return null;  

    }

    public int FindTheSmallestColumnForPlayeA()
    {
        int res = 24;
        foreach (var VARIABLE in CounterManager.instance.PlayerCounter)
        {
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                if (VARIABLE.GetComponent<CounterState>().i == 25)
                {

                }
                else 
                {
                    if (VARIABLE.GetComponent<CounterState>().i<res)
                    {
                        res= VARIABLE.GetComponent<CounterState>().i;
                    }
                }
            }
        }
       // print(res+"lmnoq");
        return res; 
    }

    public  bool CheckIfCurrentPlayersAllCoutersReachedLastQuarter()
    {
        bool val=true;
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            foreach (var VARIABLE in CounterManager.instance.PlayerCounter )
            {
                if (VARIABLE.GetComponent<CounterState>().i == 25)
                {
                    
                }
                else if (VARIABLE.GetComponent<CounterState>().i < 18 || VARIABLE.GetComponent<CounterState>().i >23)
                {
                    val = false;
                }
            }
        }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            foreach (var VARIABLE in CounterManager.instance.OpponentCounter )
            {
                if (VARIABLE.GetComponent<CounterState>().i == -25)
                {
                    
                }
                else if (VARIABLE.GetComponent<CounterState>().i > 5 || VARIABLE.GetComponent<CounterState>().i < 0)
                {
                    val = false;
                }
            }  
        }

        return val;
    }
    int GetPlayerDirection()
    {
        int movingDirection;
        if (GameStateManager.instance.playerA)
        {
            movingDirection = 1;
        }
        else
        {
            movingDirection = -1;
        }

        return movingDirection;
    }

    public delegate void ClearCoutersAvailableDiceCache();
    public static event ClearCoutersAvailableDiceCache clear ;

    [ContextMenu("PlayAbleCouters")] 
    public void findPlayAbleCouters()
    {
        print("dcbug4");
        clear();
        if(BackGamonBoard.instance.PlayerAFinalCouter.Count>14) return;
        if(GameStateManager.instance.gameState==GameStateManager.GameState.GameFinished) return;
        if (GameStateManager.instance.PlayerNumber == 2) return;
        print("dcbug5");
        LandableColumns.Clear();

        int movingDirection = GetPlayerDirection();

        PlayAbleCouters = new Dictionary<int, int>();

        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            return;
            if (ServerConnector.instance != null)
            {
                if (GameStateManager.instance.OnlineGame)
                {
                    return;
                }
            }
            foreach (var couter in CouterHitting.instance.PlayerBHittedCouters)
            {
                couter.GetComponent<CounterState>().CanMoveIn = false;
            }
            
            if (CouterHitting.instance.PlayerBHittedCouters.Count > 0)
            {
                bool Blocked = true;
                //print("playerBCouter oustside");
                    foreach (var VARIABLE in Dice.instance.DiceMoves)
                    {
                        if (EnemyDetector(24-VARIABLE) < 2)
                        {
                            var couter = CouterHitting.instance.PlayerBHittedCouters.Last();
                            
                                couter.GetComponent<CounterState>().StartFlashing();
                                couter.GetComponent<CounterState>().CanMoveIn = true;
                                Blocked = false;
                            
                        }
                    }

                    if (Blocked)
                    {
                        MessageBoard.instance.blocked();
                    }
                    return;
            }
        }
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            print("player A");
            FindTheSmallestColumnForPlayeA();
            print(CouterHitting.instance.PlayerAHittedCouters.Count);
            if (CouterHitting.instance.PlayerAHittedCouters.Count > 0)
            {
                bool Blocked = true;
                //print("playerBCouter oustside");
                foreach (var couter in CouterHitting.instance.PlayerAHittedCouters)
                {
                    couter.GetComponent<CounterState>().CanMoveIn=false;
                }
                foreach (var VARIABLE in Dice.instance.DiceMoves)
                {
                    if (EnemyDetector(-1+VARIABLE) < 2)
                    {
                        print(CouterHitting.instance.PlayerAHittedCouters.Count);
                        var couter = CouterHitting.instance.PlayerAHittedCouters.Last();
                        print("couter foune");
                        print(CouterHitting.instance.PlayerAHittedCouters.Count);
                        couter.GetComponent<CounterState>().StartFlashing();
                        couter.GetComponent<CounterState>().CanMoveIn = true;
                        Blocked = false;
                    }
                }
                if (Blocked)
                {
                    MessageBoard.instance.blocked();
                }
                return;
            }
        }

        bool AllCoutersReachedLastState=CheckIfCurrentPlayersAllCoutersReachedLastQuarter();

        bool NoMoves = true;
        
        for (int i = 0; i <= 23; i++)
        {
            int count = 0;
            for (int j = 0; j < 15; j++)
            {
                if (Board[i, j] == GameStateManager.instance.PlayerNumber)
                {
                    print("playernumis" + GameStateManager.instance.PlayerNumber);
                    count++;
                }
            }
            
            if (count > 0 )
            {
                
                foreach (var VARIABLE in Dice.instance.DiceMoves)
                {
                    if (i+(VARIABLE*movingDirection)<=23 && i+(VARIABLE*movingDirection)>=0)
                    {
                        //now this move is inside the board
                        if(EnemyDetector(i + (VARIABLE*movingDirection)) < 2)
                        {
                            
                            //check if it doesnt lands on a 1+ opponent column
                            NoMoves = false;
                            if (!PlayAbleCouters.ContainsKey(i))
                            {
                                PlayAbleCouters.Add(i, count - 1);
                                GetThecouterByindex(i, count - 1).GetComponent<CounterState>().StartFlashing();
                                
                            }
                            
                        }
                        
                    }else if (AllCoutersReachedLastState)
                    {
                        var SmallestColumn = FindTheSmallestColumnForPlayeA();
                      //  print(SmallestColumn);
                        if (i <=(24-VARIABLE) || i==SmallestColumn )
                        {
                            if (!(i + (VARIABLE * movingDirection) <= 23 && i + (VARIABLE * movingDirection) >= 0))
                            {

                                NoMoves = false;
                                if (!PlayAbleCouters.ContainsKey(i))
                                {
                                    PlayAbleCouters.Add(i, count - 1);
                                    GetThecouterByindex(i, count - 1).GetComponent<CounterState>().StartFlashing();
                                    GetThecouterByindex(i, count - 1).GetComponent<CounterState>().availableMoves.Add(VARIABLE);
                                }
                                else
                                {
                                    GetThecouterByindex(i, count - 1).GetComponent<CounterState>().availableMoves.Add(VARIABLE);
                                }

                            }
                            else
                            {
                                if (EnemyDetector(i + (VARIABLE * movingDirection)) < 2)
                                {

                                    //check if it doesnt lands on a 1+ opponent column
                                    NoMoves = false;
                                    if (!PlayAbleCouters.ContainsKey(i))
                                    {
                                        PlayAbleCouters.Add(i, count - 1);
                                        GetThecouterByindex(i, count - 1).GetComponent<CounterState>().StartFlashing();
                                    }

                                }
                            }
                        }
                      


                    }
                }
                  
                
            }
        }

        if (NoMoves)
        {
            MessageBoard.instance.NoMoves();
        }
    }
    
    public List<int> GetLandableColumns(int column)
    {
        List<int> landingColumns = new List<int>();
        int movingDirection = GetPlayerDirection();
        int dicecach = 0;
        foreach (var VARIABLE in Dice.instance.DiceMoves)
        {
            if (column+(VARIABLE*movingDirection)<=23 && column+(VARIABLE*movingDirection)>=0)
            {
                //now this move is inside the board
                if(EnemyDetector(column + (VARIABLE*movingDirection)) < 2)
                {
                    //check if it doesnt lands on a 1+ opponent column
                    if (!landingColumns.Contains(column + (VARIABLE * movingDirection)))
                    {
                        landingColumns.Add(column + (VARIABLE*movingDirection)); 
                    }
                              
                }
                        
            }

            dicecach += VARIABLE;
            //checked if CounterState outsidee and ignor dice combination
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                if (CouterHitting.instance.PlayerAHittedCouters.Count > 0)
                {
                    continue;
                }
            }
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                if (CouterHitting.instance.PlayerBHittedCouters.Count > 0)
                {
                    continue;
                }
            }
            if (landingColumns.Count == 0) continue;
            
            if (column+(dicecach*movingDirection)<=23 && column+(dicecach*movingDirection)>=0)
            {
                //now this move is inside the board
                if (Dice.instance.DiceMoves.Count == 2)
                {
                    if(EnemyDetector(column + (dicecach*movingDirection)) < 2)
                    {
                        //check if it doesnt lands on a 1+ opponent column
                        if (!landingColumns.Contains(column + (dicecach * movingDirection)))
                        {
                            landingColumns.Add(column + (dicecach*movingDirection)); 
                        }
                              
                    }  
                }else if (Dice.instance.DiceMoves.Count > 2)
                {
                    
                    if(EnemyDetector(column + (dicecach*movingDirection)) < 2)
                    {
                        //check if it doesnt lands on a 1+ opponent column
                        if (!landingColumns.Contains(column + (dicecach * movingDirection)))
                        {
                            landingColumns.Add(column + (dicecach*movingDirection)); 
                        }
                              
                    }
                    else
                    {
                        return landingColumns;
                    }  
                    
                }
                
                        
            }
            
            
        }

        return landingColumns;
    }
    public void BringCouterBackTogame()
    {
        
        print("back to board function");
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            print("back to board"+CouterHitting.instance.PlayerAHittedCouters.Count);
            ServerConnector.instance.SendToServerBringCouterBackTogame();
            print("back to board"+CouterHitting.instance.PlayerAHittedCouters.Count);
            var couter = CouterHitting.instance.PlayerAHittedCouters.Last();
            //hitted couter can move into board again cause its green 
            if (couter.GetComponent<CounterState>().CanMoveIn)
            {
                foreach (var VARIABLE in Dice.instance.DiceMoves)
                {
                    var columnToTest = -1 + VARIABLE;
                    if (EnemyDetector(columnToTest) < 2)
                    {
                        if (EnemyDetector(columnToTest) < 1)
                        {
                            var enterRow = GetTheIndexOfLastCouterInColumn(columnToTest)+1;
                            // couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                            print("t1");
                            StartCoroutine(Co_MoveCouter(couter.transform,
                            GetPositionBasedOnIndex(columnToTest, enterRow)));
                            
                            CouterHitting.instance.PlayerAHittedCouters.Remove(couter);
                            List<int> Step = new List<int>();
                            Step.Add(VARIABLE);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(VARIABLE);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(VARIABLE);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }else if (EnemyDetector(columnToTest) == 1)
                        {
                            
                            CouterHitting.instance.AddToPlayerBhitted(GetThecouterByindex(columnToTest,0).transform);
                            var enterRow = 0;
                           // couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                           print("t1");
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                            CouterHitting.instance.PlayerAHittedCouters.Remove(couter);
                            List<int> Step = new List<int>();
                            Step.Add(VARIABLE);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(VARIABLE);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(VARIABLE);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }
                    }
                }
            }
        }
        
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            var couter = CouterHitting.instance.PlayerBHittedCouters.Last();
            //hitted couter can move into board again cause its green 
            if (couter.GetComponent<CounterState>().CanMoveIn || GameStateManager.instance.OnlineGame)
            {
                var reversedice = Dice.instance.DiceMoves;
                reversedice.Reverse();
                foreach (var VARIABLE in reversedice)
                {
                    var columnToTest = 24 - VARIABLE;
                    if (EnemyDetector(columnToTest) < 2)
                    {
                        if (EnemyDetector(columnToTest) < 1)
                        {
                           
                            var enterRow = GetTheIndexOfLastCouterInColumn(columnToTest)+1;
                            print("t1");
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                          //  couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                            List<int> Step = new List<int>();
                            Step.Add(VARIABLE);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            CouterHitting.instance.PlayerBHittedCouters.Remove(couter);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            CouterHitting.instance.PlayerAHittedCouters.Remove(couter);
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(VARIABLE);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(VARIABLE);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }

                            return;
                        }else if (EnemyDetector(columnToTest) == 1)
                        {
                            CouterHitting.instance.AddToPlayerAhitted(GetThecouterByindex(columnToTest,0).transform);
                            var enterRow = 0;
                            print("t1");
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                            CouterHitting.instance.PlayerBHittedCouters.Remove(couter);
                            List<int> Step = new List<int>();
                            Step.Add(VARIABLE);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            CouterHitting.instance.PlayerAHittedCouters.Remove(couter);
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(VARIABLE);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(VARIABLE);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }
                    }
                }
            }
        }
       
    }
    Vector3 getFinalStagePos()
    {
        Vector3 pos = new Vector3();
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            pos = FinalStage.instance.PlayerALastStage.transform.position+ new Vector3(0,PlayerAFinalCouter.Count*0.2f,0);
            
        }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            pos = FinalStage.instance.PlayerBLastStage.transform.position- new Vector3(0,PlayerBFinalCouter.Count*0.2f,0);
        }

        return pos;
    }
    public void BringBackCouterTOSelectedColumn(int TargetColumn,CounterState couter)
    {
        
      if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            ServerConnector.instance.SendToServerBringCouterBackToSelected(TargetColumn);
            
            //hitted couter   can move into board again cause its green 
            if (couter.GetComponent<CounterState>().CanMoveIn)
            {
                
                    var columnToTest = TargetColumn;
                    if (EnemyDetector(columnToTest) < 2)
                    {
                        if (EnemyDetector(columnToTest) < 1)
                        {
                            var enterRow = GetTheIndexOfLastCouterInColumn(columnToTest)+1;
                           // couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                           print("t1");
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                            CouterHitting.instance.PlayerAHittedCouters.Remove(couter.transform);
                            List<int> Step = new List<int>();
                            Step.Add((columnToTest)+1);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization((columnToTest)+1);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove((columnToTest)+1);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }
                        if (EnemyDetector(columnToTest) == 1)
                        {
                            
                            CouterHitting.instance.AddToPlayerBhitted(GetThecouterByindex(columnToTest,0).transform);
                            var enterRow = 0;
                           // couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                           print("t1");
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                            CouterHitting.instance.PlayerAHittedCouters.Remove(couter.transform);
                            
                            List<int> Step = new List<int>();
                            Step.Add(columnToTest+1);
                            
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(columnToTest+1);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(columnToTest+1);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }
                    }
                
            }
        }  if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            
            //hitted couter can move into board again cause its green 
            if (couter.GetComponent<CounterState>().CanMoveIn || GameStateManager.instance.OnlineGame)
            {
                
                    var columnToTest = TargetColumn;
                    if (EnemyDetector(columnToTest) < 2)
                    {
                        if (EnemyDetector(columnToTest) < 1)
                        {
                            var enterRow = GetTheIndexOfLastCouterInColumn(columnToTest)+1;
                           // couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                            CouterHitting.instance.PlayerBHittedCouters.Remove(couter.transform);
                            List<int> Step = new List<int>();
                            Step.Add(24-columnToTest);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization((24-columnToTest));
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(24-columnToTest);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }
                        if (EnemyDetector(columnToTest) == 1)
                        {
                            
                            CouterHitting.instance.AddToPlayerAhitted(GetThecouterByindex(columnToTest,0).transform);
                            var enterRow = 0;
                            print("t1");
                           // couter.transform.position = GetPositionBasedOnIndex(columnToTest, enterRow);
                            StartCoroutine(Co_MoveCouter(couter.transform,
                                GetPositionBasedOnIndex(columnToTest, enterRow)));
                            CouterHitting.instance.PlayerBHittedCouters.Remove(couter.transform);
                            List<int> Step = new List<int>();
                            Step.Add(24-columnToTest);
                            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,
                                couter.GetComponent<CounterState>().j,columnToTest,enterRow,Step,false,true);
                            
                            couter.GetComponent<CounterState>().i = columnToTest;
                            couter.GetComponent<CounterState>().j = enterRow;
                            couter.GetComponent<CounterState>().Hitted = false;
                            couter.GetComponent<CounterState>().CanMoveIn = false;
                            
                            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(24-columnToTest);
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveConfirm.instance.UndoBtn.SetActive(true);
                            }
                            
                            Board[columnToTest, enterRow] = GameStateManager.instance.PlayerNumber;

                            BoarDictionary[GetKeyForRowANdCouterIndex(columnToTest, enterRow)] = couter.gameObject;
                            Dice.instance.DiceMoves.Remove(24-columnToTest);
                            
                            GameMaterialSource.instance.ResetPlayerMaterials();
                            RemainingMoves.instance.CalculateRamainingMoves();
                            if (Dice.instance.DiceMoves.Count == 0)
                            {
                                GameStateManager.instance.MoveFinish();
                            }
                            else
                            {
                                findPlayAbleCouters();
                            }
                            
                            return;
                        }
                    }
                
            }
        } 
    }
    void reduceComBinationDice(int step)
    {
        if (Dice.instance.DiceMoves.Contains(step))
        {
            //remove this step directly
            DiceVisualizer.instance.ReduceMoveFromDiceVisualization(step);
            Dice.instance.DiceMoves.Remove(step);
        }
        else
        {
            if (Dice.instance.DiceMoves.Count > 2)
            {
                int loop = step / Dice.instance.DiceMoves[0];
                for (int i = 0; i < loop; i++)
                {
                    DiceVisualizer.instance.ReduceMoveFromDiceVisualization(Dice.instance.DiceMoves[0]);  
                    Dice.instance.DiceMoves.Remove(Dice.instance.DiceMoves[0]);
                }
                
                
            }else if (Dice.instance.DiceMoves.Count == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    DiceVisualizer.instance.ReduceMoveFromDiceVisualization( Dice.instance.DiceMoves[0]);  
                    Dice.instance.DiceMoves.Remove(Dice.instance.DiceMoves[0]);
                }
            }
        }
        
        
    }
    public void MoveToSelectedColumn(CounterState couter, int TargetColumn,int step)
    {
        if (GameStateManager.instance.gameState != GameStateManager.GameState.PlayerBMoveTurn &&
            GameStateManager.instance.gameState != GameStateManager.GameState.PlayerAMoveturn)
        {
            return;
        }

        List<int> moveSteps = new List<int>();
        string movesStr = "";
        /*foreach (var VARIABLE in Dice.instance.DiceMoves)
        {
            movesStr+=("dz"+VARIABLE);
        }
        //print(movesStr);*/
        if (Dice.instance.DiceMoves.Contains(step))
        {
            moveSteps.Add(step);
        }
        else
        {
            if (Dice.instance.DiceMoves.Count == 2)
            {
                foreach (var VARIABLE in Dice.instance.DiceMoves)
                {
                    moveSteps.Add(VARIABLE);
                }
            }
            else
            {
                int loop = step / Dice.instance.DiceMoves[0];

                for (int i = 0; i < loop; i++)
                {
                    moveSteps.Add(Dice.instance.DiceMoves[0]);
                }
            }
        }
        int playAbleRow = 0;
        if (EnemyDetector(TargetColumn) == 0)
        {
          playAbleRow=GetTheIndexOfLastCouterInColumn(TargetColumn)+1;
        }
        else
        {
            
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                CouterHitting.instance.AddToPlayerBhitted(GetThecouterByindex(TargetColumn ,
                    0).transform);
            }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                CouterHitting.instance.AddToPlayerAhitted(GetThecouterByindex(TargetColumn ,
                    0).transform);  
            } 
            print($"player A Hitted count:{CouterHitting.instance.PlayerAHittedCouters.Count} " +
                  $"and player B Hitted count:{CouterHitting.instance.PlayerBHittedCouters.Count} and state is {GameStateManager.instance.gameState}");
        }
        
        MoveConfirm.instance.CreateMove(couter.i,couter.j,TargetColumn,playAbleRow,steps: moveSteps,tag:"moveToSel");
        
        /*couter.transform.position =
            GetPositionBasedOnIndex(TargetColumn,playAbleRow);*/

        StartCoroutine(Co_MoveCouter(couter.transform, GetPositionBasedOnIndex(TargetColumn, playAbleRow)));
        
        removeCouterFromItsPos(couter);
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            MoveConfirm.instance.UndoBtn.SetActive(true);
        }
        
        couter.i = TargetColumn;
        couter.j = playAbleRow;

        Board[couter.i, couter.j] = GameStateManager.instance.PlayerNumber;

        BoarDictionary[GetKeyForRowANdCouterIndex(couter.i, couter.j)] = couter.gameObject;
        //Dice.instance.DiceMoves.Remove(step);
        reduceComBinationDice(step);
        GameMaterialSource.instance.ResetPlayerMaterials();
        RemainingMoves.instance.CalculateRamainingMoves();
        if (Dice.instance.DiceMoves.Count == 0)
        {
            GameStateManager.instance.MoveFinish();
        }
        else
        {
            findPlayAbleCouters();
        }
    }
    public void MoveCouter(int TargetColumn)
    {
        print("mvzv3");
        if (GameStateManager.instance.gameState != GameStateManager.GameState.PlayerBMoveTurn &&
            GameStateManager.instance.gameState != GameStateManager.GameState.PlayerAMoveturn)
        {
            return;
        }
        print("mvzv4");
        int Direction = 1;

        if (GameStateManager.instance.PlayerB)
        {
            Direction = -1;
            
        }
        
        
        CounterState couter = null;
        int playAbleRow = GetTheIndexOfLastCouterInColumn(TargetColumn);
        
        bool attack = false;
        print("gola" + TargetColumn + " " + playAbleRow);
        couter = GetThecouterByindex(TargetColumn, playAbleRow).GetComponent<CounterState>();
        removeCouterFromItsPos(couter);
        int step = 0;
        bool MoveOut=false;
        foreach (var VARIABLE in Dice.instance.DiceMoves)
        {
            
            if (VARIABLE > step)
            {
                if (EnemyDetector(TargetColumn + (VARIABLE * Direction)) == 0)
                {
                    attack = false;
                    step = VARIABLE;
                    //no enemy for this move
                }else if (EnemyDetector(TargetColumn + (VARIABLE * Direction)) == 1)
                {
                    step = VARIABLE;
                    attack = true;
                   
                    //one enemy lets hit him
                }else if (EnemyDetector(TargetColumn + (VARIABLE * Direction)) >= 2)
                {
                    
                    attack = false;
                    //blocked by enemy
                }else if (EnemyDetector(TargetColumn + (VARIABLE * Direction)) == -1)
                {
                    if (CheckIfCurrentPlayersAllCoutersReachedLastQuarter())
                    {
                        print("all players in last stage");
                        if (couter.availableMoves.Contains(VARIABLE) || GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
                        {
                            print("couter has legit move");
                            int targetColumn = TargetColumn + (VARIABLE * Direction);
                            print($"touched column is{TargetColumn} and landing index is {targetColumn}");
                            if (targetColumn > 23 || targetColumn < 0)
                            {
                                step = VARIABLE;
                                MoveOut = true;
                            }
                        }


                    }
                }
            }

            
        }

        if (attack)
        {
            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
            {
                CouterHitting.instance.AddToPlayerBhitted(GetThecouterByindex(TargetColumn + (step * Direction),
                    GetTheIndexOfLastCouterInColumn(TargetColumn + (step * Direction))).transform);
            }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
            {
                CouterHitting.instance.AddToPlayerAhitted(GetThecouterByindex(TargetColumn + (step * Direction),
                    GetTheIndexOfLastCouterInColumn(TargetColumn + (step * Direction))).transform);  
            }
        }

        
        
        
        DiceVisualizer.instance.ReduceMoveFromDiceVisualization(step);
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            MoveConfirm.instance.UndoBtn.SetActive(true);
        }
       


        int targetJ = 0;
        int targetI = TargetColumn + (step*Direction);
        if (!MoveOut)
        {
            targetJ= GetTheIndexOfLastCouterInColumn(TargetColumn + (step*Direction)) + 1; 
        }
        else
        {
            targetI = 25*GetPlayerDirection();
            targetJ = getRowForWinGameStage();
        }
        
       
        if (attack) targetJ = 0;
        List<int> moves = new List<int>();
        moves.Add(step);
        
        MoveConfirm.instance.CreateMove(TargetColumn,GetTheIndexOfLastCouterInColumn(TargetColumn)+1,targetI,targetJ,moves,Reached:MoveOut
            ,tag: "moveToSel");

        if (MoveOut)
        {
            AddCouterToWinStage(couter.transform);
            //couter.transform.position = getFinalStagePos();
            if (PlayerAFinalCouter.Count == 15)
            {

            }
            else
            {
                StartCoroutine(Co_MoveCouter(couter.transform, getFinalStagePos()));
                couter.transform.Rotate(couter.transform.right, 90);
            }

           
        }
        else
        {
            print("t1");
            StartCoroutine(Co_MoveCouter(couter.transform, GetPositionBasedOnIndex(targetI,targetJ)));  
        }
        

        couter.i = targetI;
        couter.j = targetJ;
        if (!MoveOut)
        {
            Board[couter.i, couter.j] = GameStateManager.instance.PlayerNumber;
        }
        

        print("mvzv5");
        BoarDictionary[GetKeyForRowANdCouterIndex(couter.i, couter.j)] = couter.gameObject;
        Dice.instance.DiceMoves.Remove(step);
        GameMaterialSource.instance.ResetPlayerMaterials();
        RemainingMoves.instance.CalculateRamainingMoves();
        if (Dice.instance.DiceMoves.Count == 0)
        {
           GameStateManager.instance.MoveFinish();
        }
        else
        {
            findPlayAbleCouters();
        }
    }
    public Animator CameraAnim,finalcouterAnimator;
    void AddCouterToWinStage(Transform couter)
    {
        
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            
            PlayerAFinalCouter.Add(couter);
            
            print(PlayerAFinalCouter.Count);
            if(PlayerAFinalCouter.Count==15)
            {
                FinalAnim(couter.gameObject);

            }
        }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            PlayerBFinalCouter.Add(couter);
        }
        ServerConnector.instance.SendWinstageToServer();
    }
    int  getRowForWinGameStage()
    {
        int val = 0;
        if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
        {
            val = PlayerAFinalCouter.Count;
        }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
        {
            val = PlayerBFinalCouter.Count;
        }

        return val;
    }

    public void WinstageLoader(CounterState couter,bool player)
    {
        print("setting win stage");
       //bb AddCouterToWinStage(couter.transform);
       if (player)
       {
           PlayerAFinalCouter.Add(couter.transform);
       }
       else
       {
           PlayerBFinalCouter.Add(couter.transform);
       }
       couter.transform.position=player?  FinalStage.instance.PlayerALastStage.transform.position+ new Vector3(0,PlayerAFinalCouter.Count*0.2f,0): FinalStage.instance.PlayerBLastStage.transform.position- new Vector3(0,PlayerBFinalCouter.Count*0.2f,0);       
       couter.transform.Rotate(couter.transform.right,90);
       couter.i =player ? 25 : -25;
       couter.j = player ? PlayerAFinalCouter.Count : PlayerBFinalCouter.Count;
       
       BoarDictionary[GetKeyForRowANdCouterIndex(couter.i, couter.j)] = couter.gameObject;
    }
    public Transform FinalCheckpoint,testCouter,WinstagePos;


    [ContextMenu("FinalTest")]

    public void test()
    {
       // Time.timeScale = 0.5f;
        FinalAnim(TestingCouter);
    }
    public GameObject TestingCouter;

    public void FinalAnim(GameObject finalCouter)
    {
        MoveConfirm.instance.ConfirmBtn.gameObject.SetActive(false);
        var couterTo = finalCouter;
        print(finalcouterAnimator.transform.position);
        LeanTween.move(couterTo.gameObject, finalcouterAnimator.transform.position, 0.8f).setOnComplete(() =>
        {
            couterTo.gameObject.SetActive(false);
            finalcouterAnimator.gameObject.SetActive(true);
            CameraAnim.SetTrigger("final");
            finalcouterAnimator.SetTrigger("final");
        });
    
    }



    public void CouterToWinStage(CounterState couter,Transform WinStage)
    {
        bool NoFinal = true;
        foreach (var VARIABLE in Dice.instance.DiceMoves)
        {
            var dir = GetPlayerDirection();
            if (couter.i + (dir * VARIABLE) < 0 || couter.i + (dir * VARIABLE) > 23)
            {
                if (WinStage.CompareTag(couter.tag))
                {
                    NoFinal = false;
                    //move couter to final stage
                    print("sending win stagezzzz");
                    ServerConnector.instance.SendWinstageMessageToServer(couter.i,couter.j);
                    
                    DiceVisualizer.instance.ReduceMoveFromDiceVisualization(VARIABLE);
                    if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                    {
                        MoveConfirm.instance.UndoBtn.SetActive(true);
                    }
                    
                    removeCouterFromItsPos(couter);
                    
                    AddCouterToWinStage(couter.transform);
                    List<int> moves = new List<int>();
                    moves.Add(VARIABLE);
                    
                    MoveConfirm.instance.CreateMove(couter.i,couter.j,25*GetPlayerDirection(),0,steps: moves,Reached:true,tag:"winstage");

                    // couter.transform.position = getFinalStagePos();
                    if(PlayerAFinalCouter.Count==15)
                    {

                    }
                    else
                    {
                        StartCoroutine(Co_MoveCouter(couter.transform, getFinalStagePos()));
                        couter.transform.Rotate(couter.transform.right, 90);
                    }
                    
                    //print("here");
                    couter.i = 25*GetPlayerDirection();
                    couter.j = getRowForWinGameStage();
                    
                    BoarDictionary[GetKeyForRowANdCouterIndex(couter.i, couter.j)] = couter.gameObject;
                    
                    Dice.instance.DiceMoves.Remove(VARIABLE);
                    GameMaterialSource.instance.ResetPlayerMaterials();
                    RemainingMoves.instance.CalculateRamainingMoves();
                    if (Dice.instance.DiceMoves.Count == 0)
                    {
                        GameStateManager.instance.MoveFinish();
                    }
                    else
                    {
                        findPlayAbleCouters();
                    }
                    
                    break;
                }
                else
                {
                    print("t1");
                  //  StartCoroutine(Co_MoveCouter(couter.transform, GetPositionBasedOnIndex(couter.i, couter.j)));
                    couter.transform.position = GetPositionBasedOnIndex(couter.i, couter.j);
                }
            }   
           
        }
        if(NoFinal)
        {
            print("t2");
            couter.transform.position = GetPositionBasedOnIndex(couter.i, couter.j);
        }
    }
    
    public void ServerOrderCouterToWinStage(int column,int row)
    {
        print($"column is {column} and row is {row}");
        var couter = GetThecouterByindex(column, row).GetComponent<CounterState>();
        
        foreach (var VARIABLE in Dice.instance.DiceMoves)
        {
            var dir = GetPlayerDirection();
            if (couter.i + (dir * VARIABLE) < 0 || couter.i + (dir * VARIABLE) > 23)
            {
                   
                    //move couter to final stage
                    DiceVisualizer.instance.ReduceMoveFromDiceVisualization(VARIABLE);
                    if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                    {
                        MoveConfirm.instance.UndoBtn.SetActive(true);
                    }
                    
                    removeCouterFromItsPos(couter);
                    
                    AddCouterToWinStage(couter.transform);
                    List<int> moves = new List<int>();
                    moves.Add(VARIABLE);

                    
                        MoveConfirm.instance.CreateMove(couter.i,couter.j,25*GetPlayerDirection(),0,steps: moves,Reached:true,tag:"winstage");    
                    
                    

                    // couter.transform.position = getFinalStagePos();
                    
                    StartCoroutine(Co_MoveCouter(couter.transform, getFinalStagePos()));
                    couter.transform.Rotate(couter.transform.right,90);
                    //print("here");
                    couter.i = 25*GetPlayerDirection();
                    couter.j = getRowForWinGameStage();
                    
                    BoarDictionary[GetKeyForRowANdCouterIndex(couter.i, couter.j)] = couter.gameObject;
                    
                    Dice.instance.DiceMoves.Remove(VARIABLE);
                    GameMaterialSource.instance.ResetPlayerMaterials();
                    RemainingMoves.instance.CalculateRamainingMoves();
                    if (Dice.instance.DiceMoves.Count == 0)
                    {
                        GameStateManager.instance.MoveFinish();
                    }
                    else
                    {
                        findPlayAbleCouters();
                    }
                    
                    break;
            }
           
        }
        
    }
    
    public void removeCouterFromItsPos(CounterState couter)
    {
        Board[couter.i, couter.j] = 0;
        BoarDictionary[GetKeyForRowANdCouterIndex(couter.i, couter.j)] = null;
        
    }
    public int EnemyDetector(int column)
    {
        
        if (column < 0 || column > 23)
        {
            return -1;
        }
        
        int count = 0;
        for (int i = 0; i < 15; i++)
        {
            if (Board[column, i] != GameStateManager.instance.EnemyNumber)
                return count;
            count++;
        }

        return count;
    }
    public GameObject GetThecouterByindex(int coloumn, int row)
    {
        BoarDictionary.TryGetValue(GetKeyForRowANdCouterIndex(coloumn,row), out GameObject value);
        return value;
    }

    
    public void SetBoard()
    {
        BoarDictionary = new Dictionary<string, GameObject>();
        Board = new int[24,15];
        /*for (int i = 0; i < 0; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    Board[i, j] = 1;
                }
            }else if (i == 11)
            {
                for (int j = 0; j < 5; j++)
                {
                    Board[i, j] = 1;
                }
            }else if (i == 16)
            {
                for (int j = 0; j < 3; j++)
                {
                    Board[i, j] = 1;
                }
            }else if (i == 18)
            {
                for (int j = 0; j < 5; j++)
                {
                    Board[i, j] = 1;
                }
            }else if (i == 5)
            {
                for (int j = 0; j < 5; j++)
                {
                    Board[i, j] = 2;
                }
            }else if (i == 7)
            {
                for (int j = 0; j < 3; j++)
                {
                    Board[i, j] = 2;
                }
            }else if (i == 12)
            {
                for (int j = 0; j < 5; j++)
                {
                    Board[i, j] = 2;
                }
            }else if (i == 23)
            {
                for (int j = 0; j < 2; j++)
                {
                    Board[i, j] = 2;
                }
            }
        }*/

        Board = ServerConnector.instance.ServerBoard;
        ServerConnector.GameState state = new ServerConnector.GameState();
       // state.brd = Board;
        StartCoroutine(visualizeBoard());

    }
    
    public void SetBoardForEndStageTest()
    {
        BoarDictionary = new Dictionary<string, GameObject>();
        Board = new int[24,15];
        for (int i = 0; i < 24; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < 2; j++)
                {
                   // Board[i, j] = 2;
                }
            }else if (i == 11)
            {
                for (int j = 0; j < 5; j++)
                {
                   // Board[i, j] = 1;
                }
            }else if (i == 16)
            {
                for (int j = 0; j < 3; j++)
                {
                   // Board[i, j] = 1;
                }
            }else if (i == 18)
            {
                for (int j = 0; j < 5; j++)
                {
                   // Board[i, j] = 1;
                }
            }
            else if (i == 21)
            {
                for (int j = 0; j < 1; j++)
                {
                     Board[i, j] = 1;
                }
            }
            else if (i == 19)
            {
                for (int j = 0; j < 1; j++)
                {
                     Board[i, j] = 1;
                }
            }else if (i == 5)
            {
                for (int j = 0; j < 1; j++)
                {
                    Board[i, j] = 2;
                }
            }else if (i == 3)
            {
                for (int j = 0; j < 1; j++)
                {
                    Board[i, j] = 2;
                }
            }else if (i == 12)
            {
                for (int j = 0; j < 5; j++)
                {
                   // Board[i, j] = 2;
                }
            }else if (i == 23)
            {
                for (int j = 0; j < 2; j++)
                {
                    //Board[i, j] = 1;
                }
            }
        }
        visualizeBoard();
        
    }
    void AssignPosToCouter(CounterState couter, int i, int j)
    {
        couter.i = i;
        couter.j = j;
    }
    public string GetKeyForRowANdCouterIndex(int row, int couter)
    {
        return row + "-" + couter;
    }
    IEnumerator visualizeBoard()
    {
        
        ////print("visualizing");
        for (int i = 0; i < 24; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                
                if (Board[i, j] == 0)
                {
                    break;
                }
                //
                ////print("--"+i+"--"+j);
                GameObject temp = null;
                if (Board[i, j] == 1)
                {
                    temp= GetComponent<CounterManager>().GetPlayerCounter();
                }
                 

                if ((Board[i, j] == 2))
                {
                    temp= GetComponent<CounterManager>().GetOpponentCounter();
                    temp.GetComponent<CounterState>().Opponent = true;
                }
                  
                BoarDictionary.Add(GetKeyForRowANdCouterIndex(i,j),temp);
                if (ServerConnector.instance.Resume)
                {
                    print("t1");
                    temp.transform.position = GetPositionBasedOnIndex(i, j);
                    AssignPosToCouter(temp.GetComponent<CounterState>(),i,j);
                }
                else
                {

                    if (i < 6)
                    {
                        temp.transform.position = RightStartPos.position;
                    }else if (i < 12)
                    {
                        temp.transform.position = leftStartPos.position;
                    }else if (i < 18)
                    {
                        temp.transform.position = leftStartPos.position;
                    }else if (i < 24)
                    {
                        temp.transform.position = RightStartPos.position;
                    }
                    //l
                    yield return null;
                    
                    StartCoroutine(Co_MoveCouter(temp.transform, GetPositionBasedOnIndex(i, j)));
                
                    AssignPosToCouter(temp.GetComponent<CounterState>(),i,j);

                    yield return new WaitForSeconds(0.02f); 
                }
            }
            
        }

        
        while (ServerConnector.instance.numberOfhitted > 0 || ServerConnector.instance.OppnumberOfhitted > 0
               ||ServerConnector.instance.numberOfStage > 0||ServerConnector.instance.OppnumberOfStage > 0)
        {
            if (ServerConnector.instance.numberOfhitted > 0)
            {
                var temp= GetComponent<CounterManager>().GetPlayerCounter();
                print(CounterManager.instance.PlayerCounter.Count);
                CouterHitting.instance.AddToPlayerAhitted(temp.transform,true);
                
                ServerConnector.instance.numberOfhitted--;
            }
            if (ServerConnector.instance.OppnumberOfhitted > 0)
            {
                var temp= GetComponent<CounterManager>().GetOpponentCounter();
                CouterHitting.instance.AddToPlayerBhitted(temp.transform,true);
                ServerConnector.instance.OppnumberOfhitted--;
            }
            if (ServerConnector.instance.numberOfStage > 0)
            {
                var temp= GetComponent<CounterManager>().GetPlayerCounter();
                WinstageLoader(temp.GetComponent<CounterState>(),player:true);
                ServerConnector.instance.numberOfStage--;
            }
            if (ServerConnector.instance.OppnumberOfStage > 0)
            {
                var temp= GetComponent<CounterManager>().GetOpponentCounter();
                WinstageLoader(temp.GetComponent<CounterState>(),player:false);
                ServerConnector.instance.OppnumberOfStage--;
            }  
        }


        DiceVisualizer.instance.InitDiceVisu();
        if (!ServerConnector.instance.Resume)
        {
            GameStateManager.instance.GameStartdice();  
        }
        
        ServerConnector.instance.declareEnteringGame();
        
    }
    
    public  Vector3 GetPositionBasedOnIndex(int i, int j)
    {
        print($"i is {i} and j is{j}");
        
        float dir = 0;
        if (i < 12)
        {
            dir = -0.71f;
        }
        else
        {
            dir = .71f;
        }

        var pile = j / 5;
        j %= 5;
//        //print($"{pile} is pile --{j} is j");
        
            return new Vector3(BoarTransformPoses[i].transform.position.x,
                BoarTransformPoses[i].transform.position.y + (dir*(j+pile)), BoarTransformPoses[i].transform.position.z-(pile*0.3f));
        
        
        
        
        
    }
    
    
}
