using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnlyRacingLineVisualizer : MonoBehaviour
{
    // --------------------------------------------------------------

    [Header("Gizmos configuration")]
    // Keep the track visualizer visible?
    [SerializeField]
    private bool m_KeepTrackVisible = true;

    // Number of lines to draw next to each other to give the impression of line thickness
    [SerializeField]
    private int m_LineThickness = 5;

    // Offset between multiple Gizmo lines in the DrawMultiGizmoLine function
    [SerializeField]
    private Vector3 m_LineOffset = new Vector3(0.01f, 0.0f, 0.0f);

    // Color used for lines that visualize the regular track course
    [SerializeField]
    private Color m_RegularTrackLineColor = Color.green;

    // Color used for lines that visualize the pit lane track
    [SerializeField]
    private Color m_PitLaneLineColor = Color.red;

    // Color used to visualize the two options for AI when a lane splits in two
    [SerializeField]
    private Color m_TrackSplitLineColor = Color.cyan;

    [Header("Nodes")]
    // Transform that holds all nodes for the ideal racing line for AI
    [SerializeField]
    private Transform m_RacingLineContainer = null;

    [Header("Node configuration")]
    // Tag that indicates a split of the racing line after this node
    [SerializeField]
    private string m_TrackSplitTag = "TrackSplit";

    // Tag that indicates the start of the pit lane after this node
    [SerializeField]
    private string m_PitSplitTag = "PitSplit";

    // --------------------------------------------------------------

    // Holds all nodes in the track (including special nodes and extra paths)
    private List<Transform> m_TrackNodes = null;

    // --------------------------------------------------------------

    private void RetrieveAllTrackNodes()
    {
        // The ideal racing line is assumes to use a single container node with all of its child nodes after it
        m_TrackNodes = new List<Transform>(m_RacingLineContainer.childCount);

        // Save every child transform of the container in the list for future use
        for (int i = 0; i < m_RacingLineContainer.childCount; ++i)
            m_TrackNodes.Add(m_RacingLineContainer.GetChild(i));
    }

    // Not using OnDrawGizmosSelected here, because sometimes you may want to see the track while working on other objects
    private void OnDrawGizmos()
    {
        // Track should not be visible, hide it
        if (!m_KeepTrackVisible)
            return;

        // Not enough nodes for race line visualization
        if (m_RacingLineContainer.childCount < 2)
            return;

        for (int i = 0; i < m_RacingLineContainer.childCount; ++i)
        {
            int thisIndex = i;
            int nextIndex = i + 1;

            // Loop the index around when the first node equals the last node of the list
            if (thisIndex == m_RacingLineContainer.childCount - 1)
                nextIndex = 0;

            Transform thisNode = m_RacingLineContainer.GetChild(thisIndex);
            Transform nextNode = m_RacingLineContainer.GetChild(nextIndex);

            // Visualize the connections between nodes
            Gizmos.color = m_RegularTrackLineColor;

            if (thisNode.gameObject.CompareTag(m_TrackSplitTag))
            {
                // Track splits in two
                VisualizeTrackSplit(thisNode, nextNode);
            }
            else if (thisNode.gameObject.CompareTag(m_PitSplitTag))
            {
                // Pit lane starts here
                VisualizePitLane(thisNode, nextNode);
            }
            else
            {
                // Draw the regular track
                DrawMultiGizmoLine(thisNode.position, nextNode.position);
            }
        }
    }

    private void VisualizeTrackSplit(Transform container, Transform nextNode)
    {
        // Invalid split
        if (container.childCount != 2)
            return;

        // Left path invalid (not enough nodes)
        if (container.GetChild(0).childCount < 2)
            return;

        // Right path invalid (not enough nodes)
        if (container.GetChild(1).childCount < 2)
            return;

        Gizmos.color = m_TrackSplitLineColor;

        // Track splits, assume child[0] is left and child[1] is right
        Transform leftPathContainer = container.GetChild(0);
        Transform rightPathContainer = container.GetChild(1);

        // Line from the container node to the first left child node
        DrawMultiGizmoLine(container.position, leftPathContainer.GetChild(0).position);
        
        // Line from the container node to the first right child node
        DrawMultiGizmoLine(container.position, rightPathContainer.GetChild(0).position);

        // Left path
        for (int j = 0; j < leftPathContainer.childCount - 1; ++j)
        {
            DrawMultiGizmoLine(leftPathContainer.GetChild(j).position, leftPathContainer.GetChild(j + 1).position);
        }

        // Right path
        for (int j = 0; j < rightPathContainer.childCount - 1; ++j)
        {
            DrawMultiGizmoLine(rightPathContainer.GetChild(j).position, rightPathContainer.GetChild(j + 1).position);
        }

        // Line from the last left node to the next node in the main track
        DrawMultiGizmoLine(leftPathContainer.GetChild(leftPathContainer.childCount - 1).position, nextNode.position);

        // Line from the last right node to the next node in the main track
        DrawMultiGizmoLine(rightPathContainer.GetChild(rightPathContainer.childCount - 1).position, nextNode.position);
    }

    private void VisualizePitLane(Transform container, Transform nextNode)
    {
        // Too few nodes for a track render
        if (container.childCount < 2)
            return;

        // Line from the last main track node to the main track node after the pit lane
        Gizmos.color = m_RegularTrackLineColor;
        DrawMultiGizmoLine(container.position, nextNode.position);

        Gizmos.color = m_PitLaneLineColor;

        // Line from container node to the first node
        DrawMultiGizmoLine(container.position, container.GetChild(0).position);

        // Assume the pit lane nodes are children of the split node
        for (int j = 0; j < container.childCount - 1; ++j)
        {
            DrawMultiGizmoLine(container.GetChild(j).position, container.GetChild(j + 1).position);
        }

        // Line from the last child to the next node in the main track
        DrawMultiGizmoLine(container.GetChild(container.childCount - 1).position, nextNode.position);
    }

    // Since Unity draws lines 1px thin, a custom method is required
    private void DrawMultiGizmoLine(Vector3 from, Vector3 to)
    {
        for (int i = 0; i < m_LineThickness; ++i)
        {
            // Increase the offset every loop to avoid drawing lines on top of the previous lines
            Vector3 offsetForIteraction = m_LineOffset * i;
            Gizmos.DrawLine(from + offsetForIteraction, to + offsetForIteraction);
        }
    }
}
