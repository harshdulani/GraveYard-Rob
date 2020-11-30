using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    //Make sure your EventHandler class is on a gameobject
    public static PlayerEvents current;

    public Action playerDeath, playerBirth;
    public Action startCombatStrike, endCombatStrike;

    public Action updateHealth, updateStamina;

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
    
    public void InvokeHealthChange()
    {
        updateHealth?.Invoke();
    }
    
    public void InvokeStaminaChange()
    {
        updateStamina?.Invoke();
    }
}