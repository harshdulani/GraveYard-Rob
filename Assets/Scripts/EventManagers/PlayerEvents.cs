using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    //Make sure your EventHandler class is on a gameobject
    public static PlayerEvents current;

    public Action playerDeath, playerBirth;
    public Action startCombatStrike, endCombatStrike;

    public Func<float, bool> healthChange, staminaChange;

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
    
    public bool InvokeHealthChange(float amount)
    {
        if (healthChange != null)
            return healthChange.Invoke(amount);
        
        //return false if health is not 0
        return false;
    }
    
    public bool InvokeStaminaChange(float amount)
    {
        if(staminaChange != null)
            return staminaChange.Invoke(amount);
        
        //return false if stamina is not 0
        return false;
    }
}