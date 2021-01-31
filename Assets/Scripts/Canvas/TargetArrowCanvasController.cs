using UnityEngine;

public class TargetArrowCanvasController : MonoBehaviour
{
    public int targetObjective = 1;

    private GameObject _arrowParent;
    private bool _arrowsShown;
    
    private void OnEnable()
    {
        GameFlowEvents.current.updateObjective += UpdateObjective;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.updateObjective -= UpdateObjective;
    }

    private void Start()
    {
        _arrowParent = transform.GetChild(0).gameObject;
        _arrowParent.SetActive(false);
    }

    private void UpdateObjective()
    {
        if (!GameStats.current.isPlayerAlive) return;

        //using this bandage fix - because i don't know how to guarantee a specific subscriber to be called first in observer pattern
        //hence no way to guarantee that when this subscriber is called, it has the required currentObjective
        //you will find bandages for this issue in this design pattern, in more files like FindTargetGrave, ObjectiveCanvasController
        //no don't open this code after weeks/months and try to repair this.

        if (!_arrowsShown)
        {
            _arrowParent.SetActive(true);
            _arrowsShown = true;
            print("arrows shown");
        }
        else
        {
            print("arrows hid");
            if (_arrowParent.activeSelf)
                _arrowParent.SetActive(false);
        }
    }
}
