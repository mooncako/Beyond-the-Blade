//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

namespace Esper.SkillWeb.Graph
{
    /// <summary>
    /// A connection between two nodes.
    /// </summary>
    [System.Serializable]
    public class Connection : GraphElement
    {
        /// <summary>
        /// The output node ID.
        /// </summary>
        public int outputNodeID;

        /// <summary>
        /// The input node ID.
        /// </summary>
        public int inputNodeID;

        /// <summary>
        /// The output port index.
        /// </summary>
        public int outputPortIndex;

        /// <summary>
        /// The input port index.
        /// </summary>
        public int inputPortIndex;

        /// <summary>
        /// The index of the connection prefab.
        /// </summary>
        public int styleIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputNodeID">The output node ID.</param>
        /// <param name="inputNodeID">The input node ID.</param>
        /// <param name="outputPortIndex">The output port index.</param>
        /// <param name="inputPortIndex">The input port index.</param>
        /// <param name="styleIndex">The index of the connection prefab.</param>
        public Connection(int outputNodeID, int inputNodeID, int outputPortIndex, int inputPortIndex, int styleIndex)
        {
            this.outputNodeID = outputNodeID;
            this.inputNodeID = inputNodeID;
            this.outputPortIndex = outputPortIndex;
            this.inputPortIndex = inputPortIndex;
            this.styleIndex = styleIndex;
        }

        /// <summary>
        /// If the data matches this node's conection data.
        /// </summary>
        /// <param name="outputNodeID">The output node ID.</param>
        /// <param name="inputNodeID">The input node ID.</param>
        /// <param name="outputPortIndex">The output port index.</param>
        /// <param name="inputPortIndex">The input port index.</param>
        /// <returns>True if all data matches. Otherwise, false.</returns>
        public bool Matches(int outputNodeID, int inputNodeID, int outputPortIndex, int inputPortIndex)
        {
            return this.outputNodeID == outputNodeID && this.inputNodeID == inputNodeID && this.outputPortIndex == outputPortIndex && this.inputPortIndex == inputPortIndex;
        }

        /// <summary>
        /// If another connection matches this connection.
        /// </summary>
        /// <param name="connection">The other connection.</param>
        /// <returns>True if all data matches. Otherwise, false.</returns>
        public bool Matches(Connection connection)
        {
            return outputNodeID == connection.outputNodeID && inputNodeID == connection.inputNodeID && outputPortIndex == connection.outputPortIndex && inputPortIndex == connection.inputPortIndex;
        }

        /// <summary>
        /// Creates a copy of this connection.
        /// </summary>
        /// <returns>The copy.</returns>
        public Connection CreateCopy()
        {
            var copy = new Connection(outputNodeID, inputNodeID, outputPortIndex, inputPortIndex, styleIndex);
            copy.webGraphID = webGraphID;
            return copy;
        }
    }
}