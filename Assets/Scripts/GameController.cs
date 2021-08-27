using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Cursor
    [SerializeField]
    private Texture2D cursorTexture;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    private SceneController sceneController;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneController = GetComponent<SceneController>();

        Vector2 hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void loadNextScene()
    {
        sceneController.loadScene();
    }
}
