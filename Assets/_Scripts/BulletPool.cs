using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    public static BulletPool instance;
    private List<GameObject> pooledBullets = new List<GameObject>();
    private int bulletsAmount = 30;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        for(int i = 0; i <= bulletsAmount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            pooledBullets.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i <= pooledBullets.Count; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                return pooledBullets[i];
            }
        }

        return null;
    }
}
