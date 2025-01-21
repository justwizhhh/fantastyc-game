using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayTypes
{
    // ------------------------------
    //
    //   Class for storing static references to the "types" that characters and objects can be
    //   Used for comparing between them (e.g, only processing collision if the player and object are of different types)
    //
    //   Created: 15/06/2024
    //
    // ------------------------------

    public enum Types
    {
        Fire,
        Water,
        Electric
    }
}
