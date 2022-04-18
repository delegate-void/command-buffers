using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeatureInvisibility : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        [Range(0f, 20f)]
        public float blurRadius;
        
        [Range(2,15)]
        public int blurPasses = 2;
        
        public Material blurMaterial = null;
    }

    [SerializeField] 
    private Settings _settings = new Settings();

    private RenderFeatureInvisibilityPass _pass = default;

    public override void Create()
    {
        _pass = new RenderFeatureInvisibilityPass(_settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_settings.blurMaterial == null)
        {
            return;
        }
        
        _pass.Setup(renderer);
        renderer.EnqueuePass(_pass);
    }
}
