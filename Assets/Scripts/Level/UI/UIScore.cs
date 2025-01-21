using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    // ------------------------------
    //
    //   Uses info passed from GameController to update the score values on the results screen
    //
    //   Created: 22/07/2024
    //
    // ------------------------------

    [SerializeField] private Text text;

    public void UpdateText(string newText)
    {
        // TO-DO: make this actually interesting and not just a number display lmao
        text.text = newText;
    }
}
