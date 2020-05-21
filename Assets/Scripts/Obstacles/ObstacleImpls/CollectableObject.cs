using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Obstacles.ObstacleImpls
{
    public class CollectableObject : MonoBehaviour, IObstacle
    {
        public EObstacleType ObstacleType => EObstacleType.CollectableObject;

        public void Destroy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        public void Init(bool needActivate)
        {
            if (needActivate)
            {
                gameObject.SetActive(true);
                StartCoroutine(RotateCollectableObjectCoroutine());
            }
        }
        private IEnumerator RotateCollectableObjectCoroutine()
        {
            while (true)
            {
                transform.Rotate(Vector3.up);
                yield return null;
            }
        }
    }
}
