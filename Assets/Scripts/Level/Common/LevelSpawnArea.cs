using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawnArea : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for defining the area that the player(s) can spawn in
    //
    //   Created: 04/06/2024
    //
    // ------------------------------

    public Vector2 SpawnDirection;
    public float SpawnDistance;

    private PlayerGameplay player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerGameplay>();
    }

    public Vector2[] GetSpawnPositions(int playerCount)
    {
        // Get the direction of the line that the players will be standing on
        Vector2 newDir = new Vector2(SpawnDirection.normalized.y, SpawnDirection.normalized.x);

        // Set the player positions so that they are slightly separated from each other
        Vector2[] posList = new Vector2[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            posList[i] = transform.position + (Vector3)(newDir * SpawnDistance * (i - playerCount / 2));
        }

        return posList;
    }

    private void OnDrawGizmosSelected()
    {
        // Hardcoded value used for rough player collider size, just for editor use
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            transform.position, 
            new Vector2(0.6f + Mathf.Abs(SpawnDirection.y), 0.6f + Mathf.Abs(SpawnDirection.x)) * 4);
    }
}
