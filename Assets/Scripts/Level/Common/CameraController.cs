using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraController : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for moving the player(s) camera around during gameplay, for both editing and playing modes
    //
    //   Created: 14/05/2024
    //
    // ------------------------------

    enum CamStates
    {
        INTRO,
        EDITOR_MODE,
        GAMEPLAY_MODE,
        RESULTS
    }

    // Toggleable variables
    [SerializeField] private Transform CamAnchor;

    [Space(10)]
    [Header("Intro Camera")]
    [Space(2)]
    public AnimationClip IntroAnimation;

    [Space(10)]
    [Header("Editor Camera")]
    [Space(2)]
    [SerializeField] private Transform EditorTarget;
    [SerializeField] private float EditorResizeSpeed;
    [SerializeField] private float EditorResizeDamping;
    [SerializeField] private float EditorCamSize;

    [Space(10)]
    [Header("Gameplay Camera")]
    [Space(2)]
    [SerializeField] private float ResizeSpeed;
    [SerializeField] private float ResizeDamping;
    [SerializeField] private float MinCamSize;
    [SerializeField] private float MaxCamSize;
    [SerializeField] private float CamSizeMargin;

    [Space(10)]
    [Header("Results Camera")]
    [Space(2)]
    [SerializeField] private float ResultsResizeDamping;

    // Private variables
    private List<Transform> GameplayTargets = new List<Transform>();
    private CamStates currentCamState = CamStates.GAMEPLAY_MODE;

    private Vector2 targetPos;
    private float targetSize;

    private Vector2 afterIntroPos;

    // Component references
    private CinemachineBrain camBrain;
    private CinemachineVirtualCamera virtualCam;
    private CinemachineTransposer virtualCamTarget;
    private Animation virtualCamAnim;

    private void Awake()
    {
        camBrain = GetComponent<CinemachineBrain>();
        virtualCam = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamTarget = FindObjectOfType<CinemachineTransposer>();
        virtualCamAnim = virtualCam.GetComponent<Animation>();
    }

    public void FindPlayers()
    {
        PlayerGameplay[] newTargets = FindObjectsByType<PlayerGameplay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerGameplay target in newTargets)
        {
            GameplayTargets.Add(target.transform);
        }
    }

    private void OnEnable()
    {
        EventManager.SwitchToIntro += EventManager_SwitchToIntro;
        EventManager.SwitchToEditing += EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay += EventManager_SwitchToGameplay;
        EventManager.SwitchToResults += EventManager_SwitchToResults;
    }

    private void EventManager_SwitchToIntro()
    {
        currentCamState = CamStates.INTRO;
        IntroCamera();
    }

    private void EventManager_SwitchToGameplay()
    {
        currentCamState = CamStates.GAMEPLAY_MODE;
        virtualCamTarget.m_XDamping = ResizeDamping;
        virtualCamTarget.m_YDamping = ResizeDamping;
    }

    private void EventManager_SwitchToEditing()
    {
        currentCamState = CamStates.EDITOR_MODE;
        virtualCam.transform.position = afterIntroPos;
        virtualCamTarget.m_XDamping = EditorResizeDamping;
        virtualCamTarget.m_YDamping = EditorResizeDamping;
    }

    private void EventManager_SwitchToResults()
    {
        currentCamState = CamStates.RESULTS;
        virtualCamTarget.m_XDamping = ResultsResizeDamping;
        virtualCamTarget.m_YDamping = ResultsResizeDamping;
    }

    // Play a brief animation before the game starts
    private void IntroCamera()
    {
        virtualCam.Follow = null;
        virtualCamAnim.clip = IntroAnimation;
        virtualCamAnim.Play();
        StartCoroutine(IntroCameraTimer());
    }

    private IEnumerator IntroCameraTimer()
    {
        yield return new WaitForSeconds(IntroAnimation.length);
        afterIntroPos = transform.position;

        GameController.instance.SwitchGameState(
            GameStates.State.Editor, 
            GameController.instance.EditorTransitionTime);
    }

    private void EditorCamera()
    {
        virtualCam.Follow = CamAnchor;

        // Zoom out the camera, to let the player see the entire level
        targetPos = EditorTarget.transform.position;
        targetSize = EditorCamSize;
    }

    private void GameplayCamera()
    {
        virtualCam.Follow = CamAnchor;

        // Get the centre position between each and all target objects, making sure all objects are in view
        Vector2 combinedPos = Vector2.zero;
        foreach (Transform target in GameplayTargets)
        {
            combinedPos += (Vector2)target.position;
        }

        targetPos = combinedPos / GameplayTargets.Count;

        // Then, resize the camera by measuring the distance between it and the target objects
        float combinedDistance = CamSizeMargin;
        foreach (Transform target in GameplayTargets)
        {
            combinedDistance += Vector2.Distance(target.position, virtualCam.transform.position);
        }

        targetSize = combinedDistance / GameplayTargets.Count;
    }

    private void Update()
    {
        switch (currentCamState)
        {
            case CamStates.EDITOR_MODE:
            case CamStates.RESULTS:
                EditorCamera();

                virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
                    virtualCam.m_Lens.OrthographicSize,
                    targetSize,
                    EditorResizeSpeed * Time.deltaTime);

                CamAnchor.position = targetPos;

                break;

            case CamStates.GAMEPLAY_MODE:
                GameplayCamera();

                virtualCam.m_Lens.OrthographicSize = Mathf.Clamp(
                    Mathf.Lerp(
                        virtualCam.m_Lens.OrthographicSize,
                        targetSize,
                        ResizeSpeed * Time.deltaTime
                    ),
                    MinCamSize,
                    MaxCamSize);

                CamAnchor.position = targetPos;
                break;
        }
    }

    private void OnDisable()
    {
        EventManager.SwitchToIntro -= EventManager_SwitchToIntro;
        EventManager.SwitchToEditing -= EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay -= EventManager_SwitchToGameplay;
    }
}
