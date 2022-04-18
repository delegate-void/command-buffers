using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeatureBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class BlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

        [Range(0f, 20f)]
        public float blurRadius;
        
        [Range(2,15)]
        public int blurPasses = 2;
        
        public Material blurMaterial = null;
    }

    [SerializeField] 
    private BlurSettings _settings = new BlurSettings();

    private RenderFeatureBlurPass _pass = default;

    public override void Create()
    {
        _pass = new RenderFeatureBlurPass(_settings);
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
