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
        [SerializeField] private VotingHandler _votingHandler;
        [SerializeField] private RankHandler _rankHandler;
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

        private bool _userIsPresenting = false;
        private bool _canAdvancePrompt;

        private bool _answerShown;

        private void Start()
        {
            // Adds a listener to when we receive a message from the server.
            Sockets.ServerUtil.manager.OnSocketEvent.AddListener(OnSocket);
        }
        private void OnSocket(string name, SocketIOResponse response)
        {
            if (name != "on-next-presentation-slide" && name != "on-get-presenter-name")
            {
                return;
            }

            switch (name)
            {
                case "on-next-presentation-slide":
                    OnFinishPrompt();
                    break;
                case "on-get-presenter-name":
                    Sockets.ServerUtil.manager.SendEvent("send-presenter-name", _lobbyHandler.names[_currentPresenter]);
                    break;
            }
        }

        public void StartPresentations()
        {
            // Choose User
            _users = new List<string>(_lobbyHandler.hashedIPs);

            SetupNewPresentation();
        }

        private void SetupNewPresentation()
        {
            ChooseUser();

            // Menus
            SwitchToPrePresent();
            Invoke(nameof(SwitchToPresenter), _timeToPresenter);
            Invoke(nameof(SendUserPresenting), _timeToPresenter);
            Invoke(nameof(SendUserVoting), _timeToPresenter + _timeToPresentation);
            Invoke(nameof(SwitchToPresentation), _timeToPresenter + _timeToPresentation);
        }

        private void ChooseUser()
        {
            int index = Random.Range(0, _users.Count);
            _currentPresenter = _users[index];
            _users.RemoveAt(index);

            SetupPlayer(_currentPresenter);
            _userIsPresenting = false;
        }

        private void SendUserPresenting()
        {
            Sockets.ServerUtil.manager.SendEvent("presentation-start", _currentPresenter);
        }

        private void SendUserVoting()
        {
            _votingHandler.StartVoting();
            Sockets.ServerUtil.manager.SendEvent("voting-start", _currentPresenter);
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
            _userIsPresenting = true;
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

            _answerText.gameObject.SetActive(false);
            _answerShown = false;

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

            Invoke(nameof(ShowGraph), 5f);
        }

        private void ShowGraph()
        {
            _menuHandler.SwitchState(EMenuState.Graph);
            _votingHandler.SendToGraph(_currentPresenter);

            _votingHandler.StopVoting();

            Sockets.ServerUtil.manager.SendEvent("move-all-to-waiting");

            Invoke(nameof(PostGraph), 10f);
        }

        private void PostGraph()
        {
            if (_users.Count > 0)
            {
                Sockets.ServerUtil.manager.SendEvent("between-presentations");
                SetupNewPresentation();
            }
            else
            {
                _rankHandler.StartPostGame();
            }
        }

        [ContextMenu("Finish Prompt")]
        private void OnFinishPrompt()
        {
            if (!_canAdvancePrompt || !_userIsPresenting) { return; }

            StopCoroutine(_advanceTimer);

            if (_currentPromptIndex >= _profileHandler.maxAnswers)
            {
                return;
            }


            if (!_answerShown)
            {
                _answerText.gameObject.SetActive(true);
                _answerShown = true;

                _advanceTimer = StartCoroutine(AutoFinishPrompt(_timeBeforeAutoProgress));
                return;
            }

            AddPromptToProfile(_currentPrompt);

            _currentPromptIndex++;
            if (_currentPromptIndex >= _profileHandler.maxAnswers)
            {
                FinishAnswering();
                return;
            }
            SetupAnswer(_currentPromptIndex);
        }

        private IEnumerator AutoFinishPrompt(float time)
        {
            _canAdvancePrompt = false;
            if (time > 1f)
            {
                time -= 1f;
                yield return new WaitForSeconds(1f);
                _canAdvancePrompt = true;
            }
            else
            {
                _canAdvancePrompt = true;
            }

            yield return new WaitForSeconds(time);

            OnFinishPrompt();
        }
    }
}

