using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AbilityType
{
    None,
    Fear,
    Paralyze,
    Damage,
    Shield,
    Attraction,
    SpeedBoost,
    Tackle,
    LaserRay,
    TakeAway,
    Heal,
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

    public void ActivateAbility(AbilityType effectType, GameObject companion, float strength = 0.5f, float radius = 3f, float effectDuration = 3f)
    {
        Vector2 origin = companion.transform.position;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);

        List<GameObject> enemies = new List<GameObject>();
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                enemies.Add(hit.gameObject);
        }

        if (enemies.Count == 0) return;

        Debug.Log($"Activating {effectType} on {enemies.Count} enemies with strength {strength}, radius {radius}, duration {effectDuration}");

        switch (effectType)
        {
            case AbilityType.Fear:
                foreach (GameObject enemy in enemies)
                    RepelEnemy(enemy, companion, strength);
                break;
            case AbilityType.Paralyze:
                foreach (GameObject enemy in enemies)
                    SlowEnemy(enemy, strength, effectDuration);
                break;
            case AbilityType.Damage:
                foreach (GameObject enemy in enemies)
                    DamageEnemy(enemy, strength);
                break;
            case AbilityType.Attraction:
                foreach (GameObject enemy in enemies)
                    companion.GetComponent<CompanionManager>()?.StartCoroutine(AttractEnemy(enemy, companion, effectDuration));
                break;
            case AbilityType.Tackle:
                companion.GetComponent<CompanionManager>()?.StartCoroutine(TackleEnemies(companion, enemies, strength, effectDuration));
                break;
            case AbilityType.LaserRay:
                companion.GetComponent<CompanionManager>()?.StartCoroutine(LaserRayAttack(companion, enemies, strength, effectDuration));
                break;
            case AbilityType.TakeAway:
                companion.GetComponent<CompanionManager>()?.StartCoroutine(TakeAwayEnemy(companion, player, enemies, strength, effectDuration));
                break;
        }

        // switch (effectType)
        // {
        //     case AbilityType.Shield:
        //         ApplyShield(companion, player, strength, effectDuration);
        //         break;
        //     case AbilityType.SpeedBoost:
        //         companion.GetComponent<CompanionManager>()?.StartCoroutine(ApplySpeedBoost(companion, player, strength, effectDuration));
        //         break;
        //     case AbilityType.Heal:
        //         HealPlayer(companion, player, strength);
        //         break;
        //     default:
            // case AbilityType.Attraction:
            // case AbilityType.Tackle:
            // case AbilityType.LaserRay:
            // case AbilityType.TakeAway:
            // case AbilityType.Fear:
            // case AbilityType.Paralyze:
            // case AbilityType.Damage:
                // ProcessEnemyAbility(effectType, companion, player, strength, radius, effectDuration);
        //         break;
        // }
    }

    private void ProcessEnemyAbility(AbilityType effectType, GameObject companion, GameObject player, float strength, float radius, float effectDuration)
    {
        Vector2 origin = companion.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);

        List<GameObject> enemies = new List<GameObject>();
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                enemies.Add(hit.gameObject);
        }

        if (enemies.Count == 0) return;

        switch (effectType)
        {
            case AbilityType.Fear:
                foreach (GameObject enemy in enemies)
                    RepelEnemy(enemy, companion, strength);
                break;
            case AbilityType.Paralyze:
                foreach (GameObject enemy in enemies)
                    SlowEnemy(enemy, strength, effectDuration);
                break;
            case AbilityType.Damage:
                foreach (GameObject enemy in enemies)
                    DamageEnemy(enemy, strength);
                break;
            case AbilityType.Attraction:
                foreach (GameObject enemy in enemies)
                    companion.GetComponent<CompanionManager>()?.StartCoroutine(AttractEnemy(enemy, companion, effectDuration));
                break;
            case AbilityType.Tackle:
                companion.GetComponent<CompanionManager>()?.StartCoroutine(TackleEnemies(companion, enemies, strength, effectDuration));
                break;
            case AbilityType.LaserRay:
                companion.GetComponent<CompanionManager>()?.StartCoroutine(LaserRayAttack(companion, enemies, strength, effectDuration));
                break;
            case AbilityType.TakeAway:
                companion.GetComponent<CompanionManager>()?.StartCoroutine(TakeAwayEnemy(companion, player, enemies, strength, effectDuration));
                break;
        }
    }

    public void ActivateAbility(string effectName, GameObject companion, float strength = 0.5f, float radius = 3f)
    {
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
                return AbilityType.Fear;
            case "slow":
                return AbilityType.Paralyze;
            case "damage":
                return AbilityType.Damage;
            case "shield":
                return AbilityType.Shield;
            case "attraction":
                return AbilityType.Attraction;
            case "speedboost":
            case "speed":
                return AbilityType.SpeedBoost;
            case "tackle":
                return AbilityType.Tackle;
            case "laserray":
            case "laser":
                return AbilityType.LaserRay;
            case "takeaway":
            case "take":
                return AbilityType.TakeAway;
            case "heal":
                return AbilityType.Heal;
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

    private void ApplyShield(GameObject companion, GameObject player, float strength, float duration)
    {
        PlayerManager pm = player.GetComponent<PlayerManager>();
        if (pm != null)
            companion.GetComponent<CompanionManager>()?.StartCoroutine(ShieldCoroutine(pm, duration));
    }

    private IEnumerator ShieldCoroutine(PlayerManager playerManager, float duration)
    {
        yield return playerManager.StartCoroutine(playerManager.PlayerImmune());
    }

    private IEnumerator ApplySpeedBoost(GameObject companion, GameObject player, float strength, float duration)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        PlayerManager pm = player.GetComponent<PlayerManager>();
        if (playerRb == null || pm == null) yield break;

        float speedBoostAmount = pm.GetSpeed() * strength;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (playerRb.velocity.magnitude > 0.1f)
            {
                Vector2 direction = playerRb.velocity.normalized;
                float currentSpeed = pm.GetSpeed() * (1f + strength);
                playerRb.velocity = direction * currentSpeed;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void HealPlayer(GameObject companion, GameObject player, float healPercent)
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            healPercent = Mathf.Clamp01(healPercent);
            ph.UpdateHealth(healPercent * 10, false);
        }
    }

    private IEnumerator AttractEnemy(GameObject enemy, GameObject companion, float duration)
    {
        EnemyManager em = enemy.GetComponent<EnemyManager>();
        if (em == null) yield break;

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        float elapsedTime = 0f;
        Vector2 companionPos = companion.transform.position;

        while (elapsedTime < duration && enemy != null)
        {
            companionPos = companion.transform.position;
            Vector2 direction = (companionPos - (Vector2)enemy.transform.position).normalized;
            rb.velocity = direction * 4f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    private IEnumerator TackleEnemies(GameObject companion, List<GameObject> enemies, float strength, float duration)
    {
        if (enemies.Count == 0) yield break;

        Vector3 companionStartPos = companion.transform.position;
        Rigidbody2D companionRb = companion.GetComponent<Rigidbody2D>();
        if (companionRb == null) yield break;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            Vector3 enemyPos = enemy.transform.position;
            Vector3 direction = (enemyPos - companionStartPos).normalized;
            float distance = Vector3.Distance(companionStartPos, enemyPos);
            float tackleSpeed = 8f;
            float tackleTime = distance / tackleSpeed;

            float elapsedTime = 0f;
            while (elapsedTime < tackleTime && enemy != null)
            {
                companionRb.velocity = direction * tackleSpeed;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (enemy != null)
            {
                DamageEnemy(enemy, strength);
                EnemyManager em = enemy.GetComponent<EnemyManager>();
                if (em != null)
                    em.EnemyDamage(strength);
            }
        }

        companionRb.velocity = Vector2.zero;

        float returnTime = 0.5f;
        float returnElapsed = 0f;
        while (returnElapsed < returnTime)
        {
            Vector3 direction = (companionStartPos - companion.transform.position).normalized;
            companionRb.velocity = direction * 6f;
            returnElapsed += Time.deltaTime;
            yield return null;
        }

        companionRb.velocity = Vector2.zero;
    }

    private IEnumerator LaserRayAttack(GameObject companion, List<GameObject> enemies, float strength, float duration)
    {
        float elapsedTime = 0f;
        float raySpawnInterval = 0.3f;
        float lastRayTime = 0f;

        while (elapsedTime < duration)
        {
            lastRayTime += Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (lastRayTime >= raySpawnInterval)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                foreach (GameObject enemy in enemies)
                {
                    if (enemy == null) continue;

                    Vector2 toEnemy = ((Vector2)enemy.transform.position - (Vector2)companion.transform.position).normalized;
                    if (Vector2.Dot(randomDirection, toEnemy) > 0.3f)
                    {
                        EnemyManager em = enemy.GetComponent<EnemyManager>();
                        if (em != null)
                            em.EnemyDamage(strength * 0.3f);
                    }
                }
                lastRayTime = 0f;
            }

            yield return null;
        }
    }

    private IEnumerator TakeAwayEnemy(GameObject companion, GameObject player, List<GameObject> enemies, float strength, float duration)
    {
        if (enemies.Count == 0) yield break;

        GameObject targetEnemy = enemies[Random.Range(0, enemies.Count)];
        if (targetEnemy == null) yield break;

        Vector3 companionStartPos = companion.transform.position;
        Vector3 takeAwayDirection = (companion.transform.position - player.transform.position).normalized;
        Vector3 targetPos = companion.transform.position + takeAwayDirection * 5f;

        Rigidbody2D enemyRb = targetEnemy.GetComponent<Rigidbody2D>();
        if (enemyRb == null) yield break;

        float moveTime = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < moveTime && targetEnemy != null)
        {
            Vector3 direction = (targetPos - targetEnemy.transform.position).normalized;
            enemyRb.velocity = direction * 5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (targetEnemy != null)
        {
            EnemyManager em = targetEnemy.GetComponent<EnemyManager>();
            if (em != null)
                em.EnemyDamage(strength * 2f);

            enemyRb.velocity = Vector2.zero;
        }
    }
}
