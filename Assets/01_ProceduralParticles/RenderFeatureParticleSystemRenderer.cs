using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeatureParticleSystemRenderer : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        public ComputeShader psComputeShader;

        public Material psMaterial = null;
    }

    [SerializeField] 
    private Settings _settings = new Settings();

    private RenderFeatureParticleSystemRendererPass _pass = default;

    public override void Create()
    {
        _pass = new RenderFeatureParticleSystemRendererPass(_settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_settings.psMaterial == null || _settings.psComputeShader == null)
        {
            return;
        }
        
        renderer.EnqueuePass(_pass);
    }
}
