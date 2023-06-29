using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;
using Unity.Profiling;

public class EnemyBehavior : MonoBehaviour
{
    ProfilerRecorder _totalReservedMemoryRecorder;
    AIDestinationSetter destinationSetter;
    Patrol patrol;
    AIPath path;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject exit;
    [SerializeField]
    GameObject teammate;
    [SerializeField]
    float activateDistance;
    [SerializeField]
    bool followEnabled;
    [SerializeField]
    bool isSmart;
    public bool isChasing = false;
    bool teammateChasing = false;
    void Start()
    {
        _totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        destinationSetter = GetComponent<AIDestinationSetter>();
        patrol = GetComponent<Patrol>();
        path = GetComponent<AIPath>();
        if (isSmart)
        {
            destinationSetter.target = exit.transform;
        } else
        {
            destinationSetter.target = player.transform;
        }
        
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, destinationSetter.target.transform.position) < activateDistance;
    }

    void regularEnemy()
    {
        
        var sb = new StringBuilder(500);
        if (_totalReservedMemoryRecorder.Valid) {
            sb.AppendLine($"Total Reserved Memory: {_totalReservedMemoryRecorder.LastValue}");
            Debug.Log(sb);
        }
            
        var temp = Time.realtimeSinceStartup;
        if (TargetInDistance() && followEnabled)
        {
            patrol.enabled = false;
            destinationSetter.enabled = true;
            path.maxSpeed = 3;
            isChasing = true;
        } else
        {
            destinationSetter.enabled = false;
            patrol.enabled = true;
            path.maxSpeed = 1;
            isChasing = false;
        }
        print("Time: " + (Time.realtimeSinceStartup - temp).ToString("f10"));
    }

    void smartEnemy()
    {
        teammateChasing = teammate.GetComponent<EnemyBehavior>().isChasing;
        if (teammateChasing && !TargetInDistance())
        {
            patrol.enabled = false;
            destinationSetter.enabled = true;
            path.maxSpeed = 3;
        } else if (teammateChasing && TargetInDistance())
        {
            destinationSetter.target = player.transform;
        } else if (!teammateChasing)
        {
            destinationSetter.enabled = false;
            patrol.enabled = true;
            path.maxSpeed = 1;
        }
    }

    void Update() {
        if (isSmart)
        {
            smartEnemy();
        } else
        {
            regularEnemy();
        }
    }
}
