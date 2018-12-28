using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    // --------------------------------------------------------------

    // Maps the controller to the player index
    // List[0] --> player 1
    // List[1] --> player 2
    // List[2] --> player 3
    // List[3] --> player 4
    private List<int> m_RegisteredControllersIndices = new List<int>();

    private enum Controllers
    {
        // Starts at -2 to keep the enum mapping simple when converting to integers in the code below
        Invalid = -2,

        Keyboard,   // -1
        Joystick0,  //  0
        Joystick1,  //  1
        Joystick2,  //  2
        Joystick3,  //  3

        TotalNumberOfControllers    // Always equal to the number of controllers if this kind of enum style is used
    }

    private void OnGUI()
    {
        for (int i = 0; i < m_RegisteredControllersIndices.Count; ++i)
        {
            if (m_RegisteredControllersIndices[i] == (int)Controllers.Invalid)
                GUI.Label(new Rect(200, 20 * i, 200, 20), "Player " + i + " & index invalid");
            else
                GUI.Label(new Rect(200, 20 * i, 200, 20), "Player " + i + " & index " + m_RegisteredControllersIndices[i]);
        }
    }

    private void Awake()
    {
        for (int index = 0; index < (int)Controllers.TotalNumberOfControllers; ++index)
        {
            m_RegisteredControllersIndices.Add((int)Controllers.Invalid);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddGameController(Controllers.Keyboard);
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            AddGameController(Controllers.Joystick0);
        }

        if (Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            AddGameController(Controllers.Joystick1);
        }

        if (Input.GetKeyDown(KeyCode.Joystick3Button0))
        {
            AddGameController(Controllers.Joystick2);
        }

        if (Input.GetKeyDown(KeyCode.Joystick4Button0))
        {
            AddGameController(Controllers.Joystick3);
        }
    }

    void AddGameController(Controllers type)
    {
        // The controller that is being added exceeds the maximum number of controllers allowed, no need to continue
        if (m_RegisteredControllersIndices.Count > (int)Controllers.TotalNumberOfControllers)
            return;

        foreach (int controllerIndex in m_RegisteredControllersIndices)
        {
            if (controllerIndex == (int)type)
            {
                // Controller already in use
                return;
            }
        }

        // Register the controller index
        for (int controllerIndex = 0; controllerIndex < (int)Controllers.TotalNumberOfControllers; ++controllerIndex)
        {
            if (m_RegisteredControllersIndices[controllerIndex] == (int)Controllers.Invalid)
            {
                m_RegisteredControllersIndices[controllerIndex] = (int)type;
                break;  // Only set one controller, if the others are invalid, leave them for the next controller
            }
        }
    }

    void RemoveGameController(int index)
    {
        // No more controllers left to unregister
        if (m_RegisteredControllersIndices.Count < 1)
            return;
        
        foreach (int controllerIndex in m_RegisteredControllersIndices)
        {
            if (controllerIndex == index)
            {
                m_RegisteredControllersIndices[controllerIndex] = (int)Controllers.Invalid;
            }
        }
    }
}
