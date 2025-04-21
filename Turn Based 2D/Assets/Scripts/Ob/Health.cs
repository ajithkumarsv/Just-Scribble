using JetBrains.Annotations;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health=100;
    [SerializeField] private int maxHealth=100;
    public void OnHit(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            
        }
    }
}
