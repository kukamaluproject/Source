using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles.ObstacleImpls
{
    public class Stairs : MonoBehaviour, IObstacle
    {
        public EObstacleType ObstacleType => EObstacleType.Stairs;

        public bool IsUpStairs;
    }
}
