using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    // ------------------------------
    //
    //   Manager script for showing different UI assets during different states
    //
    //   Created: 12/06/2024
    //
    // ------------------------------

    // Public variables
    [Header("Editor Screen Assets")]
    public GameObject[] EditorContainers;

    [Space(10)]
    [Header("Gameplay Screen Assets")]
    public Animation GameplayCountdownAnim;

    [Space(10)]
    [Header("Results Screen Assets")]
    public Transform PlayerResultsParent;

    // Private variables
    private List<GameObject> UIParents = new List<GameObject>(5);
    private UIScore[] UIScoreDisplays;

    // Component references
    private Canvas canvas;
    private PlayerInput input;

    private void OnEnable()
    {
        EventManager.SwitchToIntro += EventManager_SwitchToIntro;
        EventManager.SwitchToEditing += EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay += EventManager_SwitchToGameplay;
        EventManager.SwitchToResults += EventManager_SwitchToResults;
        EventManager.SwitchToGameOver += EventManager_SwitchToGameOver;
    }

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            UIParents.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).gameObject.SetActive(false);
        }
        UIScoreDisplays = FindObjectsByType<UIScore>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        canvas = GetComponent<Canvas>();
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        CentrePlayerResultDisplays();
    }

    [ContextMenu("CentrePlayerResultDisplays")]
    private void CentrePlayerResultDisplays()
    {
        // Move all of the player score displays to be in the centre of the screen
        List<Transform> movedDisplays = new List<Transform>();

        for (int i = 0; i < 4; i++)
        {
            if (i < GameController.instance.PlayerNum)
            {
                PlayerResultsParent.GetChild(i).gameObject.SetActive(true);
                movedDisplays.Add(PlayerResultsParent.GetChild(i));
            }
            else
            {
                PlayerResultsParent.GetChild(i).gameObject.SetActive(false);
            }
        }

        PlayerResultsParent.DetachChildren();

        Vector2 centroid = Vector2.zero;
        foreach (Transform t in movedDisplays)
        {
            centroid += (Vector2)t.localPosition;
        }
        centroid /= GameController.instance.PlayerNum;

        PlayerResultsParent.position = centroid;

        foreach (Transform t in movedDisplays)
        {
            t.SetParent(PlayerResultsParent, true);
        }

        PlayerResultsParent.position = canvas.pixelRect.center;
    }

    // Make all other UI assets invisible except for one
    private void SwitchUI(int ui)
    {
        for (int i = 0; i < UIParents.Count - 1; i++)
        {
            if (i == ui)
            {
                UIParents[i].SetActive(true);
                for (int j = 0; j < UIParents[i].transform.childCount - 1; j++)
                {
                    UIParents[i].transform.GetChild(j).gameObject.SetActive(true);
                }
            }
            else
            {
                UIParents[i].SetActive(false);
            }
        }
    }

    private void EventManager_SwitchToIntro()
    {
        SwitchUI(0);
        input.enabled = false;
    }

    private void EventManager_SwitchToEditing()
    {
        SwitchUI(1);
        input.enabled = false;
    }

    private void EventManager_SwitchToGameplay()
    {
        SwitchUI(2);
        input.enabled = false;
    }

    private void EventManager_SwitchToResults()
    {
        SwitchUI(3);
        input.enabled = true;
    }

    private void EventManager_SwitchToGameOver()
    {
        SwitchUI(4);
        input.enabled = true;
    }

    // Turn off the sidebar containers after everyone has selected an object
    public void TurnOffEditorContainers()
    {
        foreach (GameObject obj in EditorContainers)
        {
            obj.SetActive(false);
        }
    }

    // Turn them back on
    public void TurnOnEditorContainers()
    {
        foreach (GameObject obj in EditorContainers)
        {
            obj.SetActive(true);
        }
    }

    // Functions for progressing further in the results/game-over screens
    private void OnSelect(InputValue value)
    {
        switch (GameController.instance.currentGameState)
        {
            case GameStates.State.Results:
                GameController.instance.SwitchGameState(
                    GameStates.State.Editor,
                    GameController.instance.EditorTransitionTime);

                input.enabled = false;
                break;

            default:
                break;
        }
    }

    private void OnBack(InputValue value)
    {
        switch (GameController.instance.currentGameState)
        {
            default:

                break;
        }
    }

    // Updating UI features and statistics
    public void UpdateScores(int[] newScores)
    {
        for (int i = 0; i < GameController.instance.PlayerNum; i++)
        {
            UIScoreDisplays[i].UpdateText(newScores[i].ToString());
        }
    }

    private void OnDisable()
    {
        EventManager.SwitchToIntro -= EventManager_SwitchToIntro;
        EventManager.SwitchToEditing -= EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay -= EventManager_SwitchToGameplay;
        EventManager.SwitchToResults -= EventManager_SwitchToResults;
        EventManager.SwitchToGameOver -= EventManager_SwitchToGameOver;
    }
}
