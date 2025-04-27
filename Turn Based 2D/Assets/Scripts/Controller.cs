using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Controller : MonoBehaviour
{
    
    [SerializeField] protected List<Troop> troopList = new List<Troop>();

    

     
    public virtual  IEnumerator TakeTurn()
    {
        throw new System.NotImplementedException("TakeTurn not implemented in base class Controller");
    }
    public virtual void EndTurn()
    {
        
    }

    public List<Troop> GetTroopsList()
    {
        return troopList;
    }
}
