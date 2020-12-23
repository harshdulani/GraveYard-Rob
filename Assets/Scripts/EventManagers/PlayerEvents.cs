using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    //Make sure your EventHandler class is on a gameobject
    public static PlayerEvents current;

    public Action playerDeath, playerBirth;
    public Action startCombatStrike, endCombatStrike;

    public Func<int, AttackType, bool> healthChange;
    public Func<int, bool> staminaChange;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(this);
    }

    public void InvokePlayerBirth()
    {
        playerBirth?.Invoke();
    }

    public void InvokePlayerDeath()
    {
        playerDeath?.Invoke();
    }

    public void InvokePlayerCombatStrikeStart()
    {
        startCombatStrike?.Invoke();
    }
    
    public void InvokePlayerCombatStrikeEnd()
    {
        endCombatStrike?.Invoke();
    }
    
    public bool InvokeHealthChange(int amount, AttackType type)
    {
        if (healthChange != null)
            return healthChange.Invoke(amount, type);
        
        //return false if health is not 0
        return false;
    }
    
    public bool InvokeStaminaChange(int amount)
    {
        if(staminaChange != null)
            return staminaChange.Invoke(amount);
        
        //return false if stamina is not 0
        return false;
    }
}