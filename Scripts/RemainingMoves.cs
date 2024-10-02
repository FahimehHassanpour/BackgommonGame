using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingMoves : Singleton<RemainingMoves>
{
    
    public int RemainingA, RemainigB;
    public BoardCounter RemainingAText, RemainingBText;
    [ContextMenu("remaining moves")]
    public int CalculateRamainingMoves()
    {
        RemainigB = 0;
        RemainingA = 0;
        foreach (var VARIABLE in CounterManager.instance.PlayerCounter)
        {
            int couterColumn = VARIABLE.GetComponent<CounterState>().i;
            if (couterColumn == 25)
            {
                couterColumn = 24;
            }
            RemainingA += (24 - couterColumn);
            RemainingAText.TurnNumberToImage(RemainingA);
            
        }
        foreach (var VARIABLE in CounterManager.instance.OpponentCounter)
        {
            int couterColumn = VARIABLE.GetComponent<CounterState>().i;
            if (couterColumn == -25)
            {
                couterColumn = -1;
            }
            RemainigB +=  couterColumn+1;
            RemainingBText.TurnNumberToImage(RemainigB); 
            
        }
        return RemainingA;  
        /*if (RemainingA==0)
        {
            MessageBoard.instance.GameFinished("Player A Won");
        }
        if (RemainigB == 0)
        {
            MessageBoard.instance.GameFinished("Player B Won");
        }*/
        
    }
    
    
    
}
