using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemySO properties;
    private float howMuchDamage;
    private float enemySpeed;
    private float health;
    private float localScaleX;
    private bool isFacingLeft;
    private bool enemyDead;

    private Vector2 playerPosition;
    private Vector2 enemyPosition;

    private EnemySpawner spawner;
    private PlayerPoints playerPoints;
    private PlayerManager playerManager;
    private ItemSpawner itemSpawner;

    private GameObject player;
    private Animator anim;

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
        howMuchDamage = properties.damage;
        health = properties.health;
        anim.SetFloat("speed", enemySpeed);
        enemyDead = false;
    }

    void Update()
    {
        MoveEnemy();
        CheckFlip();
    }

    private void MoveEnemy()
    {
        if (!enemyDead)
        {
            playerPosition = player.transform.position;
            enemyPosition = Vector2.MoveTowards(transform.position, playerPosition, enemySpeed * Time.deltaTime);
            transform.position = enemyPosition;
        }
    }

    void CheckFlip()
    {
        isFacingLeft = transform.localScale.x < 0 ? true : false;
        localScaleX = transform.localScale.x * -1;

        if ((enemyPosition.x < playerPosition.x && isFacingLeft) || (enemyPosition.x > playerPosition.x && !isFacingLeft))
        {
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enemyDead)
        {
            if (collision.CompareTag("Player"))
                playerManager.TakeDamage(howMuchDamage);
        }
    }

    public void EnemyDamage(Collider2D collision, float damage)
    {
        health -= damage;

        if (health <= 0 && !enemyDead) {
            enemyDead = true;
            itemSpawner.GetItem(collision.transform.localPosition.x, collision.transform.localPosition.y);

            if (spawner.enemiesCounter > 0)
                spawner.enemiesCounter -= 1;

            playerPoints.UpdatePoints();
            gameObject.GetComponent<Rigidbody2D>().simulated = false;

            anim.SetFloat("speed", 0);
            anim.Play("enemy_dead");
            
            Destroy(gameObject, 0.5f);
            //StartCoroutine(DestroyEnemy());
        }
    }

    private IEnumerator DestroyEnemy()
    {
        WaitForSeconds waiting = new WaitForSeconds(0.5f);
        yield return waiting;
        
        Destroy(gameObject);
    }
}