using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    private float rotationSpeed = 6;
    private float radius = 1.2f;
    private Transform parent;
    private int rounds = 1;
    private Vector3 currentEulerAngles;
    private float originalRotationAngle = 0;
    private float rotationAngle;
    private bool isActive = true;
    private float actualDegrees;

    private void Start()
    {
        rotationAngle = 0;
        originalRotationAngle = rotationAngle;

        int numberOfWeapons = FindObjectsOfType<SpinAttack>().Length;
        float angleBetweenWeapons = 360f / numberOfWeapons;
        float modifiedAngle = 0;

        // Position other SpinAttack instances evenly around the player
        foreach (SpinAttack weaponSpin in FindObjectsOfType<SpinAttack>())
        {
            if (weaponSpin != this)
            {
                modifiedAngle += angleBetweenWeapons * (-1);
                weaponSpin.SetRotationAngle(modifiedAngle);
                weaponSpin.ResetTranslation();
            }
        }
    }

    void Update()
    {
        currentEulerAngles += new Vector3(0, 0, -180) * Time.deltaTime * rotationSpeed;
        transform.eulerAngles = currentEulerAngles;

        if (isActive)
            StartTranslation();
    }

    // Setters Region
#region
    public void SetRounds(int roundNumber)
    {
        rounds = roundNumber;
    }

    public void SetRotationSpeed(float rotationSpeed)
    {
        this.rotationSpeed = rotationSpeed;
    }
    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
    public void SetParent(Transform spinOn)
    {
        parent = spinOn;
        transform.position = spinOn.position;
    }

    public void SetRotationAngle(float angle)
    {
        originalRotationAngle = angle;
    }
#endregion
    private void StartTranslation()
    {
        // Calculate new position using trigonometry
        float rad = rotationAngle * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius
        );

        // Position update for orbit around the player
        transform.position = (Vector2)parent.position + offset;

        if (actualDegrees == 0)
        {
            // Store the initial absolute rotation angle
            actualDegrees = Mathf.Abs(rotationAngle);
        }

        // Update rotation angle based on rotation speed and time
        rotationAngle += (-45) * rotationSpeed * Time.deltaTime;

        // Check if the weapon has completed the required number of rotations
        float angleDiff = Mathf.Abs(rotationAngle) - actualDegrees;

        if (angleDiff > 375 * rounds)
        {
            StartCoroutine(RestartTranslation());
        }            
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            float damage = GetComponent<WeaponInstance>().GetProperties().damage;
            collision.GetComponent<EnemyManager>().EnemyDamage(collision, damage);
        }
    }

    // Coroutine to handle the restart delay
    IEnumerator RestartTranslation()
    {
        isActive = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(3f);
        ResetTranslation();
    }

    // Reset the translation to initial state
    public void ResetTranslation() 
    {
        StopAllCoroutines();
        isActive = true;
        rotationAngle = originalRotationAngle;
        actualDegrees = 0;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }
}
