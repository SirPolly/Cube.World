using UnityEngine;

namespace Cube.World {
    [AddComponentMenu("Cube.World/Sky")]
    public class Sky : MonoBehaviour {
        [SerializeField]
        Material _skyMaterial;
        
        [SerializeField]
        Gradient _daySkyTintGradient;
        [SerializeField]
        Gradient _nightSkyTintGradient;

        [SerializeField]
        AnimationCurve _dayAtmosphereThickness;
        [SerializeField]
        AnimationCurve _nightAtmosphereThickness;

        [SerializeField]
        Gradient _dayFogGradient;
        [SerializeField]
        Gradient _nightFogGradient;

        IDayNightSystem _dayNightSystem;

        void Start() {
            _dayNightSystem = gameObject.GetSystem<IDayNightSystem>();
        }

        void Update() {
            var dayPercentage = _dayNightSystem.dayPercentage;

            Color skyTint;
            float atmosphereThickness;
            Color fog;
            var sunRotation = dayPercentage * 360;
            if (sunRotation < 180) {
                // Day
                var a = dayPercentage * 2;
                skyTint = _daySkyTintGradient.Evaluate(a);
                atmosphereThickness = _dayAtmosphereThickness.Evaluate(a);
                fog = _dayFogGradient.Evaluate(a);
            } else {
                // Night
                sunRotation -= 180;

                var a = (dayPercentage - 0.5f) * 2;
                skyTint = _nightSkyTintGradient.Evaluate(a);
                atmosphereThickness = _nightAtmosphereThickness.Evaluate(a);
                fog = _nightFogGradient.Evaluate(a);
            }

            _skyMaterial.SetColor("_SkyTint", skyTint);
            _skyMaterial.SetFloat("_AtmosphereThickness", atmosphereThickness);

            RenderSettings.fogColor = fog;
        }
    }
}