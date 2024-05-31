using System.Collections.Generic;
using UnityEngine;

public class BoxCollisionHandler : MonoBehaviour
{
    [SerializeField] private List<BoardBoxController> upperBoxList = new();

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out BoardBoxController boardBoxController))
        {
            if (!upperBoxList.Contains(boardBoxController) &&
                boardBoxController != transform.parent.GetComponent<BoardBoxController>())
                upperBoxList.Add(boardBoxController);
        }
    }

    public List<BoardBoxController> GetUpperBoxes()
    {
        // List<BoardBoxController> updatedList = new();
        // for (int i = 0; i < upperBoxList.Count; i++)
        // {
        //     if(ReferenceEquals(upperBoxList[i], null))
        //         updatedList.Add(upperBoxList[i]);
        // }
        return upperBoxList;
    }
}