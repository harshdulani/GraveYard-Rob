using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashRepeatedly : MonoBehaviour
{
    public int doTimes = 2;
    public AnimationCurve curve;

    public bool _isFlashing;
    private int _doneTimes;
    private float _time, _endTime;

    private Image _fillBar, _siblingBar;

    private void Start()
    {
        _fillBar = GetComponent<Image>();
        _siblingBar = transform.parent.GetChild(0).GetComponent<Image>();
        
        _endTime = curve.keys[curve.keys.Length - 1].time;
    }

    private void Update()
    {
        if (_isFlashing)
            Flash();
    }

    public void StartFlashing()
    {
        if(!_isFlashing)
            _isFlashing = true;
    }

    private void Flash()
    {
        _time += Time.deltaTime;

        _fillBar.fillAmount = _siblingBar.fillAmount;
        
        //flash
        var value = curve.Evaluate(_time);
        _fillBar.color = new Color(value, value, value, value);
        
        if (_time < _endTime) return;
        _time = 0f;
        
        if(_doneTimes++ >= doTimes)
            _isFlashing = false;
    }
}
