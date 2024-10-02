using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPosStart : MonoBehaviour
{
   public int PosStart;
   public MeshRenderer Renderer;


   public void AboutToLand()
   {
      if (GameMaterialSource.instance.ActiveBoarPosStart != null)
      {
         if (GameMaterialSource.instance.ActiveBoarPosStart != this)
         {
            GameMaterialSource.instance.ActiveBoarPosStart.LightUp();
            GameMaterialSource.instance.ActiveBoarPosStart = this;
         }
      }
      else
      {
         GameMaterialSource.instance.ActiveBoarPosStart = this;
      }
      
      Renderer.enabled = true;
      if (PosStart % 2 == 0)
      {
         Renderer.material = GameMaterialSource.instance.EvenSelected;
      }
      else
      {
         Renderer.material = GameMaterialSource.instance.OddSelected; 
      } 
   }
   
   
   
   public void LightUp()
   {
      Renderer.enabled = true;
      if (PosStart % 2 == 0)
      {
         Renderer.material = GameMaterialSource.instance.Even;
      }
      else
      {
         Renderer.material = GameMaterialSource.instance.Odd; 
      }
      
   }

   public void TurnOff()
   {
      Renderer.enabled = false;
   }
}
