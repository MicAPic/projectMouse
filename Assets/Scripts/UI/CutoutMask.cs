using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI
{
    public class CutoutMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material matRorRendering = new Material(base.materialForRendering);
                matRorRendering.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
                return matRorRendering;
            }
        }
    }
}
