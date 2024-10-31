using System.Buffers;
using System.Collections.Generic;
using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class UnsupportedShadersPassData :
        ISceneColorPassData,
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public RendererListHandle Renderers { get; set; }
    }
    
    public class UnsupportedShadersPass : TurnipRenderPass<UnsupportedShadersPassData>
    {
        [SerializeField] private List<string> m_ShaderTagIds = new()
        {
            "Always",
            "ForwardBase",
            "PrepassBase",
            "Vertex",
            "VertexLMRGBM",
            "VertexLM"
        };

        private Material m_ErrorMaterial;
        
        protected override void SetupPass(UnsupportedShadersPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var shaderTagIdsArray = ArrayPool<ShaderTagId>.Shared.Rent(m_ShaderTagIds.Count);
            for (var i = 0; i < m_ShaderTagIds.Count; ++i)
                shaderTagIdsArray[i] = new ShaderTagId(m_ShaderTagIds[i]);

            if (!m_ErrorMaterial)
                m_ErrorMaterial = CoreUtils.CreateEngineMaterial("Hidden/InternalErrorShader");

            data.SceneColor =
                builder.UseColorBuffer(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.ReadWrite);

            var renderers = new RendererListDesc(shaderTagIdsArray, cameraContext.CullResults, cameraContext.Camera)
            {
                overrideMaterial = m_ErrorMaterial,
                renderQueueRange = RenderQueueRange.all
            };

            data.Renderers = builder.UseRendererList(passContext.Graph.CreateRendererList(in renderers));
            
            builder.AllowRendererListCulling(true);
            builder.AllowPassCulling(true);
            
            builder.SetRenderFunc<UnsupportedShadersPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
            
            ArrayPool<ShaderTagId>.Shared.Return(shaderTagIdsArray);
        }
    }
}