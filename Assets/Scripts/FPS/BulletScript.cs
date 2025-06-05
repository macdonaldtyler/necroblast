/**************************************************************************
 * Filename: BulletScript.cs
 * Original Author: Easy FPS asset
 * Modified By: Amir Tarbiyat B00882695
 * Description:
 *     This script handles the behavior of a bullet, including raycasting to
 *     detect collisions with objects, spawning decals or effects upon impact,
 *     and dealing damage to various enemy types (e.g., dummies, turrets,
 *     flying enemies). The script also ignores specified layers during the
 *     raycast. Heavily modified from the original to include additional 
 *     functionality and enemy interactions.
 * 
 **************************************************************************/


using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    [Tooltip("Furthest distance bullet will look for target")]
    public float maxDistance = 1000000;
    RaycastHit hit;
    [Tooltip("Prefab of wall damage hit. The object needs 'LevelPart' tag to create decal on it.")]
    public GameObject decalHitWall;
    [Tooltip("Decal will need to be slightly in front of the wall so it doesn't cause rendering problems. Best feel is from 0.01-0.1.")]
    public float floatInfrontOfWall;
    [Tooltip("Blood prefab particle this bullet will create upon hitting enemy")]
    public GameObject bloodEffect;
    [Tooltip("Put Weapon layer and Player layer to ignore bullet raycast.")]
    public LayerMask ignoreLayer;
    public int bulletDamage = 5; // Damage that bullet will deal

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
        {
            if (decalHitWall)
            {
                if (hit.transform.CompareTag("LevelPart"))
                {
                    Instantiate(decalHitWall, hit.point + hit.normal * floatInfrontOfWall, Quaternion.LookRotation(hit.normal));
                    Destroy(gameObject);
                }
                else if (hit.transform.CompareTag("Dummie"))
                {
                    // Spawn blood effect for dummy targets only
                    if (bloodEffect)
                    {
                        Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    }

                    // Deal damage to enemy
                    EnemyMovement enemy = hit.transform.GetComponent<EnemyMovement>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(bulletDamage);
                    }

                    Destroy(gameObject);
                }
                else if (hit.transform.CompareTag("Turret"))
                {
                    // Deal damage to turret without blood effect
                    TurretEnemy turret = hit.transform.GetComponent<TurretEnemy>();
                    if (turret != null)
                    {
                        turret.TakeDamage(bulletDamage);
                    }

                    Destroy(gameObject);
                }
                else if (hit.transform.CompareTag("FlyingEnemy"))
                {
                    // Deal damage to flying enemy without blood effect
                    FlyingEnemyMovement flyingEnemy = hit.transform.GetComponent<FlyingEnemyMovement>();
                    if (flyingEnemy != null)
                    {
                        flyingEnemy.TakeDamage(bulletDamage);
                    }

                    Destroy(gameObject);
                }
            }

            Destroy(gameObject);
        }
        Destroy(gameObject, 0.1f);
    }
}
