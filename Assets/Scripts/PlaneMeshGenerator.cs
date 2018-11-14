using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Even though the standard Unity3D plane mesh works well for the snow deformation tessellation shader, it lacks
// details once the plane is scaled. Therefore, it is recommended to make use of this class instead.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlaneMeshGenerator : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Configuration")]
    [SerializeField] private string m_MeshName = "GeneratedPlane";

    [SerializeField] private float m_GizmoMeshRepresentationThickness = 0.1f;

    [SerializeField] private int m_NumberOfHorizontalCells = 10;
    [SerializeField] private int m_NumberOfVerticalCells = 10;

    // --------------------------------------------------------------

    private MeshFilter m_MeshFilter = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_MeshFilter = GetComponent<MeshFilter>();

        Generate();
    }

    // TODO: Refactor
    private void Generate()
    {
        Mesh newMesh = new Mesh();
        newMesh.name = m_MeshName;
        
        int totalNumberOfVertices = (m_NumberOfHorizontalCells + 1) * (m_NumberOfVerticalCells + 1);
        int totalNumberOfIndices = m_NumberOfHorizontalCells * m_NumberOfVerticalCells * 6;

        Vector3[] vertices = new Vector3[totalNumberOfVertices];
        Vector2[] textureCoordinates = new Vector2[totalNumberOfVertices];
        int[] indices = new int[totalNumberOfIndices];

        // Generate all vertices of the plane mesh
        for (int index = 0, z = 0; z <= m_NumberOfVerticalCells; ++z)
        {
            for (int x = 0; x <= m_NumberOfHorizontalCells; ++x, ++index)
            {
                vertices[index] = new Vector3(x, 0.0f, z);
                textureCoordinates[index] = new Vector2((float)x / m_NumberOfHorizontalCells, (float)z / m_NumberOfVerticalCells);
            }
        }

        // Calculate the correct indices for the triangle grid mesh
        for (int triangleIndex = 0, vertexIndex = 0, z = 0; z < m_NumberOfVerticalCells; ++z, ++vertexIndex)
        {
            for (int x = 0; x < m_NumberOfHorizontalCells; ++x, triangleIndex += 6, ++vertexIndex)
            {
                indices[triangleIndex + 0] = vertexIndex;
                indices[triangleIndex + 1] = vertexIndex + m_NumberOfHorizontalCells + 1;
                indices[triangleIndex + 2] = vertexIndex + 1;
                indices[triangleIndex + 3] = vertexIndex + 1;
                indices[triangleIndex + 4] = vertexIndex + m_NumberOfHorizontalCells + 1;
                indices[triangleIndex + 5] = vertexIndex + m_NumberOfHorizontalCells + 2;
            }
        }

        newMesh.vertices = vertices;
        newMesh.triangles = indices;
        newMesh.uv = textureCoordinates;

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        
        m_MeshFilter.mesh = newMesh;

        // Adding the box collider here will force Unity to calculate the bounding box, which is convenient
        Destroy(GetComponent<BoxCollider>());   // Remove it to be sure
        gameObject.AddComponent<BoxCollider>(); // Add it again
    }

    // Since the mesh is invisible in the editor, a box is used to indicate the mesh bounds
    private void OnDrawGizmos()
    {
        Vector3 meshBounds = new Vector3(m_NumberOfHorizontalCells, m_GizmoMeshRepresentationThickness, m_NumberOfVerticalCells);

        Gizmos.color = Color.gray;
        Gizmos.DrawCube(transform.position + (meshBounds * 0.5f), meshBounds);
    }
}
