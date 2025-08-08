using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private float enemySpeed;
    private float localScaleX;
    private bool isFacingLeft;
    private bool enemyDead;

    private Vector2 playerPosition;
    private Vector2 enemyPosition;

    private EnemySpawner spawner;
    private PlayerPoints playerPoints;
    private ItemSpawner itemSpawner;

    private GameObject player;
    private Animator anim;

    private void Awake()
    {
        enemySpeed = Random.Range(0.5f, 2.5f);
        anim = GetComponent<Animator>();
        anim.SetFloat("speed", enemySpeed);
        enemyDead = false;
    }

    private void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("Spawners").GetComponent<EnemySpawner>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPoints = player.GetComponent<PlayerPoints>();
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
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
        
        if (collision.CompareTag("Bullet") && !enemyDead)
        {
            enemyDead = true;
            itemSpawner.GetItem(collision.transform.localPosition.x, collision.transform.localPosition.y);
            
            if (spawner.enemiesCounter > 0)
                spawner.enemiesCounter -= 1;

            playerPoints.UpdatePoints();
            gameObject.GetComponent<Rigidbody2D>().simulated = false;
        }
    }

    private IEnumerator DestroyEnemy()
    {
        WaitForSeconds waiting = new WaitForSeconds(0.5f);
        yield return waiting;
        
        Destroy(gameObject);
    }
}