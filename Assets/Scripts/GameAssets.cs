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

    public GameObject playerUIPrefab;
    public GameObject levelPrefab_01;
    public PlayerSettings playerSettings_default;
    public GameSettings gameSettings_default;
    public Material defaultMaterial;
    public Material blur;
    public Material blur_foreground;
    public Material blur_background;
    public Material blur_interactables;
}
