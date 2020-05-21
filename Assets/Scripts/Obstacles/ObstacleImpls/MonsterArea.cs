using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles.ObstacleImpls
{
    public class MonsterArea : MonoBehaviour, IObstacle
    {
        [SerializeField] private Monster monster;
        public EObstacleType ObstacleType => EObstacleType.MonsterArea;

        public void SetTarget(Player player)
        {
            monster.SetTarget(player);
            //Destroy(gameObject);
        }
    }
}
