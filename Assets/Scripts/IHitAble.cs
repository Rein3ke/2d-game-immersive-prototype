using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitAble
{
    /// <summary>
    /// Called after the GameObject has been hit by a raycast.
    /// </summary>
    public void HandleHit();
}
