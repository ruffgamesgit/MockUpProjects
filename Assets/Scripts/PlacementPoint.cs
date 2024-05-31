using UnityEngine;

public class PlacementPoint : MonoBehaviour
{
   public bool isOccupied;
   public BottleController bottle;
   public void SetOccupied(BottleController _bottle)
   {
      isOccupied = true;
      bottle = _bottle;
   }

   public void SetFree()
   {
      isOccupied = false;
      bottle = null;
   }

   public BottleController GetBottle()
   {
      return bottle;
   }
}
