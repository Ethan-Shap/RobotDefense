using UnityEngine;
using System.Linq;

public class Projectile : MonoBehaviour {

    private Transform target;
    private float speed;
    private bool canMove = true;
    [SerializeField]
    private int damage;
    private bool initialized = false;
    private bool targetEnemies = false;

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return canMove;
        }

        set
        {
            canMove = value;
        }
    }

    private void LateUpdate()
    {
        if (initialized)
        {
            if (CanMove)
            {
                if (target && target.gameObject.activeInHierarchy)
                    MoveTowardsTarget();
                else
                {
                    target = targetEnemies == true ? GetClosestEnemy() : GetClosestTower();
                    if (!target && !target.gameObject.activeInHierarchy)
                        Destroy(gameObject); 
                }
            }
        }
    }

    private Transform GetClosestEnemy()
    {
        Enemy[] sortedEnemies = EnemyManager.instance.GetActiveEnemies();
        sortedEnemies = sortedEnemies.OrderBy(x => Vector3.SqrMagnitude(transform.position - x.transform.position)).ToArray();
        return sortedEnemies.Length == 0 ? null : sortedEnemies[0].transform;
    }

    private Transform GetClosestTower()
    {
        Tower[] sortedTowers = TowerManager.instance.GetActiveTowers().ToArray();
        sortedTowers = sortedTowers.OrderBy(x => Vector3.SqrMagnitude(transform.position - x.transform.position)).ToArray();
        return sortedTowers.Length == 0 ? null : sortedTowers[0].transform;
    }

    private void MoveTowardsTarget()
    {
        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * Speed);
        //Debug.Log(target.gameObject.activeInHierarchy);
    }

    public void SetProjectileVars(Transform target, float speed, int damage, bool targetEnemies)
    {
        this.target = target;
        this.Speed = speed;
        this.damage = damage;
        this.targetEnemies = targetEnemies;
        initialized = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (targetEnemies)
        {
            if (col.tag == "Enemy")
            {
                if (col.GetComponent<Health>())
                {
                    col.GetComponent<Health>().TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        } else
        {
            Debug.Log(col.tag);
            if (col.tag == "Tower")
            {
                if (col.GetComponent<Health>())
                {
                    col.GetComponent<Health>().TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}