using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    // ------------------------------
    //
    //   Global event manager for all in-game and UI events
    //
    //   Created: 27/05/2024
    //
    // ------------------------------

    public GameStates.State currentGameState;

    public static GameController instance;

    [Header("General Toggles")]
    public int PlayerNum;
    public GameObject PlayerInputSpreader;
    public GameObject PlayerGameplayObj;
    public GameObject PlayerEditorObj;
    public int MaxEditorTurns = 1;

    [Space(10)]
    public bool IsRunning;

    [Space(15)]
    [Header("Score Toggles")]
    public int TotalWinScore;
    public int TopScore;
    public int BottomScoreMax;
    public int BottomScoreMin;
    public float BottomScoreTimeMargin;

    [Space(10)]
    public List<int> PlayerScores = new List<int>();

    [Space(15)]
    [Header("Time Toggles")]
    public float GameplayTransitionTime;
    public float GameplayCountdownTime;
    public float EditorTransitionTime;
    public float ResultsTransitionTime;

    [Space(15)]
    [Header("Misc. Values")]
    public int FlaggingPlayers;

    // Private variables
    private bool eventTransition;

    private List<bool> winningPlayers = new List<bool>();
    private List<float> winningPlayerTimes = new List<float>();
    private int firstWinningPlayer;

    private int currentEditorTurn;
    private int currentRound; // Currently unused
    private float currentTimer;

    // Component references
    [Space(10)]
    private List<InputDevice> inputDevices = new List<InputDevice>();
    private List<PlayerGameplay> gameplayPlayers = new List<PlayerGameplay>();
    private List<PlayerEditor> editorPlayers = new List<PlayerEditor>();
    private List<PlayerInputSpreader> playerInputs = new List<PlayerInputSpreader>();

    private CameraController camController;
    private UIController uiController;
    private Canvas canvas;

    // Sets up singleton and all component references
    private void Awake()
    {
        instance = this;

        camController = (CameraController)FindFirstObjectByType(typeof(CameraController));
        uiController = (UIController)FindFirstObjectByType(typeof(UIController));
        canvas = (Canvas)FindFirstObjectByType(typeof(Canvas));
    }

    private void CreatePlayers()
    {
        // Find all valid input devices
        inputDevices = InputDeviceManager.FilterInputDevices();

        // Create all of the players for the start of the game
        for (int i = 0; i < PlayerNum; i++)
        {
            GameObject newPlayerGameplay = Instantiate(PlayerGameplayObj);
            newPlayerGameplay.transform.SetSiblingIndex(8);
            gameplayPlayers.Add(newPlayerGameplay.GetComponent<PlayerGameplay>());

            GameObject newEditorGameplay = Instantiate(PlayerEditorObj);
            newEditorGameplay.transform.SetParent(canvas.transform.GetChild(1).transform);
            editorPlayers.Add(newEditorGameplay.GetComponent<PlayerEditor>());

            // If there are multiple players, but only one device, the first device gets used as a failsafe
            InputDevice targetInput;
            if (i >= inputDevices.Count)
            {
                targetInput = inputDevices[0];
            }
            else
            {
                targetInput = inputDevices[i];
            }

            // All device-specific inputs get sent to the input spreader only
            PlayerInputSpreader newInputSpreader =
                PlayerInput.Instantiate(PlayerInputSpreader, i, null, -1, targetInput).GetComponent<PlayerInputSpreader>();
            newInputSpreader.GameplayPlayer = newPlayerGameplay.GetComponent<PlayerGameplay>();
            newInputSpreader.EditorPlayer = newEditorGameplay.GetComponent<PlayerEditor>();
            playerInputs.Add(newInputSpreader);

            PlayerScores.Add(0);
            winningPlayers.Add(false);
            winningPlayerTimes.Add(0);
        }
    }

    // Sets up the start of the actual game
    private void Start()
    {
        CreatePlayers();
        camController.FindPlayers();
        SwitchGameState(currentGameState, 0);
    }

    private void DisableEditorPlayers()
    {
        foreach (PlayerEditor player in editorPlayers)
        {
            player.gameObject.SetActive(false);
        }
    }

    private void DisableGameplayPlayers()
    {
        foreach (PlayerGameplay player in gameplayPlayers)
        {
            player.gameObject.SetActive(false);
        }
    }

    public void DisableInput()
    {
        foreach (PlayerInputSpreader input in playerInputs)
        {
            input.Active = false;
        }
    }

    public void EnableInput()
    {
        foreach (PlayerInputSpreader input in playerInputs)
        {
            input.Active = true;
            input.ProcessHeldInputs();
        }
    }

    // Temporarily disables player input, and re-enables it again
    public IEnumerator GameplayCountdown(float seconds)
    {
        DisableInput();
        yield return new WaitForSeconds(seconds);
        EnableInput();
        LevelController.instance.ActivateGameplayObjects();
        StopCoroutine(GameplayCountdown(seconds));
    }

    private void ResetEditors()
    {
        foreach (PlayerEditor player in editorPlayers)
        {
            player.gameObject.SetActive(true);
            player.currentMoveInput = Vector2.zero;
            player.transform.SetAsLastSibling();
            player.GetComponent<SpriteRenderer>().enabled = false;

            player.SwitchToUI();
        }
    }

    private void ResetGameplays()
    {
        // Put the players back to their initial spawn positions
        Vector2[] newPlayerPos = LevelController.instance.LevelSpawnArea.GetSpawnPositions(gameplayPlayers.Count);
        for (int i = 0; i < newPlayerPos.Length; i++)
        {
            gameplayPlayers[i].gameObject.SetActive(true);
            gameplayPlayers[i].rb.position = newPlayerPos[i];
            gameplayPlayers[i].transform.localScale = Vector3.one;

            gameplayPlayers[i].col.enabled = true;

            gameplayPlayers[i].velocity = LevelController.instance.LevelSpawnArea.SpawnDirection;
            gameplayPlayers[i].prevMoveInput = LevelController.instance.LevelSpawnArea.SpawnDirection;
            gameplayPlayers[i].currentMoveInput = Vector2.zero;

            gameplayPlayers[i].flagged = false;

            gameplayPlayers[i].stateMachine.ChangeState(typeof(PlayerIdle));
        }
    }

    public void UpdateFlaggingPlayers()
    {
        FlaggingPlayers++;
        // TO-DO : Add a UI for how many flags have been raised
    }

    public void UpdateWinningPlayers(PlayerGameplay newPlayer)
    {
        winningPlayers[gameplayPlayers.IndexOf(newPlayer)] = true;
        winningPlayerTimes[gameplayPlayers.IndexOf(newPlayer)] = currentTimer;

        // Keep track of the first player to win
        int winningCount = 0;
        foreach (bool player in winningPlayers)
        {
            if (player)
            {
                winningCount++;
            }
        }

        if (winningCount == 0)
        {
            firstWinningPlayer = gameplayPlayers.IndexOf(newPlayer);
        }
    }

    private void UpdateScores()
    {
        // Give each player their scores
        // ... but not if everyone in the game has won
        if (winningPlayers.FindAll(x => x == true).Count >= gameplayPlayers.Count)
        {
            Debug.Log("No contest!");
        }
        else
        {
            for (int i = 0; i <= PlayerScores.Count() - 1; i++)
            {
                if (winningPlayers[i] == false)
                {
                    continue;
                }
                else
                {
                    if (i == firstWinningPlayer)
                    {
                        PlayerScores[i] += TopScore;
                    }
                    else
                    {
                        // The rest of the players get a fraction of the top score based on how long they took to reach the goal
                        float timeDif = winningPlayerTimes[i] - winningPlayerTimes[firstWinningPlayer];
                        float scoreMultiplier = Mathf.Clamp((timeDif / BottomScoreTimeMargin * -1) + 1, 0, 1);
                        PlayerScores[i] += (int)Mathf.Lerp(BottomScoreMin, BottomScoreMax, scoreMultiplier);
                    }
                }
            }
        }

        // Update the UI
        uiController.UpdateScores(PlayerScores.ToArray());

        // End game if a player has reached the max score for winning (WIP)
        if (PlayerScores.Contains(TotalWinScore))
        {
            Debug.Log("Player" + PlayerScores.FindIndex(x => x == TotalWinScore) + " wins!");
        }
    }

    // Sets up global variables before the game transitions into a new state
    private IEnumerator SwitchState(GameStates.State newState, float delayTime)
    {
        eventTransition = true;
        
        // Pause for a few seconds before switching to a new state
        yield return new WaitForSeconds(delayTime);

        // ... and then start switching
        currentTimer = 0;

        currentGameState = newState;
        switch (currentGameState)
        {
            case GameStates.State.Intro:
                EventManager.OnSwitchToIntro();
                DisableEditorPlayers();
                DisableGameplayPlayers();
                DisableInput();
                break;

            case GameStates.State.Editor:
                EventManager.OnSwitchToEditing();

                currentRound++;
                for (int i = 0; i < gameplayPlayers.Count(); i++)
                {
                    winningPlayers[i] = false;
                    winningPlayerTimes[i] = 0;
                }

                // Set up the player cursor objects to start placing objects
                ResetEditors();
                EnableInput();

                foreach (PlayerGameplay player in gameplayPlayers)
                {
                    player.gameObject.SetActive(false);
                }
                break;

            case GameStates.State.Gameplay:
                EventManager.OnSwitchToGameplay();
                currentEditorTurn = 0;
                FlaggingPlayers = 0;

                // Set up the in-level player objects to start moving around
                ResetGameplays();

                foreach (PlayerEditor player in editorPlayers)
                {
                    player.gameObject.SetActive(false);
                }

                // Start gameplay countdown
                StartCoroutine(GameplayCountdown(GameplayCountdownTime));

                break;

            case GameStates.State.Results:
                EventManager.OnSwitchToResults();
                UpdateScores();
                break;

            case GameStates.State.GameOver:
            default:
                DisableEditorPlayers();
                DisableGameplayPlayers();
                EventManager.OnSwitchToGameOver();
                break;
        }

        eventTransition = false;

        StopCoroutine(SwitchState(newState, delayTime));
    }

    public void SwitchGameState(GameStates.State newState, float delayTime)
    {
        StartCoroutine(SwitchState(newState, delayTime));
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;

        // Transition to editing
        // If everyone has either reached the end of the level, or died, go back to editing mode
        if (currentGameState == GameStates.State.Gameplay)
        {
            int endPlayers = 0;
            foreach (PlayerGameplay player in gameplayPlayers)
            {
                if (player.stateMachine.currentPlayerState.GetType() == typeof(PlayerWin)
                    || player.stateMachine.currentPlayerState.GetType() == typeof(PlayerDead))
                {
                    endPlayers++;
                }
            }
            if (endPlayers >= gameplayPlayers.Count)
            {
                if (!eventTransition)
                {
                    SwitchGameState(GameStates.State.Results, ResultsTransitionTime);
                }
            }
        }

        if (currentGameState == GameStates.State.Editor) 
        {
            // If everyone has selected an object, move all players off of the canvas
            if (LevelController.instance.selectedPrevObjects.Count >= editorPlayers.Count)
            {
                foreach (PlayerEditor player in editorPlayers)
                {
                    player.SwitchToSprite();
                }
                uiController.TurnOffEditorContainers();
            }

            // Transition to gameplay
            // If every player has placed down their objects, switch to gameplay mode
            if (LevelController.instance.placedPrevObjects.Count >= editorPlayers.Count)
            {
                LevelController.instance.GenerateGameplayObjects();

                // Allow the player to place more objects down, if the game settings are set up to allow so
                if (currentEditorTurn < MaxEditorTurns - 1)
                {
                    currentEditorTurn++;
                    ResetEditors();
                    LevelController.instance.GeneratePreviewObjects();
                    uiController.TurnOnEditorContainers();
                }
                else
                {
                    if (!eventTransition)
                    {
                        SwitchGameState(GameStates.State.Gameplay, GameplayTransitionTime);
                    }
                }
                
            }
        }
    }
}
