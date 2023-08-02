using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPosition : MonoBehaviour
{
    [SerializeField]
    private Transform _entrancePosition;
    public Transform EntrancePosition
    {
        get { return _entrancePosition; }
        set { _entrancePosition = value; }
    }
}
