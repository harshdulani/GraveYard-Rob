using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class EnemyScreenShakes : MonoBehaviour
{
    [SerializeField] private float lightShake;
    [SerializeField] private float heavyShake;
    
    private CinemachineImpulseSource _impulse;

    private void Start()
    {
        _impulse = GetComponent<CinemachineImpulseSource>();
    }

    public void Light()
    {
        _impulse.GenerateImpulse(lightShake);
    }

    public void Heavy()
    {
        _impulse.GenerateImpulse(heavyShake);
    }
}
