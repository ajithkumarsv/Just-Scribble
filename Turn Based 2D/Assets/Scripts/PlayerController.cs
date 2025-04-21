using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerController : Controller
{
    TileManager tileManager{get{ return TileManager.Instance;} }
    [SerializeField] private Troop troop;

    public bool isPlaying {get ; private set;}
    public void Start()
    {
        troops = TileManager.Instance.playerTroops;
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
