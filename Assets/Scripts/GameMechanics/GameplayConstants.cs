using UnityEngine;

public class GameplayConstants : ScriptableObject {
    public const float RESPAWN_HEIGHT = 32f;
    public const float SLIP_ZONE_WIDTH = 0.015f;
    public const float SPAWN_ZONE_DROP_HEIGHT = 4f;
    public const float SPAWN_ZONE_MINIMUM_HEIGHT = 3f;
    public const float SPAWN_ZONE_MINIMUM_WIDTH = 2f;
    public const float START_DISTANCE = 4.5f;


    public const int LAYER_MINI_MAP = 8;
    public const int MAXIMUM_SECTIONS = 6;
    public const int SCORE_DISTANCE_MULTIPLIER = 5;
    public const int SCORE_ENEMY_MULTIPLIER = 50;
    public const int STARTING_LIVES = 100;
}