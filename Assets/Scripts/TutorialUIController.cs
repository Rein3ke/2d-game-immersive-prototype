using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private Image m_rightMouseClickImage; 
    [SerializeField] private Image m_spacebarPressImage;

    private bool m_isOnCooldown = false;
    
    public PlayerSettings PlayerSettings { get; set; }

    private void Update()
    {
        if (m_isOnCooldown) return;
        
        if (PlayerSettings.RightClickedCount < 5)
        {
            m_rightMouseClickImage.color = new Color(
                m_rightMouseClickImage.color.r,
                m_rightMouseClickImage.color.g,
                m_rightMouseClickImage.color.b,
                .8f
            );
        }
        else
        {
            m_rightMouseClickImage.color = new Color(
                m_rightMouseClickImage.color.r,
                m_rightMouseClickImage.color.g,
                m_rightMouseClickImage.color.b,
                0f
            );
        }
        
        StartCoroutine(WaitForSeconds(5f));
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        m_isOnCooldown = true;
        yield return new WaitForSeconds(seconds);
        m_isOnCooldown = false;
    }
}
