using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class GizmoPassData : 
        ISceneColorPassData,
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public RendererListHandle Renderers { get; set; }
    }
    
    public sealed class GizmoPass : TurnipRenderPass<GizmoPassData>
    {
        [SerializeField] private GizmoSubset m_Subset = GizmoSubset.PostImageEffects;
        
        protected override void SetupPass(GizmoPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.SceneColor =
                builder.UseColorBuffer(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.Read);

            var renderers = passContext.Graph.CreateGizmoRendererList(in cameraContext.Camera, in m_Subset);
            data.Renderers = builder.UseRendererList(in renderers);
            
            builder.AllowRendererListCulling(false);

            builder.SetRenderFunc<GizmoPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
        }

        protected override bool ShouldCullPass(in TurnipRenderPassContext passContext, in CameraContext cameraContext)
        {
#if UNITY_EDITOR
            return !UnityEditor.Handles.ShouldRenderGizmos();
#else
            return true;
#endif
        }
    }
}