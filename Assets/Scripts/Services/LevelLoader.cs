using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Animators;
using Commands;
using Enumerations;
using Models;
using Packages._2DTileMapLevelEditor.Scripts;
using UnityEngine;
using Random = System.Random;

namespace Services
{
    public class LevelLoader : MonoBehaviour
    {
        // The instance of the LevelEditor
        public static LevelLoader Instance;

        private static readonly Random _random = new Random();

        public string testFileName;

        // The list of tiles the user can use to create maps. Public so the user can add all user-created prefabs
        public Tileset Tileset;

        // Dictionary as the parent for all the GameObjects per layer
        private readonly Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();
        private Board _solution;

        // GameObject as the parent for all the layers (to keep the Hierarchy window clean)
        private GameObject _tileLevelParent;

        private string currentLevelData;

        public Board InitialBoard;
        private List<Transform> Tiles => Tileset.Tiles;
        public static string DepthSeparator { get; } = ",";
        public static string WidthSeparator { get; } = ";";

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            //var path = Path.Combine(Application.dataPath, "Levels", LevelFileName ?? testFileName);
            //initialBoardState = LoadLevel(path);
        }

        private string GetFullPathToLevel(string filename)
        {
            return Path.Combine(Application.dataPath, "Resources", "Levels", filename);
        }

        public Board LoadLevel(string fileData)
        {
            currentLevelData = fileData;
            var lines = fileData.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            var boardParameters = lines.First().Split(',').Select(int.Parse).ToList();
            var width = boardParameters[0];
            var height = boardParameters[1];
            var board = new Board(width, height);

            for (var y = 0; y < height; y++)
            {
                var line = lines[y + 1].Split(WidthSeparator.ToCharArray());
                for (var x = 0; x < width; x++)
                {
                    var cell = line[x].Split(DepthSeparator.ToCharArray());

                    var piecePrefabName = cell[0];
                    var botPrefabName = cell[1];
                    var tilePrefabName = cell[2];

                    Piece piece = null;
                    Bot bot = null;
                    var tileType = TileType.Hole;

                    if (piecePrefabName != string.Empty)
                    {
                        var isRandom = piecePrefabName == "piece";
                        var pieceNumber = isRandom
                            ? _random.Next(100)
                            : int.Parse(piecePrefabName.Substring(5));
                        var pieceTransform = CreateGameObject("piece", x, y, 1);
                        piece = new Piece(new Vector2Int(x, y), pieceNumber, pieceTransform, isRandom);
                    }

                    if (botPrefabName != string.Empty)
                    {
                        var botTransform = CreateGameObject(botPrefabName, x, y, 1);
                        var botAnimator = botTransform.GetComponent<BotAnimator>();
                        bot = new Bot(new Vector2Int(x, y), botAnimator);
                        botAnimator.Bot = bot;
                    }

                    if (tilePrefabName != string.Empty)
                    {
                        var rotation = Quaternion.identity;
                        if (y == 0 || y == height - 1)
                        {
                            rotation = Quaternion.AngleAxis(90, Vector3.forward);
                            if (x == 0 && y == 0) rotation = Quaternion.AngleAxis(90, Vector3.back);
                            if (x == 0 && y == height - 1) rotation = Quaternion.AngleAxis(180, Vector3.forward);
                            if (x == width - 1 && y == 0) rotation = Quaternion.AngleAxis(0, Vector3.forward);
                        }

                        var tile = CreateGameObject(tilePrefabName, x, y, 2, rotation).GetComponent<Tile>();
                        tileType = tile.TileType;
                    }

                    var field = new Field(tileType, bot, piece);
                    board[x, y] = field;
                }
            }

            InitialBoard = board;

            GameObject.Find("TileLevel").transform.localScale = new Vector3(100, 100);
            GameObject.Find("TileLevel").transform.position = new Vector3(-800, -300);
            DontDestroyOnLoad(GameObject.Find("TileLevel"));
            return board;
        }

        public void LoadSolution(string levelSolutionText)
        {
            _solution = CreateAcceptingBoard(levelSolutionText);
        }

        public Board CreateAcceptingBoard(string fileData)
        {
            currentLevelData = fileData;
            var lines = fileData.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            var boardParameters = lines.First().Split(',').Select(int.Parse).ToList();
            var width = boardParameters[0];
            var height = boardParameters[1];
            var board = new Board(width, height);

            for (var y = 0; y < height; y++)
            {
                var line = lines[y + 1].Split(WidthSeparator.ToCharArray());
                for (var x = 0; x < width; x++)
                {
                    var cell = line[x].Split(DepthSeparator.ToCharArray());

                    var piecePrefabName = cell[0];
                    var botPrefabName = cell[1];

                    Piece piece = null;
                    Bot bot = null;

                    if (piecePrefabName != string.Empty)
                    {
                        var isRandom = piecePrefabName == "piece";
                        var pieceNumber = isRandom ? -1 : int.Parse(piecePrefabName.Substring(5));
                        piece = new Piece(new Vector2Int(x, y), pieceNumber, null, isRandom);
                    }

                    if (botPrefabName != string.Empty) bot = new Bot(new Vector2Int(x, y), null);

                    board[x, y] = new Field(TileType.Normal, bot, piece);
                }
            }

            return board;
        }

        public void StartExecution(List<ICommand> commands)
        {
            StartCoroutine("InterpretCode", commands);
        }

        public IEnumerator InterpretCode(List<ICommand> commands)
        {
            var board = InitialBoard;

            while (true)
            {
                foreach (var bot in board.Bots)
                {
                    var command = commands[bot.CommandId];
                    if (!(command is FinishCommand))
                    {
                        bot.CommandId = command.Execute(board, bot);
                    }
                }
                if (_solution.AcceptsBoard(board))
                {
                    print("Player won the game");
                }
                yield return new WaitForSeconds(1.2f);
            }
        }

        public void StopExecution()
        {
            StopAllCoroutines();
        }

        public void StopAndReload()
        {
            StopExecution();
            ReloadLevel();
        }

        public void ReloadLevel()
        {
            _layerParents.Clear();
            DestroyCurrentLevelData();
            LoadLevel(currentLevelData);
        }

        private void DestroyCurrentLevelData()
        {
            var levelInstance = GameObject.Find("TileLevel");
            DestroyImmediate(levelInstance);
        }

        private Transform CreateGameObject(string prefabName, float xPos, float yPos, int zPos,
            Quaternion rotation = new Quaternion())
        {
            var prefab = Tiles.First(t => t.name == prefabName);
            var parent = GetLayerParent(zPos).transform;
            if (prefab.name == "botHead")
            {
                prefab = Resources.Load<Transform>("Bot/Bot");
                yPos -= 0.472f;
            }
            else if (prefab.name == "piece")
            {
                yPos -= 0.255f;
            }

            var newObject = Instantiate(prefab, new Vector3(xPos, yPos, prefab.position.z), rotation);
            newObject.name = prefab.name;
            newObject.parent = parent;

            return newObject;
        }

        private GameObject GetLayerParent(int layer)
        {
            if (_layerParents.ContainsKey(layer))
                return _layerParents[layer];
            var layerParent = new GameObject("Layer " + layer);
            _tileLevelParent = GameObject.Find("TileLevel") ?? new GameObject("TileLevel");
            layerParent.transform.parent = _tileLevelParent.transform;
            _layerParents.Add(layer, layerParent);
            return _layerParents[layer];
        }
    }
}