using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaterialSource : Singleton<GameMaterialSource>
{
    public Material GreenCouter,WhiteCouter,BlackCouter,Odd,Even,OddSelected,EvenSelected;

    public List<int> LandAbleColumns;

    public BoarPosStart ActiveBoarPosStart;
    public void ResetPlayerMaterials()
    {
//        ////print(CounterManager.instance.PlayerCounter.Count);
        foreach (var VARIABLE in CounterManager.instance.PlayerCounter)
        {
            VARIABLE.GetComponent<CounterState>().StopFlashing();
        }
        foreach (var VARIABLE in CounterManager.instance.OpponentCounter)
        {
            
            VARIABLE.GetComponent<CounterState>().StopFlashing();
        }
    }


    public void ContinueVisualize()
    {
        VisualizeLandableColumns(LandAbleColumns);
    }
    
    public void VisualizeLandableColumns(List<int> columns)
    {
        LandAbleColumns = new List<int>();
        LandAbleColumns = columns;
        foreach (var VARIABLE in columns)
        {
            //ll
            if (BackGamonBoard.instance.EnemyDetector(VARIABLE) == 1)
            {
                BackGamonBoard.instance.GetThecouterByindex(VARIABLE,0).GetComponent<CounterState>().ShowSword();
            }
            BackGamonBoard.instance.BoarTransformPoses[VARIABLE].GetComponent<BoarPosStart>().LightUp();
        }
    }

    public void TrunOffColumnLights()
    {
        
        foreach (var VARIABLE in BackGamonBoard.instance.BoarTransformPoses)
        {
           VARIABLE.GetComponent<BoarPosStart>().TurnOff();
        }
    }
    
    
}
