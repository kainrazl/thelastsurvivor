using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttack : MonoBehaviour
{
    private float rotationSpeed = 1.5f;
    private float radius = 1.2f;

    private Vector3 currentEulerAngles;
    private float rotationAngle;
    private bool hasThrown = false;
    private Vector2 startPosition;
    private float spin = 0;

    private void Start()
    {
        rotationAngle = 0;
        startPosition = transform.localPosition;
    }

    void Update()
    {
        if(spin < -360)
        {
            spin = 0;
            currentEulerAngles = Vector3.zero;
        }
        currentEulerAngles += new Vector3(0, 0, -180) * Time.deltaTime * rotationSpeed;
        transform.eulerAngles = currentEulerAngles;

        spin = currentEulerAngles.z;

        Debug.Log(currentEulerAngles);

        if (!hasThrown)
        {
            StartCoroutine(ActionAttack());
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
        Vector2 force = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y) * GetComponent<WeaponInstance>().GetProperties().travelSpeed;

        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(ReturnPosition());

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            float damage = GetComponent<WeaponInstance>().GetProperties().damage;
            collision.GetComponent<EnemyManager>().EnemyDamage(collision, damage);
        }
    }

    IEnumerator ActionAttack()
    {
        hasThrown = true;
        float waitTime = GetComponent<WeaponInstance>().GetProperties().attackRate;
        yield return new WaitForSeconds(waitTime);
        ThrowWeapon();
        hasThrown = false;
    }

    IEnumerator ReturnPosition()
    {
        yield return new WaitForSeconds(1);
        transform.position = startPosition;
    }
}
