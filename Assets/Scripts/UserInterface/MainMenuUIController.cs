using Controller;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    /// <summary>
    /// A controller to be able to exchange panels and handle user input.
    /// </summary>
    public class MainMenuUIController : MonoBehaviour
    {
        public Button startButton;
        public Button introductionButton;
        public Button exitButton;
        public Button backButton;

        public VisualElement menuContainer;
        public VisualElement introductionContainer;
        
        /// <summary>
        /// Standard unity method. Sets all necessary references and click events.
        /// </summary>
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
            GameController.Instance.SceneController.LoadNextScene();
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
            GameController.CloseApplication();
        }
    }
}
