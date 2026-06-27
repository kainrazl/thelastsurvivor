using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Animator attackAnimator;
    private bool isActive = false;
    private Vector2 actualPosition;
    private PlayerManager player;
    private float actualDamage = 0;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
        actualDamage = GetComponentInParent<WeaponInstance>().GetProperties().damage;
    }

    private void Update()
    {
        if (isActive)
            HitEnemies();
    }

    public void Hit()
    {
        isActive = true;
        attackAnimator.SetTrigger("attack");
        actualPosition = player.transform.position + (player.isFacingLeft ? new Vector3(-1.318f, 0.12f, 0) : new Vector3(1.318f, 0.12f, 0));
        transform.parent.position = actualPosition;
        transform.parent.localScale = player.isFacingLeft ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
    }

    private void HitEnemies()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in enemyColliders)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyManager>().EnemyDamage(actualDamage);
            }
                
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void SetDamage(float damage)
    {
        actualDamage = damage;
    }
    
    public float GetDamage()
    {
        return actualDamage;
    }
}
