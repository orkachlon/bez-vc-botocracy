using DG.Tweening;
using UnityEngine;

namespace PostProcessing {
    [ExecuteInEditMode]
    public class MVignetteController : MonoBehaviour {

        [Header("Dependencies"), SerializeField] private Shader vignetteShader;
        [Header("Visuals"), SerializeField] private float radius;
        [SerializeField] private float feather;
        [SerializeField] private Color tint;
        [SerializeField] private bool invertColor;

        [Header("Animation"), SerializeField] private float animationDuration;
        
        public Color Color { get; set; }
        public float Radius { get; set; }
        public float Feather { get; set; }

        private Material _vignetteMaterial;
        private Tween _animation;
        
        private void Awake() {
            _vignetteMaterial = new Material(vignetteShader);
        }

        private void OnEnable() {
            _vignetteMaterial = new Material(vignetteShader);
            SetValues(tint, radius, feather);
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            var width = src.width;
            var height = src.height;
            
            var startRenderTexture = RenderTexture.GetTemporary(width, height);
            if (!Application.isPlaying) {
                SetValues(tint, radius, feather);
            }

            _vignetteMaterial.SetFloat("_Radius", Radius);
            _vignetteMaterial.SetFloat("_Feather", Feather);
            _vignetteMaterial.SetColor("_Tint", Color);
            _vignetteMaterial.SetInt("_Invert", invertColor ? 1 : 0);
            Graphics.Blit(src, startRenderTexture, _vignetteMaterial);
            Graphics.Blit(startRenderTexture, dest);
            RenderTexture.ReleaseTemporary(startRenderTexture);
        }

        public void SetValues(Color tintTo, float radiusTo, float featherTo, bool animate = false) {
            if (!animate) {
                Color = tintTo;
                Radius = radiusTo;
                Feather = featherTo;
                return;
            }
            _animation?.Complete();
            _animation?.Kill();
            _animation = DOTween.Sequence()
                .Append(DOVirtual.Color(Color, tintTo, animationDuration,
                    c => Color = c))
                .Join(DOVirtual.Float(Radius, radiusTo, animationDuration, f => Radius = f))
                .Join(DOVirtual.Float(Feather, featherTo, animationDuration, f => Feather = f))
                .OnComplete(() => _animation = null);
        }

        public void SetDefaults(bool animate = false) {
            SetValues(tint, radius, feather, animate);
        }
    }
}