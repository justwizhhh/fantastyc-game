using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    // ------------------------------
    //
    //   Manager script for configuring level settings, and for spawning in gameplay objects
    //
    //   Created: 31/05/2024
    //
    // ------------------------------

    public static LevelController instance;

    // Public variables
    public string LevelName;
    public string LevelDescription;
    public Transform LevelCenter;
    public LevelSpawnArea LevelSpawnArea;

    [Space(10)]
    public int UIObjectCount;
    public List<PreviewObject> LevelObjects = new List<PreviewObject>();
    public List<PreviewObject> DestructiveObjects = new List<PreviewObject>();

    [HideInInspector] public List<PreviewObject> currentPrevObjects = new List<PreviewObject>();
    [HideInInspector] public List<PreviewObject> selectedPrevObjects = new List<PreviewObject>();
    [HideInInspector] public List<PreviewObject> placedPrevObjects = new List<PreviewObject>();
    [HideInInspector] public List<BaseObject> currentLevelObjects = new List<BaseObject>();

    [HideInInspector] public List<ObjectPlacementSpace> objectPlaceSpaces = new List<ObjectPlacementSpace>();

    private List<UIObjectSlot> uiObjectPositions = new List<UIObjectSlot>();

    // Component references
    private Canvas canvas;
    private RectTransform canvasRt;

    private void Awake()
    {
        instance = this;

        objectPlaceSpaces.AddRange(FindObjectsByType<ObjectPlacementSpace>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        uiObjectPositions.AddRange(FindObjectsByType<UIObjectSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        canvas = FindObjectOfType<Canvas>();
        canvasRt = canvas.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventManager.SwitchToEditing += EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay += EventManager_SwitchToGameplay;
        EventManager.SwitchToResults += EventManager_SwitchToResults;
    }

    private void DestroyDeletedObjects()
    {
        // Go through the current level object list, and destroy those that we know won't be used again in the scene
        foreach (BaseObject obj in currentLevelObjects)
        {
            if (obj.Deleted)
            {
                // Destroy their accompanying projectile objects if they have any
                if (obj.TryGetComponent(out ObjectPooler pool))
                {
                    foreach (GameObject poolObj in pool.PooledObjects)
                    {
                        currentLevelObjects.Remove(poolObj.GetComponent<BaseObject>());
                        Destroy(poolObj);
                    }
                }

                Destroy(obj);
            }
        }
    }

    private void CreateObject(PreviewObject obj)
    {
        PreviewObject newObjSpawn = Instantiate(obj);
        currentPrevObjects.Add(newObjSpawn);
        newObjSpawn.transform.SetParent(uiObjectPositions[0].transform.parent, false);
        newObjSpawn.transform.SetAsLastSibling();
    }

    // Fill up the list of objects the players will be able to pick from
    public void GeneratePreviewObjects()
    {
        // Clear previous object references
        currentPrevObjects.Clear();
        selectedPrevObjects.Clear();
        placedPrevObjects.Clear();

        // First, spawn in all of the needed objects
        if (LevelObjects.Count < UIObjectCount)
        {
            Debug.LogError("Object generation cannot be completed - not enough object varieties!");
        }
        else
        {
            // Adding additional bombs into the item list, if any player has been found flagging the previous round
            for (int i = 0; i < GameController.instance.FlaggingPlayers; i++)
            {
                CreateObject(DestructiveObjects[Random.Range(0, DestructiveObjects.Count)]);
            }
            
            List<int> objectIndexes = new List<int>();
            while (currentPrevObjects.Count < UIObjectCount)
            {
                int newIndex = Random.Range(0, LevelObjects.Count);
                if (!objectIndexes.Contains(newIndex))
                {
                    objectIndexes.Add(newIndex);

                    var newObj = LevelObjects[newIndex];
                    var chance = Random.Range(0.0f, 1.0f);

                    if (chance < newObj.Rarity)
                    {
                        CreateObject(newObj);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            // Then, position them on the screen for selection
            for (int i = 0; i < currentPrevObjects.Count; i++)
            {
                currentPrevObjects[i].transform.position = uiObjectPositions[i].transform.position;
            }
        }
    }

    // Get rid of all unselected preview objects
    public void ClearPreviewObjects()
    {
        foreach (PreviewObject obj in currentPrevObjects)
        {
            Destroy(obj.gameObject);
        }
        currentPrevObjects.Clear();
    }

    // Replace the preview objects with the actual interactive objects
    public void GenerateGameplayObjects()
    {
        foreach (PreviewObject obj in selectedPrevObjects)
        {
            if (obj.SourceObjects.Length == 0)
            {
                continue;
            }
            else
            {
                var newObj = Instantiate(obj.SourceObjects[obj.Variation].gameObject, obj.transform.position, obj.transform.rotation);

                if (newObj != null)
                {
                    // Activate the new object
                    BaseObject newBaseObj = newObj.GetComponent<BaseObject>();

                    newBaseObj.Animated = true;
                    currentLevelObjects.Add(newBaseObj);

                    Destroy(obj.gameObject);
                }
            }
        }
        selectedPrevObjects.Clear();
    }

    public void ActivateGameplayObjects()
    {
        foreach (BaseObject obj in currentLevelObjects)
        {
            if (!obj.Deleted)
            {
                obj.gameObject.SetActive(true);
                obj.OnActivate();
            }
        }
    }

    private void DeactivateGameplayObjects()
    {
        foreach (BaseObject obj in currentLevelObjects)
        {
            obj.OnDeactivate();
        }
    }

    private void EventManager_SwitchToEditing()
    {
        DestroyDeletedObjects();
        GeneratePreviewObjects();
    }

    private void EventManager_SwitchToGameplay()
    {
        GenerateGameplayObjects();
    }

    private void EventManager_SwitchToResults()
    {
        DeactivateGameplayObjects();
    }

    private void OnDisable()
    {
        EventManager.SwitchToEditing -= EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay -= EventManager_SwitchToGameplay;
        EventManager.SwitchToResults -= EventManager_SwitchToResults;
    }
}
