using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public enum EMenuState
    {
        None,
        MainMenu,
        Lobby
    }

    [System.Serializable]
    public struct MenuObject
    {
        public EMenuState state;
        public GameObject gameObject;
    }

    public class MenuHandler : MonoBehaviour
    {
        [SerializeField] private MenuObject _currentState;
        [SerializeField] private MenuObject[] _menuObjs;

        private void Start()
        {
            DisableAllStates();
            SwitchState(EMenuState.MainMenu);
        }

        private void DisableAllStates()
        {
            foreach (MenuObject menuObj in _menuObjs)
            {
                menuObj.gameObject.SetActive(false);
            }
        }

        public void SwitchState(int state)
        {
            SwitchState((EMenuState)state);
        }

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
    }
}

