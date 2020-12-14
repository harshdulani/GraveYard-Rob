using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private Canvas _canvas;
    private Transform _mainCam;

    //private Quaternion _originalCanvasRotation;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
        _mainCam = Camera.main.transform;
        
        //for dota cam
        //_originalCanvasRotation = _canvas.transform.rotation;
    }

    private void LateUpdate()
    {
        //so that canvas always looks at camera in DotaCam
        //_canvas.transform.rotation = _originalCanvasRotation;

        _canvas.transform.rotation = Quaternion.LookRotation(_mainCam.forward, _mainCam.up);
    }
}
