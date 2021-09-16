using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    [Header("Prefabs")]
    public GameObject playerUIPrefab;
    public GameObject levelPrefab_01;
    [Header("Settings")]
    public PlayerSettings playerSettings_default;
    [Space(4)]
    public GameSettings gameSettings_default;
    [Header("Materials")]
    public Material defaultMaterial;
    public Material blur;
    public Material blur_foreground;
    public Material blur_background;
    public Material universalBackgroundImage;
    public Material blur_interactables;
    public Material blur_enemies;
    public Material groundMaterial;
}
