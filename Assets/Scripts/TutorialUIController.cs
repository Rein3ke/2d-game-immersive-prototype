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
    private bool m_isMouseImageShown = false;
    private bool m_isSpacebarImageShown = false;
    
    public PlayerSettings PlayerSettings { get; set; }
    public bool ShowRightMouseButton { get; set; }
    public bool ShowSpacebar { get; set; }

    private void Update()
    {
        if (m_isOnCooldown) return;
        
        if (PlayerSettings.RightClickedCount < 5 && !m_isSpacebarImageShown && ShowRightMouseButton)
        {
            m_rightMouseClickImage.color = new Color(
                m_rightMouseClickImage.color.r,
                m_rightMouseClickImage.color.g,
                m_rightMouseClickImage.color.b,
                .8f
            );
            m_isMouseImageShown = true;
        }
        else
        {
            m_rightMouseClickImage.color = new Color(
                m_rightMouseClickImage.color.r,
                m_rightMouseClickImage.color.g,
                m_rightMouseClickImage.color.b,
                0f
            );
            m_isMouseImageShown = false;
        }

        if (PlayerSettings.SpacebarPressedCount < 5 && !m_isMouseImageShown && ShowSpacebar)
        {
            m_spacebarPressImage.color = new Color(
                m_spacebarPressImage.color.r,
                m_spacebarPressImage.color.g,
                m_spacebarPressImage.color.b,
                .8f
            );

            m_isSpacebarImageShown = true;
        }
        else
        {
            m_spacebarPressImage.color = new Color(
                m_spacebarPressImage.color.r,
                m_spacebarPressImage.color.g,
                m_spacebarPressImage.color.b,
                0f
            );

            m_isSpacebarImageShown = false;
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
