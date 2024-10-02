using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTouchManager : Singleton<GameTouchManager>
{
    // Start is called before the first frame update
    private LayerMask Board,couter,Drag,FinalStage;
    void Start()
    {
        FirstHitted = null;
        Board=LayerMask.GetMask("Board");
        couter=LayerMask.GetMask("couter");
        Drag=LayerMask.GetMask("Drag");
        FinalStage=LayerMask.GetMask("winBoard");
        CouterToDrag = TestCouter.transform;
        initYeular = CouterToDrag.transform.eulerAngles.y;
        initX = CouterToDrag.transform.eulerAngles.x;   
    }
   public float initYeular,initX;
    public delegate void SwordDeActive();

    public static event SwordDeActive OnSwordDeactive;
    
    
    public Transform CouterToDrag;
    public GameObject FirstHitted;
    // Update is called once per frame
    private bool Moved;
    public bool IgnorTouch,PlayerMoved;
    public GameObject TestCouter;
    
    void Update()
    {
        if(Dice.instance.DiceMoves.Count<1)
        {
            return;
        }
        if (GameStateManager.instance.gameState != GameStateManager.GameState.PlayerAMoveturn)
            return;
        if (IgnorTouch)
        {
            return;
        }
       
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            PlayerMoved = true;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            //click on Board
            if (touch.phase == TouchPhase.Began)
            {
             
                if (Physics.Raycast(ray, out RaycastHit hit,1000,Board))
                {
                    //print("board begin");
                    FirstHitted = hit.collider.gameObject;
                    
                }  
                
            }
            if (touch.phase == TouchPhase.Ended)
            {
                if(CouterToDrag!=null)
                {
                    CouterToDrag.transform.eulerAngles = new Vector3(initX, initYeular, CouterToDrag.transform.eulerAngles.z);
                }
                bool DragFinishedOutOfBoard=true;
                OnSwordDeactive();
                GameMaterialSource.instance.ActiveBoarPosStart = null;
                GameMaterialSource.instance.TrunOffColumnLights();
                if (Physics.Raycast(ray, out RaycastHit hit,1000,couter))
                {
                    if(hit.transform.GetComponent<CounterState>().Hitted && hit.transform.GetComponent<CounterState>().active)
                    {
                        if (!Moved)
                        {
                            print("back to game");
                            BackGamonBoard.instance.BringCouterBackTogame();    
                        }
                        else
                        {
                            //bsck couter to out Space
                        }
                    }
                }  
               
                if (Physics.Raycast(ray, out RaycastHit hitboard,1000,Board))
                {

                    DragFinishedOutOfBoard = false;
                    if(FirstHitted==null)
                        return;
                    if (FirstHitted == hitboard.collider.gameObject)
                    {
                        if (!Moved)
                        {
                            if((BackGamonBoard.instance.PlayAbleCouters.ContainsKey( (hitboard.transform.parent.GetComponent<BoarPosStart>().PosStart))))
                            {
                                //print("board end");
                                BackGamonBoard.instance.MoveCouter(hitboard.transform.parent.GetComponent<BoarPosStart>().PosStart);
                            }
                        }
                        else
                        {
                            print("t1");
                                CouterToDrag.transform.position = BackGamonBoard.instance.GetPositionBasedOnIndex(
                                CouterToDrag.GetComponent<CounterState>().i,
                                CouterToDrag.GetComponent<CounterState>().j);
                        }
                        

                        FirstHitted = null;
                    }
                    else
                    {
                        if (GameMaterialSource.instance.LandAbleColumns.Contains(hitboard.transform.parent
                                .GetComponent<BoarPosStart>().PosStart)
                            && CouterToDrag != null)
                        {

                            int MoveSteps = 0;
                            if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerAMoveturn)
                            {
                                MoveSteps=(hitboard.transform.parent.GetComponent<BoarPosStart>().PosStart-CouterToDrag.GetComponent<CounterState>().i);
                            }else if (GameStateManager.instance.gameState == GameStateManager.GameState.PlayerBMoveTurn)
                            {
                                MoveSteps=(CouterToDrag.GetComponent<CounterState>().i-hitboard.transform.parent.GetComponent<BoarPosStart>().PosStart);
                            }

                            if (!CouterToDrag.GetComponent<CounterState>().Hitted)
                            {
                                BackGamonBoard.instance.MoveToSelectedColumn(CouterToDrag.GetComponent<CounterState>(),hitboard.transform.parent
                                    .GetComponent<BoarPosStart>().PosStart,MoveSteps);    
                            }
                            else
                            {
                                BackGamonBoard.instance.BringBackCouterTOSelectedColumn(hitboard.transform.parent
                                    .GetComponent<BoarPosStart>().PosStart,CouterToDrag.GetComponent<CounterState>());
                            }
                            
                        
                        }
                        else
                        {
                            if (!Reached)
                            {
                                if (CouterToDrag != null)
                                {
                                    if (CouterToDrag.GetComponent<CounterState>().Hitted)
                                    {
                                        //print(GameStateManager.instance.gameState);
                                        CouterToDrag.transform.position = GameStateManager.instance.gameState ==
                                                                          GameStateManager.GameState.PlayerBMoveTurn
                                            ? CouterHitting.instance.playerBGetFirstOUtPosition()
                                            : CouterHitting.instance.playerAGetFirstOUtPosition();
                                    }
                                    else
                                    {
                                        print("t1");
                                        CouterToDrag.transform.position = BackGamonBoard.instance.GetPositionBasedOnIndex(
                                            CouterToDrag.GetComponent<CounterState>().i,
                                            CouterToDrag.GetComponent<CounterState>().j);  
                                    }  
                                }

                            }
                            
                        }
                    }
                    
                }
                if ((Physics.Raycast(ray, out RaycastHit FinalStagehit, 1000, FinalStage)))
                {
                    print("couter drop on winstage");
                    if (BackGamonBoard.instance.CheckIfCurrentPlayersAllCoutersReachedLastQuarter())
                    {
                        print("all couters in lastQ");
                        if (CouterToDrag != null && CouterToDrag.GetComponent<CounterState>().ifCanMoveToWinStageByDrag())
                        {
                            print("sending to winstageaa");
                            
                            DragFinishedOutOfBoard = false;
                           BackGamonBoard.instance.CouterToWinStage(CouterToDrag.GetComponent<CounterState>(),FinalStagehit.transform);
                        }
                    }
                    else
                    {
                        print("all couters Not in lastQ");
                    }
                }

                if (CouterToDrag != null)
                {
                    if (DragFinishedOutOfBoard)
                    {
                        if (CouterToDrag.GetComponent<CounterState>().Hitted)
                        {
                            CouterToDrag.transform.position = GameStateManager.instance.gameState ==
                                                              GameStateManager.GameState.PlayerBMoveTurn
                                ? CouterHitting.instance.playerBGetFirstOUtPosition()
                                : CouterHitting.instance.playerAGetFirstOUtPosition();
                        }
                        else
                        {
                            print("t1");
                            StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(CouterToDrag.transform,
                                BackGamonBoard.instance.GetPositionBasedOnIndex(
                                    CouterToDrag.GetComponent<CounterState>().i,
                                    CouterToDrag.GetComponent<CounterState>().j)));

                        }
                    }
                }
                
                //ss
                GameMaterialSource.instance.LandAbleColumns.Clear();
                CouterToDrag = null;
                Moved = false;
            }
            //click on couter

            
            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out RaycastHit hit,1000,couter))
                {
                   // print(hit.collider.name + "  " + hit.collider.GetComponent<CounterState>().active);
                    if (hit.collider.GetComponent<CounterState>().active)
                    {
                        CouterToDrag = hit.transform;
                        GameMaterialSource.instance
                            .VisualizeLandableColumns(BackGamonBoard.instance.GetLandableColumns(hit.collider.GetComponent<CounterState>().i));
                    }
                    if (hit.transform.GetComponent<CounterState>().Hitted)
                    {
                        FirstHitted = hit.collider.gameObject;
                    }
                    
                   
                }  
            }
            
            if (touch.phase == TouchPhase.Moved)
            {   
                
                
                Reached = false;
                if (Physics.Raycast(ray, out RaycastHit hit,1000,Drag))
                {
                   // print($"{CouterToDrag.gameObject.name}+ movz");
                    if (CouterToDrag != null)
                    {
                        CouterToDrag.transform.eulerAngles = new Vector3(initX - (touch.deltaPosition.y), initYeular - (touch.deltaPosition.x), CouterToDrag.transform.eulerAngles.z);
                        CouterToDrag.position = new Vector3(hit.point.x,hit.point.y+TouchCouterDelta,hit.point.z);
                        Moved = true;
                    }
                    
                }

                if (Physics.Raycast(ray, out RaycastHit boardDetect, 1000, Board))
                {
                   
                   // OnSwordDeactive();
                    if(GameMaterialSource.instance.LandAbleColumns.Contains(boardDetect.transform.parent.GetComponent<BoarPosStart>().PosStart) 
                       && CouterToDrag!=null)
                    {
                        print("a1");
                        boardDetect.transform.parent.GetComponent<BoarPosStart>().AboutToLand();
                    }
                    else
                    {
                        print("a2");
                        //GameMaterialSource.instance.ContinueVisualize();
                    }
                }

                if ((Physics.Raycast(ray, out RaycastHit FinalStagehit, 1000, FinalStage)))
                {
                    print("move on win stage");
                    if (BackGamonBoard.instance.CheckIfCurrentPlayersAllCoutersReachedLastQuarter())
                    {
                        if (CouterToDrag != null)
                        {
                            Reached = true;
                        }
                    }
                }

            }

        }
    }

    public bool Reached;
    public float TouchCouterDelta;
}
