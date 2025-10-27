//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    [CustomEditor(typeof(PlayerWebLink))]
    public class PlayerWebLinkEditor : UnityEditor.Editor
    {
        private TextField webNameField;
        private Toggle linkAutomaticallyField;
        private VisualElement linkedIndicator;
        private Label linkLabel;
        private VisualElement skillContent;
        private Dictionary<SkillNodeEditorElement, Skill.State> loadedSkillElements = new();

        private bool prevLinkStatus;
        private int prevObtainedSkillCount;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new VisualElement();
            var visualTree = AssetSearch.Find<VisualTreeAsset>("SkillWeb", "Scripts/Editor/PlayerWebLinkEditor.uxml");
            visualTree.CloneTree(inspector);

            webNameField = inspector.Q<TextField>("WebNameField");
            linkAutomaticallyField = inspector.Q<Toggle>("LinkAutomaticallyField");
            linkedIndicator = inspector.Q<VisualElement>("LinkedIndicator");
            linkLabel = inspector.Q<Label>("LinkLabel");
            skillContent = inspector.Q<VisualElement>("SkillContent");

            var serializedObject = new SerializedObject(target);
            webNameField.BindProperty(serializedObject.FindProperty("webName"));
            linkAutomaticallyField.BindProperty(serializedObject.FindProperty("linkAutomatically"));

            linkAutomaticallyField.RegisterValueChangedCallback(x =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                if (x.newValue)
                {
                    var playerWebLink = target as PlayerWebLink;

                    if (!playerWebLink.IsLinked)
                    {
                        playerWebLink.StartCoroutine(playerWebLink.AutoLinkCoroutine());
                    }
                }
            });

            inspector.schedule.Execute(UpdateInspector).Every(1000);

            return inspector;
        }

        private void UpdateInspector()
        {
            var playerWebLink = target as PlayerWebLink;

            if (prevLinkStatus != playerWebLink.IsLinked)
            {
                if (playerWebLink.IsLinked)
                {
                    linkedIndicator.RemoveFromClassList("unlinked");
                    linkedIndicator.AddToClassList("linked");
                }
                else
                {
                    linkedIndicator.RemoveFromClassList("linked");
                    linkedIndicator.RemoveFromClassList("blank");
                    linkedIndicator.AddToClassList("unlinked");
                    skillContent.Clear();
                    loadedSkillElements.Clear();
                }
            }

            if (prevObtainedSkillCount != playerWebLink.obtainedSkills.Count)
            {
                skillContent.Clear();
                loadedSkillElements.Clear();

                foreach (var skillNode in playerWebLink.obtainedSkills)
                {
                    var skillNodeElement = new SkillNodeEditorElement();
                    skillNodeElement.SetSkillNode(skillNode, false);
                    skillNodeElement.SetEnabled(false);
                    skillNodeElement.style.opacity = 1;
                    skillContent.Add(skillNodeElement);
                    loadedSkillElements.Add(skillNodeElement, skillNode.state);
                }
            }
            else
            {
                for (int i = 0; i < loadedSkillElements.Count; i++)
                {
                    var element = loadedSkillElements.ElementAt(i);

                    if (element.Key.skillNode.state != element.Value)
                    {
                        element.Key.Refresh();
                        loadedSkillElements[element.Key] = element.Key.skillNode.state;
                    }
                }
            }

            if (playerWebLink.IsLinked)
            {
                linkLabel.text = $"Linked - {playerWebLink.obtainedSkills.Count}";
            }
            else
            {
                linkLabel.text = "Unlinked";
            }

            prevLinkStatus = playerWebLink.IsLinked;
            prevObtainedSkillCount = playerWebLink.obtainedSkills.Count;
        }
    }
}
#endif