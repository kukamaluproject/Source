using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles.ObstacleImpls
{
    public class Monster : MonoBehaviour, IObstacle
    {
        private static readonly Vector2 MIN_POSITIONS = new Vector2(-2.5f, -2.5f);
        private static readonly Vector2 MAX_POSITIONS = new Vector2(2.5f, 2.5f);

        private Player player;
        private Rigidbody rigidbody;
        private Vector3 target;

        //[Header("Debug parameters")]
        //[SerializeField] private Vector3 localPosition;
        //[SerializeField] private Vector3 targetPosition;
        //[SerializeField] private Vector3 normalizedDirection;
        //[SerializeField] private Vector3 direction;
        //[SerializeField] private float distance;

        public EObstacleType ObstacleType => EObstacleType.Monster;

        private IEnumerator MonsterProcess()
        {
            while (true)
            {
                Vector3 heading = Vector3.zero;

                if (player == null && target != Vector3.zero)
                {
                    if (Vector3.Distance(this.transform.localPosition, target) < .5f)
                        target = Vector3.zero;
                }

                if (player != null)
                {
                    if (Vector3.Distance(this.transform.position, player.transform.position) <= 1f)
                        GameManager.Instance.GameOver(false);
                }

                if (player == null)
                {
                    if (target == Vector3.zero)
                    {
                        float xPosition = (MAX_POSITIONS.x - MIN_POSITIONS.x) * GameManager.Random.Next(0, 100) / 100 + MIN_POSITIONS.x;
                        float zPosition = (MAX_POSITIONS.y - MIN_POSITIONS.y) * GameManager.Random.Next(0, 100) / 100 + MIN_POSITIONS.y;
                        target = new Vector3(xPosition, transform.localPosition.y, zPosition);
                    }

                    heading = target - this.transform.localPosition;
                }
                else
                {
                    target = player.transform.position;
                    heading = target - this.transform.position;
                }

                var distance = heading.magnitude;
                var direction = heading / distance;
                rigidbody.AddForce(direction);

                yield return null;
            }
        }
        public void Initialize()
        {
            this.rigidbody = GetComponent<Rigidbody>();
            StartCoroutine(MonsterProcess());
        }

        public void SetTarget(Player player)
        {
            this.player = player;
            if (player == null)
                target = Vector3.zero;
        }
    }
}
