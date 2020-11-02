using System.Collections.Generic;
using UnityEngine;

namespace Packages._2DTileMapLevelEditor.Scripts {

    public class Tileset : MonoBehaviour {

        // The list of tiles the user can use to create maps
        // Public so the user can add all user-created prefabs
        public List<Transform> Tiles;

    }
}

