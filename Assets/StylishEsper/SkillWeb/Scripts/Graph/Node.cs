//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System.Collections.Generic;
using UnityEngine;

namespace Esper.SkillWeb.Graph
{
    /// <summary>
    /// The base graph node.
    /// </summary>
    [System.Serializable]
    public class Node : GraphElement
    {
        /// <summary>
        /// The node ID.
        /// </summary>
        public int id;

        /// <summary>
        /// A guid generated when this node was initially created.
        /// </summary>
        public string guid;

        /// <summary>
        /// The position in the graph.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// A list of all connections.
        /// </summary>
        public List<Connection> connections = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The node ID.</param>
        /// <param name="position">The position in the graph.</param>
        /// <param name="connections">A list of all connections.</param>
        public Node(int id, Vector2 position, List<Connection> connections = null) 
        {
            this.id = id;
            guid = SkillWebUtility.GenerateNewGuid();
            this.position = position;
            this.connections = connections;
        }
    }
}