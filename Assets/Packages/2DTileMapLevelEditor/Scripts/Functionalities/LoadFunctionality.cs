using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packages._2DTileMapLevelEditor.Common.Scripts;
using Packages._2DTileMapLevelEditor.SimpleFileBrowser.Scripts;
using Services;
using UnityEngine;

namespace Packages._2DTileMapLevelEditor.Scripts.Functionalities
{
    public class LoadFunctionality : MonoBehaviour
    {
        // The file browser
        private GameObject _fileBrowserPrefab;

        // The file extension of the file to load
        private string[] _fileExtensions;

        // ----- PRIVATE VARIABLES -----

        // The level editor
        private LevelEditor _levelEditor;

        // Method to identifiction the tiles when loading
        private TileIdentificationMethod _loadMethod;

        // Temporary variable to save state of level editor before opening file browser and restore it after save/load
        private bool _preFileBrowserState = true;

        // Starting path of the file browser
        private string _startPath;

        // The tiles used to build the level
        private List<Transform> _tiles;

        // ----- SETUP -----

        public void Setup(GameObject fileBrowserPrefab, string[] fileExtensions, TileIdentificationMethod loadMethod,
            List<Transform> tiles, string startPath)
        {
            _levelEditor = LevelEditor.Instance;
            _fileBrowserPrefab = fileBrowserPrefab;
            _fileExtensions = fileExtensions;
            _loadMethod = loadMethod;
            _tiles = tiles;
            _startPath = startPath;
            SetupClickListeners();
        }

        // Hook up Save/Load Level method to Save/Load button
        private void SetupClickListeners()
        {
            Utilities.FindButtonAndAddOnClickListener("LoadButton", OpenFileBrowser);
        }

        // ----- PUBLIC METHODS -----

        // Load from a file using a path
        public void LoadLevelUsingPath(string path)
        {
            // Enable the LevelEditor when the fileBrowser is done
            _levelEditor.ToggleLevelEditor(_preFileBrowserState);
            if (path.Length != 0)
            {
                // Reset the level
                _levelEditor.ResetBeforeLoad();
                // BinaryFormatter bFormatter = new BinaryFormatter();
                // FileStream file = File.OpenRead(path);
                // string levelData = bFormatter.Deserialize(file) as string;
                // file.Close();
                LoadLevelFromStringLayers(path);
            }
            else
                Debug.Log("Invalid path given");
        }

        // ----- PRIVATE METHODS -----

        // Open a file browser to load files
        private void OpenFileBrowser()
        {
            _preFileBrowserState = _levelEditor.GetScriptEnabled();
            // Disable the LevelEditor while the fileBrowser is open
            _levelEditor.ToggleLevelEditor(false);
            // Create the file browser and name it
            var fileBrowserObject = Instantiate(_fileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            var fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(ViewMode.Landscape, _startPath);
            fileBrowserScript.OpenFilePanel(_fileExtensions);
            // Subscribe to OnFileSelect event (call LoadLevelUsingPath using path) 
            fileBrowserScript.OnFileSelect += LoadLevelUsingPath;
            // Subscribe to OnFileBrowserClose event (call ReopenLevelEditor)
            fileBrowserScript.OnFileBrowserClose += ReopenLevelEditor;
        }

        // Reopens the level editor after closing the file browser
        private void ReopenLevelEditor()
        {
            _levelEditor.ToggleLevelEditor(true);
        }

        // Method that loads the layers
        private void LoadLevelFromStringLayers(string path)
        {
            var lines = File.ReadAllLines(path);
            var boardParameters = lines.First().Split(',').Select(int.Parse).ToList();
            var width = boardParameters[0];
            var height = boardParameters[1];
            var depth = boardParameters[2];

            for (var y = 0; y < height; y++)
            {
                var line = lines[y + 1].Split(LevelLoader.WidthSeparator.ToCharArray());
                for (var x = 0; x < width; x++)
                {
                    var cell = line[x].Split(LevelLoader.DepthSeparator.ToCharArray());
                    for (var z = 0; z < depth; z++)
                    {
                        var blockString = cell[z];
                        _levelEditor.CreateBlock(TileStringRepresentationToInt(blockString), x, y, z);
                    }
                }
            }
        }

        // Transforms the tile identification type read from the file to a integer used as internal representation in the level editor
        // For index, parse the string to int
        // For name, transverse the Tiles and try to match on game object name or EMPTY
        // Defaults to LevelEditor.GetEmpty() (-1)
        private int TileStringRepresentationToInt(string tileString)
        {
            switch (_loadMethod)
            {
                case TileIdentificationMethod.Index:
                    try
                    {
                        return int.Parse(tileString);
                    }
                    catch (FormatException)
                    {
                        Debug.LogError("Error: Trying to load a Name based level using Index loading");
                        return LevelEditor.GetEmpty();
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.LogError("Error: Encountered a null in the file");
                        return LevelEditor.GetEmpty();
                    }
                case TileIdentificationMethod.Name:
                    if (tileString == "EMPTY")
                        return LevelEditor.GetEmpty();
                    for (var i = 0; i < _tiles.Count; i++)
                        if (_tiles[i].name == tileString)
                            return i;

                    return LevelEditor.GetEmpty();
                default:
                    return LevelEditor.GetEmpty();
            }
        }
    }
}