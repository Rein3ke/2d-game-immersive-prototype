/// <summary>
/// Interface to mark interactive objects as hitable.
/// </summary>
public interface IHitAble
{
    /// <summary>
    /// Called after the GameObject has been hit by a raycast.
    /// </summary>
    public void HandleHit();
}
