using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterManager : Singleton<CounterManager>
{
    public List<GameObject> PlayerCounter;
    public List<GameObject> OpponentCounter;
    public GameObject Board;
    public GameObject whitecouter, BlackCouter;
    public Transform couterGenerationPoint;
    public void CreatePlayerCouterPool()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject temp = Instantiate(whitecouter);
            temp.transform.position = couterGenerationPoint.position;
            PlayerCounter.Add(temp);
        }
    }
    
    public void CreateOpponentCouterPool()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject temp = Instantiate(BlackCouter);
            temp.transform.position = couterGenerationPoint.position;
            OpponentCounter.Add(temp);
        }
    }
    
    public void SetBoardStarPoses()
    {
        foreach (var VARIABLE in Board.GetComponentsInChildren<BoarPosStart>())
        {
            GetComponent<BackGamonBoard>().BoarTransformPoses[VARIABLE.PosStart] = VARIABLE.transform;
        }
    }

    public GameObject GetPlayerCounter()
    {
        foreach (var VARIABLE in PlayerCounter)
        {
            if (!VARIABLE.GetComponent<CounterState>().InGame)
            {
                VARIABLE.GetComponent<CounterState>().InGame = true;
                print("zzz");
                return VARIABLE;
            }
        }

        return null;
    }
    
    public GameObject GetOpponentCounter()
    {
        foreach (var VARIABLE in OpponentCounter)
        {
            if (!VARIABLE.GetComponent<CounterState>().InGame)
            {
                VARIABLE.GetComponent<CounterState>().InGame = true;
                return VARIABLE;
            }
        }

        return null;
    }
}
