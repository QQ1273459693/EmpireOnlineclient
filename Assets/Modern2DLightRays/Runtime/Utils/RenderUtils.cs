using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LeafUtilsRays
{
    public static class RenderUtils
    {
        public static void RenderToRT2D(Rect viewport, Material mat, RenderTexture result)
        {
            //get and clear gpu command buffer
            var cmd = CommandBufferPool.Get();
            cmd.Clear();

            //sets the result render texture as render target 
            CoreUtils.SetRenderTarget(cmd, result);

            //sets the area that will be rendered
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.SetViewport(viewport);

            //draws mesh with out material
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, mat, 0, 0);

            Graphics.ExecuteCommandBuffer(cmd);
            cmd.Release();
        }

        public static RenderTexture DeepCopy(this RenderTexture tex)
        {
            RenderTexture tex2 = new RenderTexture(tex.width, tex.height, tex.depth, tex.format, tex.mipmapCount);
            if (tex.enableRandomWrite) tex2.enableRandomWrite = true;
            tex2.Create();
            Graphics.CopyTexture(tex, tex2);
            return tex2;
        }
    }

}