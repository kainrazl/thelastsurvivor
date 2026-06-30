using System.Collections;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public bool companionDead;
    [SerializeField] private bool isFacingLeft = false;
    [SerializeField] private float maxDistanceToPlayer = 3f;
    [SerializeField] private float minDistanceToPlayer = 1f;
    private float companionSpeed;
    private float health;
    private float localScaleX;
    private Vector2 playerPosition;
    private Vector2 companionPosition;

    private PlayerPoints playerPoints;
    private PlayerManager playerManager;
    private ItemSpawner itemSpawner;

    private static GameObject player;
    private Animator anim;
    private Vector3 realPlayerPosition;

private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        realPlayerPosition = player.transform.position;
        playerPoints = player.GetComponent<PlayerPoints>();
        playerManager = player.GetComponent<PlayerManager>();
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
    }

    private void Start()
    {
        companionSpeed = playerManager.GetSpeed() - 0.3f;
        companionDead = false;
    }

    void Update()
    {
        realPlayerPosition = player.transform.position;
        CheckFlip();

        Vector2 distanceToPlayer = realPlayerPosition - transform.position;
        int speedMultiplier = 1;

        if (distanceToPlayer.magnitude >= maxDistanceToPlayer)
        {
            speedMultiplier = 2;
        }

        if (distanceToPlayer.magnitude > minDistanceToPlayer)
        {
            MoveCompanion(speedMultiplier);
        }
    }

    private void MoveCompanion(int speedMultiplier)
    {
        if (!companionDead)
        {
            playerPosition = isFacingLeft ? player.transform.position + new Vector3(0.9f, -0.36f, 1) : player.transform.position - new Vector3(0.9f, 0.36f, 1);
            companionPosition = Vector2.MoveTowards(transform.position, playerPosition, companionSpeed * speedMultiplier * Time.deltaTime);
            transform.position = companionPosition;
        }
    }

    void CheckFlip()
    {
        isFacingLeft = playerManager.isFacingLeft;

        localScaleX = isFacingLeft ? -1 : 1;
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
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