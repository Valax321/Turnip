using UnityEngine;

namespace Radish.Rendering
{
    [CreateAssetMenu(menuName = "Rendering/Turnip/Turnip Render Pipeline Resources", order = -1000)]
    public sealed class TurnipRenderPipelineResources : ScriptableObject
    {
        [SerializeField] private Shader m_DefaultShader;
        [SerializeField] private Material m_DefaultMaterial;

        public Shader DefaultShader => m_DefaultShader;
        public Material DefaultMaterial => m_DefaultMaterial;
    }
}