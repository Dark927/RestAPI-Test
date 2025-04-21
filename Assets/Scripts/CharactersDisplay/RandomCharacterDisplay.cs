using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CharactersDisplay
{
    public class RandomCharacterDisplay : MonoBehaviour, IDisposable
    {
        #region Fields 

        private const string DefaultCharactersHttpURL = "https://anapioficeandfire.com/api/characters";
        private const int MaxRandomCharacterIndex = 100;

        [Header("Panel Settings")]
        [SerializeField] private bool _generateOnStart = true;
        [SerializeField] private TextMeshProUGUI _characterNameTextMesh;
        [SerializeField] private TextMeshProUGUI _characterGenderTextMesh;
        [SerializeField] private TextMeshProUGUI _characterCultureTextMesh;
        [SerializeField] private Button _regenerateButton;

        [Space]
        [Header("Extra Settings")]
        [Tooltip("Must contain an array with characters. Each character must have name, gender and culture")]
        [SerializeField] private string _customCharactersHttpURL;
        [SerializeField] private bool _logToConsole = false;

        private CancellationTokenSource _loadingCts;

        #endregion


        #region Methods

        #region Init & Dispose

        private void Awake()
        {
            _regenerateButton.onClick.AddListener(RegenerateDisplayCharacterInfo);
        }

        private void Start()
        {
            if (_generateOnStart)
            {
                RegenerateDisplayCharacterInfo();
            }
        }

        public void Dispose()
        {
            CancelCurrentGeneration();
            _regenerateButton.onClick.RemoveListener(RegenerateDisplayCharacterInfo);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion

        private void RegenerateDisplayCharacterInfo()
        {
            CancelCurrentGeneration();
            _loadingCts = new CancellationTokenSource();

            RegenerateDisplayCharacterInfoAsync(_loadingCts.Token)
                .Forget();
        }

        private async UniTaskVoid RegenerateDisplayCharacterInfoAsync(CancellationToken token = default)
        {
            CharacterInfo characterInfo = await GenerateRandomCharacterInfo(_loadingCts.Token);
            DisplayCharacterInfo(characterInfo);
        }

        private async UniTask<CharacterInfo> GenerateRandomCharacterInfo(CancellationToken token = default)
        {
            string targetRequestURL = DefaultCharactersHttpURL;
            int targetIndex = UnityEngine.Random.Range(0, MaxRandomCharacterIndex);

            if (!string.IsNullOrEmpty(_customCharactersHttpURL))
            {
                targetRequestURL = _customCharactersHttpURL;
            }

            string characterInfoJson = await RequestCharacterDataHttp(targetRequestURL, targetIndex, token);

            CharacterInfo characterInfo = null;
            CharacterApiResponse characterApiResponse = null;

            try
            {
                characterApiResponse = JsonUtility.FromJson<CharacterApiResponse>(characterInfoJson);
                characterInfo = new CharacterInfo(characterApiResponse.name, characterApiResponse.gender, characterApiResponse.culture);
            }
            catch (ArgumentException e)
            {
                Debug.LogError("JSON Parse Error: " + e.Message);
                Debug.LogWarning(characterInfoJson);
            }

            return characterInfo;
        }

        private async UniTask<string> RequestCharacterDataHttp(string sourceURL, int index, CancellationToken token = default)
        {
            try
            {
                using UnityWebRequest webRequest = UnityWebRequest.Get($"{sourceURL}/{index}");
                await webRequest.SendWebRequest().ToUniTask(cancellationToken: token);

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    return webRequest.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"# Error: {webRequest.error}");
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log(" # Request was cancelled.");
                return null;
            }
        }

        private void DisplayCharacterInfo(CharacterInfo characterInfo)
        {
            _characterNameTextMesh.text = $"Name : {characterInfo.Name}";
            _characterGenderTextMesh.text = $"Gender : {characterInfo.Gender}";
            _characterCultureTextMesh.text = $"Culture : {characterInfo.Culture}";

            if (_logToConsole)
            {
                Debug.Log(characterInfo.ToString() + "\n\n\n");
            }
        }

        private void CancelCurrentGeneration()
        {
            if (_loadingCts != null)
            {
                _loadingCts.Cancel();
                _loadingCts.Dispose();
                _loadingCts = null;
            }
        }

        #endregion

    }
}
