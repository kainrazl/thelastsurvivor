using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Animator attackAnimator;
    private bool isActive = false;

    private void Update()
    {
        if (isActive)
            HitEnemies();
    }

    public void Hit()
    {
        isActive = true;
        attackAnimator.SetTrigger("attack");
    }

    private void HitEnemies()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in enemyColliders)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float damage = GetComponentInParent<WeaponInstance>().GetProperties().damage;
                enemy.GetComponent<EnemyManager>().EnemyDamage(enemy, damage);
            }
                
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
