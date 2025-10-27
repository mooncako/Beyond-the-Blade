//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class SkillEditorElement : Button
    {
        public VisualElement icon;
        public Label nameLabel;
        public Label idLabel;

        public Skill skill;
        public bool selected;

        public SkillEditorElement() 
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

        public void SetSkill(Skill skill, bool selected)
        {
            this.skill = skill;
            this.selected = selected;
            Refresh();
        }

        public void Refresh()
        {
            var skillIcon = skill.GetIcon(Skill.State.Maxed, true);
            icon.style.backgroundImage = new StyleBackground(skillIcon.icon);
            icon.style.unityBackgroundImageTintColor = skillIcon.color;
            nameLabel.text = skill.skillName;
            nameLabel.style.color = selected ? Color.green : StyleKeyword.Null;
            idLabel.text = $"ID: {skill.id}";
        }
    }
}
#endif