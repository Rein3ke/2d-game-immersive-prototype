using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    /// <summary>
    /// Controller that manages the loading of scenes.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        private int _sceneIndex;

        public static Scene CurrentScene => SceneManager.GetActiveScene();

        private void Start()
        {
            _sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Event Subscription
            GameController.Instance.onLoadingMainMenuScene += LoadMenu;
        }

        public void LoadNextScene()
        {
            if (_sceneIndex == (SceneManager.sceneCountInBuildSettings - 1))
            {
                LoadMenu();
            } else
            {
                SceneManager.LoadScene(++_sceneIndex);
            }
        }

        private void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            GameController.Instance.onLoadingMainMenuScene -= LoadMenu;
        }
    }
}
