using UnityEngine;

namespace Cube.World {
    [AddComponentMenu("Cube.World/Sun")]
    public class Sun : MonoBehaviour {
        [SerializeField]
        Gradient _daySunColor;
        [SerializeField]
        Gradient _nightSunColor;
        
        IDayNightSystem _dayNightSystem;

        void Start() {
            _dayNightSystem = gameObject.GetSystem<IDayNightSystem>();
        }

        void Update() {
            var dayPercentage = _dayNightSystem.dayPercentage;

            Color sunColor;
            var sunRotation = dayPercentage * 360;
            if (sunRotation < 180) {
                // Day
                var f = dayPercentage * 2;
                sunColor = _daySunColor.Evaluate(f);
            } else {
                // Night
                sunRotation -= 180;

                var f = (dayPercentage - 0.5f) * 2;
                sunColor = _nightSunColor.Evaluate(f);
            }
            transform.rotation = Quaternion.Euler(sunRotation, 0, 0);

            var light = GetComponent<Light>();
            light.color = sunColor;
        }
    }
}