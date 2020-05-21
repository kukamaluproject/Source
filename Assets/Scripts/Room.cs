using Assets.Scripts.Obstacles.ObstacleImpls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private CollectableObject collectableObject;

        [Header("Sides")]
        [SerializeField] private Transform northWall;
        [SerializeField] private Transform southWall;
        [SerializeField] private Transform easthWall;
        [SerializeField] private Transform westhWall;
        [SerializeField] private Transform upStairs;
        [SerializeField] private Transform downStairs;

        public Vector2 Position;

        public bool HaveCollectableObject { get; private set; }

        public void Init(List<EDoorPosition> openedDoors, Vector2 position, bool needCollectableObject)
        {
            Position = position;
            HaveCollectableObject = needCollectableObject;
            collectableObject.Init(needCollectableObject);

            var closeDoors = new List<EDoorPosition>();
            if (!openedDoors.Contains(EDoorPosition.east))
                closeDoors.Add(EDoorPosition.east);
            if (!openedDoors.Contains(EDoorPosition.west))
                closeDoors.Add(EDoorPosition.west);
            if (!openedDoors.Contains(EDoorPosition.south))
                closeDoors.Add(EDoorPosition.south);
            if (!openedDoors.Contains(EDoorPosition.north))
                closeDoors.Add(EDoorPosition.north);
            if (!openedDoors.Contains(EDoorPosition.up))
                closeDoors.Add(EDoorPosition.up);
            if (!openedDoors.Contains(EDoorPosition.down))
                closeDoors.Add(EDoorPosition.down);

            foreach (var closetDoor in closeDoors)
            {
                var door = northWall;
                switch (closetDoor)
                {
                    case EDoorPosition.south:
                        door = southWall;
                        door.localPosition += new Vector3(0, 0, 3);
                        door.localScale += new Vector3(0, 0, 6);
                        break;
                    case EDoorPosition.north:
                        door = northWall;
                        door.localPosition += new Vector3(0, 0, 3);
                        door.localScale += new Vector3(0, 0, 6);
                        break;
                    case EDoorPosition.east:
                        door = easthWall;
                        door.localPosition += new Vector3(3, 0, 0);
                        door.localScale += new Vector3(6, 0, 0);
                        break;
                    case EDoorPosition.west:
                        door = westhWall;
                        door.localPosition += new Vector3(3, 0, 0);
                        door.localScale += new Vector3(6, 0, 0);
                        break;
                    case EDoorPosition.up:
                        upStairs.gameObject.SetActive(false);
                        break;
                    case EDoorPosition.down:
                        downStairs.gameObject.SetActive(false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}