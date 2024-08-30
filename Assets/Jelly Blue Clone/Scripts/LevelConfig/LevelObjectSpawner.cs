using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelObjectSpawner : ScriptableObject
{
    public int Move;
    public int ScoreRequest;
    public int ColorSpawnRate;

    public BubbleConfig[] bubbles;
    public Vector2[] icePos;
}
