using UnityEngine;

namespace Assets
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets I
        {
            get
            {
                if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject)?.GetComponent<GameAssets>();
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
        public Material universalDefault;
        public Material universalForeground;
        public Material universalBackground;
        public Material universalBackgroundImage;
        public Material universalInteractables;
        public Material universalEnemies;
        public Material universal3dDefault;
    }
}