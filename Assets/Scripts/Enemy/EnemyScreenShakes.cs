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

    public void FirstEnemy(int impulseStrength, float impulseSustainTime)
    {
        var oldTime = _impulse.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime;
        _impulse.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = impulseSustainTime;
        
        _impulse.GenerateImpulse(impulseStrength);
        
        _impulse.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = oldTime;
    }
}
