using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 6;
    [SerializeField] float radius;
    private Transform player;
    Vector3 currentEulerAngles;
    float rotationAngle;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        rotationAngle = 0;
    }

    void Update()
    {
        currentEulerAngles += new Vector3(0, 0, -180) * Time.deltaTime * rotationSpeed;
        transform.eulerAngles = currentEulerAngles;

        StartTranslation();
    }

    private void StartTranslation()
    {
        // Calcula la nueva posición usando trigonometría
        float rad = rotationAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius
        );

        // Actualiza la posición del objeto para que orbite alrededor del jugador
        transform.position = (Vector2)player.position + offset;

        rotationAngle += (-270) * Time.deltaTime;

        if (Mathf.Abs(rotationAngle) > 375)
            StartCoroutine(RestartTranslation());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyManager>().EnemyDamage(collision);
        }
    }

    IEnumerator RestartTranslation()
    {
        rotationAngle = 0;
        transform.position = player.position;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        this.enabled = false;
        yield return new WaitForSeconds(3f);
        this.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }
}
