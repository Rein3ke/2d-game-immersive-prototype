using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Setting")]
public class PlayerSettings : ScriptableObject
{
    // Player Stats
    [NonSerialized]
    public float playerHealth;
    public float playerMaxHealth;

    // Gun Stats
    [NonSerialized]
    public float playerAmmunition;
    public float playerMaxAmmunition;

    public float playerGunSpreadFactor;
}
