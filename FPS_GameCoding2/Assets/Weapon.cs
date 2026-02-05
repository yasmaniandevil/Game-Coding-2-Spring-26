using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    public float bulletVelocity = 20f;

    // Update is called once per frame
    void Update()
    {
        //so it doesnt shoot before we pick it up, it has to be activre
        //when we click the left mouse button and we have the weapon and its active
        if(this.gameObject.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        //spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
        //shoot bullet
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPos.forward.normalized * bulletVelocity, ForceMode.Impulse);
        //destroy bullet after some time (after 3 seconds)
        Destroy(bullet, 3f);
    }
}
