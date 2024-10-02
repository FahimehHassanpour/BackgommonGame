using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceVisualizer : Singleton<DiceVisualizer>
{
    public GameObject DiceImage;
    public Transform RightDicePanel, LeftDicePanel,RightDicePanelB, LeftDicePanelB;
    public Sprite s1,s2,s3,s4,s5,s6;
    private Dictionary<Image, int> leftDicePanelDic;
    private Dictionary<Image, int> RightDicePanelDic;


    private void Start()
    {
        InitDiceVisu(); 
       /* if(ServerConnector.instance != null)
        {
            print("st1");
            if(ServerConnector.instance.DiceA!=0)
            {
                print("st1");
                initDice.Add(ServerConnector.instance.DiceA);
                initDice.Add(ServerConnector.instance.DiceB);
                ServerConnector.instance.DiceA = 0;
                ServerConnector.instance.DiceB = 0;
                VisuaLizeDice(initDice);
            }
        }*/
    }


    public void InitDiceVisu(bool haidepanelFlag=false)
    {
        print("DiceInit");
        ReducedDiceImages = new List<Image>();
        leftDicePanelDic = new Dictionary<Image, int>();
        RightDicePanelDic = new Dictionary<Image, int>();
        if(haidepanelFlag)
        {
            HideDicePanels();
        }
       
    }

    private List<Image> ReducedDiceImages;
    private List<int> initDice;
    public void DiceUndo(List<int> move)
    {
        foreach (var VARIABLE in move)
        {
            var lastImage = ReducedDiceImages[ReducedDiceImages.Count - 1];
            lastImage.color = new Color(lastImage.color.r,
                lastImage.color.g,
                lastImage.color.b, 1);
            ReducedDiceImages.RemoveAt(ReducedDiceImages.Count - 1); 
        }
        
           
        

        /*foreach (var VARIABLE in leftDicePanelDic)
        {
            if (VARIABLE.Value == move)
            {
                if (VARIABLE.Key.GetComponent<Image>().color.a == 0.3f)
                {
                    VARIABLE.Key.GetComponent<Image>().color = new Color(VARIABLE.Key.GetComponent<Image>().color.r,
                        VARIABLE.Key.GetComponent<Image>().color.g,
                        VARIABLE.Key.GetComponent<Image>().color.b, 1);
                    return;
                }
            }
        }
        
        foreach (var VARIABLE in RightDicePanelDic)
        {
            if (VARIABLE.Value == move)
            {
                if (VARIABLE.Key.GetComponent<Image>().color.a == 0.3f)
                {
                    VARIABLE.Key.GetComponent<Image>().color = new Color(VARIABLE.Key.GetComponent<Image>().color.r,
                        VARIABLE.Key.GetComponent<Image>().color.g,
                        VARIABLE.Key.GetComponent<Image>().color.b, 1);
                    return;
                }
            }
        }*/
    }
    
    
    public void ReduceMoveFromDiceVisualization(int move)
    {
        foreach (var VARIABLE in leftDicePanelDic)
        {
            if (VARIABLE.Value == move)
            {
                if (VARIABLE.Key.GetComponent<Image>().color.a == 1.0f)
                {
                    VARIABLE.Key.GetComponent<Image>().color = new Color(VARIABLE.Key.GetComponent<Image>().color.r,
                        VARIABLE.Key.GetComponent<Image>().color.g,
                        VARIABLE.Key.GetComponent<Image>().color.b, 0.3f);
                    ReducedDiceImages.Add(VARIABLE.Key.GetComponent<Image>());
                    return;
                }
            }
        }
        
        foreach (var VARIABLE in RightDicePanelDic)
        {
            if (VARIABLE.Value == move)
            {
                if (VARIABLE.Key.GetComponent<Image>().color.a == 1.0f)
                {
                    VARIABLE.Key.GetComponent<Image>().color = new Color(VARIABLE.Key.GetComponent<Image>().color.r,
                        VARIABLE.Key.GetComponent<Image>().color.g,
                        VARIABLE.Key.GetComponent<Image>().color.b, 0.3f);
                    ReducedDiceImages.Add(VARIABLE.Key.GetComponent<Image>());
                    return;
                }
            }
        }
    }

    public void HideDicePanels()
    {
        LeftDicePanel.parent.gameObject.SetActive(false);
        LeftDicePanelB.parent.gameObject.SetActive(false);
    }
    
    
    public void VisuaLizeDice(List<int> dice)
    {
        //ffprint("dize visual");
        print("vizzz"+dice.Count);
        if (leftDicePanelDic.Count > 0)
        {
            foreach (var VARIABLE in leftDicePanelDic)
            {
                var image = VARIABLE.Key;
                
                Destroy(image.gameObject);
            }  
        }

        if (RightDicePanelDic.Count > 0)
        {
            foreach (var VARIABLE in RightDicePanelDic)
            {
                var image = VARIABLE.Key;
                
                Destroy(image.gameObject);
            }
        }
        RightDicePanelDic.Clear();
        leftDicePanelDic.Clear();
        
        initDice = dice;
        Transform left=null, right = null;
        
         //print(GameStateManager.instance.PlayerNumber);
         
        if (GameStateManager.instance.playerA)
        {
            LeftDicePanel.parent.gameObject.SetActive(true);
            left = LeftDicePanel;
            right = RightDicePanel;
        }
        else if(GameStateManager.instance.PlayerB)
        {
            LeftDicePanelB.parent.gameObject.SetActive(true);
            left = LeftDicePanelB;
            right = RightDicePanelB;
        }
        else
        {
            LeftDicePanel.parent.gameObject.SetActive(true);
            left = LeftDicePanel;
            right = RightDicePanel;
        }
        
        
        if (dice.Count > 2)
        {
            GameObject r1 = Instantiate(DiceImage,right);
            GameObject r2 = Instantiate(DiceImage,right);
            RightDicePanelDic.Add(r1.GetComponent<Image>(),dice[0]);
            RightDicePanelDic.Add(r2.GetComponent<Image>(),dice[0]);
            
            
            GameObject l1 = Instantiate(DiceImage,left);
            GameObject l2 = Instantiate(DiceImage,left);
            RightDicePanelDic.Add(l1.GetComponent<Image>(),dice[0]);
            RightDicePanelDic.Add(l2.GetComponent<Image>(),dice[0]);

            if (dice[0] == 1)
            {
                r1.GetComponent<Image>().sprite = s1;
                r2.GetComponent<Image>().sprite = s1;
                
                l1.GetComponent<Image>().sprite = s1;
                l2.GetComponent<Image>().sprite = s1;
            }else if (dice[0] == 2)
            {
                r1.GetComponent<Image>().sprite = s2;
                r2.GetComponent<Image>().sprite = s2;
                l1.GetComponent<Image>().sprite = s2;
                l2.GetComponent<Image>().sprite = s2;
            }else if (dice[0] == 3)
            {
                r1.GetComponent<Image>().sprite = s3;
                r2.GetComponent<Image>().sprite = s3;
                l1.GetComponent<Image>().sprite = s3;
                l2.GetComponent<Image>().sprite = s3;
            }else if (dice[0] == 4)
            {
                r1.GetComponent<Image>().sprite = s4;
                r2.GetComponent<Image>().sprite = s4;
                l1.GetComponent<Image>().sprite = s4;
                l2.GetComponent<Image>().sprite = s4;
            }else if (dice[0] == 5)
            {
                r1.GetComponent<Image>().sprite = s5;
                r2.GetComponent<Image>().sprite = s5;
                l1.GetComponent<Image>().sprite = s5;
                l2.GetComponent<Image>().sprite = s5;
            }else if (dice[0] == 6)
            {
                r1.GetComponent<Image>().sprite = s6;
                r2.GetComponent<Image>().sprite = s6;
                l1.GetComponent<Image>().sprite = s6;
                l2.GetComponent<Image>().sprite = s6;
            }
            
        }
        else
        {
            GameObject r1 = Instantiate(DiceImage,right);
            GameObject l1 = Instantiate(DiceImage,left);
            
            RightDicePanelDic.Add(r1.GetComponent<Image>(),dice[0]);
            leftDicePanelDic.Add(l1.GetComponent<Image>(),dice[1]);
            
            if (dice[0] == 6)
            {
                r1.GetComponent<Image>().sprite = s6;
                
            }else if (dice[0] == 5)
            {
                r1.GetComponent<Image>().sprite = s5;
                
            }else if (dice[0] == 4)
            {
                r1.GetComponent<Image>().sprite = s4;
                
            }else if (dice[0] == 3)
            {
                r1.GetComponent<Image>().sprite = s3;
                
            }else if (dice[0] == 2)
            {
                r1.GetComponent<Image>().sprite = s2;
                
            }else if (dice[0] == 1)
            {
                r1.GetComponent<Image>().sprite = s1;
                
            }
            
            
            if (dice[1] == 6)
            {
                l1.GetComponent<Image>().sprite = s6;
                
            }else if (dice[1] == 5)
            {
                l1.GetComponent<Image>().sprite = s5;
                
            }else if (dice[1] == 4)
            {
                l1.GetComponent<Image>().sprite = s4;
                
            }else if (dice[1] == 3)
            {
                l1.GetComponent<Image>().sprite = s3;
                
            }else if (dice[1] == 2)
            {
                l1.GetComponent<Image>().sprite = s2;
                
            }else if (dice[1] == 1)
            {
                l1.GetComponent<Image>().sprite = s1;
                
            }
            
            
            
        }
    }
    

}
