using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class LevelGenerator
    {
        public static void InitRooms(int countOfTraps, int countOfEnemies, int countOfMainRooms, Vector3 actualMapSize)
        {
            Room newRoom = null;
            List<Room> freeRooms = new List<Room>();
            var matrix = new bool[(int)actualMapSize.x, (int)actualMapSize.y, (int)actualMapSize.z];
            var vertexVariants = GenerateFullMatrixPositions(actualMapSize);
            var vertices = GenerateMainVertices(countOfMainRooms, vertexVariants, matrix);
            var vertexPairs = GenerateVertexPairs(vertices);

            for (int i = 0; i < actualMapSize.x; i++)
            {
                for (int j = 0; j < actualMapSize.y; j++)
                {
                    for (int k = 0; k < actualMapSize.z; k++)
                    {
                        var currentVertex = new Vector3(i, j, k);
                        var needCollectableObject = false;

                        // Если комната является вершиной и является частью пары, определяем нужно ли создавать дополнительную комнату для соединения пары и добавляем ее к вершинам
                        if (matrix[i, j, k])
                        {
                            foreach (var vertexPair in vertexPairs)
                            {
                                if (vertexPair.Key.Equals(currentVertex) || vertexPair.Value.Equals(currentVertex))
                                {
                                    needCollectableObject = true;
                                    //Debug.Log($"The pair: {vertexPair.Key} X {vertexPair.Value}, contains the curent vertex: {currentVertex}");
                                    var secondVertexOfPair = vertexPair.Key.Equals(currentVertex) ? vertexPair.Value : vertexPair.Key;
                                    LinkRooms(currentVertex, secondVertexOfPair, matrix);
                                }
                            }

                            var deltaLocalPosition = new Vector3(GameManager.DELTA_X * i, GameManager.DELTA_Z * k, GameManager.DELTA_Y * j);
                            newRoom = GameManager.Instance.GenerateRoom(deltaLocalPosition);
                            var openedDoors = CheckOpenedDoors(matrix, currentVertex);
                            newRoom.Init(openedDoors, currentVertex, needCollectableObject);

                            if (!newRoom.HaveCollectableObject)
                                freeRooms.Add(newRoom);
                        }
                    }
                }
            }
            CheckAndGenerateTrapsAndMonsters(freeRooms, countOfTraps, countOfEnemies);
        }

        private static List<Vector3> GenerateMainVertices(int countOfRooms, List<Vector3> vertexVariants, bool[,,] matrix)
        {
            var vertices = new List<Vector3>();
            for (int i = 0; i < countOfRooms; i++)
            {
                var index = GameManager.Random.Next(0, vertexVariants.Count);
                vertices.Add(vertexVariants[index]);
                matrix[(int)vertexVariants[index].x, (int)vertexVariants[index].y, (int)vertexVariants[index].z] = true;

                vertexVariants.RemoveAt(index);
            }
            Debug.Log($"Generate {vertices.Count} main vertices");
            return vertices;
        }

        private static List<Vector3> GenerateFullMatrixPositions(Vector3 actualMapSize)
        {
            var vertexVariants = new List<Vector3>();
            for (int i = 0; i < actualMapSize.x; i++)
            {
                for (int j = 0; j < actualMapSize.y; j++)
                {
                    for (int k = 0; k < actualMapSize.z; k++)
                    {
                        vertexVariants.Add(new Vector3(i, j, k));
                    }
                }
            }
            Debug.Log($"Generate {vertexVariants.Count} vertices");
            return vertexVariants;
        }

        private static List<KeyValuePair<Vector3, Vector3>> GenerateVertexPairs(List<Vector3> vertices)
        {
            var vertexPairs = new List<KeyValuePair<Vector3, Vector3>>();
            while (vertices.Count > 1)
            {
                // Первая вершина удаляется после того как ее объединяют в пару
                var vertexIndex = GameManager.Random.Next(0, vertices.Count);
                var firstVertex = vertices[vertexIndex];
                vertices.RemoveAt(vertexIndex);

                // Вторая вершина не удалятся, для того чтобы ее объединить с любой другой вершиной, чтобы исключить обособленность текущей пары от остальных
                vertexIndex = GameManager.Random.Next(0, vertices.Count);
                var secondVertex = vertices[vertexIndex];

                //Debug.Log($"Vertex pair: {firstVertex} X {secondVertex}");
                vertexPairs.Add(new KeyValuePair<Vector3, Vector3>(firstVertex, secondVertex));
            }
            Debug.Log($"Generate {vertexPairs.Count} vertex pairs");
            return vertexPairs;
        }

        private static List<EDoorPosition> CheckOpenedDoors(bool[,,] matrix, Vector3 currentVertex)
        {
            var openedDoors = new List<EDoorPosition>();
            bool southVertex;
            bool eastVertex;
            bool northVertex;
            bool westVertex;
            bool upVertex;
            bool downVertex;

            try
            {
                southVertex = matrix[(int)currentVertex.x + 1, (int)currentVertex.y, (int)currentVertex.z];
            }
            catch (IndexOutOfRangeException)
            {
                southVertex = false;
            }
            try
            {
                westVertex = matrix[(int)currentVertex.x, (int)currentVertex.y - 1, (int)currentVertex.z];
            }
            catch (IndexOutOfRangeException)
            {
                westVertex = false;
            }
            try
            {
                northVertex = matrix[(int)currentVertex.x - 1, (int)currentVertex.y, (int)currentVertex.z];
            }
            catch (IndexOutOfRangeException)
            {
                northVertex = false;
            }
            try
            {
                eastVertex = matrix[(int)currentVertex.x, (int)currentVertex.y + 1, (int)currentVertex.z];
            }
            catch (IndexOutOfRangeException)
            {
                eastVertex = false;
            }
            try
            {
                upVertex = matrix[(int)currentVertex.x, (int)currentVertex.y, (int)currentVertex.z + 1];
            }
            catch (IndexOutOfRangeException)
            {
                upVertex = false;
            }
            try
            {
                downVertex = matrix[(int)currentVertex.x, (int)currentVertex.y, (int)currentVertex.z - 1];
            }
            catch (IndexOutOfRangeException)
            {
                downVertex = false;
            }

            if (southVertex) openedDoors.Add(EDoorPosition.south);
            if (eastVertex) openedDoors.Add(EDoorPosition.east);
            if (northVertex) openedDoors.Add(EDoorPosition.north);
            if (westVertex) openedDoors.Add(EDoorPosition.west);
            if (upVertex) openedDoors.Add(EDoorPosition.up);
            if (downVertex) openedDoors.Add(EDoorPosition.down);

            return openedDoors;
        }

        private static void LinkRooms(Vector3 currentRoom, Vector3 secondRoom, bool[,,] matrix)
        {
            while (currentRoom.x < secondRoom.x)
            {
                currentRoom.x++;
                matrix[(int)currentRoom.x, (int)currentRoom.y, (int)currentRoom.z] = true;
            }
            while (currentRoom.y < secondRoom.y)
            {
                currentRoom.y++;
                matrix[(int)currentRoom.x, (int)currentRoom.y, (int)currentRoom.z] = true;
            }
            while (currentRoom.z < secondRoom.z)
            {
                currentRoom.z++;
                matrix[(int)currentRoom.x, (int)currentRoom.y, (int)currentRoom.z] = true;
            }
        }

        private static void CheckAndGenerateTrapsAndMonsters(List<Room> freeRooms, int countOfTraps, int countOfMonsters)
        {
            var freeRoomsCopy = new List<Room>(freeRooms);
            SpawnToRandomRooms(freeRooms, countOfTraps, GameManager.Instance.SpawnTrap);
            SpawnToRandomRooms(freeRoomsCopy, countOfMonsters, GameManager.Instance.SpawnMonster);
        }
        private static void SpawnToRandomRooms(List<Room> freeRooms, int countOFRooms, Action<Room> roomAction)
        {
            for (int i = 0; i < countOFRooms; i++)
            {
                var roomIndex = GameManager.Random.Next(0, freeRooms.Count);
                roomAction.Invoke(freeRooms[roomIndex]);
                freeRooms.RemoveAt(roomIndex);
            }
        }
    }
}
