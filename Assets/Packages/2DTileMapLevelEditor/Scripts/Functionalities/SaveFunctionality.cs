using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Animators;
using Packages._2DTileMapLevelEditor.Common.Scripts;
using Packages._2DTileMapLevelEditor.SimpleFileBrowser.Scripts;
using Services;
using UnityEngine;

namespace Packages._2DTileMapLevelEditor.Scripts.Functionalities
{
    public class SaveFunctionality : MonoBehaviour
    {
        // The file browser
        private GameObject _fileBrowserPrefab;

        // The file extension for the saved file
        private string[] _fileExtensions;
        // ----- PRIVATE VARIABLES -----

        // The level editor
        private LevelEditor _levelEditor;

        // Temporary variable to save level before getting the path using the FileBrowser
        private string _levelToSave;

        // Temporary variable to save state of level editor before opening file browser and restore it after save/load
        private bool _preFileBrowserState = true;

        // Method to identifiction the tiles when saving
        private TileIdentificationMethod _saveMethod;

        // Starting path of the file browser
        private string _startPath;

        // The tiles used to build the level
        private List<Transform> _tiles;

        // ----- SETUP -----

        public void Setup(GameObject fileBrowserPrefab, string[] fileExtensions, TileIdentificationMethod saveMethod,
            List<Transform> tiles, string startPath)
        {
            _levelEditor = LevelEditor.Instance;
            _fileBrowserPrefab = fileBrowserPrefab;
            _fileExtensions = fileExtensions;
            _saveMethod = saveMethod;
            _tiles = tiles;
            _startPath = startPath;
            SetupClickListeners();
        }

        // Hook up Save/Load Level method to Save/Load button
        private void SetupClickListeners()
        {
            Utilities.FindButtonAndAddOnClickListener("SaveButton", SaveLevel);
        }

        // ----- PUBLIC METHODS -----

        // Save to a file using a path
        public void SaveLevelUsingPath(string path)
        {
            File.WriteAllText(path, _levelToSave);
            _levelToSave = null;
            _levelEditor.ToggleLevelEditor(_preFileBrowserState);
        }

        // ----- PRIVATE METHODS -----

        // Method to determine whether a layer is empty (empty layers are not saved)
        private bool EmptyLayer(int[,,] level, int width, int height, int layer, int empty)
        {
            var result = true;
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                if (level[x, y, layer] != empty)
                    result = false;

            return result;
        }

        // Converts the internal level represtation (integer) to the tile idenfication type
        // Tiles can be identified using their index in the Tileset array or the name of the prefab game object
        // Empty tiles will be saved using the name "EMPTY"
        // Default will be LevelEditor.GetEmpty() (-1 default value)
        private string TileSaveRepresentationToString(int[,,] levelToSave, int x, int y, int layer)
        {
            return levelToSave[x, y, layer] == LevelEditor.GetEmpty()
                ? "EMPTY"
                : _tiles[levelToSave[x, y, layer]].gameObject.name;
        }

        private void SaveLevel()
        {
            var file = new StringBuilder();
            var levelToSave = _levelEditor.GetLevel();
            var width = _levelEditor.Width;
            var height = _levelEditor.Height;
            var depth = _levelEditor.Layers;

            file.Append($"{width},{height},{depth}");

            for (var y = 0; y < height; y++)
            {
                file.AppendLine();
                for (var x = 0; x < width; x++)
                {
                    for (var z = 0; z < depth; z++)
                    {
                        var tileIndex = levelToSave[x, y, z];
                        if (tileIndex != LevelEditor.GetEmpty())
                        {
                            var go = _tiles[tileIndex].gameObject;
                            file.Append(go.name);
                            if (go.name == "piece")
                            {
                                var piece = GameObject.FindGameObjectsWithTag("piece")
                                    .First(p => p.transform.position.x == x && p.transform.position.y == y);

                                var text = piece.GetComponent<PieceText>().Text;
                                if (!string.IsNullOrEmpty(text))
                                    file.Append(text);
                            }
                        }

                        if (z != depth - 1) file.Append(LevelLoader.DepthSeparator);
                    }

                    if (x != width - 1) file.Append(LevelLoader.WidthSeparator);
                }
            }

            _levelToSave = file.ToString();
            OpenFileBrowser();
        }

        // Open a file browser to save files
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
            fileBrowserScript.SaveFilePanel("Level", _fileExtensions);
            // Subscribe to OnFileSelect event (call SaveLevelUsingPath using path) 
            fileBrowserScript.OnFileSelect += SaveLevelUsingPath;
            // Subscribe to OnFileBrowserClose event (call ReopenLevelEditor) 
            fileBrowserScript.OnFileBrowserClose += ReopenLevelEditor;
        }

        // Reopens the level editor after closing the file browser
        private void ReopenLevelEditor()
        {
            _levelEditor.ToggleLevelEditor(true);
        }
    }
}