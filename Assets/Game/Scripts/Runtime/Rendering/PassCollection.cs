using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Radish.Rendering.Passes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Radish.Rendering
{
    [CreateAssetMenu(menuName = "Rendering/Turnip/Pass Collection", order = -1000)]
    public sealed class PassCollection : ScriptableObject, IEnumerable<TurnipRenderPassBase>
    {
        [ValidateInput(nameof(ValidatePassSetup))]
        [InlineProperty]
        [ListDrawerSettings(ShowPaging = false, ShowFoldout = false, DefaultExpandedState = true)]
        [PolymorphicDrawerSettings(ShowBaseType = false)]
        [SerializeReference] private List<TurnipRenderPassBase> m_Passes = new();
        public IReadOnlyList<TurnipRenderPassBase> Passes => m_Passes;
        
        public IEnumerator<TurnipRenderPassBase> GetEnumerator()
        {
            return m_Passes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Passes).GetEnumerator();
        }

        private static bool ValidatePassSetup(List<TurnipRenderPassBase> passes, ref string errorMessage)
        {
            if (passes.LastOrDefault() is not CameraTargetBlitPass)
            {
                errorMessage = $"Last pass must be {nameof(CameraTargetBlitPass)}";
                return false;
            }

            return true;
        }
    }
}