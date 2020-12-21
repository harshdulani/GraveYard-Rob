using System;
using UnityEngine;

public class DiagonalJumpRegulator : MonoBehaviour
{
    public enum SideOfTheMap
    {
        Left,
        Right,
        Front,
        Back
    }
    
    public SideOfTheMap mySideOfTheMap;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy")) return;
        if (!other.TryGetComponent(out EnemyDiagonalMovement diagonalMovement)) return;
        
        switch (mySideOfTheMap)
        {
            case SideOfTheMap.Left:
                diagonalMovement.availableAngles.Remove(315);
                diagonalMovement.availableAngles.Remove(225);
                break;
            case SideOfTheMap.Right:
                diagonalMovement.availableAngles.Remove(45);
                diagonalMovement.availableAngles.Remove(135);
                break;
            case SideOfTheMap.Front:
                diagonalMovement.availableAngles.Remove(45);
                diagonalMovement.availableAngles.Remove(315);
                break;
            case SideOfTheMap.Back:
                diagonalMovement.availableAngles.Remove(135);
                diagonalMovement.availableAngles.Remove(225);
                break;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy")) return;
        if (!other.TryGetComponent(out EnemyDiagonalMovement diagonalMovement)) return;
        
        switch (mySideOfTheMap)
        {
            case SideOfTheMap.Left:
                if(!diagonalMovement.availableAngles.Contains(315))
                    diagonalMovement.availableAngles.Add(315);
                if(!diagonalMovement.availableAngles.Contains(225))
                    diagonalMovement.availableAngles.Add(225);
                break;
            case SideOfTheMap.Right:
                if(!diagonalMovement.availableAngles.Contains(45))
                    diagonalMovement.availableAngles.Add(45);
                if(!diagonalMovement.availableAngles.Contains(135))
                    diagonalMovement.availableAngles.Add(135);
                break;
            case SideOfTheMap.Front:
                if(!diagonalMovement.availableAngles.Contains(45))
                    diagonalMovement.availableAngles.Add(45);
                if(!diagonalMovement.availableAngles.Contains(315))
                    diagonalMovement.availableAngles.Add(315);
                break;
            case SideOfTheMap.Back:
                if(!diagonalMovement.availableAngles.Contains(135))
                    diagonalMovement.availableAngles.Add(135);
                if(!diagonalMovement.availableAngles.Contains(225))
                    diagonalMovement.availableAngles.Add(225);
                break;
        }
    }
}
