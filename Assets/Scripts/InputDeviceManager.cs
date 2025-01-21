using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public class InputDeviceManager : MonoBehaviour
{
    // ------------------------------
    //
    //   Takes incoming input devices and returns valid, usable, devices
    //
    //   Created: 12/07/2024
    //
    // ------------------------------

    private static string[] invalidDevices =
    {
        "Mouse",
        "Pointer",
        "Sensor",
        "TrackedDevice",
        "Nintendo Wireless Gamepad"
    };

    public static List<InputDevice> FilterInputDevices()
    {
        List<InputDevice> validDevices = new List<InputDevice>();

        foreach (InputDevice device in InputSystem.devices)
        {
            if (!invalidDevices.Contains(device.name))
            {
                validDevices.Add(device);
                Debug.Log(device.name);
            }
        }

        return validDevices;
    }
}
