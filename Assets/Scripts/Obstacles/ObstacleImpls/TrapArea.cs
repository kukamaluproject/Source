using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles.ObstacleImpls
{
    public class TrapArea : MonoBehaviour, IObstacle
    {
        public EObstacleType ObstacleType => EObstacleType.TrapArea;
        [SerializeField] private Trap trap;

        public void ShowTrap()
        {
            trap.ShowTrap();
            Destroy(gameObject);
        }
    }
}
