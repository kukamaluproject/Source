using Assets.Scripts.Obstacles;
using Assets.Scripts.Obstacles.ObstacleImpls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private static Vector3 deltaCameraPosition = new Vector3(0, 10, -10);
        private Rigidbody rigidbody;
        private bool canUseObject;
        private bool isUpStairs;

        public void Init(Vector3 startPosition)
        {
            this.rigidbody = GetComponent<Rigidbody>();

            transform.position = startPosition;
            StartCoroutine(InputCoroutine());
        }

        private void OnTriggerEnter(Collider other)
        {
            var obstacle = other.GetComponent<IObstacle>();
            if (obstacle != null)
            {
                switch (obstacle.ObstacleType)
                {
                    case EObstacleType.CollectableObject:
                        (obstacle as CollectableObject).Destroy();
                        GameManager.Instance.IncCollectableObjects();
                        break;
                    //case EObstacleType.Monster:
                    case EObstacleType.Trap:
                        StopAllCoroutines();
                        Destroy(gameObject);
                        GameManager.Instance.GameOver(false);
                        break;
                    case EObstacleType.Stairs:
                        canUseObject = true;
                        isUpStairs = (obstacle as Stairs).IsUpStairs;
                        GameManager.Instance.SetActiveUseTip(true);
                        break;
                    case EObstacleType.TrapArea:
                        (obstacle as TrapArea).ShowTrap();
                        break;
                    case EObstacleType.MonsterArea:
                        (obstacle as MonsterArea).SetTarget(this);
                        break;
                    default:
                        break;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var obstacle = other.GetComponent<IObstacle>();
            if (obstacle != null)
            {
                switch (obstacle.ObstacleType)
                {
                   case EObstacleType.Stairs:
                        canUseObject = false;
                        GameManager.Instance.SetActiveUseTip(false);
                        break;
                    case EObstacleType.MonsterArea:
                        (obstacle as MonsterArea).SetTarget(null);
                        break;
                    default:
                        break;
                }
            }
        }
        private IEnumerator InputCoroutine()
        {
            while (true)
            {
                if (canUseObject && Input.GetKeyUp(KeyCode.E))
                {
                    var upModificator = isUpStairs ? 1.01f : -.99f;
                    this.transform.position += (Vector3.up * GameManager.DELTA_Z * upModificator);
                }

                var horizontal = Input.GetAxis("Horizontal");
                var vertical = Input.GetAxis("Vertical");
                if (horizontal != 0 || vertical != 0)
                {
                    var speedModificator = Input.GetKey(KeyCode.LeftShift) ? GameManager.SPEED_MODIFICATOR : 1f;
                    rigidbody.AddForce((Vector3.forward * vertical * speedModificator) + (Vector3.right * horizontal * speedModificator));
                }
                Camera.main.transform.position = this.transform.position + deltaCameraPosition;
                GameManager.Instance.UpdateMinimapCamera(transform.position);
                yield return null;
            }
        }
    }
}
