using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    private float companionSpeed;
    private float health;
    private float localScaleX;
    [SerializeField] private bool isFacingLeft = false;
    public bool companionDead;

    private Ability companionAbility;
    [SerializeField] private float maxDistanceToPlayer = 3f;
    [SerializeField] private float minDistanceToPlayer = 1f;
    [SerializeField] private AbilityType activeAbilityType = AbilityType.Fear;
    [SerializeField] private float abilityPower = 3f;
    [SerializeField] private float abilityRadius = 3f;
    [SerializeField] private float abilityCooldown = 12f;
    private float lastAbilityTime = 0f;

    private Vector2 playerPosition;
    private Vector2 companionPosition;

    private PlayerPoints playerPoints;
    private PlayerManager playerManager;
    private ItemSpawner itemSpawner;

    private GameObject player;
    private Animator anim;
    private Vector2 realPlayerPosition;

private void Awake()
    {
        anim = GetComponent<Animator>();
        companionAbility = new Ability();
        player = GameObject.FindGameObjectWithTag("Player");
        realPlayerPosition = player.transform.position;
        playerPoints = player.GetComponent<PlayerPoints>();
        playerManager = player.GetComponent<PlayerManager>();
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
    }

    private void Start()
    {
        companionSpeed = player.GetComponent<PlayerManager>().GetSpeed() - 0.3f;
        companionDead = false;
    }

    void Update()
    {
        realPlayerPosition = player.transform.position;
        CheckFlip();

        Vector2 distanceToPlayer = player.transform.position - transform.position;
        int speedMultiplier = 1;

        if (distanceToPlayer.magnitude >= maxDistanceToPlayer)
        {
            speedMultiplier = 2;
        }

        if (distanceToPlayer.magnitude > minDistanceToPlayer)
        {
            MoveCompanion(speedMultiplier);
        }

        lastAbilityTime += Time.deltaTime;
        
        ExecuteAbility(activeAbilityType);
    }

    private void MoveCompanion(int speedMultiplier)
    {
        if (!companionDead)
        {
            playerPosition = isFacingLeft ? player.transform.position + (new Vector3(0.9f, -0.36f, 1)) : player.transform.position - (new Vector3(0.9f, 0.36f, 1));
            companionPosition = Vector2.MoveTowards(transform.position, playerPosition, companionSpeed * Time.deltaTime);
            transform.position = companionPosition;
        }
    }

    void CheckFlip()
    {
        isFacingLeft = player.GetComponent<PlayerManager>().isFacingLeft;

        localScaleX = isFacingLeft ? -1 : 1;
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(realPlayerPosition, abilityRadius);
    }
    public void ExecuteAbility(AbilityType abilityType)
    {
        if (companionDead || lastAbilityTime < abilityCooldown) return;

        companionAbility.ActivateAbility(abilityType, player, abilityPower, abilityRadius, abilityCooldown);
        lastAbilityTime = 0f;

        StartCoroutine(AbilityCooldownIndicator());
    }

    public void ExecuteAbility(string abilityName)
    {
        if (companionDead || lastAbilityTime < abilityCooldown) return;

        companionAbility.ActivateAbility(abilityName, player, abilityPower, abilityRadius);
        lastAbilityTime = 0f;
    }

    public void CompanionDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !companionDead) {
            companionDead = true;
            itemSpawner.GetItem(transform.localPosition.x, transform.localPosition.y);

            playerPoints.UpdatePoints();
            gameObject.GetComponent<Rigidbody2D>().simulated = false;

            anim.SetFloat("speed", 0);
            anim.Play("companion_dead");
            
            Destroy(gameObject, 0.5f);
        }
        else
        {
            StartCoroutine(DamageIndicator());
        }
    }

    private IEnumerator DamageIndicator()
    {
        if (health > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);

            yield return new WaitForSeconds(0.1f);

            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private IEnumerator AbilityCooldownIndicator()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < abilityCooldown)
        {
            // Agregar una barra de cooldown o un indicador visual
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Cooldown terminado, actualizar el indicador visual para mostrar que la habilidad está lista
        Debug.Log("Ability is ready again!");
    }
}