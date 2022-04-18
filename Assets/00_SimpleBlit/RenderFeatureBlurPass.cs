using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderFeatureBlurPass : ScriptableRenderPass
{
    private RenderFeatureBlur.BlurSettings _settings = default;
    private ScriptableRenderer _renderer = default;
    
    private int _rtId1 = Shader.PropertyToID("tmpRT1");
    private int _rtId2 = Shader.PropertyToID("tmpRT2");

    private int _blurRadiusId = Shader.PropertyToID("_BlurRadius");

    private RenderTargetIdentifier _rtHandle1 = default;
    private RenderTargetIdentifier _rtHandle2 = default;

    public RenderFeatureBlurPass(RenderFeatureBlur.BlurSettings settings)
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
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Blur Pass");
        
        cmd.SetGlobalFloat(_blurRadiusId, _settings.blurRadius);
        
        cmd.Blit(_renderer.cameraColorTargetHandle, _rtHandle1, _settings.blurMaterial);

        for (int i = 1; i < _settings.blurPasses - 1; ++i)
        {
            cmd.SetGlobalFloat(_blurRadiusId, _settings.blurRadius * (i * 0.5f));
            cmd.Blit(_rtHandle1, _rtHandle2, _settings.blurMaterial);

            (_rtHandle1, _rtHandle2) = (_rtHandle2, _rtHandle1);
        }
        
        cmd.SetGlobalFloat(_blurRadiusId, _settings.blurRadius * (_settings.blurPasses * 0.5f));
        cmd.Blit(_rtHandle1, _renderer.cameraColorTargetHandle);
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        
        CommandBufferPool.Release(cmd);
    }
}
