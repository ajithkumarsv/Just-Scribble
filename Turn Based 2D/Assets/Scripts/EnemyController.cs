using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class EnemyController : Controller
{
    TileManager tileManager { get { return TileManager.Instance; } }
    [SerializeField] private Troop troop;

    public bool isPlaying { get; private set; }
    public IEnumerator Start()
    {
        yield return null;
        troopList = TileManager.Instance.enemyTroops.ToList().Select(x => x.Value).ToList();
         Debug.Log("Enemy Count  after start " +troopList.Count);
        // troopQueuue = new Queue<Troop>(troopList);
    }




    public override IEnumerator TakeTurn()
    {
       
        for(int i = 0; i < troopList.Count; i++)
        {
            if (troopList[i] == null)
            {
                troopList.RemoveAt(i);
                i--;
            }
        }
        
        yield return null;
        foreach (var t in troopList)
        {
            Debug.Log("Player Turn");
            if (t != null)
                yield return StartCoroutine(t.TakeTurn());
        }


    }

}
