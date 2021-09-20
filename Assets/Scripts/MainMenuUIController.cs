using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    public Button startButton;
    public Button introductionButton;
    public Button exitButton;
    public Button backButton;

    public VisualElement menuContainer;
    public VisualElement introductionContainer;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("start-button");
        introductionButton = root.Q<Button>("introduction-button");
        exitButton = root.Q<Button>("exit-button");
        backButton = root.Q<Button>("back-button");

        startButton.clicked += StartButtonPressed;
        introductionButton.clicked += IntroductionButtonPressed;
        exitButton.clicked += ExitButtonPressed;
        backButton.clicked += BackButtonPressed;

        menuContainer = root.Q<VisualElement>("menu-container");
        introductionContainer = root.Q<VisualElement>("introduction-container");
    }

    void StartButtonPressed()
    {
        GameController.CurrentGameController.SceneController.loadNextScene();
    }

    void IntroductionButtonPressed()
    {
        introductionContainer.style.display = DisplayStyle.Flex;
        menuContainer.style.display = DisplayStyle.None;
    }

    void BackButtonPressed()
    {
        introductionContainer.style.display = DisplayStyle.None;
        menuContainer.style.display = DisplayStyle.Flex;
    }

    void ExitButtonPressed()
    {
        GameController.CurrentGameController.ExitGame();
    }
}
