using UnityEngine;

namespace Cube.World {
    public class Sky : MonoBehaviour {
        [SerializeField]
        GameObject _sun;
        [SerializeField]
        Material _skyMaterial;

        [SerializeField]
        Gradient _daySunColor;
        [SerializeField]
        Gradient _nightSunColor;

        [SerializeField]
        Gradient _daySkyTintGradient;
        [SerializeField]
        Gradient _nightSkyTintGradient;

        [SerializeField]
        AnimationCurve _dayAtmosphereThickness;
        [SerializeField]
        AnimationCurve _nightAtmosphereThickness;

        IDayNightSystem _dayNightSystem;

        void Start() {
            _dayNightSystem = gameObject.GetSystem<IDayNightSystem>();
        }

        void Update() {
            var dayPercentage = _dayNightSystem.dayPercentage;

            Color sunColor, skyTint;
            float atmosphereThickness;
            var sunRotation = dayPercentage * 360;
            if (sunRotation < 180) {
                // Day
                var f = dayPercentage * 2;
                sunColor = _daySunColor.Evaluate(f);
                skyTint = _daySkyTintGradient.Evaluate(f);
                atmosphereThickness = _dayAtmosphereThickness.Evaluate(f);
            } else {
                // Night
                sunRotation -= 180;

                var f = (dayPercentage - 0.5f) * 2;
                sunColor = _nightSunColor.Evaluate(f);
                skyTint = _nightSkyTintGradient.Evaluate(f);
                atmosphereThickness = _nightAtmosphereThickness.Evaluate(f);
            }
            _sun.transform.rotation = Quaternion.Euler(sunRotation, 0, 0);
            _sun.GetComponent<Light>().color = sunColor;

            _skyMaterial.SetColor("_SkyTint", skyTint);
            _skyMaterial.SetFloat("_AtmosphereThickness", atmosphereThickness);
        }
    }
}