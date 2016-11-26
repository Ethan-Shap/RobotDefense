using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour {

    public bool testZero;

    [SerializeField]
    private GameObject healthBarPrefab;

    [SerializeField]
    private int health = 100;
    private int defaultHealth;

    public bool showHealthBar = false;
    private List<Transform> healthBars;
    private Material fullBarMat;
    private Material emptyBarMat;

    private int CurrentHealth
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
            if(showHealthBar)
                UpdateHealthBar();
        }
    }

    private void Start()
    {
        healthBars = new List<Transform>();

        defaultHealth = CurrentHealth;
        if (showHealthBar)
        {
            CreateHealthBar();
            UpdateHealthBar();
        }
    }

    private void Update()
    {
        if (testZero)
        {
            TakeDamage(defaultHealth);
            testZero = false;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            if (GetComponent<Enemy>())
            {
                Player.instance.currentNumEnemiesKilled++;
                ResetHealth();
                this.GetComponent<Enemy>().Reset();
            } else if (GetComponent<Tower>())
            {
                GetComponent<Tower>().Pause();
            }
        }
    }

    public int GetHealth()
    {
        return CurrentHealth;
    }

    public void AddHealth(int health)
    {
        if (CurrentHealth == 0)
        {
            if (GetComponent<Tower>())
                GetComponent<Tower>().Unpause();
        }
     
        if (health > 0)
            this.CurrentHealth += health;
    }

    private void ResetHealth()
    {
        CurrentHealth = defaultHealth;
    }

    private void UpdateHealthBar()
    {
        float fullPercent = (float)CurrentHealth / defaultHealth;
        float emptyPercent = (float)CurrentHealth % defaultHealth;

        //Debug.Log(health);
        //Debug.Log(CurrentHealth);
        //Debug.Log(defaultHealth);

        //Debug.Log(fullPercent);
        //Debug.Log(emptyPercent);

        int full = (int)(10 * fullPercent);
        int empty = 10 - full;

        //Debug.Log("Full " + full);
        //Debug.Log("Empty " + empty);

        Debug.Log(healthBars.Count);

        for(int i = 0; i < 10; i++)
        {
            if(i < full)
            {
                healthBars[9 - i].GetComponent<Renderer>().material = fullBarMat;
            } else
            {
                healthBars[9 - i].GetComponent<Renderer>().material = emptyBarMat;
            }
        }
    }

    private void CreateHealthBar()
    { 
        GameObject healthBar = (GameObject)Instantiate(healthBarPrefab, transform);

        Transform[] healthBarsWithParent = healthBar.transform.Find("Bars").GetComponentsInChildren<Transform>();

        fullBarMat = healthBarsWithParent[healthBarsWithParent.Length - 1].GetComponent<Renderer>().material;
        emptyBarMat = healthBarsWithParent[1].GetComponent<Renderer>().material;

        foreach (Transform currentHB in healthBarsWithParent)
        {
            if (currentHB.transform != healthBar.transform.Find("Bars"))
            {
                healthBars.Add(currentHB);
            }
        }

        if (GetComponent<Tower>())
        {
            healthBar.transform.position = Constants.towerOffset + transform.position;
        }
    }

    internal class Constants
    {
        public static Vector3 towerOffset = new Vector3(0, 2f, 0);
        public static float ringsOffset = 0.0030154f;
        public static float healthBarOffset = 0.001507702f;
        public static float containerOffset = 0.00340854f;
    }

}