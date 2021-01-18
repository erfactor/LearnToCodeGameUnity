using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Commands;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public static class CodeLineFactory
    {
        private static GameObject ContainerPrefab => GameObject.Find("InstructionContainer");
        private static GameObject RootContainer => GameObject.Find("RootContainer");

        public static CodeLine GetStandardCodeLine(GameObject instruction)
        {
            var newLine = new CodeLine(GetContainerGameObject(), instruction);
            return newLine;
        }

        private static GameObject GetContainerGameObject()
        {
            var container = GameObject.Instantiate(ContainerPrefab);
            container.transform.SetParent(RootContainer.transform);
            container.tag = "Container";
            return container;
        }
    }
}
