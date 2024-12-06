/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Handles the transition between different menu sections.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public enum EMenuState
    {
        None,
        MainMenu,
        Lobby,
        Progress,
        PrePresenting,
        Presenter,
        Presenting,
        Graph,
        PostGame,
        Customization,
    }

    [System.Serializable]
    public struct MenuObject
    {
        public EMenuState state;
        public GameObject gameObject;
    }

    public class MenuHandler : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private MenuObject _currentState;
        [SerializeField] private MenuObject[] _menuObjs;
        [Space(5)]
        [SerializeField] private float _delayedTime = 5f;

        private void Start()
        {
            DisableAllStates();
            SwitchState(EMenuState.MainMenu);
        }

        /// <summary>
        /// Disables all of the states before they will be set to be active.
        /// </summary>
        private void DisableAllStates()
        {
            foreach (MenuObject menuObj in _menuObjs)
            {
                menuObj.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Reloads the scene.
        /// </summary>
        public void ReloadScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        /// <summary>
        /// Quits the game.
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Switches the state with a given delay as they cannot be easily passed through buttons.
        /// </summary>
        /// <param name="state"></param>
        public void SwitchStateDelayed(int state)
        {
            StartCoroutine(DelayedSwitch(state, _delayedTime));
        }

        /// <summary>
        /// Switches the state of the menu through an integer.
        /// </summary>
        /// <param name="state"></param>
        public void SwitchState(int state)
        {
            SwitchState((EMenuState)state);
        }

        /// <summary>
        /// Switches the state by disabling the previous state and enabling the next state.
        /// </summary>
        /// <param name="state"></param>
        public void SwitchState(EMenuState state)
        {
            // If we attempt to switch to the same state, simply return.
            if (_currentState.state == state)
                return;

            foreach (MenuObject menuObj in _menuObjs)
            {
                // If the menu state does not match
                if (menuObj.state != state)
                    continue;

                DisableCurrentState();
                EnableState(menuObj);
            }
        }

        /// <summary>
        /// This is called when we disable the current state, currently simply deactivates the current state object.
        /// </summary>
        private void DisableCurrentState()
        {
            if (_currentState.gameObject == null) return;
            _currentState.gameObject.SetActive(false);
        }

        /// <summary>
        /// This will activate a new state.
        /// </summary>
        /// <param name="menuObj"></param>
        private void EnableState(MenuObject menuObj)
        {
            _currentState = menuObj;
            menuObj.gameObject.SetActive(true);
        }

        IEnumerator DelayedSwitch(int state, float time)
        {
            yield return new WaitForSeconds(time);
            SwitchState(state);
        }
    }
}

