using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSnowTracksToSplatmap : MonoBehaviour
{
    // --------------------------------------------------------------

    [SerializeField] private RenderTexture m_SplatmapTexture = null;
    [SerializeField] private Shader m_DrawShader = null;
    [SerializeField] private LayerMask m_SnowtrackRayIgnoreLayer = 0;

    [Range(0.1f, 10.0f)]
    [SerializeField] private float m_SnowtrackRayLength = 3.0f;

    [Range(0.0f, 0.25f)]
    [SerializeField] private float m_SnowTrackSize = 0.01f;

    // --------------------------------------------------------------

    private Material m_DrawShaderMaterial = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_DrawShaderMaterial = new Material(m_DrawShader);
        m_DrawShaderMaterial.SetFloat("_BrushSize", m_SnowTrackSize);

        Graphics.SetRenderTarget(m_SplatmapTexture);
        GL.Clear(false, true, Color.black);
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(ray, out hitInfo, m_SnowtrackRayLength, ~m_SnowtrackRayIgnoreLayer))
        {
            m_DrawShaderMaterial.SetVector("_BrushCoordinate", new Vector4(hitInfo.textureCoord.x, hitInfo.textureCoord.y, 0.0f, 0.0f));
            RenderTexture tempTex = RenderTexture.GetTemporary(m_SplatmapTexture.width, m_SplatmapTexture.height, 0, m_SplatmapTexture.format);

            // Grab A, apply shader, save in temp
            Graphics.Blit(m_SplatmapTexture, tempTex, m_DrawShaderMaterial);
            
            // Copy temp to B
            Graphics.Blit(tempTex, m_SplatmapTexture);

            RenderTexture.ReleaseTemporary(tempTex);
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 128, 128), m_SplatmapTexture, ScaleMode.ScaleToFit, false, 1.0f);
    }
}
