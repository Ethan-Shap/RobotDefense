using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Enemy : MonoBehaviour {

    public EnemyManager.EnemyType type;

    public Path currentPath;
    public bool timing = false;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float range = 1;
    [SerializeField]
    private int damage = 1;

    private float defaultSpeed;
    private int currentWaypoint = 0;
    private List<Tower> sortedTowers;

    private Tower currentTower;

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

    public float DefaultSpeed
    {
        get
        {
            return defaultSpeed;
        }

        set
        {
            defaultSpeed = value;
        }
    }

    public int Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    private void Start()
    {
        DefaultSpeed = Speed;
    }

	// Update is called once per frame
	void Update ()
    {
        if (currentPath)
        {
            MoveTowardsWaypoint();
            SortNearestTowers();
        }
	}

    private void MoveTowardsWaypoint()
    {
        if ((int)speed > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentPath.GetWaypointPosition(currentWaypoint).position, speed * Time.deltaTime);

            if (currentPath.WithinDistance(transform.position, currentWaypoint))
            {
                currentWaypoint = currentPath.NextWaypoint(currentWaypoint);
            }

            if (currentWaypoint < 0)
            {
                Reset();
            }
        }
    }

    public void Reset()
    {
        currentWaypoint = 0;
        currentPath = null;
        EnemyManager.instance.ResetPosition(this);
    }

    public Tower GetClosestTower()
    {
        return sortedTowers[0];
    }

    public Tower GetCurrentEnemy()
    {
        return currentTower;
    }

    private void SortNearestTowers()
    {
        sortedTowers = TowerManager.instance.GetActiveTowers();
        sortedTowers = sortedTowers.OrderBy(x => Vector3.SqrMagnitude(transform.position - x.transform.position)).ToList();

        if (sortedTowers.Count > 0)
        {
            if (Vector3.SqrMagnitude(transform.position - sortedTowers[0].transform.position) <= range * range)
            {
                currentTower = sortedTowers[0];
            }
            else
            {
                currentTower = null;
            }
        }
    }
}
