using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [CreateAssetMenu(menuName = "Rendering/Turnip/Turnip Render Pipeline", order = -1000)]
    public sealed class TurnipRenderPipelineAsset : RenderPipelineAsset<TurnipRenderPipeline>
    {
        [Title("Global Data")]
        [SerializeField] private TurnipRenderPipelineResources m_Resources;

        [Title("Rendering")]
        [SerializeField] private PassCollection m_Passes;
        [SerializeField] private MSAASamples m_msaaSamples = MSAASamples.None;
        [Range(0, 1)]
        [SerializeField] private float m_RenderScale = 1;
        
        public override Material defaultMaterial => m_Resources.DefaultMaterial;
        public override Shader defaultShader => m_Resources.DefaultShader;

        public PassCollection Passes => m_Passes;
        public MSAASamples MSAASamples => m_msaaSamples;
        public float RenderScale => m_RenderScale;

        protected override RenderPipeline CreatePipeline()
        {
            return new TurnipRenderPipeline(this);
        }
    }
}