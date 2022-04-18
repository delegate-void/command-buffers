using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderFeatureBlurPass : ScriptableRenderPass
{
    private RenderFeatureInvisibility.BlurSettings _settings = default;
    private ScriptableRenderer _renderer = default;
    
    private int _rtId1 = Shader.PropertyToID("tmpRT1");
    private int _rtId2 = Shader.PropertyToID("tmpRT2");
    
    private int _blurredEnvironmentTexId = Shader.PropertyToID("_BlurredEnvironmentTex");
    private int _viewProjectionId = Shader.PropertyToID("_ViewProjection");

    private int _blurRadiusId = Shader.PropertyToID("_BlurRadius");

    private RenderTargetIdentifier _rtHandle1 = default;
    private RenderTargetIdentifier _rtHandle2 = default;
    
    private int _secondaryColorTexId = Shader.PropertyToID("_Secondary_ColorTex");
    private int _secondaryDepthTexId = Shader.PropertyToID("_Secondary_DepthTex");
    
    private RenderTargetIdentifier _secondaryColorTex = default;
    private RenderTargetIdentifier _secondaryDepthTex = default;

    public RenderFeatureBlurPass(RenderFeatureInvisibility.BlurSettings settings)
    {
        _settings = settings;
    }
    
    public void Setup(ScriptableRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cmd.GetTemporaryRT(_rtId1, cameraTextureDescriptor);
        cmd.GetTemporaryRT(_rtId2, cameraTextureDescriptor);

        _rtHandle1 = new RenderTargetIdentifier(_rtId1);
        _rtHandle2 = new RenderTargetIdentifier(_rtId2);
        
        cmd.GetTemporaryRT(_secondaryColorTexId, cameraTextureDescriptor);
        cmd.GetTemporaryRT(_secondaryDepthTexId, cameraTextureDescriptor);

        _secondaryColorTex = new RenderTargetIdentifier(_secondaryColorTexId);
        _secondaryDepthTex = new RenderTargetIdentifier(_secondaryDepthTexId);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Blur Pass");
        
        cmd.SetGlobalFloat(_blurRadiusId, _settings.blurRadius);
        
        cmd.Blit(_renderer.cameraColorTargetHandle, _rtHandle1, _settings.blurMaterial);

        for (int i = 1; i < _settings.blurPasses; ++i)
        {
            cmd.SetGlobalFloat(_blurRadiusId, _settings.blurRadius * (i * 0.5f));
            cmd.Blit(_rtHandle1, _rtHandle2, _settings.blurMaterial);

            (_rtHandle1, _rtHandle2) = (_rtHandle2, _rtHandle1);
        }
        
        cmd.SetGlobalTexture(_blurredEnvironmentTexId, _rtHandle2);

        var objects = Object.FindObjectsOfType<InvisibleObject>();

        if (objects != null && objects.Length > 0)
        {
            var camera = renderingData.cameraData.camera;
            var viewProjection = camera.nonJitteredProjectionMatrix * camera.transform.worldToLocalMatrix;
            cmd.SetGlobalMatrix(_viewProjectionId, viewProjection);
            
            cmd.SetRenderTarget(_renderer.cameraColorTargetHandle);
            //cmd.SetRenderTarget(_secondaryColorTex, _secondaryDepthTex);
            
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out MeshRenderer meshRenderer))
                {
                    cmd.DrawRenderer(meshRenderer, meshRenderer.sharedMaterial, 0, 0);
                }
            }
        }
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        CommandBufferPool.Release(cmd);
    }
}
