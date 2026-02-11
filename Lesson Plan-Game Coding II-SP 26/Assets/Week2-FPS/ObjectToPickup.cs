using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToPickup : MonoBehaviour
{
    public GameObject bulletPrefab;

    //public Transform bulletSpawnPos;

    public float bulletVelocity = 30f;

    public float bulletPrefabLifeTime = 3f;

    private Vector3 bulletSpawnPosition;

 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //so it doesnt shoot before we pick it up, has to be active
        if (this.gameObject.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        //spawn bullet
        Vector3 direction = transform.up;
        bulletSpawnPosition = transform.position + transform.up * 0.5f ;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
        //shoot bullet
        bullet.GetComponent<Rigidbody>().AddForce(direction * bulletVelocity, ForceMode.Impulse);
                                                  
        //destroy bullet after sometime
        Destroy(bullet, bulletPrefabLifeTime);
    }
}
