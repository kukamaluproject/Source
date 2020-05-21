using Assets.Scripts.Obstacles.ObstacleImpls;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private Text collectableObjectsText;
        [SerializeField] private Camera miniMapCamera;
        [SerializeField] private GameObject useTip;

        [Header("Prefabs")]
        [SerializeField] private Room roomPrefab;
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Trap trapPrefab;
        [SerializeField] private Monster monsterPrefab;

        [Header("Difficult modificators")]
        [SerializeField] private float mapSizeModificator = 1;
        [SerializeField] private float countOfTrapsModificator = 1;
        [SerializeField] private float countOfMonstersModificator = 1;
        [SerializeField] private float minCountOfVerticesModificator = 1;
        [SerializeField] private float maxCountOfVerticesModificator = 1;

        private Vector3 actualMapSize;
        private Player player;
        private List<Room> rooms;
        private System.Random random = new System.Random();
        private int currentDifficult = 5;
        private int collectedObjects;
        private int countOfCollectableObjects;

        public static GameManager Instance;
        public static System.Random Random => Instance.random;

        public static readonly float DELTA_Z = 30f;
        public static readonly float DELTA_X = 10f;
        public static readonly float DELTA_Y = 10f;
        public static readonly float SPEED_MODIFICATOR = 5f;

        public void GameOver(bool isWin)
        {
            foreach (var item in rooms)
            {
                Destroy(item.gameObject);
            }
            rooms.Clear();

            if (player != null)
            {
                Destroy(player.gameObject);
            }
            collectedObjects = 0;
            countOfCollectableObjects = 0;
            UpdateUI();

            currentDifficult = isWin ? currentDifficult + 1 : currentDifficult - 1;
            currentDifficult = Mathf.Max(currentDifficult, 5);

            StartGame();
        }

        public void IncCollectableObjects()
        {
            collectedObjects++;
            UpdateUI();

            if (collectedObjects >= countOfCollectableObjects)
                GameOver(true);
        }

        [ContextMenu("RestartGameWin")]
        public void RestartGameWin()
        {
            GameOver(true);
        }
        [ContextMenu("RestartGameLose")]
        public void RestartGameLose()
        {
            GameOver(false);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            rooms = new List<Room>();

            StartGame();
        }

        private void StartGame()
        {
            var countOfTraps = random.Next(0, (int)(currentDifficult * countOfTrapsModificator));
            var countOfEnemies = (int)(currentDifficult * countOfMonstersModificator);
            actualMapSize = new Vector3(currentDifficult * mapSizeModificator, currentDifficult * mapSizeModificator, currentDifficult * mapSizeModificator);
            var countOfVertices = random.Next((int)(currentDifficult * minCountOfVerticesModificator), (int)(currentDifficult * maxCountOfVerticesModificator));
            countOfVertices = countOfVertices % 2 == 0 ? countOfVertices : countOfVertices - 1;
            countOfCollectableObjects = countOfVertices;
            UpdateUI();

            Debug.Log($"New level generating. Difficult: {currentDifficult}\n" +
                      $"Count of traps: {countOfTraps}\n" +
                      $"Count of Enemies: {countOfEnemies}\n" +
                      $"Map size: {actualMapSize.x} X {actualMapSize.y} X {actualMapSize.z}\n" +
                      $"Count of vertices and collectable objects: {countOfVertices}");

            LevelGenerator.InitRooms(countOfTraps, countOfEnemies, countOfVertices, actualMapSize);

            InitPlayer();
        }

        private void InitPlayer()
        {
            player = Instantiate(playerPrefab);
            player.Init(rooms[0].transform.position);
        }

        private void UpdateUI()
        {
            collectableObjectsText.text = $"{collectedObjects} / {countOfCollectableObjects}";
        }
        public void UpdateMinimapCamera(Vector3 newPosition)
        {
            miniMapCamera.transform.position = newPosition + (Vector3.up * 25);
        }

        public Room GenerateRoom(Vector3 deltaLocalPosition)
        {
            // Создание комнаты
            var newRoom = Instantiate(roomPrefab, transform, false);
            newRoom.transform.localPosition += deltaLocalPosition;
            rooms.Add(newRoom);
            return newRoom;
        }

        public void SetActiveUseTip(bool isActive)
        {
            useTip.SetActive(isActive);
        }

        public void SpawnMonster(Room room)
        {
            var monster = Instantiate(monsterPrefab, room.transform, false);
            monster.Initialize();
        }

        public void SpawnTrap(Room room)
        {
            var trap = Instantiate(trapPrefab, room.transform, false);
            trap.Initialize();
        }
    }
}
