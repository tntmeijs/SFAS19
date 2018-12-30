using System;
using UnityEngine;

/// <summary>
/// The purpose of this class is to provide a centralized storage of data for multiple scripts.
/// It makes it easy to change certain values, as all other scripts get their values from this class.
/// </summary>
public static class Global
{
    public const int MAXIMUM_NUMBER_OF_PLAYERS = 4;

    // Fool-proof player number range
    public enum Player
    {
        PlayerOne = 0,
        PlayerTwo = 1,
        PlayerThree = 2,
        PlayerFour = 3
    }

    // A keyboard is considered a controller as well
    public enum Controllers
    {
        None = -1,

        Keyboard,   // 0
        Joystick1,  // 1
        Joystick2,  // 2
        Joystick3,  // 3
        Joystick4,  // 4
    }

    // This mapping assumes an XboxOne controller
    public enum JoystickButton
    {
        A, B, X, Y,

        LeftBumper,
        RightBumper,

        View,   // Called "back" on Xbox360
        Menu,   // Called "start" on Xbox360
    }

    // Sadly, this does not work the same as the input buttons do. It still depends 100% on the input manager.
    // Would love a workaround for this, but for now, this enumeration will only be used as a better way to convery
    // joystick axis set-up to designers through the inspector window. Please have a look at the input manager to see
    // how it is set-up under the hood.
    public enum JoystickAxis
    {
        LeftStickHorizontal,
        LeftStickVertical,
        
        RightStickHorizontal,
        RightStackVertical,

        TriggerLeft,
        TriggerRight,

        DPadVertical,
        DPadHorizontal
    }

    public static KeyCode ConvertJoystickButtonToKeycode(Controllers joystickID, JoystickButton button)
    {
        // Convert to one of the key code enum values (it is safe to assume that the Unity enumeration names will never
        // change. This is why the hard-coded values are justified. If, for some reason, these value change in the future,
        // please expose them to the inspector. However, they have been the same for the last few years, so there is no
        // real reason to assume that this will change anytime soon.
        return (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + (int)joystickID + "Button" + (int)button);
    }

    public struct PlayerInputData
    {
        // True when pressing the throttle button
        public bool throttle;

        // True when pressing the brake button
        public bool braking;

        // Steering value clamped between -1.0f and 1.0f
        public float steeringValue;

        // True when the power-up should be fired (does not know whether the player has a power-up, just a dumb flag)
        public bool activatedPowerUp;
    };
}
