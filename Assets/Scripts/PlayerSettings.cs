using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Setting")]
public class PlayerSettings : ScriptableObject, ISerializationCallbackReceiver
{
    [NonSerialized]
    public float score;
    // Player Stats
    [NonSerialized]
    public float playerHealth;
    [Header("Player Stats")]
    public float playerMaxHealth;
    [Space(8)]
    // Gun Stats
    [NonSerialized]
    public float playerAmmunition;
    [Header("Gun Stats")]
    public float playerMaxAmmunition;
    public float playerGunSpreadFactor;

    public void OnAfterDeserialize()
    {
        playerHealth = playerMaxHealth;
        playerAmmunition = playerMaxAmmunition;
        score = 0.0f;
    }

    public void OnBeforeSerialize()
    {
    }
}
