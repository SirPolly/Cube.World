using Cube.Gameplay;
using UnityEngine;

namespace Cube.World {
    public class DayNightSystem : MonoBehaviour, IDayNightSystem {
        public DayNightSettings settings;

        public bool isNight {
            get { return _dayPercentage > 0.5f; }
        }

        [Range(0f, 1f)]
        [SerializeField]
        float _dayPercentage = 0f;
        public float dayPercentage {
            get { return _dayPercentage; }
        }

        GameStateSystem _gameStateSystem;

        void Awake() {
            gameObject.SetSystem<IDayNightSystem>(this);

#if UNITY_EDITOR
            if (settings.disableInEditor) {
                _dayPercentage = 0.3f;
            }
#endif
        }

        void Start() {
            _gameStateSystem = gameObject.GetSystem<GameStateSystem>();
        }

        void Update() {
#if UNITY_EDITOR
            if (settings.disableInEditor)
                return;
#endif

            if (_gameStateSystem.current == null)
                return;

            var initialTimeOffset = settings.dayLengthInSeconds;
            var time = initialTimeOffset + _gameStateSystem.current.time;
            var localTime = time - Mathf.Floor(time / settings.dayLengthInSeconds) * settings.dayLengthInSeconds;
            _dayPercentage = localTime / settings.dayLengthInSeconds;
        }
    }
}