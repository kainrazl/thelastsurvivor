using System.Collections;
using System.Collections.Generic;
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
    public void ActivateAbility(AbilityType effectType, GameObject source, float strength = 0.5f, float radius = 3f, float cooldown = 3f, CreatureType creatureType = CreatureType.Companion, float duration = 5f)
    {
        Vector2 origin = source.transform.position;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);

        List<GameObject> targetSubjects = new List<GameObject>();

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") && creatureType == CreatureType.Companion)
                targetSubjects.Add(hit.gameObject);
            else if (hit.CompareTag("Player") && creatureType == CreatureType.Boss)
                targetSubjects.Add(hit.gameObject);
        }

        if (targetSubjects.Count == 0) return;

        switch (effectType)
        {
            case AbilityType.Fear:
                foreach (GameObject subject in targetSubjects)
                    ApplyRepel(subject, source, strength, creatureType);
                break;
            case AbilityType.Shield:
                ApplyShield(source, creatureType);
                break;            
            case AbilityType.Paralyze:
                foreach (GameObject subject in targetSubjects)
                    SlowEnemy(subject, strength, duration);
                break;
            case AbilityType.Damage:
                foreach (GameObject subject in targetSubjects)
                    DamageEnemy(subject, strength);
                break;
            case AbilityType.Attraction:
                foreach (GameObject subject in targetSubjects)
                    source.GetComponent<AbilityManager>()?.StartCoroutine(AttractEnemy(subject, source, cooldown));
                break;
            case AbilityType.Tackle:
                source.GetComponent<AbilityManager>()?.StartCoroutine(TackleEnemies(source, targetSubjects, strength, cooldown));
                break;
            case AbilityType.LaserRay:
                source.GetComponent<AbilityManager>()?.StartCoroutine(LaserRayAttack(source, targetSubjects, strength, cooldown));
                break;
            case AbilityType.TakeAway:
                source.GetComponent<AbilityManager>()?.StartCoroutine(TakeAwayEnemy(source, player, targetSubjects, strength, cooldown));
                break;
            case AbilityType.SpeedBoost:
                source.GetComponent<AbilityManager>()?.StartCoroutine(ApplySpeedBoost(source, player, strength, cooldown));
                break;
            case AbilityType.Heal:
                HealPlayer(source, player, strength);
                break;
            default:
                break;
        }
    }

    private void ApplyRepel(GameObject subject, GameObject companion, float forceValue, CreatureType creatureType)
    {
        Vector2 origin = companion.transform.position;
        Vector2 direction = ((Vector2)subject.transform.position - origin).normalized;
        
        if (creatureType == CreatureType.Companion)
        {
            Rigidbody2D rb = subject.GetComponent<Rigidbody2D>();
            if (rb == null) return;
            rb.AddRelativeForce(direction * forceValue, ForceMode2D.Impulse);
        }
        else if (creatureType == CreatureType.Boss)
        {            
            subject.GetComponent<PlayerManager>()?.UpdateRepelledStatus(true, direction, forceValue);
        }        
    }

    private void SlowEnemy(GameObject subject, float slowFactor, float cooldown)
    {
        EnemyManager enemyManager = subject.GetComponent<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.ApplySlow(slowFactor, cooldown);
            return;
        }

        Animator animator = subject.GetComponent<Animator>();
        if (animator != null)
            animator.speed = Mathf.Clamp01(animator.speed * Mathf.Max(0f, 1f - slowFactor));

        Rigidbody2D rb = subject.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearDamping = Mathf.Max(rb.linearDamping, slowFactor * 2f);
    }

    private void DamageEnemy(GameObject subject, float damageValue)
    {
        EnemyManager enemyManager = subject.GetComponent<EnemyManager>();
        if (enemyManager != null)
            enemyManager.EnemyDamage(damageValue);
    }

    private void ApplyShield(GameObject subject, CreatureType creatureType)
    {
        if (creatureType == CreatureType.Companion)
        {
            subject.GetComponent<PlayerManager>()?.StartCoroutine(subject.GetComponent<PlayerManager>().PlayerImmune());
        }
        else if (creatureType == CreatureType.Boss)
        {
            subject.GetComponent<EnemyManager>().StartCoroutine(subject.GetComponent<EnemyManager>().EnemyImmune());
        }
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
            if (playerRb.linearVelocity.magnitude > 0.1f)
            {
                Vector2 direction = playerRb.linearVelocity.normalized;
                float currentSpeed = pm.GetSpeed() * (1f + strength);
                playerRb.linearVelocity = direction * currentSpeed;
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

    private IEnumerator AttractEnemy(GameObject subject, GameObject companion, float duration)
    {
        EnemyManager em = subject.GetComponent<EnemyManager>();
        if (em == null) yield break;

        Rigidbody2D rb = subject.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        float elapsedTime = 0f;
        Vector2 companionPos = companion.transform.position;

        while (elapsedTime < duration && subject != null)
        {
            companionPos = companion.transform.position;
            Vector2 direction = (companionPos - (Vector2)subject.transform.position).normalized;
            rb.linearVelocity = direction * 4f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator TackleEnemies(GameObject companion, List<GameObject> targetSubjects, float strength, float duration)
    {
        if (targetSubjects.Count == 0) yield break;

        Vector3 companionStartPos = companion.transform.position;
        Rigidbody2D companionRb = companion.GetComponent<Rigidbody2D>();
        if (companionRb == null) yield break;

        foreach (GameObject subject in targetSubjects)
        {
            if (subject == null) continue;

            Vector3 enemyPos = subject.transform.position;
            Vector3 direction = (enemyPos - companionStartPos).normalized;
            float distance = Vector3.Distance(companionStartPos, enemyPos);
            float tackleSpeed = 8f;
            float tackleTime = distance / tackleSpeed;

            float elapsedTime = 0f;
            while (elapsedTime < tackleTime && subject != null)
            {
                companionRb.linearVelocity = direction * tackleSpeed;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (subject != null)
            {
                DamageEnemy(subject, strength);
                EnemyManager em = subject.GetComponent<EnemyManager>();
                if (em != null)
                    em.EnemyDamage(strength);
            }
        }

        companionRb.linearVelocity = Vector2.zero;

        float returnTime = 0.5f;
        float returnElapsed = 0f;
        while (returnElapsed < returnTime)
        {
            Vector3 direction = (companionStartPos - companion.transform.position).normalized;
            companionRb.linearVelocity = direction * 6f;
            returnElapsed += Time.deltaTime;
            yield return null;
        }

        companionRb.linearVelocity = Vector2.zero;
    }

    private IEnumerator LaserRayAttack(GameObject companion, List<GameObject> targetSubjects, float strength, float duration)
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
                foreach (GameObject subject in targetSubjects)
                {
                    if (subject == null) continue;

                    Vector2 toEnemy = ((Vector2)subject.transform.position - (Vector2)companion.transform.position).normalized;
                    if (Vector2.Dot(randomDirection, toEnemy) > 0.3f)
                    {
                        EnemyManager em = subject.GetComponent<EnemyManager>();
                        if (em != null)
                            em.EnemyDamage(strength * 0.3f);
                    }
                }
                lastRayTime = 0f;
            }

            yield return null;
        }
    }

    private IEnumerator TakeAwayEnemy(GameObject companion, GameObject player, List<GameObject> targetSubjects, float strength, float duration)
    {
        if (targetSubjects.Count == 0) yield break;

        GameObject targetEnemy = targetSubjects[Random.Range(0, targetSubjects.Count)];
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
            enemyRb.linearVelocity = direction * 5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (targetEnemy != null)
        {
            EnemyManager em = targetEnemy.GetComponent<EnemyManager>();
            if (em != null)
                em.EnemyDamage(strength * 2f);

            enemyRb.linearVelocity = Vector2.zero;
        }
    }
}
