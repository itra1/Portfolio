using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script removes table representation from list, bcos all objects in listdestroyed when you refresh list

public class NullTableRepresentation : MonoBehaviour
{
    [SerializeField] Transform thatTable;
    public void SetUp()
    {
        thatTable.gameObject.SetActive(false);
        
        thatTable.SetParent(thatTable.parent.parent);
    }
}
