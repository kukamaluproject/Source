using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles.ObstacleImpls
{
    public class Trap : MonoBehaviour, IObstacle
    {
        private static readonly Vector2 MIN_POSITIONS = new Vector2(-2.5f, -2.5f);
        private static readonly Vector2 MAX_POSITIONS = new Vector2(2.5f, 2.5f);
        private static readonly float MIN_SIZE = 1f;
        private static readonly float MAX_SIZE = 2f;


        private MeshRenderer meshRenderer;
        
        public EObstacleType ObstacleType => EObstacleType.Trap;

        public void Initialize()
        {
            transform.localPosition += Vector3.down * .2f;

            float size = (MAX_SIZE - MIN_SIZE) * GameManager.Random.Next(0, 100) / 100 + MIN_SIZE;
            transform.localScale = new Vector3(size, 0.1f, size);

            float xPosition = (MAX_POSITIONS.x - MIN_POSITIONS.x) * GameManager.Random.Next(0, 100) / 100 + MIN_POSITIONS.x;
            float zPosition = (MAX_POSITIONS.y - MIN_POSITIONS.y) * GameManager.Random.Next(0, 100) / 100 + MIN_POSITIONS.y;
            transform.localPosition = new Vector3(xPosition, transform.localPosition.y, zPosition);
        }

        public void ShowTrap()
        {
            transform.localPosition += Vector3.up * .2f;

        }
    }
}
