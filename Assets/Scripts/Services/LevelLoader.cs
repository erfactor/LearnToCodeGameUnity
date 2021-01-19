using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animators;
using Commands;
using Config;
using Enumerations;
using Models;
using Packages._2DTileMapLevelEditor.Scripts;
using Profiles;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Services
{
    public class LevelLoader : MonoBehaviour
    {
        private static readonly Random Random = new Random();
        public static Level Level;
        public Tileset Tileset;

        //Prefab with which the board will be measured in pixels
        public GameObject referenceSizeTile;

        // Dictionary as the parent for all the GameObjects per layer
        private readonly Dictionary<int, GameObject> _layerParents = new Dictionary<int, GameObject>();
        private readonly Dictionary<int, GameObject> _layerParentsSolution = new Dictionary<int, GameObject>();

        private Board _board;
        private bool _codeExecutionOn;
        private int _commandsToExecuteCount;

        private Coroutine _gameCoroutine;
        private Board _solution;
        private GameObject _tileLevelParent;
        private float _animationSpeed = 1;
        private float _animationTime = 1;
        private List<Transform> Tiles => Tileset.Tiles;
        public static string DepthSeparator { get; } = ",";
        public static string WidthSeparator { get; } = ";";

        private void Start()
        {
            LoadLevel();
            LoadSolutionToSolutionWindow();
            LoadSolution();
        }

        public void LoadLevel()
        {
            var lines = Level.File.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
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
                            ? Random.Next(100)
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

            _board = board;
            ClipLevel();
            GameObject.Find("ExecutionIndicatorManager").GetComponent<ExecutionIndicatorManager>()
                .AssignColorsToBots(_board.Bots);
        }

        private void ClipLevel()
        {
            var tileSize = referenceSizeTile.GetComponent<SpriteRenderer>().bounds.size.x;
            var tileLevel = GameObject.Find("TileLevel");

            var topIndicator = GameObject.Find("LevelPositionIndicatorTop");
            var bottomIndicator = GameObject.Find("LevelPositionIndicatorBottom");
            var centerIndicator = GameObject.Find("LevelPositionIndicatorCenter");

            var tileLevelRect = new Rect(0, 0, tileSize * _board.Width, tileSize * _board.Height);

            var finalHeight = topIndicator.transform.position.y - bottomIndicator.transform.position.y;
            var finalScale = finalHeight / tileLevelRect.height;
            var finalWidth = tileLevelRect.width * finalScale;

            var finalPosition = centerIndicator.transform.position + new Vector3(-finalWidth / 2, -finalHeight / 2, 10);

            tileLevel.transform.localScale = new Vector3(finalScale, finalScale, 1);
            tileLevel.transform.position = finalPosition;
        }

        private void LoadSolutionToSolutionWindow()
        {
            var lines = Level.SolutionFile.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
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
                            ? Random.Next(100)
                            : int.Parse(piecePrefabName.Substring(5));
                        var pieceTransform = CreateGameObjectInSolution("piece", x, y, 1);
                        piece = new Piece(new Vector2Int(x, y), pieceNumber, pieceTransform, isRandom);
                    }

                    if (botPrefabName != string.Empty)
                    {
                        var botTransform = CreateGameObjectInSolution(botPrefabName, x, y, 1);
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

                        var tile = CreateGameObjectInSolution(tilePrefabName, x, y, 2, rotation).GetComponent<Tile>();
                        tileType = tile.TileType;
                    }

                    var field = new Field(tileType, bot, piece);
                    board[x, y] = field;
                }
            }

            GameObject.Find("TileLevelSolution").transform.localScale = new Vector3(100, 100, 100);
            GameObject.Find("TileLevelSolution").transform.position = new Vector3(100, 100, 100);
            IncreaseVisibility(GameObject.Find("TileLevelSolution").transform);
            ClipLevelSolution();
        }

        private void IncreaseVisibility(Transform t)
        {
            var sr = t.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder += 20;
            }
            for(int i=0; i<t.childCount; i++)
            {
                IncreaseVisibility(t.GetChild(i));
            }
        }

        private void ClipLevelSolution()
        {
            var tileSize = referenceSizeTile.GetComponent<SpriteRenderer>().bounds.size.x;
            var tileLevel = GameObject.Find("TileLevelSolution");

            var solutionHeight = 700;

            var tileLevelRect = new Rect(0, 0, tileSize * _board.Width, tileSize * _board.Height);

            var finalHeight = solutionHeight;
            var finalScale = finalHeight / tileLevelRect.height;
            var finalWidth = tileLevelRect.width * finalScale;

            var center = new Vector3(0, 0);

            var finalPosition = center + new Vector3(-finalWidth / 2 + 0.5f * tileSize * finalScale, -finalHeight / 2, 10);

            Debug.Log($"tileSize: {tileSize} final width: {finalWidth}");

            tileLevel.transform.localScale = new Vector3(finalScale, finalScale, 1);
            tileLevel.transform.position = finalPosition;
        }

        private void LoadSolution()
        {
            var lines = Level.SolutionFile.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
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

                    if (botPrefabName != string.Empty)
                    {
                        bot = new Bot(new Vector2Int(x, y), null);
                    }

                    board[x, y] = new Field(TileType.Normal, bot, piece);
                }
            }

            _solution = board;
        }

        

        public IEnumerator InterpretCode(List<ICommand> commands)
        {
            _codeExecutionOn = true;
            var executionIndicatorManager =
                GameObject.Find("ExecutionIndicatorManager").GetComponent<ExecutionIndicatorManager>();
            executionIndicatorManager.InstantiateIndicators(_board.Bots);
            ChangeAnimationSpeed();

            while (_commandsToExecuteCount-- > 0)
            {
                if (_board.Bots.All(b => commands[b.CommandId] is FinishCommand))
                {
                    executionIndicatorManager.ClearIndicators();
                    StopExecution();
                    yield break;
                }

                foreach (var bot in _board.Bots)
                {
                    var command = commands[bot.CommandId];
                    if (bot.HasFinished) continue;
                    var traversedJumpCommands = new List<int>();
                    while (command is JumpCommand)
                    {
                        if (traversedJumpCommands.Contains(command.NextCommandId))
                        {
                            Debug.LogWarning("Bot has fallen into an infinite loop.");
                            executionIndicatorManager.RemoveIndicatorForBot(bot);
                            bot.HasFinished = true;
                            break;
                        }

                        traversedJumpCommands.Add(command.NextCommandId);
                        bot.CommandId = command.NextCommandId;
                        command = commands[command.NextCommandId];
                    }

                    if (!(command is FinishCommand))
                    {
                        executionIndicatorManager.UpdateIndicator(bot, bot.CommandId);
                        bot.CommandId = command.Execute(_board, bot);
                        print(commands[bot.CommandId]);
                    }
                }

                if (_solution.AcceptsBoard(_board))
                {
                    print("Accepted");
                    yield return StartCoroutine(LevelCompleted());
                    yield break;
                }

                yield return new WaitForSeconds(_animationTime + 0.1f);
                ChangeAnimationSpeed();
            }

            _codeExecutionOn = false;
            //executionIndicatorManager.ClearIndicators();
        }

        private void ChangeAnimationSpeed()
        {
            _animationTime = 1 / _animationSpeed;
            _board.Bots.ForEach(b => b.Animator.animator.speed = _animationSpeed);
        }

        public void SetAnimationSpeed(float speed)
        {
            var speedToSpeed = new Dictionary<float, float>()
            {
                {1, 0.5f},
                {2, 1},
                {3, 2},
                {4, 4},
            };
            _animationSpeed = speedToSpeed[speed];
        }

        public bool ShouldDisplayExecutionIndicators()
        {
            return _codeExecutionOn || _commandsToExecuteCount > 0;
        }

        public void PauseExecution()
        {
            _commandsToExecuteCount = 0;
        }

        public void StartExecution(List<ICommand> commands)
        {
            _commandsToExecuteCount = int.MaxValue;
            if (!_codeExecutionOn) StartCoroutine("InterpretCode", commands);
        }

        public void StepOnce(List<ICommand> commands)
        {
            _commandsToExecuteCount = 1;
            if (!_codeExecutionOn) StartCoroutine("InterpretCode", commands);
        }

        public void StopExecution()
        {
            GameObject.Find("ExecutionIndicatorManager").GetComponent<ExecutionIndicatorManager>().ClearIndicators();
            _codeExecutionOn = false;
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
            DestroyImmediate(GameObject.Find("TileLevel"));
            LoadLevel();
        }

        public IEnumerator LevelCompleted()
        {
            yield return new WaitForSeconds(Timing.WinWindowOnLevelCompletionDelay);

            Debug.Log("Level completed!");
            GameObject.Find("ProfileManager").GetComponent<ProfileManager>().UnlockLevel(Level.Number + 1);
            GameObject.Find("WinWindow").GetComponent<WinWindow>().Show();
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

        private Transform CreateGameObjectInSolution(string prefabName, float xPos, float yPos, int zPos,
            Quaternion rotation = new Quaternion())
        {
            var prefab = Tiles.First(t => t.name == prefabName);
            var parent = GetLayerParentInSolution(zPos).transform;
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

        private GameObject GetLayerParentInSolution(int layer)
        {
            if (_layerParentsSolution.ContainsKey(layer))
                return _layerParentsSolution[layer];
            var layerParent = new GameObject("Layer " + layer);
            _tileLevelParent = GameObject.Find("TileLevelSolution") ?? new GameObject("TileLevelSolution");
            _tileLevelParent.transform.SetParent(GameObject.Find("SolutionWindow").transform);
            layerParent.transform.parent = _tileLevelParent.transform;
            _layerParentsSolution.Add(layer, layerParent);
            return _layerParentsSolution[layer];
        }
    }
}