//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A uGUI object that represents a SkillNode.
    /// </summary>
    public class SkillNodeUGUI : MonoBehaviour, ISkillNodeUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// The skill node.
        /// </summary>
        [HideInInspector]
        public SkillNode skillNode;

        /// <summary>
        /// The web view reference.
        /// </summary>
        [HideInInspector]
        public WebViewUGUI webView;

        /// <summary>
        /// The RectTransform component.
        /// </summary>
        public RectTransform rectTransform;

        /// <summary>
        /// The image component that displays the skill icon.
        /// </summary>
        public Image iconImage;

        /// <summary>
        /// The text component that displays the level text.
        /// </summary>
        public TextMeshProUGUI levelText;

        /// <summary>
        /// If the pointer is hovering this node.
        /// </summary>
        [NonSerialized]
        public bool hasPointerHover;

        /// <summary>
        /// The pointer upgrade button.
        /// </summary>
        [SerializeField]
        protected PointerEventData.InputButton pointerUpgradeButton = PointerEventData.InputButton.Left;

        /// <summary>
        /// The pointer downgrade button.
        /// </summary>
        [SerializeField]
        protected PointerEventData.InputButton pointerDowngradeButton = PointerEventData.InputButton.Right;

        /// <summary>
        /// A callback for when any skill node's state is changed. This accepts 1 argument: the affected skill node (SkillNodeUGUI).
        /// </summary>
        public static UnityEvent<SkillNodeUGUI> onStateChanged = new();

        /// <summary>
        /// A callback for when any skill node is upgraded. This accepts 1 argument: the affected skill node (SkillNodeUGUI).
        /// </summary>
        public static UnityEvent<SkillNodeUGUI> onUpgrade = new();

        /// <summary>
        /// A callback for when any skill node is downgraded. This accepts 1 argument: the affected skill node (SkillNodeUGUI).
        /// </summary>
        public static UnityEvent<SkillNodeUGUI> onDowngrade = new();

        /// <summary>
        /// A callback for when any skill node is hovered. This accepts 1 argument: the affected skill node (SkillNodeUGUI).
        /// </summary>
        public static UnityEvent<SkillNodeUGUI> onPointerEnter = new();

        /// <summary>
        /// A callback for when any skill node stops being hovered. This accepts 1 argument: the affected skill node (SkillNodeUGUI).
        /// </summary>
        public static UnityEvent<SkillNodeUGUI> onPointerExit = new();

        public void SetSkill(SkillNode skillNode, WebViewUGUI webView)
        {
            this.skillNode = skillNode;
            skillNode.uiRefresher = ReloadUI;
            skillNode.uiOnStateChanged = () => onStateChanged.Invoke(this);
            this.webView = webView;
            Refresh();
        }

        public void ResetLevel()
        {
            skillNode.ResetLevel();
        }

        public void Refresh()
        {
            float size = 100;

            switch (skillNode.skill.size)
            {
                case Skill.Size.Tiny:
                    size = SkillWeb.Settings.skillNodeSizes.tiny;
                    break;

                case Skill.Size.Small:
                    size = SkillWeb.Settings.skillNodeSizes.small;
                    break;

                case Skill.Size.Medium:
                    size = SkillWeb.Settings.skillNodeSizes.medium;
                    break;

                case Skill.Size.Large:
                    size = SkillWeb.Settings.skillNodeSizes.large;
                    break;

                case Skill.Size.Giant:
                    size = SkillWeb.Settings.skillNodeSizes.giant;
                    break;
            }

            rectTransform.sizeDelta = new Vector2(size, size);

            if (skillNode != null)
            {
                var skillIcon = skillNode.GetIcon(true);
                iconImage.sprite = skillIcon.icon;
                iconImage.color = skillIcon.color;
                iconImage.enabled = skillIcon.icon;

                if (levelText)
                {
                    levelText.text = $"{skillNode.Level}/{skillNode.MaxLevel}";
                }
            }
            else
            {
                iconImage.sprite = null;
                iconImage.color = Color.white;
                iconImage.enabled = false;

                if (levelText)
                {
                    levelText.text = "0/0";
                }
            }
        }

        /// <summary>
        /// Creates a list of all connections of this skill node.
        /// </summary>
        /// <param name="hasObtainedSkillOnly">If only connections to obtained skills should be included.</param>
        /// <returns>A list of all connections.</returns>
        public List<ConnectionUGUI> GetConnections(bool hasObtainedSkillOnly = false)
        {
            var connectionUIs = new List<ConnectionUGUI>();

            foreach (var connection in skillNode.connections)
            {
                if (webView.loadedConnections.ContainsKey(connection))
                {
                    var connectionUI = webView.loadedConnections[connection];
                    var input = connectionUI.Input;
                    var output = connectionUI.Output;

                    if (input == this)
                    {
                        input = null;
                    }
                    else if (output == this)
                    {
                        output = null;   
                    }

                    if (!hasObtainedSkillOnly || (input && input.skillNode.HasConnectionToPrerequisiteSkill()) || (output && output.skillNode.HasConnectionToPrerequisiteSkill()))
                    {
                        connectionUIs.Add(connectionUI);
                    }
                }
            }

            return connectionUIs;
        }

        public virtual bool TryUpgrade()
        {
            bool result = skillNode.TryUpgrade();

            if (result)
            {
                onUpgrade.Invoke(this);

                if (HovercardUGUI.Instance && HovercardUGUI.Instance.Target == this)
                {
                    HovercardUGUI.Instance.Refresh();
                }

                if (hasPointerHover || WebViewSelectorUGUI.Instance?.focusedNode == this)
                {
                    UnhighlightPaths();
                }
            }

            return result;
        }

        public virtual bool TryDowngrade()
        {
            bool result = skillNode.TryDowngrade();

            if (result)
            {
                onDowngrade.Invoke(this);

                if (HovercardUGUI.Instance && HovercardUGUI.Instance.Target == this)
                {
                    HovercardUGUI.Instance.Refresh();
                }

                if (hasPointerHover || WebViewSelectorUGUI.Instance?.focusedNode == this)
                {
                    HighlightPaths();
                }
            }

            return result;
        }

        /// <summary>
        /// Refreshes the skill UI and the UI for each connection.
        /// </summary>
        public void ReloadUI()
        {
            Refresh();

            foreach (var connection in skillNode.connections)
            {
                if (webView.loadedConnections.ContainsKey(connection))
                {
                    webView.loadedConnections[connection].Refresh();
                }
            }
        }

        /// <summary>
        /// Highlights direct connections to this node if the other node in the connection is obtained or doesn't exist.
        /// </summary>
        public void HighlightPaths()
        {
            foreach (var connection in skillNode.connections)
            {
                var ui = webView.loadedConnections[connection];
                var output = ui.Output;
                var input = ui.Input;
                SkillNodeUGUI node;
                SkillNodeUGUI other;

                if (output == this)
                {
                    node = output;
                    other = input;
                }
                else
                {
                    node = input;
                    other = output;
                }

                if (node.skillNode.state == Skill.State.Unlocked && (!other || other.skillNode.IsObtained))
                {
                    ui.Highlight();
                }
            }
        }

        /// <summary>
        /// Unhighlights direct connections to this node.
        /// </summary>
        public void UnhighlightPaths()
        {
            foreach (var connection in skillNode.connections)
            {
                var ui = webView.loadedConnections[connection];

                if (ui.isHighlighted)
                {
                    ui.Unhighlight();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hasPointerHover = true;
            HighlightPaths();
            onPointerEnter.Invoke(this);

            if (HovercardUGUI.Instance)
            {
                HovercardUGUI.Instance.Open(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hasPointerHover = false;
            UnhighlightPaths();
            onPointerExit.Invoke(this);

            if (HovercardUGUI.Instance)
            {
                HovercardUGUI.Instance.Close();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == pointerUpgradeButton)
            {
                TryUpgrade();
            }
            else if (eventData.button == pointerDowngradeButton)
            {
                TryDowngrade();
            }

            if (HovercardUGUI.Instance && HovercardUGUI.Instance.IsOpen)
            {
                HovercardUGUI.Instance.Close();
            }
        }
    }
}