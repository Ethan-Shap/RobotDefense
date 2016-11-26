using UnityEngine;
using System.Collections;

public class TowerDamage : MonoBehaviour 
{
    public enum DamageType { Projectile, AreaOfEffect, Healing }
    public DamageType damageType;

    // General Vars
    public Transform partToRotate;
    public bool rotateLaunchPos = false;
    private int damage;
    private Enemy currentEnemy;

    public float attackSpeed;
    private float nextAttackTime = 0;
    private float rotSpeed = 1f;

    private bool canAttack = false;

    private Tower tower;

    // Area of Effect Vars
    [Header("Area of effect Vars")]
    public float damageAngle;
    private ParticleSystem partSystem;

    // Projectile Vars
    [Header("Projectile Vars")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    private static Transform projectilesParent;
    private Projectile[] projectiles;

    private void Awake()
    {
        tower = GetComponent<Tower>();
        damage = tower.damage;

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
        }

        if(damageType == DamageType.Healing)
        {

        }

        nextAttackTime = 0;
    }

    private void Update()
    {
        if (rotateLaunchPos && tower.GetClosestEnemy())
            PointAtEnemy();

        currentEnemy = tower.GetCurrentEnemy();

        if (currentEnemy && CanAttack())
        {
            if (nextAttackTime <= Time.time)
            {
                nextAttackTime = Time.time + attackSpeed;
                // Deal damage to enemy
                if (damageType == DamageType.AreaOfEffect)
                {
                    currentEnemy.GetComponent<Health>().TakeDamage(tower.damage);
                    partSystem.Play();
                }
                else if (damageType == DamageType.Projectile)
                    ShootProjectile();
            }
        }
        else
        {
            if (damageType == DamageType.AreaOfEffect)
                partSystem.Stop();
        }
    }

    public void Pause()
    {
        if (damageType == DamageType.AreaOfEffect)
        {
            partSystem.Pause();
        }
        else if (damageType == DamageType.Projectile)
        {
            foreach(Projectile projectile in projectiles)
            {
                projectile.CanMove = false;
            }
        }
    }

    public void Unpause()
    {
        if (damageType == DamageType.AreaOfEffect)
        {
            partSystem.Play();
        }
        else if (damageType == DamageType.Projectile)
        {
            foreach (Projectile projectile in projectiles)
            {
                projectile.CanMove = true;
            }
        }
    }

    private void ShootProjectile()
    {
        Debug.Log("Shot");
        GameObject newProjectile = Instantiate(projectilePrefab, partToRotate.position, Quaternion.identity) as GameObject;
        newProjectile.AddComponent<Projectile>().SetProjectileVars(currentEnemy.transform, projectileSpeed, damage, true);
        newProjectile.transform.SetParent(projectilesParent);
    }

    private void PointAtEnemy()
    {
        Vector3 dir = tower.GetClosestEnemy().transform.position - partToRotate.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        partToRotate.transform.rotation = Quaternion.Lerp(partToRotate.transform.rotation, lookRot, 5f * Time.deltaTime);
    }

    private bool CanAttack()
    {
        return tower.IsPurchased() && tower.IsPreviewing();
    }

    private void CalculateAndCreateProjectiles()
    {
        float maxTravelTime = tower.range / projectileSpeed;
    }

}
