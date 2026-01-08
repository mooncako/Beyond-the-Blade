//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public abstract class GraphNodeEditor : Node
    {
        public Graph.Node value;

        public List<Port> inputPorts = new();
        public List<Port> outputPorts = new();

        public int Id { get => value.id; }

        public bool HasConnections { get => value.connections.Count > 0; }

        public GraphNodeEditor(Graph.Node value)
        {
            m_CollapseButton.style.display = DisplayStyle.None;
            this.Q<VisualElement>("node-border").style.overflow = Overflow.Visible;
            this.value = value;
            style.left = value.position.x;
            style.top = value.position.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        public abstract void Refresh();

        protected abstract void CreateInputPorts();

        protected abstract void CreateOutputPorts();

        public void SetPosition(Vector2 position)
        {
            value.position = position;
            style.left = position.x;
            style.top = position.y;
        }

        public override void OnSelected()
        {
            base.OnSelected();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
        }

        public int GetTotalConnectionCount()
        {
            var ports = inputContainer.Children()
                .Concat(outputContainer.Children())
                .OfType<Port>();

            return ports.Sum(p => p.connections.Count());
        }
    }
}
#endif