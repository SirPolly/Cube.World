using UnityEngine;
using UnityEngine.Serialization;

namespace Cube.World {
    [AddComponentMenu("Cube.World/Sky")]
    public class Sky : MonoBehaviour {
        public Material skyMaterial;
        
        public Gradient daySkyTintGradient;
        public Gradient nightSkyTintGradient;
        
        public AnimationCurve dayAtmosphereThickness;
        public AnimationCurve nightAtmosphereThickness;
        
        public Gradient dayFogGradient;
        public Gradient nightFogGradient;

        public AnimationCurve dayAmbientLightIntensity;
        public AnimationCurve nightAmbientLightIntensity;

        public AnimationCurve dayReflectionIntensity;
        public AnimationCurve nightReflectionIntensity;

        IDayNightSystem _dayNightSystem;
        float _baseFogDensity;

        void Start() {
            _dayNightSystem = gameObject.GetSystem<IDayNightSystem>();
            _baseFogDensity = RenderSettings.fogDensity;
        }

        void Update() {
            var dayPercentage = _dayNightSystem.dayPercentage;

            Color skyTint;
            float atmosphereThickness;
            Color fog;
            float ambientIntensity;
            float ri;

            var sunRotation = dayPercentage * 360;
            if (sunRotation < 180) {
                // Day
                var a = dayPercentage * 2;
                skyTint = daySkyTintGradient.Evaluate(a);
                atmosphereThickness = dayAtmosphereThickness.Evaluate(a);
                fog = dayFogGradient.Evaluate(a);
                ambientIntensity = dayAmbientLightIntensity.Evaluate(a);
                ri = dayReflectionIntensity.Evaluate(a);
            } else {
                // Night
                sunRotation -= 180;

                var a = (dayPercentage - 0.5f) * 2;
                skyTint = nightSkyTintGradient.Evaluate(a);
                atmosphereThickness = nightAtmosphereThickness.Evaluate(a);
                fog = nightFogGradient.Evaluate(a);
                ambientIntensity = nightAmbientLightIntensity.Evaluate(a);
                ri = nightReflectionIntensity.Evaluate(a);
            }

            skyMaterial.SetColor("_SkyTint", skyTint);
            skyMaterial.SetFloat("_AtmosphereThickness", atmosphereThickness);

            RenderSettings.fogColor = fog;
            RenderSettings.fogDensity = fog.a * _baseFogDensity;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.reflectionIntensity = ri;
        }
    }
}