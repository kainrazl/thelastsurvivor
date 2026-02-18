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
    private bool playerFliped;
    public bool enemyDead;
    public bool isCompanion = false;

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
        if (isCompanion)
        {
            enemySpeed = player.GetComponent<PlayerManager>().GetSpeed() - 0.3f;
        }
        else
        {
            float minSpeed = properties.minSpeed;
            float maxSpeed = properties.maxSpeed;
            enemySpeed = Random.Range(minSpeed, maxSpeed);
        }
            
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
            playerPosition = isCompanion ? (isFacingLeft ? player.transform.position + (new Vector3(1.3f, -0.6f, 1)) : player.transform.position - (new Vector3(1.3f, 0.6f, 1))) : player.transform.position;
            enemyPosition = Vector2.MoveTowards(transform.position, playerPosition, enemySpeed * Time.deltaTime);
            transform.position = enemyPosition;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((!isCompanion) && (!enemyDead))
        {
            if (collision.CompareTag("Player"))
                playerManager.TakeDamage(howMuchDamage);
        }
    }

    public void EnemyDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !enemyDead) {
            enemyDead = true;
            itemSpawner.GetItem(transform.localPosition.x, transform.localPosition.y);

            if (spawner.enemiesCounter > 0)
                spawner.enemiesCounter -= 1;

            playerPoints.UpdatePoints();
            gameObject.GetComponent<Rigidbody2D>().simulated = false;

            anim.SetFloat("speed", 0);
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
            //GetComponent<SpriteRenderer>().color = Color.Lerp(Color.yellow, Color.green, 0.5f);//new Color(1, 0, 0.1f);
            GetComponent<SpriteRenderer>().color = Random.ColorHSV();//new Color(1, 0, 0.1f);

            yield return new WaitForSeconds(0.5f);

            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}