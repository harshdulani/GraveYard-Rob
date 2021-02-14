using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Text sensitivityText;
    
    private float _initialMaxSpeedX;
    private bool _isInitialised;

    private CinemachineFreeLook _cm;
    
    private void Update()
    {
        if (!_isInitialised)
            _isInitialised = FindCinemachine();
    }

    public void OnSensitivityUpdate(float value)
    {
        if (value <= 0.15f)
            sensitivityText.text = "Min";
        else if(value >= 0.95f)
            sensitivityText.text = "Max";
        else
            sensitivityText.text = value.ToString("0.00");
        
        _cm.m_XAxis.m_MaxSpeed = value * _initialMaxSpeedX;
        print("set to " + _cm.m_XAxis.m_MaxSpeed);
    }

    private bool FindCinemachine()
    {
        foreach (var controller in GameObject.FindGameObjectsWithTag("GameController"))
        {
            if (controller.name != "LevelFlowController") continue;
            
            _cm = controller.GetComponent<LevelFlowController>().tpsCamera;
            _initialMaxSpeedX = _cm.m_XAxis.m_MaxSpeed;
            return true;
        }
        return false;
    }
}
