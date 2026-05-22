using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable]
class NormalsPass : CustomPass
{
    public Material overrideMaterial;
    public LayerMask layerMask = -1;

    RTHandle normalsRT;
    ShaderTagId[] shaderTags;
    static readonly int kNormalsTexID = Shader.PropertyToID("_ReconstructedNormals");

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        normalsRT = RTHandles.Alloc(
            Vector2.one,
            TextureXR.slices,
            dimension: TextureDimension.Tex2D,
            colorFormat: UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat,
            useDynamicScale: true,
            name: "Mesh Normals"
        );

        shaderTags = new ShaderTagId[]
        {
            new ShaderTagId("Forward"),
            new ShaderTagId("ForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("DepthOnly"),
            new ShaderTagId("DepthForwardOnly")
        };
    }

    protected override void Execute(CustomPassContext ctx)
    {
        if (overrideMaterial == null) return;

        CoreUtils.SetRenderTarget(ctx.cmd, normalsRT, ClearFlag.Color, new Color(0, 0, 1, 1));

        ctx.renderContext.ExecuteCommandBuffer(ctx.cmd);
        ctx.cmd.Clear();

        var drawingSettings = new DrawingSettings(shaderTags[0], new SortingSettings(ctx.hdCamera.camera)
        {
            criteria = SortingCriteria.CommonOpaque
        })
        {
            overrideMaterial = overrideMaterial,
            overrideMaterialPassIndex = 0
        };

        for (int i = 1; i < shaderTags.Length; i++)
            drawingSettings.SetShaderPassName(i, shaderTags[i]);

        var filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);

#pragma warning disable 618
        ctx.renderContext.DrawRenderers(ctx.cullingResults, ref drawingSettings, ref filteringSettings);
#pragma warning restore 618

        // Set globally with BOTH methods to ensure it's available
        Shader.SetGlobalTexture(kNormalsTexID, normalsRT);
        ctx.cmd.SetGlobalTexture(kNormalsTexID, normalsRT);
    }

    protected override void Cleanup()
    {
        if (normalsRT != null)
        {
            normalsRT.Release();
            normalsRT = null;
        }
    }
}
