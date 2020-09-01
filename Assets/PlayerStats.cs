using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    #region Singleton
    //to make this a singleton
    private static PlayerStats _instance;

    private PlayerStats()
    {

    }

    void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(gameObject);

            //Awake code goes here

        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    public int playerHealth;
    public int maxHealth = 1000;

    public bool isAiming = false;

    private void Start()
    {
        playerHealth = maxHealth;
    }
}
