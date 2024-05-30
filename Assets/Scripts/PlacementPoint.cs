using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPoint : MonoBehaviour
{
   public bool isOccupied;

   public void SetOccupied()
   {
      isOccupied = true;
   }

   public void SetFree()
   {
      isOccupied = false;
   }
}
