using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowAttack : MonoBehaviour
{
    private float rotationSpeed = 1.5f;

    private Vector3 currentEulerAngles = Vector3.right;
    private float radius = 1.5f;
    private bool isSpinning = false;
    private bool returnToPlayer = true;
    private float rotationAngle = 0;
    private bool hasThrown = false;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if ((!player.GetComponent<PlayerManager>().isPaused) && player.GetComponent<PlayerHealth>().currentHealth > 0) {
            currentEulerAngles += rotationSpeed * Time.deltaTime * new Vector3(90, 0, 180);
        
            if (rotationAngle > 360)
            {
                rotationAngle = 0;
                currentEulerAngles = Vector3.right;
            }

            if (!isSpinning)
            {
                StartCoroutine(ActionAttack());
            }

            if (returnToPlayer)
            {
                Vector2 playerPosition = player.transform.position;
                transform.position = playerPosition;
            }

            if (!hasThrown)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 90));
                rotationAngle += 0.5f;
            }
        }
    }

    public void SetRotationSpeed(float rotationSpeed)
    {
        this.rotationSpeed = rotationSpeed;
    }
    public void SetRadius(float radius) 
    {
        this.radius = radius;
    }

    public void ThrowWeapon()
    {
        float rad = rotationAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        float speed = GetComponent<WeaponInstance>().GetProperties().travelSpeed;

        gameObject.GetComponent<Rigidbody2D>().velocity = direction * speed;


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            float damage = GetComponent<WeaponInstance>().GetProperties().damage;
            collision.GetComponent<EnemyManager>().EnemyDamage(damage);
        }
    }

    IEnumerator ActionAttack()
    {
        isSpinning = true;
        TryGetComponent(out CapsuleCollider2D capsuleCollider);

        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }

        float waitTime = GetComponent<WeaponInstance>().GetProperties().coolDown;

        yield return new WaitForSeconds(waitTime);

        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
        }

        ThrowWeapon();
        hasThrown = true;
        returnToPlayer = false;
        
        yield return new WaitForSeconds(1);
        
        returnToPlayer = true;
        isSpinning = false;
        hasThrown = false;
    }
}
