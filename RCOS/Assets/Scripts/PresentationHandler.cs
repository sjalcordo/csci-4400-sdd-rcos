using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using UnityEngine.UI;
using TMPro;
using Menus;

namespace Gameplay
{
    public class PresentationHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyHandler _lobbyHandler;
        [SerializeField] private MenuHandler _menuHandler;
        [SerializeField] private ProfileHandler _profileHandler;
        [Space(8)]
        [SerializeField] private RectTransform _sideSection;
        [SerializeField] private RectTransform _questionSection;
        [Space(8)]
        [SerializeField] private RawImage _prePresentPFP;
        [SerializeField] private TMP_Text _prePresentName;
        [Space(5)]
        [SerializeField] private RawImage _presentPFP;
        [SerializeField] private TMP_Text _presentName;
        [SerializeField] private TMP_Text _presentTextBox;
        [Space(5)]
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TMP_Text _promptText;
        [SerializeField] private TMP_Text _answerText;

        [Header("Parameters")]
        [SerializeField] private float _timeToPresenter = 3f;
        [SerializeField] private float _timeToPresentation = 10f;
        [SerializeField] private float _sideSectionOffset = -500f;
        [SerializeField] private float _timeBeforeAutoProgress = 20f;

        private string _currentPresenter;
        public string currentPresenter => _currentPresenter;
        private List<string> _users = new List<string>();

        private Prompt _currentPrompt;
        private int _currentPromptIndex;

        private Coroutine _advanceTimer;

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }
        private void OnSocket(string name, SocketIOResponse response)
        {
            if (name != "on-next-presentation-slide")
            {
                return;
            }

            OnFinishPrompt();
        }

        public void StartPresentations()
        {
            // Choose User
            _users = _lobbyHandler.hashedIPs;

            ChooseUser();

            // Menus
            SwitchToPrePresent();
            Invoke(nameof(SwitchToPresenter), _timeToPresenter);
            Invoke(nameof(SwitchToPresentation), _timeToPresenter + _timeToPresentation);
        }

        private void ChooseUser()
        {
            int index = Random.Range(0, _users.Count);
            _currentPresenter = _users[index];
            _users.RemoveAt(index);

            SetupPlayer(_currentPresenter);
        }

        private void SwitchToPrePresent()
        {
            _menuHandler.SwitchState(EMenuState.PrePresenting);
        }

        private void SwitchToPresenter()
        {
            _menuHandler.SwitchState(EMenuState.Presenter);
        }

        private void SwitchToPresentation()
        {
            _menuHandler.SwitchState(EMenuState.Presenting);
        }

        private void SetupPlayer(string hashedIP)
        {
            if (!_lobbyHandler.names.ContainsKey(hashedIP) || !_lobbyHandler.b64Textures.ContainsKey(hashedIP))
            {
                return;
            }

            _presentName.text = _lobbyHandler.names[hashedIP];
            _prePresentName.text = _lobbyHandler.names[hashedIP];

            Texture texture = b64toTex.convert(_lobbyHandler.b64Textures[hashedIP]);
            _presentPFP.texture = texture;
            _prePresentPFP.texture = texture;

            _presentTextBox.text = "";

            _currentPromptIndex = 0;
            SetupAnswer(_currentPromptIndex);
            _sideSection.anchoredPosition = Vector2.right * _sideSectionOffset;
            _questionSection.gameObject.SetActive(true);
        }

        private void SetupAnswer(int index)
        {
            if (!_profileHandler.playerResponses.ContainsKey(_currentPresenter)) {
                return;
            }

            _currentPrompt = _profileHandler.playerResponses[_currentPresenter][index];
            _questionText.text = "Question " + (index + 1);
            _promptText.text = _currentPrompt.prompt;
            _answerText.text = _currentPrompt.response;

            _advanceTimer = StartCoroutine(AutoFinishPrompt(_timeBeforeAutoProgress));
        }

        private void AddPromptToProfile(Prompt prompt)
        {
            string text = "";
            text += "<b>" + prompt.prompt + "</b>\n";
            text += prompt.response + "\n\n";
            _presentTextBox.text += text;
        }

        private void FinishAnswering()
        {
            _sideSection.anchoredPosition = Vector2.zero;
            _questionSection.gameObject.SetActive(false);

            if (_users.Count > 0)
            {
                Invoke(nameof(ChooseUser), 5f);
            }
            else
            {

            }
        }

        [ContextMenu("Finish Prompt")]
        private void OnFinishPrompt()
        {
            StopCoroutine(_advanceTimer);

            if (_currentPromptIndex >= _profileHandler.maxAnswers)
            {
                return;
            }

            AddPromptToProfile(_currentPrompt);

            _currentPromptIndex++;
            if (_currentPromptIndex >= _profileHandler.maxAnswers - 1)
            {
                FinishAnswering();
                return;
            }
            SetupAnswer(_currentPromptIndex);
        }

        private IEnumerator AutoFinishPrompt(float time)
        {
            yield return new WaitForSeconds(time);

            OnFinishPrompt();
        }
    }
}

