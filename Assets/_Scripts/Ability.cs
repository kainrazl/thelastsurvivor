using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AbilityType
{
    None,
    Repel,
    Slow,
    Damage
}

public class Ability
{
    private string abilityName;
    private AbilityType abilityType;
    private float power;
    private float radius;
    private float duration;
    
    public Ability() { }

    public Ability(string name, AbilityType type, float power, float radius, float duration)
    {
        this.abilityName = name;
        this.abilityType = type;
        this.power = power;
        this.radius = radius;
        this.duration = duration;
    }

    public void ActivateAbility(AbilityType effectType, GameObject companion, float strength = 0.5f, float radius = 3f)
    {
        Vector2 origin = companion.transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                GameObject enemy = hit.gameObject;
                switch (effectType)
                {
                    case AbilityType.Repel:
                        RepelEnemy(enemy, companion, strength);
                        break;
                    case AbilityType.Slow:
                        SlowEnemy(enemy, strength, duration);
                        break;
                    case AbilityType.Damage:
                        DamageEnemy(enemy, strength);
                        break;
                }
            }
        }
    }

    public void ActivateAbility(string effectName, GameObject companion, float strength = 0.5f, float radius = 3f)
    {
        Vector2 origin = companion.transform.position;
        AbilityType effectType = ParseAbilityType(effectName);
        ActivateAbility(effectType, companion, strength, radius);
    }

    private AbilityType ParseAbilityType(string effectName)
    {
        if (string.IsNullOrWhiteSpace(effectName))
            return AbilityType.None;

        switch (effectName.Trim().ToLowerInvariant())
        {
            case "repel":
            case "repeler":
                return AbilityType.Repel;
            case "slow":
            case "ralentizar":
                return AbilityType.Slow;
            case "damage":
            case "daño":
            case "dano":
                return AbilityType.Damage;
            default:
                return AbilityType.None;
        }
    }

    private void RepelEnemy(GameObject enemy, GameObject companion, float forceValue)
    {
        Vector2 origin = companion.transform.position;

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        
        if (rb == null) return;

        Vector2 direction = ((Vector2)enemy.transform.position - origin).normalized;
        rb.AddRelativeForce(direction * forceValue, ForceMode2D.Impulse);
        enemy.GetComponent<EnemyManager>()?.StartCoroutine(enemy.GetComponent<EnemyManager>().ApplyRepeledStatus());
    }

    private void SlowEnemy(GameObject enemy, float slowFactor, float effectDuration)
    {
        EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.ApplySlow(slowFactor, effectDuration);
            return;
        }

        Animator animator = enemy.GetComponent<Animator>();
        if (animator != null)
            animator.speed = Mathf.Clamp01(animator.speed * Mathf.Max(0f, 1f - slowFactor));

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.drag = Mathf.Max(rb.drag, slowFactor * 2f);
    }

    private void DamageEnemy(GameObject enemy, float damageValue)
    {
        EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
        if (enemyManager != null)
            enemyManager.EnemyDamage(damageValue);
    }
}
