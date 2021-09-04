using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeController : MonoBehaviour
{
    [SerializeField]
    Color decorationTargetColor;
    [SerializeField]
    Color enemyTargetColor;
    [SerializeField]
    Color interactableObjectTargetColor;

    private Color colorReset = new Color(255, 255, 255, 255); 
    private bool isColored = false;

    // Start is called before the first frame update
    void Start()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed += changeColor;
    }

    private void changeColor()
    {
        GameObject[] decorativeGameObjects = GameObject.FindGameObjectsWithTag("Decoration");
        GameObject[] enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] interactableGameObjects = GameObject.FindGameObjectsWithTag("InteractableObject");

        Debug.Log("Objects found: Decorative (" + decorativeGameObjects.Length + "), Enemies (" + enemyGameObjects.Length + "), Interactables (" + interactableGameObjects.Length + ")");

        SpriteRenderer spriteRenderer;
        if (!isColored)
        {
            foreach (GameObject gameObject in decorativeGameObjects)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = decorationTargetColor;
            }
            foreach (GameObject gameObject in enemyGameObjects)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = enemyTargetColor;
            }
            foreach (GameObject gameObject in interactableGameObjects)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = interactableObjectTargetColor;
            }

            isColored = true;
        }
        else
        {
            foreach (GameObject gameObject in decorativeGameObjects)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = colorReset;
            }
            foreach (GameObject gameObject in enemyGameObjects)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = colorReset;
            }
            foreach (GameObject gameObject in interactableGameObjects)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.color = colorReset;
            }

            isColored = false;
        }
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed -= changeColor;
    }
}
