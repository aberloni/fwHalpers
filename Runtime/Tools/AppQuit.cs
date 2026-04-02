using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.halpers.tools
{
    /// <summary>
    /// press DELETE to quit the app
    /// </summary>
    public class AppQuit : MonoBehaviour
    {
        /// <summary>
        /// pressing key will show a button to quit
        /// </summary>
        [SerializeField] bool guiFlow = false;
        bool _visible = false;

        public bool KeyQuitReleased
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return UnityEngine.InputSystem.Keyboard.current != null &&
                    UnityEngine.InputSystem.Keyboard.current.deleteKey.wasReleasedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
                return Input.GetKeyUp(KeyCode.Backspace);
#else
                return false;
#endif
            }
        }

        private void Update()
        {
            //combinaison ?
            if (KeyQuitReleased)
            {
                if (guiFlow) _visible = !_visible;
                else quit();
            }

        }

        private void OnGUI()
        {
            if (!_visible) return;

            if (GUI.Button(new Rect(10, 10, 150, 75), "quit"))
            {
                quit();
            }

        }

        private void quit()
        {
            Debug.LogWarning("app quit", this);

            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }

        }

    }
}

