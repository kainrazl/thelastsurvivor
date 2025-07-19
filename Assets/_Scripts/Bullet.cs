using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 18f;
    private Vector2 enemyDirection = Vector2.zero;
    private GameObject enemyToFollow = null;
    private Vector2 enemyPosition = Vector2.zero;

    private void FixedUpdate()
    {
        enemyPosition = GetEnemyPosition();

        if (enemyPosition != Vector2.zero)
        {
            enemyPosition = new Vector2(enemyPosition.x, enemyPosition.y + 0.5f);
            enemyDirection = Vector2.MoveTowards(transform.position, enemyPosition, speed * Time.deltaTime);
            transform.position = enemyDirection;
        }
    }

    public void SetEnemyToFollow(GameObject enemy)
    {
        enemyToFollow = enemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.SetActive(false);
        }
    }

    private Vector2 GetEnemyPosition()
    {
        enemyPosition = enemyToFollow.transform.position;

        return enemyPosition;
    }
}
