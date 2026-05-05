using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 1.5f;
    private Vector2 enemyDirection = Vector2.zero;
    private GameObject enemyToFollow = null;
    private Vector2 enemyPosition = Vector2.zero;
    private float bulletDamage;
    private float distance;

    private void FixedUpdate()
    {
        enemyPosition = GetEnemyPosition();

        if (enemyPosition != Vector2.zero)
        {
            //0.5f added in order to shoot at the middle of the sprite, not the gameobject pivot
            enemyPosition = new Vector2(enemyPosition.x, enemyPosition.y + 0.5f);
            enemyDirection = Vector2.MoveTowards(transform.position, enemyPosition, speed * Time.deltaTime * distance);
            transform.position = enemyDirection;
        }
    }

    public void SetEnemyToFollow(GameObject enemy, float damage, float range)
    {
        enemyToFollow = enemy;
        bulletDamage = damage;
        distance = range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
        {
            if (collision.CompareTag("Enemy"))
            {
                enemyToFollow.GetComponent<EnemyManager>().EnemyDamage(bulletDamage);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.SetActive(false);
            }
        }
    }

    private Vector2 GetEnemyPosition()
    {
        enemyPosition = Vector2.zero;

        if (enemyToFollow != null){
            enemyPosition = new Vector2(enemyToFollow.transform.position.x, enemyToFollow.transform.position.y - 0.06f);
        }
        else
        {
            gameObject.SetActive(false);
        }

        return enemyPosition;
    }
}
