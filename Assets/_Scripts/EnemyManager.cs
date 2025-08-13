using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private float howManyDamage;
    private float enemySpeed;
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
        enemySpeed = Random.Range(0.5f, 2.5f);
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
        else
        {
            anim.SetFloat("speed", 0);
            anim.Play("enemy_dead");
            
            StartCoroutine(DestroyEnemy());
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
            //if (collision.CompareTag("Attack"))
            //    EnemyDamage(collision);

            if (collision.CompareTag("Player"))
                playerManager.TakeDamage(howManyDamage);
        }
    }

    public void EnemyDamage(Collider2D collision)
    {
        enemyDead = true;
        itemSpawner.GetItem(collision.transform.localPosition.x, collision.transform.localPosition.y);

        if (spawner.enemiesCounter > 0)
            spawner.enemiesCounter -= 1;

        playerPoints.UpdatePoints();
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
    }

    private IEnumerator DestroyEnemy()
    {
        WaitForSeconds waiting = new WaitForSeconds(0.5f);
        yield return waiting;
        
        Destroy(gameObject);
    }
}