using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour 
{
    public enum DamageType { Projectile, AreaOfEffect }
    public DamageType damageType;

    // General Vars
    public Transform partToRotate;
    public bool rotateLaunchPos = false;
    private int damage;
    private Tower currentTower;

    public float attackSpeed;
    private float nextAttackTime = 0;
    private float rotSpeed = 1f;

    private bool canAttack = false;

    private Enemy enemy;

    // Area of Effect Vars
    [Header("Area of effect Vars")]
    public float damageAngle;
    private ParticleSystem partSystem;

    // Projectile Vars
    [Header("Projectile Vars")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    private static Transform projectilesParent;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        damage = enemy.Damage;

        if (damageType == DamageType.AreaOfEffect)
        {
            partSystem = GetComponentInChildren<ParticleSystem>();
        }

        if (damageType == DamageType.Projectile)
        {
            if (!projectilePrefab)
                throw new System.NullReferenceException("No projectile for " + name + " to shoot");

            // makes the hierarchy less cluttered with projectiles
            if(!projectilesParent)
                projectilesParent = new GameObject("Projectiles").transform;

            nextAttackTime = 0;
        }
    }
     

    private void Update()
    {
        if (rotateLaunchPos && enemy.GetClosestTower())
            PointAtEnemy();

        currentTower = enemy.GetCurrentEnemy();
        if (currentTower && CanAttack())
        {
            if (nextAttackTime <= Time.time)
            {
                nextAttackTime = Time.time + attackSpeed;

                // Deal damage to enemy
                if (damageType == DamageType.AreaOfEffect)
                    currentTower.GetComponent<Health>().TakeDamage(enemy.Damage);
                else if (damageType == DamageType.Projectile)
                    ShootProjectile();
            }
            
            if (damageType == DamageType.AreaOfEffect)
                partSystem.Play();
        }
        else
        {
            if (damageType == DamageType.AreaOfEffect)
                partSystem.Stop();
        }
    }

    public void RemoveProjectiles()
    {
        foreach (Projectile projectile in projectilesParent.GetComponentsInChildren<Projectile>())
        {
            Destroy(projectile.gameObject);
        }
    }

    private void ShootProjectile()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, partToRotate != null ? partToRotate.position : transform.position , Quaternion.identity) as GameObject;
        newProjectile.AddComponent<Projectile>().SetProjectileVars(currentTower.transform, projectileSpeed, damage, false);
        newProjectile.transform.SetParent(projectilesParent);
    }

    private void PointAtEnemy()
    {
        Vector3 dir = currentTower.transform.position - partToRotate.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        partToRotate.transform.rotation = Quaternion.Lerp(partToRotate.transform.rotation, lookRot, 5f * Time.deltaTime);
    }

    private bool CanAttack()
    {
        return enemy.gameObject.activeInHierarchy;
    }
}
