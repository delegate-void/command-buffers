using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderFeatureParticleSystemRendererPass : ScriptableRenderPass
{
    private RenderFeatureParticleSystemRenderer.Settings _settings = default;

    #region Compute Shader Props

    struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }

    private int _particleStructSize = 28; // 7 * 4 bytes
    
    private int _maxNumParticles = 1000000;
    
    private int _warpCount = 0;
    private int _warpSize = 256;

    private ComputeBuffer _particleBuffer = default;
    private int _kernelId;

    #endregion

    public RenderFeatureParticleSystemRendererPass(RenderFeatureParticleSystemRenderer.Settings settings)
    {
        _settings = settings;
        SetupComputeShader();
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Particle Pass");
        
        // Mouse position in worldspace
        var currentPos = renderingData.cameraData.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            renderingData.cameraData.camera.nearClipPlane + 14));
        
        float[] mousePosition2D = { currentPos.x, currentPos.y };
        
        // TODO: set compute shader parameters
        
        
        // TODO: dispatch compute shader
        
        cmd.DispatchCompute(_settings.psComputeShader, _kernelId, _warpCount, 1, 1);
        
        // TODO: draw procedural mesh
        
        
        // TODO: execute command buffer and clean up
    }
    
    private void SetupComputeShader()
    {
        _warpCount = Mathf.CeilToInt((float) _maxNumParticles / _warpSize);

        Particle[] particles = new Particle[_maxNumParticles];

        for (int i = 0; i < _maxNumParticles; i++)
        {
            float x = Random.value * 2 - 1.0f;
            float y = Random.value * 2 - 1.0f;
            float z = Random.value * 2 - 1.0f;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize();
            xyz *= Random.value;
            xyz *= 0.5f;

            particles[i].position.x = xyz.x;
            particles[i].position.y = xyz.y;
            particles[i].position.z = xyz.z + 3;

            particles[i].velocity.x = 0;
            particles[i].velocity.y = 0;
            particles[i].velocity.z = 0;

            // Initial life value
            particles[i].life = Random.value * 5.0f + 1.0f;
        }
        
        // create buffer
        _particleBuffer = new ComputeBuffer(_maxNumParticles, _particleStructSize);
        _particleBuffer.SetData(particles);
        
        // find kernel id
        _kernelId = _settings.psComputeShader.FindKernel("ParticleSimulation");
        
        // bind buffer to material
        _settings.psComputeShader.SetBuffer(_kernelId, "_ParticleBuffer", _particleBuffer);
        _settings.psMaterial.SetBuffer("_ParticleBuffer", _particleBuffer);
    }
}
