
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    [SerializeField] Image healthbar;
    [SerializeField] private float health=100;
    [SerializeField] private float maxHealth=100;
    public void TakeDamage(int damage)
    {
        Debug.Log("taking Damage" + damage);
        health -= damage;
        healthbar.fillAmount =  (health/maxHealth);
        if(health <= 0)
        {
            GameManager.Instance.OnEnemyDead(GetComponent<Troop>());
        }
    }
}
