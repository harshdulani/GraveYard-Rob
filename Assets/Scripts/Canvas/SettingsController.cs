using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Text sensitivityText;
    public VolumeProfile low, high;
    
    private float _initialMaxSpeedX;
    private bool _isInitialised;

    private CinemachineFreeLook _cm;
    private Volume _volume;
    
    private void Update()
    {
        if (!_isInitialised)
            _isInitialised = Initialise();
    }

    public void OnSensitivityUpdate(float value)
    {
        if (value <= 0.15f)
            sensitivityText.text = "Min";
        else if(value >= 0.85f)
            sensitivityText.text = "Max";
        else
            sensitivityText.text = value.ToString("0.00");
        
        _cm.m_XAxis.m_MaxSpeed = value * _initialMaxSpeedX;
    }

    private bool Initialise()
    {
        foreach (var controller in GameObject.FindGameObjectsWithTag("GameController"))
        {
            if (controller.name == "LevelFlowController")
            {
                _cm = controller.GetComponent<LevelFlowController>().tpsCamera;
                _initialMaxSpeedX = _cm.m_XAxis.m_MaxSpeed;
            }

            if (controller.name == "Global Volume")
            {
                _volume = controller.GetComponent<Volume>();
            }
        }

        return _cm && _volume;
    }

    public void OnPostProcessingToggle(bool toggle)
    {
        _volume.profile = toggle ? high : low;
    }
}