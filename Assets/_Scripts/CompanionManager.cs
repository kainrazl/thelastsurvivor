using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    [SerializeField] private EnemySO properties;
    private float howMuchDamage;
    private float companionSpeed;
    private float health;
    private float localScaleX;
    [SerializeField] private bool isFacingLeft = false;
    public bool companionDead;

    private Vector2 playerPosition;
    private Vector2 companionPosition;

    private PlayerPoints playerPoints;
    private PlayerManager playerManager;
    private ItemSpawner itemSpawner;

    private GameObject player;
    private Animator anim;

    private void Awake()
    {        
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerPoints = player.GetComponent<PlayerPoints>();
        playerManager = player.GetComponent<PlayerManager>();
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
    }

    private void Start()
    {
        companionSpeed = player.GetComponent<PlayerManager>().GetSpeed() - 0.3f;
            
        howMuchDamage = properties.damage;
        health = properties.health;
        anim.SetFloat("speed", companionSpeed);
        companionDead = false;
    }

    void Update()
    {
        CheckFlip();

        Vector2 distanceToPlayer = player.transform.position - transform.position;

        if (distanceToPlayer.magnitude > 1.2f)
        {
            MoveCompanion();
        }
    }

    private void MoveCompanion()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Something
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
}