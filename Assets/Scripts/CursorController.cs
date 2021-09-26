using System.Collections;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D cursorHitTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private Texture2D _currentTexture;

    private Vector2 _hotSpot;

    /// <summary>
    /// Standard unity method. Sets _hotSpot vector.
    /// </summary>
    private void Start()
    {
        _currentTexture = cursorTexture;
        
        _hotSpot = new Vector2(_currentTexture.width / 2.0f, _currentTexture.height / 2.0f);

        // Subscribe to events
        Level.i.onScoreChange += Level_OnScoreChange;
    }
    
    /// <summary>
    /// Standard unity method. Set the current cursor texture to the texture stored under _currentTexture.
    /// </summary>
    private void Update()
    {
        if (Application.isFocused)
        {
            Cursor.SetCursor(_currentTexture, _hotSpot, cursorMode);
        }
    }
    
    /// <summary>
    /// Starts the SwitchTexture() coroutine if the score changed.
    /// </summary>
    private void Level_OnScoreChange()
    {
        StartCoroutine(SwitchToHitTextureForSpecificTime(.2f));
    }

    /// <summary>
    /// Coroutine: Switches the texture of the cursor to HitTexture for specified time.
    /// </summary>
    /// <param name="duration">Duration in seconds</param>
    /// <returns>Nothing</returns>
    private IEnumerator SwitchToHitTextureForSpecificTime(float duration)
    {
        _currentTexture = cursorHitTexture;
        yield return new WaitForSeconds(duration);
        _currentTexture = cursorTexture;
        yield return null;
    }

    /// <summary>
    /// Unsubscribe from all events if destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Level.i.onScoreChange -= Level_OnScoreChange;
    }
}
