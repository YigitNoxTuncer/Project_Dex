using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOX
{
    public class PathWay : MonoBehaviour
    {
        [SerializeField] public int pathWayID;
        [SerializeField] public List<Transform> wayPoints;
    }

}
