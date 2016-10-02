using UnityEngine;
using System.Collections;
using Resource.Properties;
using System;

public class TestEnum : MonoBehaviour {
    [SerializeField, EnumFlags]
    private ENEMY_TYPE enemyType = ENEMY_TYPE.Any;

    [Flags]
    public enum ENEMY_TYPE {
        None = 0,
        Soldier = 1,
        Buzzer = 2,
        Healer = 4,
        Gasbag = 8,
        Drone = 16,
        Ground = Soldier | Drone,
        Flying = Buzzer | Healer | Gasbag,
        Any = Buzzer | Healer | Gasbag | Soldier | Drone
    }

    void Start() {
        foreach (ENEMY_TYPE type in EnumUtils.GetIndividualFlags(enemyType)) {
            Debug.Log(type);
        }

        Debug.Log("Next");

        foreach (ENEMY_TYPE type in EnumUtils.GetFlags(enemyType)) {
            Debug.Log(type);
        }

        if (EnumUtils.HasFlag(ENEMY_TYPE.Flying, enemyType)) {
            Debug.Log("Has Flag");
        } else {
            Debug.Log("Nope");
        }
    }

}
