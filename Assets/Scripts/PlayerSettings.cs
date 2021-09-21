using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Setting")]
public class PlayerSettings : ScriptableObject, ISerializationCallbackReceiver
{
    [NonSerialized] public float Score;
    [NonSerialized] public int RightClickedCount;
    [NonSerialized] public int SpacebarPressedCount;
    // Player Stats
    [NonSerialized] public float PlayerHealth;
    [Header("Player Stats")]
    public float playerMaxHealth;
    [Space(8)]
    // Gun Stats
    [NonSerialized] public float PlayerAmmunition;
    [Header("Gun Stats")]
    public float playerMaxAmmunition;
    public float playerGunSpreadFactor;
    public float playerReloadTime;
    [Header("Gun Sounds")]
    public AudioClip gunShotAudioClip;
    public AudioClip gunEmptyAudioClip;
    [Space(8)]
    public AudioClip gunReloadAudioClip;
    public AudioClip gunPostReloadAudioClip;

    public void OnAfterDeserialize()
    {
        PlayerHealth = playerMaxHealth;
        PlayerAmmunition = playerMaxAmmunition;
        Score = 0.0f;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnEnable()
    {
        PlayerHealth = playerMaxHealth;
        PlayerAmmunition = playerMaxAmmunition;
        Score = 0.0f;
    }
}
