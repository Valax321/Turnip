using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public class SetupCameraPassData :
        ISceneColorPassData,
        ISceneDepthPassData
    {
        public TextureHandle SceneColor { get; set; }
        public TextureHandle SceneDepth { get; set; }
        public Color ClearColor { get; set; }
        public bool ShouldClearColor { get; set; }
        public bool ShouldClearDepth { get; set; }
    }
    
    public class SetupCameraPass : TurnipRenderPass<SetupCameraPassData>
    {
        protected override void SetupPass(SetupCameraPassData data, in TurnipRenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var camera = cameraContext.Camera;
            var isPreviewCamera = camera.cameraType == CameraType.Preview;
            
            data.ShouldClearColor = ((int)camera.clearFlags <= (int)CameraClearFlags.SolidColor) || isPreviewCamera;
            data.ShouldClearDepth = ((int)camera.clearFlags <= (int)CameraClearFlags.Depth) || isPreviewCamera;
            data.ClearColor = GraphicsUtility.isSRGB ? camera.backgroundColor.linear : camera.backgroundColor;
            
            data.SceneColor =
                builder.UseColorBuffer(passContext.PassManager.GetData<ISceneColorPassData>().SceneColor, 0);
            data.SceneDepth = builder.UseDepthBuffer(passContext.PassManager.GetData<ISceneDepthPassData>().SceneDepth,
                DepthAccess.Write);
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc<SetupCameraPassData>(static (data, ctx) =>
            {
                ctx.cmd.ClearRenderTarget(data.ShouldClearDepth, data.ShouldClearColor, data.ClearColor);
            });
        }
    }
}