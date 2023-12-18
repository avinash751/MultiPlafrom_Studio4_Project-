using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullzeyeGameManager : GameManager
{
    private bool gameOverCondition;

    private void OnEnable()
    {
       TargetsNotHittDetector.TargetNotHitFound += CheckIfGameOver;
    }

    private void OnDisable()
    {
        TargetsNotHittDetector.TargetNotHitFound -= CheckIfGameOver;
    }

    protected override bool TransitionToGameOverState()
    {
       bool isInPlayState = base.TransitionToGameOverState();
        if (isInPlayState && gameOverCondition )
        {
            UpdateCurrentGameState(GameOverState);
            gameOverCondition = false;
            return true;
        }
        return false;
    }

    void CheckIfGameOver(int _currentLandTileAmount)
    {
        if (_currentLandTileAmount == 0)
        {
            gameOverCondition = true;
        }
    }

}

