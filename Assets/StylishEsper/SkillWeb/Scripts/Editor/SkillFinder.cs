//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
    public class SkillFinder
    {
        public Toolbar root;
        protected ToolbarSearchField searchField;
        protected VisualElement skillContent;
        protected Action<Skill> onSkillSelected;
        protected ToolbarMenu tagMenu;
        protected ToolbarMenu sizeMenu;

        private Skill[] allSkills;
        private List<SkillEditorElement> skillElements = new();

        private string tag;
        private int size = -1;

        public SkillFinder(Action<Skill> onSkillSelected, Toolbar root)
        {
            this.root = root;
            this.onSkillSelected = onSkillSelected;

            root.RegisterCallback<GeometryChangedEvent>(x => ClampOnScreen());

            searchField = root.Q<ToolbarSearchField>("FinderSearchField");
            skillContent = root.Q<VisualElement>("SkillContent");
            tagMenu = root.Q<ToolbarMenu>("TagMenu");
            sizeMenu = root.Q<ToolbarMenu>("SizeMenu");

            sizeMenu.menu.ClearItems();
            sizeMenu.menu.AppendAction("None", x => RemoveSizeFilter());
            sizeMenu.menu.AppendAction("Tiny", x => SetSizeFilter(Skill.Size.Tiny));
            sizeMenu.menu.AppendAction("Small", x => SetSizeFilter(Skill.Size.Small));
            sizeMenu.menu.AppendAction("Medium", x => SetSizeFilter(Skill.Size.Medium));
            sizeMenu.menu.AppendAction("Large", x => SetSizeFilter(Skill.Size.Large));
            sizeMenu.menu.AppendAction("Giant", x => SetSizeFilter(Skill.Size.Giant));

            tagMenu.menu.ClearItems();

            tagMenu.menu.AppendAction("None", x => RemoveTagFilter());
            foreach (var tag in SkillWeb.Settings.tags)
            {
                tagMenu.menu.AppendAction(tag, x => SetTagFilter(tag));
            }

            searchField.RegisterValueChangedCallback(x =>
            {
                SetSearchFilter(x.newValue);
            });

            allSkills = SkillWeb.GetAllSkills();
            CreateElements();
            SetSearchFilter(string.Empty);
        }

        protected void CreateElements()
        {
            skillContent.Clear();

            foreach (var skill in allSkills)
            {
                var skillElement = new SkillEditorElement();
                skillElement.SetSkill(skill, false);

                skillElement.clicked += () =>
                {
                    onSkillSelected(skill);
                };

                skillElements.Add(skillElement);
                skillContent.Add(skillElement);
            }
        }

        protected void SetSearchFilter(string pattern)
        {
            foreach (var element in skillElements)
            {
                var skill = element.skill;
                if (skill.skillName.ToLower().Contains(pattern.ToLower()) && 
                    (string.IsNullOrEmpty(tag) || skill.Tag == tag) && 
                    (size == -1 || size == (int)skill.size))
                {
                    element.style.display = DisplayStyle.Flex;
                }
                else
                {
                    element.style.display = DisplayStyle.None;
                }
            }
        }

        protected void SetTagFilter(string tag)
        {
            this.tag = tag;
            tagMenu.text = tag;
            SetSearchFilter(searchField.value);
        }

        protected void SetSizeFilter(Skill.Size size)
        {
            this.size = (int)size;
            sizeMenu.text = size.ToString();
            SetSearchFilter(searchField.value);
        }

        protected void RemoveSizeFilter()
        {
            size = -1;
            sizeMenu.text = "Sizes";
            SetSearchFilter(searchField.value);
        }

        protected void RemoveTagFilter()
        {
            tag = string.Empty;
            tagMenu.text = "Tags";
            SetSearchFilter(searchField.value);
        }

        public void Open(Vector2 position)
        {
            SetPosition(position);
            root.style.display = DisplayStyle.Flex;
        }

        public void Close()
        {
            root.style.display = DisplayStyle.None;
        }

        public void SetPosition(Vector2 position)
        {
            root.style.left = position.x;
            root.style.top = position.y;
        }

        public void ClampOnScreen()
        {
            var position = new Vector2(root.resolvedStyle.left, root.resolvedStyle.top);

            if (position.x + root.resolvedStyle.width > root.parent.resolvedStyle.width)
            {
                position.x = root.parent.resolvedStyle.width - root.resolvedStyle.width;
            }
            else if (position.x < 0)
            {
                position.x = 0;
            }

            if (position.y + root.resolvedStyle.height > root.parent.resolvedStyle.height)
            {
                position.y = root.parent.resolvedStyle.height - root.resolvedStyle.height;
            }
            else if (position.y < 0)
            {
                position.y = 0;
            }

            root.style.left = position.x;
            root.style.top = position.y;
        }
    }
}
#endif