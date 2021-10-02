using System;
using ScriptableObjects;
using UnityEngine;

namespace Controller
{
    public class InputController : MonoBehaviour
    {
        public PlayerSettings PlayerSettings { get; set; }

        /// <summary>
        /// Standard unity method. Handles all inputs and invokes the corresponding event.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                KeyEscapeDown();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpaceDown();
            }
            if(Input.GetKeyUp(KeyCode.Space))
            {
                SpaceLeft();
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                LeftDown();
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                RightDown();
            }
            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseDown();
            }
            if (Input.GetMouseButtonDown(1))
            {
                RightMouseDown();
            }
            if (Input.GetMouseButtonUp(1))
            {
                RightMouseUp();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                KeyRDown();
            }
        }

        #region Events
        public event Action onKeyEscapeDown;
        /// <summary>
        /// Call event if escape key was pressed.
        /// </summary>
        private void KeyEscapeDown()
        {
            onKeyEscapeDown?.Invoke();
        }

        public event Action onSpaceDown;
        /// <summary>
        /// Call event if space bar was pressed. Count up the space bar counter in PlayerSettings.
        /// </summary>
        private void SpaceDown()
        {
            PlayerSettings.SpacebarPressedCount++;
            onSpaceDown?.Invoke();
        }

        public event Action onSpaceUp;
        /// <summary>
        /// Call event if space bar was left.
        /// </summary>
        private void SpaceLeft()
        {
            onSpaceUp?.Invoke();
        }

        public event Action onLeftDown;
        /// <summary>
        /// Call event if left arrow key or A key was pressed.
        /// </summary>
        private void LeftDown()
        {
            onLeftDown?.Invoke();
        }

        public event Action onRightDown;
        /// <summary>
        /// Call event if right arrow key or D key was pressed.
        /// </summary>
        private void RightDown()
        {
            onRightDown?.Invoke();
        }

        public event Action onLeftMouseDown;
        /// <summary>
        /// Call event if left mouse button was pressed.
        /// </summary>
        private void LeftMouseDown()
        {
            onLeftMouseDown?.Invoke();
        }

        public event Action onRightMouseDown;
        /// <summary>
        /// Call event if right mouse button was pressed. Count up the right mouse click counter in PlayerSettings.
        /// </summary>
        private void RightMouseDown()
        {
            PlayerSettings.RightClickedCount++;
            onRightMouseDown?.Invoke();
        }

        public event Action onRightMouseUp;
        /// <summary>
        /// Call event if left mouse button was left.
        /// </summary>
        private void RightMouseUp()
        {
            onRightMouseUp?.Invoke();
        }

        public event Action onKeyRDown;
        /// <summary>
        /// Call event if R key was pressed.
        /// </summary>
        private void KeyRDown()
        {
            onKeyRDown?.Invoke();
        }
        #endregion
    }
}
