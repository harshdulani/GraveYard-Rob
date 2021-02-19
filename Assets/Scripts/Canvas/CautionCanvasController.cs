using System;
using UnityEngine;

public class CautionCanvasController : MonoBehaviour
{
    private GameObject _text;
    
    private void OnEnable()
    {
        EnemyEvents.current.infernalAttackCautionStart += Caution;
        EnemyEvents.current.infernalAttackCautionEnd += Safe;
    }

    private void OnDisable()
    {
        EnemyEvents.current.infernalAttackCautionStart -= Caution;
        EnemyEvents.current.infernalAttackCautionEnd -= Safe;
    }

    private void Start()
    {
        _text = transform.GetChild(0).gameObject;
        _text.SetActive(false);
    }

    private void Caution()
    {
        _text.SetActive(true);
    }

    private void Safe()
    {
        _text.SetActive(false);
    }
}
