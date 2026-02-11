using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Vector3 bulletSpawnPos;
    public float bulletVelocity = 20f;
    public float bulletLifeTime = 3f;

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
        //the direction we want the bullet to go in
        Vector3 direction = transform.up;
        //we want to take the position of our weapon, add transform.up and then offset it a little bit so the bullet is in front
        bulletSpawnPos = transform.position + transform.up * 0.5f;
        //spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos, Quaternion.identity);
        //shoot bullet
        bullet.GetComponent<Rigidbody>().AddForce(direction * bulletVelocity, ForceMode.Impulse);
        //destroy bullet after some time (after 3 seconds)
        Destroy(bullet, bulletLifeTime);
    }
}
