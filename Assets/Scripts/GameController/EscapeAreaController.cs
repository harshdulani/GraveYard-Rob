using UnityEngine;

public class EscapeAreaController : MonoBehaviour
{
    public GameObject youWinCanvas, tutorialCanvas, playerCanvas, targetCanvas, objectiveCanvas, endCamera;

    private bool _shouldStopTime;
    
    private void Update()
    {
        if (!_shouldStopTime) return;

        if (Time.timeScale > 0.1f)
            Time.timeScale -= 0.1f;
        else
            Time.timeScale = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("WheelBarrow")) return;
        
        if(!WheelBarrowController.main.isLifted) return;
        
        _shouldStopTime = true;
        
        youWinCanvas.SetActive(true);
        
        tutorialCanvas.SetActive(false);
        playerCanvas.SetActive(false);
        targetCanvas.SetActive(false);
        objectiveCanvas.SetActive(false);
        
        MovementInput.current.TakeAwayMovementControl();
        
        MovementInput.current.GetComponent<PlayerController>().tpsCamera.gameObject.SetActive(false);
        endCamera.SetActive(true);
    }
}
