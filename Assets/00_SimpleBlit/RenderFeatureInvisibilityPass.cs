using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderFeatureInvisibilityPass : ScriptableRenderPass
{
    private RenderFeatureInvisibility.Settings _settings = default;
    private ScriptableRenderer _renderer = default;
    
    private int _rtId1 = Shader.PropertyToID("tmpRT1");
    private int _rtId2 = Shader.PropertyToID("tmpRT2");
    
    private int _blurredEnvironmentTexId = Shader.PropertyToID("_BlurredEnvironmentTex");
    private int _viewProjectionId = Shader.PropertyToID("_ViewProjection");

    private int _blurRadiusId = Shader.PropertyToID("_BlurRadius");

    private RenderTargetIdentifier _rtHandle1 = default;
    private RenderTargetIdentifier _rtHandle2 = default;

    public RenderFeatureInvisibilityPass(RenderFeatureInvisibility.Settings settings)
    {
        _settings = settings;
    }
    
    public void Setup(ScriptableRenderer renderer)
    {
        _renderer = renderer;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // TODO: Configure temporary render target
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // TODO: Get command buffer
        
        
        // TODO: Set shader parameters
        

        // TODO: Blit current colour output
        
        
        
        // TODO: Set blurred texture global
        

        // TODO: Fetch invisible objects
        
        
        // TODO: Set render target
        
        
        // TODO: Draw renderers
        
        
        // TODO: Execute command buffer and clean up
    }
}
