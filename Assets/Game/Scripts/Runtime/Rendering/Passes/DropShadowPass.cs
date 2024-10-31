using Radish.Rendering.PassData;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class DropShadowPassData :
        ISceneDepthPassData,
        IShadowTexturePassData
    {
        public TextureHandle SceneDepth { get; set; }
        public TextureHandle ShadowTexture { get; set; }
    }
    
    public class DropShadowPass : TurnipRenderPass<DropShadowPassData>
    {
        protected override void SetupPass(DropShadowPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            data.ShadowTexture =
                builder.UseColorBuffer(passContext.PassManager.GetData<IShadowTexturePassData>().ShadowTexture, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.Read);
            
            builder.SetRenderFunc<DropShadowPass>(static (data, ctx) =>
            {
                //ctx.cmd.DrawMeshInstanced()
            });
        }
    }
}