using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataStructure : MonoBehaviour
{
    private int[,] BoardDB;

    private void Start()
    {
        BoardDB = GetComponent<BackGamonBoard>().Board;
    }
    
    
    
    
    
}
