using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource gameOver;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private bool isTutorial;

    public bool isFacingLeft = false;
    public bool isPaused = false;
    public GameObject attackObject;
    public bool isDead = false;
    
    private bool canTakeDamage = true;
    private bool canAutoAttack = true;

    private float speed = 3f;
    private float myCurrentHealth;
    private float spriteBlinkingTotalTimer = 0;
    private float spriteBlinkingTotalDuration = 2f;
    private bool startBlinking = false;
    private float spriteBlinkingTimer = 0;
    private float spriteBlinkingMiniDuration = 0.09f;
    private float shootCadence = 3f;

    private GameActions playerAction;
    private Animator playerAnim;
    private Rigidbody2D playerRB;
    private PlayerHealth ph;    
    private Bullet shot;
    private ItemSpawner itemSpawner;
    private GameObject playerSprite;
    private MeleeAttack meleeAttack;

    private Vector2 move;
    private Vector2 bulletDirection;
    private Vector3 spawnerOriginalPosition;
    private Collider2D playerCollider;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playerAnim = GetComponentInChildren<Animator>();
        playerAction = new GameActions();
        ph = GetComponent<PlayerHealth>();
        meleeAttack = attackObject.GetComponentInChildren<MeleeAttack>();
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
        playerSprite =  GameObject.Find("PlayerSprite");
        sprite = GetComponentInChildren<SpriteRenderer>();
        gameMusic.Play();
    }

    private void Start()
    {
        ph.SetStartHealth();

        if (isTutorial)
            ph.UpdateHealth(0.3f, true);

        spawnerOriginalPosition = bulletSpawner.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        myCurrentHealth = ph.currentHealth;

        if (playerAction.Player.Pause.triggered)
        {
            PutPause();
        }

        if (!isPaused)
        {
            Time.timeScale = 1f;
            gameMusic.volume = 0.30f;

            if (!isDead)
            {
                Vector2 playerMove = playerAction.Player.Move.ReadValue<Vector2>();
                move = new Vector2(playerMove.x, playerMove.y);
                bulletSpawner.localPosition = spawnerOriginalPosition;

                if(move != Vector2.zero) 
                {
                    PlayerMovement();
                }

                //bulletDirection = new Vector2(move.x, move.y);

                //if (playerAction.Player.Shoot.triggered)
                //{
                //    ShootBullet();
                //}
                if(canAutoAttack)
                    StartCoroutine(AutoMeleeAttack());
            }
            else
            {
                if (!isTutorial)
                {
                    //playerAnim.Play("player_dead");
                    playerAnim.SetBool("isDead", true);
                    playerCollider.enabled = false;

                    GameObject.FindGameObjectWithTag("Spawners").GetComponent<EnemySpawner>().enabled = false;
                    playerRB.velocity = Vector2.zero;

                    int enemies = 0;

                    foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        enemies++;
                        enemy.GetComponent<EnemyManager>().enabled = false;
                        enemy.GetComponent<Animator>().enabled = false;
                        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    }

                    try
                    {
                        gameOverCanvas.SetActive(true);
                    }
                    catch (AndroidJavaException e)
                    {
                        Debug.Log(e.Message);
                    }
                }
                else
                {
                    SceneManager.LoadScene("Menu");
                }
            }
        }
        else
        {
            Time.timeScale = 0f;
            gameMusic.volume = 0.06f;
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            float normalizedValueX = move.normalized.x * speed;
            float normalizedValueY = move.normalized.y * speed;
            playerRB.velocity = new Vector2(normalizedValueX, normalizedValueY);

            if (startBlinking)
            {
                SpriteBlinkingEffect();
                //canTakeDamage = false;
                //StartCoroutine(TakeDamage());
            }
        }
    }

    private void LateUpdate()
    {
        playerAnim.SetBool("isIdle", move == Vector2.zero && !isPaused && !isDead);
    }

    private void PlayerMovement()
    {
#if AZTEK
        playerAnim.Play("aztek_walk");
#endif

        if (move.x != 0)
        {
#if ZOMBIES
            playerAnim.SetTrigger("goingLeftRight");
#endif

            if (isFacingLeft && move.x > 0)
            {
                Flip();
            }
            else if (!isFacingLeft && move.x < 0)
            {
                Flip();
            }
        }
        else if (move.y != 0)
        {
            if (move.y > 0)
            {
#if ZOMBIES
                playerAnim.SetTrigger("goingUp");
#endif

                bulletSpawner.localPosition = new Vector3(0, 1, 0);
            }
            else
            {
#if ZOMBIES
                playerAnim.SetTrigger("goingDown");
#endif

                bulletSpawner.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    public void PutPause()
    {
        if (!isDead)
        {
            isPaused = !isPaused;
            pauseCanvas.SetActive(isPaused);
        }
    }

    void Flip()
    {
        isFacingLeft = !isFacingLeft;

        float localScaleX = playerSprite.transform.localScale.x;
        localScaleX *= -1;

        playerCollider.offset = new Vector2(playerCollider.offset.x * (-1), playerCollider.offset.y);
        playerSprite.transform.localScale = new Vector3(localScaleX, playerSprite.transform.localScale.y, playerSprite.transform.localScale.z);
    }

    public void TakeDamage(float howMuchDamage)
    {
        if (canTakeDamage)
        {
            StartCoroutine(PlayerImmune());

#if ZOMBIES
            playerAnim.SetTrigger("isHurt"); //added
#endif

            ph.UpdateHealth(howMuchDamage, true);

            if (ph.currentHealth <= 0)
            {
                gameMusic.Stop();
                gameOver.Play();
                isDead = true;
            }
            else
            {
                startBlinking = true;
            }
        }
    }
    private IEnumerator AutoMeleeAttack()
    {
        //ShootBullet();
        meleeAttack.Hit();
        canAutoAttack = false;
        yield return new WaitForSeconds(shootCadence);
        canAutoAttack = true;
    }
    public IEnumerator PlayerImmune()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(3);
        canTakeDamage = true;
    }

    public void SpriteBlinkingEffect()
    {
        bool isSpriteEnabled = sprite.enabled;
        sprite.color = new Color(1, 0, 0.1f);//Color.red;// Lerp(Color.white, Color.blue, Mathf.PingPong(Time.time, 1));

        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            spriteBlinkingTotalTimer = 0.0f;
            startBlinking = false;
            sprite.enabled = true;
            sprite.color = Color.white;
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;

            isSpriteEnabled = !isSpriteEnabled;
            sprite.enabled = isSpriteEnabled;

        }
    }

    private GameObject GetClosestEnemy()
    {
        GameObject enemyToShoot = null;
        float minDistance = Mathf.Infinity;
        float distance = 0;
        Vector2 playerPosition = gameObject.transform.position;
        Vector2 enemyPosition = Vector2.zero;
        string enemyName = null;

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")){
            enemyPosition = enemy.transform.position;
            distance = Vector2.Distance(playerPosition, enemyPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                enemyToShoot = enemy;
                enemyName = enemy.name;
            }
        }

        return enemyToShoot;
    }

    public float GetSpeed() {return speed;}

    private void OnEnable()
    {
        playerAction.Enable();
    }

    private void OnDisable()
    {
        playerAction.Disable();
    }
}
