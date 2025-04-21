using System.Collections;
using UnityEngine;

public class EnemyController : Controller
{
    TileManager tileManager{get{ return TileManager.Instance;} }
    [SerializeField] private Troop troop;

    public bool isPlaying {get ; private set;}
     public void Start()
    {
        troops = TileManager.Instance.enemyTroops;
    }



    public override IEnumerator TakeTurn()
    {
        yield return null;
       
            foreach(var t in troops)
            {

               yield return StartCoroutine(t.Value.TakeTurn());
            }
        
        
    }

}
