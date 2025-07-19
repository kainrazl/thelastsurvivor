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

    private bool isFacingLeft = false;
    private bool isPaused = false;
    private bool canTakeDamage = true;
    private bool isDead = false;
    private bool canShoot = true;

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
    private HeartSpawner heartSpawner;

    private Vector2 move;
    private Vector2 bulletDirection;
    private Vector3 spawnerOriginalPosition;

    // Start is called before the first frame update
    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerAction = new GameActions();
        ph = GetComponent<PlayerHealth>();
        heartSpawner = GameObject.Find("HeartSpawner").GetComponent<HeartSpawner>();
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

                if (move.x != 0)
                {
                    playerAnim.SetTrigger("goingLeftRight");

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
                        playerAnim.SetTrigger("goingUp");
                        bulletSpawner.localPosition = new Vector3(0, 1, 0);
                    }
                    else
                    {
                        playerAnim.SetTrigger("goingDown");
                        bulletSpawner.localPosition = new Vector3(0, 0, 0);
                    }
                }

                //bulletDirection = new Vector2(move.x, move.y);

                //if (playerAction.Player.Shoot.triggered)
                //{
                //    ShootBullet();
                //}
                if(canShoot)
                    StartCoroutine(AutomaticShoot());
            }
            else
            {
                if (!isTutorial)
                {
                    playerAnim.Play("player_dead");

                    GetComponent<Collider2D>().enabled = false;

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

    public void PutPause()
    {
        if (!isDead)
        {
            isPaused = !isPaused;
            pauseCanvas.SetActive(isPaused);
        }
    }

    public void ShootBullet()
    {
        if (!isPaused && !isDead)
        {
            GameObject enemy = GetClosestEnemy();

            if (enemy != null)
            {
                GameObject bullet = BulletPool.instance.GetBullet();

                if (bullet != null)
                {
                    shoot.Play();
                    bullet.transform.position = bulletSpawner.position;
                    bullet.SetActive(true);
                    shot = bullet.GetComponent<Bullet>();
                    //bulletDirection = isFacingLeft ? Vector2.left : Vector2.right;
                    shot.SetEnemyToFollow(enemy);
                }
            }
        }
    }

    void Flip()
    {
        isFacingLeft = !isFacingLeft;

        float localScaleX = transform.localScale.x;
        localScaleX *= -1;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (canTakeDamage)
            {
                canTakeDamage = false;
                playerAnim.SetTrigger("isHurt"); //added
                ph.UpdateHealth(0.1f, true);

                if (ph.currentHealth <= 0)
                {
                    gameMusic.Stop();
                    gameOver.Play();
                    isDead = true;
                }
                else {
                    startBlinking = true;
                }
            }
        }

        if (collision.CompareTag("Recover") && myCurrentHealth < 1)
        {
            ph.UpdateHealth(0.2f, false);
            heartSpawner.heartCount--;
        }
    }

    private IEnumerator AutomaticShoot()
    {
        ShootBullet();
        canShoot = false;
        yield return new WaitForSeconds(shootCadence);
        canShoot = true;
    }

    private void SpriteBlinkingEffect()
    {
        bool isSpriteEnabled = gameObject.GetComponent<SpriteRenderer>().enabled;

        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            spriteBlinkingTotalTimer = 0.0f;
            canTakeDamage = true;
            startBlinking = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;

            isSpriteEnabled = !isSpriteEnabled;
            gameObject.GetComponent<SpriteRenderer>().enabled = isSpriteEnabled;
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

    private void OnEnable()
    {
        playerAction.Enable();
    }

    private void OnDisable()
    {
        playerAction.Disable();
    }
}
