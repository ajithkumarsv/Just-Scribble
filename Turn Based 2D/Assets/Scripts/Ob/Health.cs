using JetBrains.Annotations;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health=100;
    [SerializeField] private int maxHealth=100;
    public void TakeDamage(int damage)
    {
        Debug.Log("taking Damage" + damage);
        health -= damage;
        if(health <= 0)
        {
            GameManager.Instance.OnEnemyDead(GetComponent<Troop>());
        }
    }
}
