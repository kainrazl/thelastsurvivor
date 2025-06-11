using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private AudioSource shoot;
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Transform bulletSpawner;

    private bool isFacingLeft = false;
    private bool isPaused = false;
    private bool canTakeDamage = true;
    private float speed = 3f;
    private float myCurrentHealth;

    private GameActions playerAction;
    private Animator playerAnim;
    private Rigidbody2D playerRB;
    private PlayerHealth ph;    
    private Bullet shot;

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
        gameMusic.Play();
    }

    private void Start()
    {
        ph.SetStartHealth();
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
            gameMusic.volume = 0.21f;

            Vector2 playerMove = playerAction.Player.Move.ReadValue<Vector2>();
            move = new Vector2(playerMove.x, playerMove.y);
            bulletSpawner.localPosition = spawnerOriginalPosition;
            
            if (isFacingLeft)
            {
                bulletDirection = Vector2.left;
                
            }
            else
            {
                bulletDirection = Vector2.right;
            }

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
                    bulletDirection = Vector2.up;
                    bulletSpawner.localPosition = new Vector3(0, 1, 0);
                }
                else
                {
                    playerAnim.SetTrigger("goingDown");
                    bulletDirection = Vector2.down;
                    bulletSpawner.localPosition = new Vector3(0, 0, 0);
                }
            }

            if (playerAction.Player.Shoot.triggered)
            {
                ShootBullet();
            }
        }
        else
        {
            Time.timeScale = 0f;
            gameMusic.volume = 0.05f;
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(Mathf.Abs(Time.time)); 
        float normalizedValueX = move.normalized.x * speed;
        float normalizedValueY = move.normalized.y * speed;
        playerRB.velocity = new Vector2(normalizedValueX, normalizedValueY);
    }

    private void LateUpdate()
    {
        playerAnim.SetBool("isIdle", move == Vector2.zero && !isPaused);
    }

    public void PutPause()
    {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);
    }

    public void ShootBullet()
    {
        if (!isPaused)
        {
            GameObject bullet = BulletPool.instance.GetBullet();

            if (bullet != null)
            {
                shoot.Play();
                bullet.transform.position = bulletSpawner.position;
                bullet.SetActive(true);
                shot = bullet.GetComponent<Bullet>();

                shot.SetBulletDirection(bulletDirection);
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
                StartCoroutine(TakeDamage());

                if (ph.currentHealth <= 0)
                {
                    SceneManager.LoadScene("GameOver");
                }
            }
        }

        if (collision.CompareTag("Recover") && myCurrentHealth < 1)
        {
            ph.UpdateHealth(0.2f, false);
        }
    }

    private IEnumerator TakeDamage()
    {
        canTakeDamage = false;
        playerAnim.SetTrigger("isHurt");
        
        //enter
        WaitForSeconds waiting = new WaitForSeconds(2);
        //animation
        ph.UpdateHealth(0.1f, true);
        yield return waiting;
        //end

        canTakeDamage = true;
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
