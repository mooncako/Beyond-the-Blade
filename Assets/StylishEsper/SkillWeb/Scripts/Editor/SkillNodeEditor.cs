//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using Esper.SkillWeb.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class SkillNodeEditor : GraphNodeEditor
    {
        public SkillNode Value { get => value as SkillNode; }

        private VisualElement image;

        public Toggle hasConnectionDependencyToggle;

        public Vector2Field positionField;

        public IntegerField dependencyCountField;

        private Label titleLabel;

        private Label idLabel;

        private bool fieldsHidden;

        public SkillNodeEditor(SkillNode skillNode) : base(skillNode)
        {
            idLabel = new Label();
            idLabel.AddToClassList("idLabel");
            mainContainer.Add(idLabel);

            hasConnectionDependencyToggle = new Toggle();
            hasConnectionDependencyToggle.AddToClassList("dependantField");
            hasConnectionDependencyToggle.text = "Connection Dependent";
            hasConnectionDependencyToggle.tooltip = "Connection Dependent";
            mainContainer.Add(hasConnectionDependencyToggle);

            hasConnectionDependencyToggle.RegisterValueChangedCallback(x =>
            {
                dependencyCountField.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            positionField = new Vector2Field();
            positionField.AddToClassList("positionField");
            positionField.value = new Vector2(0, 0);
            mainContainer.Add(positionField);

            dependencyCountField = new IntegerField();
            dependencyCountField.AddToClassList("dependencyCountField");
            dependencyCountField.label = "Dependency Count";
            dependencyCountField.value = 1;
            mainContainer.Add(dependencyCountField);

            var hideFieldsButton = new ToolbarButton();
            hideFieldsButton.AddToClassList("hideFieldsButton");

            var hideIcon = new VisualElement();
            hideIcon.AddToClassList("visibleIcon");
            hideFieldsButton.Add(hideIcon);

            hideFieldsButton.clicked += () =>
            {
                fieldsHidden = !fieldsHidden;

                if (fieldsHidden)
                {
                    hasConnectionDependencyToggle.style.visibility = Visibility.Hidden;
                    positionField.style.visibility = Visibility.Hidden;
                    dependencyCountField.style.visibility = Visibility.Hidden;
                    hideIcon.RemoveFromClassList("visibleIcon");
                    hideIcon.AddToClassList("hiddenIcon");
                }
                else
                {
                    hasConnectionDependencyToggle.style.visibility = Visibility.Visible;
                    positionField.style.visibility = Visibility.Visible;
                    dependencyCountField.style.visibility = Visibility.Visible;
                    hideIcon.RemoveFromClassList("hiddenIcon");
                    hideIcon.AddToClassList("visibleIcon");
                }
            };

            mainContainer.Add(hideFieldsButton);

            image = new VisualElement();
            mainContainer.Add(image);

            titleLabel = this.Q<Label>("title-label");
            titleLabel.style.textOverflow = TextOverflow.Ellipsis;
            titleLabel.style.overflow = Overflow.Hidden;

            Refresh();
        }

        public override void Refresh()
        {
            var skill = Value.skill;

            if (!skill)
            {
                idLabel.style.display = DisplayStyle.None;
                title = "Missing";
                titleContainer.style.width = 75;
                image.style.width = 75;
                image.style.height = 75;
                image.style.backgroundColor = Color.red;
                image.style.backgroundImage = new StyleBackground(AssetSearch.Find<Texture2D>("SkillWeb", "Editor/Icons/warning_icon.png"));
                image.style.display = DisplayStyle.Flex;
                hasConnectionDependencyToggle.style.display = DisplayStyle.None;
                positionField.style.display = DisplayStyle.None;
                return;
            }

            title = skill.skillName;
            idLabel.text = $"ID: {Id}";
            idLabel.style.display = DisplayStyle.Flex;
            hasConnectionDependencyToggle.style.display = DisplayStyle.Flex;
            positionField.style.display = DisplayStyle.Flex;
            image.style.backgroundColor = StyleKeyword.Auto;
            titleLabel.tooltip = skill.skillName;

            switch (skill.size)
            {
                case Skill.Size.Tiny:
                    image.style.width = 100;
                    image.style.height = 100;
                    titleContainer.style.width = 100;
                    break;

                case Skill.Size.Small:
                    image.style.width = 125;
                    image.style.height = 125;
                    titleContainer.style.width = 125;
                    break;

                case Skill.Size.Medium:
                    image.style.width = 150;
                    image.style.height = 150;
                    titleContainer.style.width = 150;
                    break;

                case Skill.Size.Large:
                    image.style.width = 175;
                    image.style.height = 175;
                    titleContainer.style.width = 175;
                    break;

                case Skill.Size.Giant:
                    image.style.width = 200;
                    image.style.height = 200;
                    titleContainer.style.width = 200;
                    break;
            }

            var icon = skill.GetIcon(Skill.State.Maxed, true);

            if (icon.icon)
            {
                image.style.backgroundImage = new StyleBackground(icon.icon);
                image.style.unityBackgroundImageTintColor = icon.color;
                image.style.display = DisplayStyle.Flex;
            }
            else
            {
                image.style.display = DisplayStyle.None;
            }
        }

        protected override void CreateInputPorts()
        {
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = string.Empty;
            inputContainer.Add(input);
            inputPorts.Add(input);
        }

        protected override void CreateOutputPorts() 
        {
            var output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            output.portName = string.Empty;
            outputContainer.Add(output);
            outputPorts.Add(output);
        }
    }
}
#endif