//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.UI.UGUI;
using UnityEngine;

namespace Esper.SkillWeb.Examples
{
    /// <summary>
    /// A script that helps set up the Runes demo.
    /// </summary>
    public class RunesDemoSetup : MonoBehaviour
    {
        /// <summary>
        /// Visual core.
        /// </summary>
        [SerializeField]
        private RectTransform core;

        /// <summary>
        /// Custom skill points.
        /// </summary>
        [SerializeField]
        private float customSkillPoints = 100f;

        private void Start()
        {
            // Get the Runes web graph
            var graph = SkillWeb.GetWebGraph("Runes");

            // Set the graph
            WebViewUGUI.Active.Load(graph);

            // Get the active web
            var web = WebViewUGUI.Active.web;

            // Bind with the custom skill points
            web.Bind(() => customSkillPoints, // Set skill points getter
                skillNode => (skillNode.dataset as DemoSkillDataset).cost, // Set skill cost getter (this example uses a custom dataset)
                x => customSkillPoints = x); // Set skill points setter 

            // Set core position to the center of the graph
            var position = WebViewUGUI.Active.GetCenterPosition();
            core.anchoredPosition = position;

            // Create extra connections connecting to the core (just as some visual customizations)
            WebViewUGUI.Active.CreateVisualConnectionLine(core.anchoredPosition, 0);
            WebViewUGUI.Active.CreateVisualConnectionLine(core.anchoredPosition, 1);
            WebViewUGUI.Active.CreateVisualConnectionLine(core.anchoredPosition, 2);
            WebViewUGUI.Active.CreateVisualConnectionLine(core.anchoredPosition, 3);
        }

        //private void Update()
        //{
        //    Debug.Log(customSkillPoints);
        //}
    }
}