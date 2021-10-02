using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class SceneController : MonoBehaviour
    {
        private int _sceneIndex;

        public static Scene CurrentScene => SceneManager.GetActiveScene();

        private void Start()
        {
            _sceneIndex = SceneManager.GetActiveScene().buildIndex;

            GameController.Instance.onLoadingMainMenuScene += LoadMenu;

            // Event Subscription
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
            GameController.Instance.onLoadingMainMenuScene -= LoadMenu;
        }
    }
}
