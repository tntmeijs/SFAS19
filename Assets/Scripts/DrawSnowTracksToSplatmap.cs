using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSnowTracksToSplatmap : MonoBehaviour
{
    // --------------------------------------------------------------

    [SerializeField] private GameObject m_SnowMesh = null;
    [SerializeField] private Shader m_DrawShader = null;
    [SerializeField] private LayerMask m_SnowtrackRayIgnoreLayer = 0;

    [SerializeField] private string m_SnowtrackShaderDisplacementTextureName = "_DisplacementTexture";
    [SerializeField] private Vector2Int m_SplatmapTextureSize = new Vector2Int(2048, 2048);

    [Range(0.1f, 10.0f)]
    [SerializeField] private float m_SnowtrackRayLength = 3.0f;

    [Range(0.0f, 0.25f)]
    [SerializeField] private float m_SnowTrackSize = 0.01f;

    // --------------------------------------------------------------

    private Material m_DrawShaderMaterial = null;

    private RenderTexture m_SplatmapTexture = null;

    // --------------------------------------------------------------
    
    // This function can be used to set the reference to the snow mesh when instantiating objects such as snowballs
    public void SetSnowMesh(GameObject snowMesh)
    {
        m_SnowMesh = snowMesh;
    }
    
    // --------------------------------------------------------------

    // This function can be used to set the reference to the snow mesh when instantiating objects such as snowballs
    public void SetSnowMesh(GameObject snowMesh)
    {
        m_SnowMesh = snowMesh;
    }

    // --------------------------------------------------------------

    private void Start()
    {
        m_DrawShaderMaterial = new Material(m_DrawShader);
        m_DrawShaderMaterial.SetFloat("_BrushSize", m_SnowTrackSize);

        m_SplatmapTexture = m_SnowMesh.GetComponent<MeshRenderer>().material.GetTexture(m_SnowtrackShaderDisplacementTextureName) as RenderTexture;

        // If the material does not have a splat map yet, create one
        if (!m_SplatmapTexture)
        {
            m_SplatmapTexture = new RenderTexture(m_SplatmapTextureSize.x, m_SplatmapTextureSize.y, 0, RenderTextureFormat.RFloat);
            m_SnowMesh.GetComponent<MeshRenderer>().material.SetTexture(m_SnowtrackShaderDisplacementTextureName, m_SplatmapTexture);
        }
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
}
