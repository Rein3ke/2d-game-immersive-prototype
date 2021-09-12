using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeController : MonoBehaviour
{
    public GameSettings GameSettings
    {
        set => _gameSettings = value;
    }
    private GameSettings _gameSettings;

    void Start()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed += OnSpacebarPressed;
        Level.i.onStateChange += OnStateChange;
    }

    private void OnStateChange()
    {
        
    }

    private void OnSpacebarPressed()
    {
        
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed -= OnSpacebarPressed;
        Level.i.onStateChange -= OnStateChange;
    }
}
