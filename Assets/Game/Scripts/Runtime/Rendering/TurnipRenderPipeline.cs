using Radish.Rendering.PassData;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public sealed class TurnipRenderPipeline : RenderPipeline
    {
        public TurnipRenderPipelineAsset asset { get; }
        public RenderGraph Graph { get; private set; }
        public RenderPassManager RenderPassManager { get; private set; }

        internal TurnipRenderPipeline(TurnipRenderPipelineAsset asset)
        {
            this.asset = asset;

            GraphicsSettings.useScriptableRenderPipelineBatching = true;
            GraphicsSettings.lightsUseLinearIntensity = true;
        }

        private void InitializeRenderGraph()
        {
            Graph ??= new RenderGraph(nameof(TurnipRenderPipeline));
            if (RenderPassManager is null)
            {
                RenderPassManager = new RenderPassManager(this, Graph);
                
                // TODO: this, works, but it isn't particularly flexible. Maybe we should have a more extensible or
                // logically structured way to ensure this...
                RenderPassManager.SetDataInitializer<ISceneColorPassData>(
                    ctx => new InputSceneColorData(ctx.Graph.CreateTexture(CreateColorTextureDesc(ctx.Camera)))
                );
                
                RenderPassManager.SetDataInitializer<ICameraColorTexturePassData>(
                    ctx =>
                    {
                        var textureDesc =
                            ctx.Graph.GetTextureDesc(ctx.RenderPassManager.GetData<ISceneColorPassData>().SceneColor);
                        textureDesc.name = "Camera Color Texture";
                        textureDesc.msaaSamples = MSAASamples.None;

                        return new InputCameraColorTextureData(ctx.Graph.CreateTexture(textureDesc));
                    }
                );
                
                RenderPassManager.SetDataInitializer<ICameraDepthTexturePassData>(
                    ctx =>
                    {
                        var textureDesc =
                            ctx.Graph.GetTextureDesc(ctx.RenderPassManager.GetData<ISceneDepthPassData>().SceneDepth);
                        textureDesc.name = "Camera Depth Texture";
                        textureDesc.msaaSamples = MSAASamples.None;

                        return new InputCameraDepthTextureData(ctx.Graph.CreateTexture(textureDesc));
                    }
                );
                
                RenderPassManager.SetDataInitializer<ISceneDepthPassData>(
                    ctx => new InputSceneDepthData(ctx.Graph.CreateTexture(CreateDepthTextureDesc(ctx.Camera)))
                );
                
                RenderPassManager.SetDataInitializer<IShadowTexturePassData>(
                    ctx => new InputShadowTextureData(ctx.Graph.CreateTexture(CreateShadowTextureDesc(ctx.Camera)))
                );
            }
        }

        private void CleanupRenderGraph()
        {
            RenderPassManager = null;
            
            if (Graph is not null)
            {
                Graph.Cleanup();
                Graph = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupRenderGraph();
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            InitializeRenderGraph();
            BeginFrameRendering(context, cameras);
            
            foreach (var camera in cameras)
                RenderCamera(camera, context);
            
            EndFrameRendering(context, cameras);
        }

        private void RenderCamera(Camera camera, ScriptableRenderContext context)
        {
            BeginCameraRendering(context, camera);
            if (!camera.TryGetCullingParameters(out var cameraCullingParams))
                return;
            
            var cameraCullResults = context.Cull(ref cameraCullingParams);
            
            context.SetupCameraProperties(camera);

            var sampler = new ProfilingSampler(camera.name);
            var buffer = CommandBufferPool.Get(camera.name);
            var graphParams = new RenderGraphParameters
            {
                commandBuffer = buffer,
                currentFrameIndex = Time.frameCount,
                scriptableRenderContext = context,
                executionName = sampler.name,
                rendererListCulling = true
            };
            
            Graph.BeginRecording(in graphParams);
            {
                using var _ = new RenderGraphProfilingScope(Graph, sampler);
                RecordCameraPasses(new CameraContext(camera, in cameraCullResults));
            }
            Graph.EndRecordingAndExecute();
            
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
            context.Submit();
            
            EndCameraRendering(context, camera);
        }

        private void RecordCameraPasses(in CameraContext cameraContext)
        {
            if (!asset.Passes)
                return;
            
            RenderPassManager.BeginCameraFrame(cameraContext.Camera);
            
            foreach (var pass in asset.Passes)
            {
                pass.AddToGraph(this, in cameraContext);
            }
            
            RenderPassManager.EndCameraFrame();
        }
        
        private TextureDesc CreateColorTextureDesc(Camera camera)
        {
            return new TextureDesc(camera.pixelWidth, camera.pixelHeight, true)
            {
                colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.DefaultHDR, GraphicsUtility.isSRGB),
                clearBuffer = false,
                depthBufferBits = DepthBits.None,
                name = "Scene Color",
                msaaSamples = asset.MSAASamples
            };
        }

        private TextureDesc CreateDepthTextureDesc(Camera camera)
        {
            return new TextureDesc(camera.pixelWidth, camera.pixelHeight, true)
            {
                colorFormat = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Depth, GraphicsUtility.isSRGB),
                clearBuffer = false,
                depthBufferBits = DepthBits.Depth24,
                name = "Scene Depth",
                msaaSamples = asset.MSAASamples
            };
        }

        private TextureDesc CreateShadowTextureDesc(Camera camera)
        {
            return new TextureDesc(camera.pixelWidth, camera.pixelHeight, true)
            {
                colorFormat =
                    GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Default, GraphicsUtility.isSRGB),
                clearBuffer = true,
                depthBufferBits = DepthBits.None,
                clearColor = Color.black,
                name = "Shadows",
                msaaSamples = asset.MSAASamples
            };
        }
    }
}