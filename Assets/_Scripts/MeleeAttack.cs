using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float radius;

    public void Hit()
    {
        Animator attackAnimator = GetComponentInParent<Animator>();
        attackAnimator.SetTrigger("attack");

        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in enemyColliders)
        {
            if (enemy.CompareTag("Enemy"))
                enemy.GetComponent<EnemyManager>().EnemyDamage(enemy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
