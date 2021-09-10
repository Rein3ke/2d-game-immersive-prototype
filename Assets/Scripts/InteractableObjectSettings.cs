using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interactable Object Setting")]
public class InteractableObjectSettings : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("The sprite that will be used after the handleHit method is executed.")]
    public Sprite sprite;
    [Space(8)]
    public AudioClip hitSoundClip;
    [Space(8)]
    public RuntimeAnimatorController animationController;
    [Space(8), Tooltip("The score gained after the object was hit. Always has to be positive.")]
    public float score;

    public void OnAfterDeserialize()
    {
        if (score < 0) score = 0;
    }

    public void OnBeforeSerialize()
    {
        
    }
}
