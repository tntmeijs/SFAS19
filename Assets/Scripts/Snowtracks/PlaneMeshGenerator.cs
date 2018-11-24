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
    // Not that important, but the user can choose to rename the "procedural" mesh
    [SerializeField] private string m_MeshName = "GeneratedPlane";
    
    // Please note that these are cells, NOT vertices.
    // This means that the actual vertex count is number of cells + 1!
    [SerializeField] private int m_NumberOfHorizontalCells = 10;
    [SerializeField] private int m_NumberOfVerticalCells = 10;

    [Header("Gizmo configuration")]
    // Since a "procedural" mesh cannot be shown in the editor, a Gizmo is used to give the user a visual representation
    // of the dimensions of the mesh.
    [SerializeField] private float m_GizmoMeshRepresentationThickness = 0.1f;

    [SerializeField] private Color m_GizmoMeshColor = Color.magenta;

    // --------------------------------------------------------------

    private MeshFilter m_MeshFilter = null;

    // --------------------------------------------------------------

    private void Awake()
    {
        m_MeshFilter = GetComponent<MeshFilter>();

        // Start generating the new plane mesh using the properties set by the user
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        int totalNumberOfVertices = GetTotalNumberOfVerticesRequired();
        int totalNumberOfIndices = GetTotalNumberOfIndicesRequired();

        // Allocate containers that will hold the mesh data
        Vector3[] vertices = new Vector3[totalNumberOfVertices];
        Vector2[] textureCoordinates = new Vector2[totalNumberOfVertices];
        int[] indices = new int[totalNumberOfIndices];

        // The vertex data that is generated consists out of positions and texture coordinates
        GenerateVertexData(totalNumberOfVertices, out vertices, out textureCoordinates);

        // The index data assumes that the vertex data is a grid of quads
        GenerateIndexData(totalNumberOfIndices, out indices);

        // All generated data should be applied to the mesh
        CreateMesh(vertices, textureCoordinates, indices);

        // A small "hack" that allows for easy collider generation
        MakeColliderFitToMesh();
    }

    private int GetTotalNumberOfVerticesRequired()
    {
        return (m_NumberOfHorizontalCells + 1) * (m_NumberOfVerticalCells + 1);
    }

    private int GetTotalNumberOfIndicesRequired()
    {
        return m_NumberOfHorizontalCells * m_NumberOfVerticalCells * 6;
    }

    private void GenerateVertexData(int numberOfVerticesToGenerate, out Vector3[] positionData, out Vector2[] uvData)
    {
        Vector3[] vertexPositions = new Vector3[numberOfVerticesToGenerate];
        Vector2[] vertexTextureCoordinates = new Vector2[numberOfVerticesToGenerate];

        for (int index = 0, z = 0; z <= m_NumberOfVerticalCells; ++z)
        {
            for (int x = 0; x <= m_NumberOfHorizontalCells; ++x, ++index)
            {
                vertexPositions[index] = new Vector3(x, 0.0f, z);
                vertexTextureCoordinates[index] = new Vector2((float)x / m_NumberOfHorizontalCells, (float)z / m_NumberOfVerticalCells);
            }
        }

        positionData = vertexPositions;
        uvData = vertexTextureCoordinates;
    }

    // Please refer to https://catlikecoding.com/unity/tutorials/procedural-grid/
    // for an in-depth explanation of this algorithm.
    private void GenerateIndexData(int numberOfIndicesToGenerate, out int[] indexData)
    {
        int[] indices = new int[numberOfIndicesToGenerate];

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

        indexData = indices;
    }

    private void CreateMesh(Vector3[] vertices, Vector2[] uv, int[] indices)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = m_MeshName;

        newMesh.vertices = vertices;
        newMesh.triangles = indices;
        newMesh.uv = uv;

        // Normals can be generated by Unity, so there is no need to do this manually
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        m_MeshFilter.mesh = newMesh;
    }

    
    private void MakeColliderFitToMesh()
    {
        Collider meshCollider = GetComponent<Collider>();

        // Any existing colliders should be removed before an updated collider can be added.
        // Without this step, the game object could have multiple colliders, which is not desired.
        if (meshCollider)
        {
            Destroy(meshCollider);
        }

        // Adding the mesh collider will force Unity to calculate the bounds of the collider automatically, which saves
        // a lot of programming work. A mesh collider is required, because it is the only type of collider that gives
        // the raycast hit information some UV data once an intersection has been found.
        gameObject.AddComponent<MeshCollider>();
    }

    // Since the mesh is invisible in the editor, a box is used to indicate the mesh bounds
    private void OnDrawGizmos()
    {
        Vector3 meshBounds = new Vector3(m_NumberOfHorizontalCells, m_GizmoMeshRepresentationThickness, m_NumberOfVerticalCells);

        Gizmos.color = m_GizmoMeshColor;
        Gizmos.DrawCube(transform.position + (meshBounds * 0.5f), meshBounds);
    }
}
