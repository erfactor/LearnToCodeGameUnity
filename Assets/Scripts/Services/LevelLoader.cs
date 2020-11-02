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

namespace Services
{
    public class LevelLoader : MonoBehaviour
    {
        // The instance of the LevelEditor
        public static LevelLoader Instance;
        // Dictionary as the parent for all the GameObjects per layer
        private readonly Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();
        // GameObject as the parent for all the layers (to keep the Hierarchy window clean)
        private GameObject _tileLevelParent;

        // The list of tiles the user can use to create maps. Public so the user can add all user-created prefabs
        public Tileset Tileset;
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
            var levelFile = "good_level.txt";
            var path = Path.Combine(Application.dataPath, "Levels", levelFile);
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

                    var botPrefabName = cell[0];
                    var tilePrefabName = cell[1];
                    Bot bot = null;
                    var tileType = TileType.Hole;

                    if (botPrefabName != string.Empty)
                    {
                        var botAnimator = CreateGameObject(botPrefabName, x, y, 1).GetComponent<BotAnimator>();
                        bot = new Bot(new Vector2Int(x, y), botAnimator);
                    }

                    if (tilePrefabName != string.Empty)
                    {
                        var tile = CreateGameObject(tilePrefabName, x, y, 2).GetComponent<Tile>();
                        tileType = tile.TileType;
                    }

                    var field = new Field(tileType, bot);
                    board[x, y] = field;
                }
            }

            StartCoroutine("InterpretCode", board);
        }

        private IEnumerator InterpretCode(Board board)
        {
            var code = new ICommand[]
            {
                new MoveCommand(Direction.Right, 1),
                new MoveCommand(Direction.Up, 2),
                new MoveCommand(Direction.Left, 3),
                new MoveCommand(Direction.Down, 4),
                new JumpCommand(0),
            };
            
            while(true)
            {
                foreach (var bot in board.Bots)
                {
                    bot.CommandId = code[bot.CommandId].Execute(board, bot);
                }
                yield return new WaitForSeconds(.5f);
            }
        }

        private Transform CreateGameObject(string prefabName, int xPos, int yPos, int zPos)
        {
            var prefab = Tiles.First(t => t.name == prefabName);
            var parent = GetLayerParent(zPos).transform;
            var newObject = Instantiate(prefab, new Vector3(xPos, yPos, prefab.position.z), Quaternion.identity);
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