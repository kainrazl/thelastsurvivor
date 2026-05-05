using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemySO properties;
    private float howMuchDamage;
    private float enemySpeed;
    private float baseSpeed;
    private float health;
    private float localScaleX;
    private bool isFacingLeft;
    private bool playerFliped;
    private bool isSlowed = false;
    private float slowMultiplier = 1f;
    private float slowDuration = 0f;
    public bool enemyDead;

    private Vector2 playerPosition;
    private Vector2 enemyPosition;

    private EnemySpawner spawner;
    private PlayerPoints playerPoints;
    private PlayerManager playerManager;
    private ItemSpawner itemSpawner;

    private GameObject player;
    private Animator anim;
    private Vector2 originalForce;

    private void Awake()
    {        
        anim = GetComponent<Animator>();
        spawner = GameObject.FindGameObjectWithTag("Spawners").GetComponent<EnemySpawner>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPoints = player.GetComponent<PlayerPoints>();
        playerManager = player.GetComponent<PlayerManager>();
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
    }

    private void Start()
    {
        float minSpeed = properties.minSpeed;
        float maxSpeed = properties.maxSpeed;
        enemySpeed = Random.Range(minSpeed, maxSpeed);
            
        baseSpeed = enemySpeed;
        howMuchDamage = properties.damage;
        health = properties.health;
        // anim.SetFloat("speed", enemySpeed);
        enemyDead = false;
    }

    void Update()
    {
        UpdateSlow();
        MoveEnemy();
        CheckFlip();
    }

   private void MoveEnemy()
    {
        if (!enemyDead)
        {
            playerPosition = player.transform.position;
            float currentSpeed = baseSpeed * slowMultiplier;
            enemyPosition = Vector2.MoveTowards(transform.position, playerPosition, currentSpeed * Time.deltaTime);
            transform.position = enemyPosition;
        }
    }

    private void UpdateSlow()
    {
        if (!isSlowed) return;

        slowDuration -= Time.deltaTime;
        if (slowDuration <= 0f)
        {
            isSlowed = false;
            slowMultiplier = 1f;
            anim.speed = 1f;
        }
    }

    void CheckFlip()
    {
        isFacingLeft = transform.localScale.x < 0 ? true : false;

        if ((enemyPosition.x < playerPosition.x && isFacingLeft) || (enemyPosition.x > playerPosition.x && !isFacingLeft))
        {
            localScaleX = transform.localScale.x * -1;
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

   private void OnCollisionEnter2D(Collision2D collision)
   {
      if (!enemyDead)
        {
            if (collision.collider.CompareTag("Player"))
                playerManager.TakeDamage(howMuchDamage);
        }
   }

    public void ApplySlow(float slowPercent, float duration)
    {
        slowPercent = Mathf.Clamp01(slowPercent);
        slowMultiplier = 1f - slowPercent;
        slowDuration = duration;
        isSlowed = slowDuration > 0f;
        anim.speed = Mathf.Clamp01(1f - slowPercent);
    }

    public IEnumerator ApplyRepeledStatus()
    {
        yield return new WaitForSeconds(1.5f); //Espera 1.5 segundos antes de quitar el estado de repelido
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void EnemyDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !enemyDead) {
            enemyDead = true;
            itemSpawner.GetItem(transform.localPosition.x, transform.localPosition.y);

            if (spawner.enemyCounter > 0)
                spawner.enemyCounter -= 1;

            playerPoints.UpdatePoints();
            gameObject.GetComponent<Rigidbody2D>().simulated = false;

            // anim.SetFloat("speed", 0);
            anim.Play("enemy_dead");
            
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
}