using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public enum ECustomizationMenuState
    {
        None,
        Answers,
        Prompts,
        Timer
    }

    [System.Serializable]
    public struct CustomizationMenuObject
    {
        public ECustomizationMenuState state;
        public GameObject gameObject;
    }

    public class CustomizationMenuHandler : MonoBehaviour
    {
        [SerializeField] private CustomizationMenuObject _currentState;
        [SerializeField] private CustomizationMenuObject[] _menuObjs;

        private void Start()
        {
            DisableAllStates();
            SwitchState(ECustomizationMenuState.Answers);
        }

        private void DisableAllStates()
        {
            foreach (CustomizationMenuObject menuObj in _menuObjs)
            {
                menuObj.gameObject.SetActive(false);
            }
        }
        

        public void SwitchState(int state)
        {
            SwitchState((ECustomizationMenuState)state);
        }

        public void SwitchState(ECustomizationMenuState state)
        {
            // If we attempt to switch to the same state, simply return.
            if (_currentState.state == state)
                return;

            foreach (CustomizationMenuObject menuObj in _menuObjs)
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
        private void EnableState(CustomizationMenuObject menuObj)
        {
            _currentState = menuObj;
            menuObj.gameObject.SetActive(true);
        }
    }
}

