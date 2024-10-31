using System.Buffers;
using System.Collections.Generic;
using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class RenderObjectsPassData :
        ISceneColorPassData,
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public RendererListHandle Renderers { get; set; }
    }
    
    public class RenderObjectsPass : TurnipRenderPass<RenderObjectsPassData>
    {
        [SerializeField] private List<string> m_ShaderTags = new();
        [SerializeField] private SortingCriteria m_SortingCriteria = SortingCriteria.CommonOpaque;
        [SerializeField] private SerializedRenderQueueRange m_RenderQueueRange = RenderQueueRange.opaque;
        [SerializeField] private bool m_NeedsCameraColorTexture;
        [SerializeField] private bool m_NeedsCameraDepthTexture;

        protected override bool NeedsColorTexture => m_NeedsCameraColorTexture;
        protected override bool NeedsDepthTexture => m_NeedsCameraDepthTexture;

        protected override void SetupPass(RenderObjectsPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.SceneColor =
                builder.UseColorBuffer(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.ReadWrite);

            var ids = ArrayPool<ShaderTagId>.Shared.Rent(m_ShaderTags.Count);
            for (var i = 0; i < m_ShaderTags.Count; ++i)
                ids[i] = new ShaderTagId(m_ShaderTags[i]);

            var renderers = new RendererListDesc(ids, cameraContext.CullResults, cameraContext.Camera)
            {
                sortingCriteria = m_SortingCriteria,
                renderQueueRange = m_RenderQueueRange,
            };

            data.Renderers = builder.UseRendererList(passContext.Graph.CreateRendererList(in renderers));
            
            builder.AllowRendererListCulling(true);
            
            builder.SetRenderFunc<RenderObjectsPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
            
            ArrayPool<ShaderTagId>.Shared.Return(ids);
        }
    }
}