
using System;
using System.Collections.Generic;

using UnityEngine;

public class CouterHitting : Singleton<CouterHitting>
{
    public Transform PlayerAHiitingPos, PlayerBHittingPos;
    public List<Transform> PlayerAHittedCouters;
    public List<Transform> PlayerBHittedCouters;

    
    private void Start()
    {
        if (PlayerAHittedCouters == null)
        {
            PlayerAHittedCouters = new List<Transform>();
        }

        if (PlayerBHittedCouters == null)
        {
            PlayerBHittedCouters = new List<Transform>();
        }
       
    }

    public void AddToPlayerAhitted(Transform couter,bool recon=false)
    {
        if (!recon)
        {
            List<int> Step = new List<int>();
            Step.Add(0);
            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,couter.GetComponent<CounterState>().j,-1-PlayerBHittedCouters.Count,0,Step,true,false,couter:couter.gameObject);    
        }
        
        couter.GetComponent<CounterState>().i = -1;
        couter.GetComponent<CounterState>().j = 0;
        couter.GetComponent<CounterState>().Hitted = true;
        
       instance.PlayerAHittedCouters.Add(couter);
        print("player a hitted couters count:"+instance.PlayerAHittedCouters.Count);
        var pos = playerAGetFirstOUtPosition();
        if (!recon)
        {
            StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couter, pos));
        }
        else
        {
            couter.transform.position = pos;
        }
        
      //  couter.transform.position = pos;
    }
    public void AddToPlayerBhitted(Transform couter,bool recon=false)
    {
        if (!recon)
        {
            List<int> Step = new List<int>();
            Step.Add(0);
            MoveConfirm.instance.CreateMove(couter.GetComponent<CounterState>().i,couter.GetComponent<CounterState>().j,24+PlayerBHittedCouters.Count,0,Step,true,false,couter:couter.gameObject);  
        }
     
        couter.GetComponent<CounterState>().i = 24;
        couter.GetComponent<CounterState>().j = 0;
        
        couter.GetComponent<CounterState>().Hitted = true;
        
        PlayerBHittedCouters.Add(couter);
        
        var pos = playerBGetFirstOUtPosition();
        if (!recon)
        {
            StartCoroutine(BackGamonBoard.instance.Co_MoveCouter(couter, pos));
        }
        else
        {
            couter.position = pos;
        }
        
    }

    
    public Vector3 playerBGetFirstOUtPosition()
    {
        var position = PlayerBHittingPos.position;
        var height = PlayerBHittedCouters.Count;
        var pile = height / 5;
        height %= 5;
        //print(position+"  Bhitted");
        var pos = new Vector3(position.x,position.y- ((height+pile) * 0.8f),
            position.z-(pile*0.1f));
        return pos;
    }
    
    
    public Vector3 playerAGetFirstOUtPosition()
    {
        var position = PlayerAHiitingPos.position;
        //print(position+" Ahitted");
        var height = PlayerAHittedCouters.Count;
        var pile = height / 5;

        height %= 5;
        
        var pos = new Vector3(position.x,position.y+ ((height+pile) * 0.7f),
            position.z-(pile*0.1f));
        return pos;
    }


    /*private void Update()
    {
        print(instance.PlayerAHittedCouters.Count+"saq");
    }*/
}
