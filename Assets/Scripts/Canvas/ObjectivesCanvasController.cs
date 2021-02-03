using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesCanvasController : MonoBehaviour
{
    public List<string> objectiveTexts;

    [Header("UI elements")] 
    public Text objectiveTitle;
    public Animation bgFlasher;

    public SlideIntoScreen objectiveCanvas, playerCanvas;
    
    private bool _hasSlidIn;

    private void OnEnable()
    {
        GameFlowEvents.current.updateObjective += UpdateObjective;
        GameFlowEvents.current.gameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameFlowEvents.current.updateObjective -= UpdateObjective;
        GameFlowEvents.current.gameOver -= OnGameOver;
    }

    private void Start()
    {
        objectiveTitle.text = objectiveTexts[GameStats.current.currentObjective];
        
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_hasSlidIn) return;
        if(!GameStats.current.isGamePlaying) return;

        objectiveCanvas.StartSliding();
        _hasSlidIn = true;
    }
    
    private void UpdateObjective()
    {
        if(!GameStats.current.isPlayerAlive) return;
        
        if(GameStats.current.currentObjective < objectiveTexts.Count)
            objectiveTitle.text = objectiveTexts[++GameStats.current.currentObjective];

        bgFlasher.Play();
        
        switch (GameStats.current.currentObjective)
        {
            case 1:
                transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                playerCanvas.StartSliding();
                break;
        }
    }

    private void OnGameOver()
    {
        gameObject.SetActive(false);
    }
}