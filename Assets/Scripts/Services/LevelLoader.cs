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

        // The list of tiles the user can use to create maps. Public so the user can add all user-created prefabs
        public Tileset Tileset;

        public string LevelFileName;

        // Dictionary as the parent for all the GameObjects per layer
        private readonly Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();

        // GameObject as the parent for all the layers (to keep the Hierarchy window clean)
        private GameObject _tileLevelParent;
        private List<Transform> Tiles => Tileset.Tiles;
        public static string DepthSeparator { get; } = ",";
        public static string WidthSeparator { get; } = ";";

        private static Random _random = new Random();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            var path = Path.Combine(Application.dataPath, "Levels", LevelFileName);
            LoadLevel(path);
        }

        private void LoadLevel(string path)
        {
            var lines = File.ReadAllLines(path);
            var boardParameters = lines.First().Split(',').Select(int.Parse).ToList();
            var width = boardParameters[0];
            var height = boardParameters[1];
            var depth = boardParameters[2];
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

                    if (piecePrefabName != String.Empty)
                    {
                        var pieceTransform = CreateGameObject(piecePrefabName, x, y, 1);
                        piece = new Piece(new Vector2Int(x, y), _random.Next(100), pieceTransform);
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

            StartCoroutine("InterpretCode", board);
        }

        private IEnumerator InterpretCode(Board board)
        {
            var code = new ICommand[]
            {
                new PickCommand(1),
                new MoveCommand(Direction.Right, 2),
                new AddCommand(3),
                new MoveCommand(Direction.Left, 4),
                new MoveCommand(Direction.Down, 5),
                new MoveCommand(Direction.Up, 6),
                new PutCommand(7),
                new JumpCommand(0)
            };

            while (true)
            {
                foreach (var bot in board.Bots) bot.CommandId = code[bot.CommandId].Execute(board, bot);
                yield return new WaitForSeconds(1.2f);
            }
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
            } else if (prefab.name == "piece")
            {
                yPos -= 0.255f;
            }

            var newObject = Instantiate(prefab, new Vector3(xPos, yPos, prefab.position.z), rotation);
            newObject.name = prefab.name;
            // Set the object's parent to the layer parent variable so it doesn't clutter our Hierarchy
            newObject.parent = parent;

            return newObject;
        }

        // Method that returns the parent GameObject for a layer
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

        private void MaybeFuture()
        {
            foreach (Transform child in _tileLevelParent.transform) Destroy(child.gameObject);
            DestroyImmediate(gameObject);
        }
    }
}