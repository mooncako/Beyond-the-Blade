//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Graph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class SkillNodeEditorElement : Button
    {
        public VisualElement icon;
        public Label nameLabel;
        public Label idLabel;

        public SkillNode skillNode;
        public bool selected;

        public SkillNodeEditorElement()
        {
            AddToClassList("skillRoot");

            var content = new VisualElement();
            content.AddToClassList("skillContent");
            Add(content);

            icon = new VisualElement();
            icon.AddToClassList("skillIcon");
            content.Add(icon);

            nameLabel = new Label();
            nameLabel.AddToClassList("skillLabel");
            nameLabel.style.fontSize = 16;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            content.Add(nameLabel);

            idLabel = new Label();
            idLabel.AddToClassList("skillLabel");
            idLabel.style.fontSize = 12;
            idLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            idLabel.style.flexShrink = 0;
            content.Add(idLabel);
        }

        public void SetSkillNode(SkillNode skillNode, bool selected)
        {
            this.skillNode = skillNode;
            this.selected = selected;
            Refresh();
        }

        public void Refresh()
        {
            var skillIcon = skillNode.GetIcon(true);
            icon.style.backgroundImage = new StyleBackground(skillIcon.icon);
            icon.style.unityBackgroundImageTintColor = skillIcon.color;
            nameLabel.text = skillNode.skill.skillName;
            nameLabel.style.color = selected ? Color.green : StyleKeyword.Null;
            idLabel.text = $"ID: {skillNode.skill.id}";
        }
    }
}
#endif