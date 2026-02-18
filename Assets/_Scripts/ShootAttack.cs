using System;
using System.Collections;
using UnityEngine;

public class ShootAttack : MonoBehaviour
{
    private GameObject player;
    private Bullet shot;
    private bool canShoot = true;
    private AudioSource shotSound;
    private float cadence;
    private float bulletsToShoot;
    private float bulletDamage;
    private float bulletDistance;

    [SerializeField] private AudioClip shoot;
    [SerializeField] private GameObject bulletPrefab;
    
    private void Update()
    {
        transform.position = player.transform.position;
        
        if (canShoot)
            StartCoroutine(ActivateShootAttack());
    }

    private IEnumerator ActivateShootAttack()
    {
        canShoot =  false;
        ShootEnemy();
        yield return new WaitForSeconds(0.5f);
        canShoot =  true;
    }

    private void ShootEnemy()
    {
        if (!player.GetComponent<PlayerManager>().isPaused && !player.GetComponent<PlayerManager>().isDead)
        {
            GameObject enemy = GetClosestEnemy();
            
            if (enemy != null)
            {
                GameObject bullet = BulletPool.instance.GetBullet();

                if (bullet != null)
                {
                    // shotSound.clip = shoot;
                    // shotSound.Play();
                    bullet.transform.position = transform.position;
                    bullet.SetActive(true);
                    shot = bullet.GetComponent<Bullet>();
                    shot.SetEnemyToFollow(enemy, bulletDamage, bulletDistance);
                }
            }
        }
    }
    
    private GameObject GetClosestEnemy()
    {
        GameObject enemyToShoot = null;
        float minDistance = Mathf.Infinity;
        float distance = 0;
        Vector2 playerPosition = player.transform.position;
        Vector2 enemyPosition = Vector2.zero;

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")){
            if (!enemy.GetComponent<EnemyManager>().enemyDead)
            {
                enemyPosition = enemy.transform.position;
                distance = Vector2.Distance(playerPosition, enemyPosition);
    
                if (distance <= minDistance && distance <= bulletDistance)
                {
                    minDistance = distance;
                    enemyToShoot = enemy;
                }
            }
        }

        return enemyToShoot;
    }
    
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void SetShootCadence(float cadence)
    {
        this.cadence = cadence;
    }

    public void SetBulletsToShoot(float bulletsToShoot)
    {
        this.bulletsToShoot = bulletsToShoot;
    }
    
    public void SetBulletDamage(float damage)
    {
        bulletDamage = damage;
    }

    public void SetBulletDistance(float range)
    {
        bulletDistance = range;
    }
}
