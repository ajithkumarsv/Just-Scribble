using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Controller : MonoBehaviour
{
    
     [SerializeField] protected Dictionary<int2, Troop> troops = new Dictionary<int2, Troop>();


     
    public virtual IEnumerator TakeTurn()
    {
        yield return null;
    }
    public virtual void EndTurn()
    {
        
    }
}
