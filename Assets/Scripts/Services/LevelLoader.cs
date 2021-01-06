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

        public static string LevelFileName;
        public string testFileName;

        private static readonly Random _random = new Random();

        // The list of tiles the user can use to create maps. Public so the user can add all user-created prefabs
        public Tileset Tileset;

        // Dictionary as the parent for all the GameObjects per layer
        private readonly Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();

        // GameObject as the parent for all the layers (to keep the Hierarchy window clean)
        private GameObject _tileLevelParent;

        public Board initialBoardState;
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
            return Path.Combine(Application.dataPath, "Levels", filename);
        }

        public Board PublicLoadLevel(string filename)
        {
            Board board = LoadLevel(GetFullPathToLevel(filename));
            initialBoardState = board;
            return board;
        }

        private Board LoadLevel(string path)
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

                    if (piecePrefabName != string.Empty)
                    {
                        int pieceNumber = piecePrefabName != "piece"
                            ? int.Parse(piecePrefabName.Substring(5))
                            : _random.Next(100);
                        var pieceTransform = CreateGameObject("piece", x, y, 1);
                        piece = new Piece(new Vector2Int(x, y), pieceNumber, pieceTransform);
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

            GameObject.Find("TileLevel").transform.localScale = new Vector3(100, 100);
            GameObject.Find("TileLevel").transform.position = new Vector3(-800, -300);
            DontDestroyOnLoad(GameObject.Find("TileLevel"));
            return board;
            //StartCoroutine("InterpretCode", board);
        }

        public void StartExecution(List<ICommand> commands)
        {
            StartCoroutine("InterpretCode", commands);
        }

        public IEnumerator InterpretCode(List<ICommand> commands)
        {
            var board = initialBoardState;

            while (true)
            {
                Debug.unityLogger.Log(board.Bots.Count);
                foreach (var bot in board.Bots) bot.CommandId = commands[bot.CommandId].Execute(board, bot);
                yield return new WaitForSeconds(1.2f);
            }
        }

        public void StopExecution()
        {
            StopAllCoroutines();
            _layerParents.Clear();
            var levelInstance = GameObject.Find("TileLevel");
            DestroyImmediate(levelInstance);
            Start();
        }

        //private IEnumerator InterpretCode(Board board)
        //{
        //    var code = new ICommand[]
        //    {
        //        new PickCommand(1),
        //        new MoveCommand(Direction.Right, 2),
        //        new DecCommand(3),
        //        new AddCommand(4),
        //        new MoveCommand(Direction.Right, 5),
        //        new IncCommand(6),
        //        new SubCommand(7),
        //        new MoveCommand(Direction.Right, 8),
        //        new JumpCommand(0)
        //    };

        //    while (true)
        //    {
        //        Debug.unityLogger.Log(board.Bots.Count);
        //        foreach (var bot in board.Bots) bot.CommandId = code[bot.CommandId].Execute(board, bot);
        //        yield return new WaitForSeconds(1.2f);
        //    }
        //}

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