//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System;
using UnityEngine;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A uGUI object that represents a skill node connection.
    /// </summary>
    public class ConnectionUGUI : MonoBehaviour, IConnectionUI
    {
        /// <summary>
        /// The connection.
        /// </summary>
        [NonSerialized]
        public Connection connection;

        /// <summary>
        /// The web view reference.
        /// </summary>
        protected WebViewUGUI webView;

        /// <summary>
        /// The connected output skill node UI. This is relevant when a visual connection line is created.
        /// </summary>
        protected SkillNodeUGUI output;

        /// <summary>
        /// If this connection is currently highlighted.
        /// </summary>
        [HideInInspector]
        public bool isHighlighted;

        /// <summary>
        /// The output skill node UI.
        /// </summary>
        public SkillNodeUGUI Output 
        {
            get
            {
                if (connection != null)
                {
                    if (!webView)
                    {
                        return null;
                    }

                    if (webView.loadedSkillNodes.ContainsKey(connection.outputNodeID))
                    {
                        return webView.loadedSkillNodes[connection.outputNodeID];
                    }
                    else
                    {
                        return null;
                    }
                }

                return output;
            }
        }

        /// <summary>
        /// The input skill node UI.
        /// </summary>
        public SkillNodeUGUI Input 
        {
            get
            {
                if (connection != null)
                {
                    if (!webView)
                    {
                        return null;
                    }

                    if (webView.loadedSkillNodes.ContainsKey(connection.inputNodeID))
                    {
                        return webView.loadedSkillNodes[connection.inputNodeID];
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
        } 

        /// <summary>
        /// Checks if this connection is active.
        /// 
        /// An active connection is one where the player has obtained all skills of the connection.
        /// </summary>
        public bool IsActive
        {
            get
            {
                var output = Output;
                var input = Input;
                return (!output || output.skillNode.IsObtained) && (!input || input.skillNode.IsObtained);
            }
        }

        /// <summary>
        /// Sets up the connection UI.
        /// </summary>
        /// <param name="connection">The connection data.</param>
        /// <param name="webView">The web view reference.</param>
        public virtual void SetConnection(Connection connection, WebViewUGUI webView)
        {
            this.connection = connection;
            this.webView = webView;
        }

        public virtual void Refresh()
        {

        }

        public virtual void UpdateLine()
        {

        }

        public virtual void Highlight()
        {
            isHighlighted = true;
        }

        public virtual void Unhighlight()
        {
            isHighlighted = false;
        }

        public virtual void ApplyActiveState()
        {

        }
    }
}