using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class UIOverlayPassData :
        ISceneColorPassData,
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public RendererListHandle Renderers { get; set; }
    }
    
    public sealed class UIOverlayPass : TurnipRenderPass<UIOverlayPassData>
    {
        protected override void SetupPass(UIOverlayPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.SceneColor =
                builder.UseColorBuffer(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.Read);
            
            builder.AllowRendererListCulling(true);
            builder.AllowPassCulling(true);
            
            builder.SetRenderFunc<UIOverlayPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
        }

        protected override bool ShouldCullPass(in TurnipRenderPassContext passContext, in CameraContext cameraContext)
        {
            return cameraContext.Camera.cameraType is not (CameraType.Game or CameraType.VR);
        }
    }
}