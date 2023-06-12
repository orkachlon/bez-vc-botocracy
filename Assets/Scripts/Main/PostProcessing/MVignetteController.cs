using System;
using UnityEngine;

namespace Main.PostProcessing {
    [ExecuteInEditMode]
    public class MVignetteController : MonoBehaviour {

        [SerializeField] private Shader vignetteShader;
        [SerializeField] private float radius;
        [SerializeField] private float feather;
        [SerializeField] private Color tint;
        [SerializeField] private bool invertColor;
        
        private Material _vignetteMaterial;

        private void Awake() {
            _vignetteMaterial = new Material(vignetteShader);
        }

        private void OnEnable() {
            _vignetteMaterial = new Material(vignetteShader);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            var width = src.width;
            var height = src.height;
            
            var startRenderTexture = RenderTexture.GetTemporary(width, height);
            
            _vignetteMaterial.SetFloat("_Radius", radius);
            _vignetteMaterial.SetFloat("_Feather", feather);
            _vignetteMaterial.SetColor("_Tint", tint);
            _vignetteMaterial.SetInt("_Invert", invertColor ? 1 : 0);
            Graphics.Blit(src, startRenderTexture, _vignetteMaterial);
            Graphics.Blit(startRenderTexture, dest);
            RenderTexture.ReleaseTemporary(startRenderTexture);
        }
    }
}