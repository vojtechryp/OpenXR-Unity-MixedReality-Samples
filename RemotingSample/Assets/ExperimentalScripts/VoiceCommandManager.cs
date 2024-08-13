using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Vojta.Experiment
{
    public class VoiceCommandManager : MonoBehaviour
    {
        public static VoiceCommandManager Instance { get; private set; }
        private KeywordRecognizer keywordRecognizer;
        private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        public static event System.Action OnNextCommand;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            keywords.Add("next", () =>
            {
                OnNextCommand?.Invoke();
            });

            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            keywordRecognizer.Start();
        }

        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;
            if (keywords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
        }

        void OnDestroy()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
                keywordRecognizer.Stop();
                keywordRecognizer.Dispose();
            }
        }
    }
}
