//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using Esper.SkillWeb.Graph;

namespace Esper.SkillWeb.Editor
{
    public class WebEditorElement : Button
    {
        public Label nameLabel;
        public Label idLabel;

        public WebGraph web;
        public bool selected;

        public WebEditorElement() 
        {
            AddToClassList("itemRoot");

            var content = new VisualElement();
            content.AddToClassList("itemContent");
            Add(content);

            nameLabel = new Label();
            nameLabel.AddToClassList("skillLabel");
            nameLabel.style.fontSize = 14;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            content.Add(nameLabel);

            idLabel = new Label();
            idLabel.AddToClassList("skillLabel");
            idLabel.style.fontSize = 12;
            idLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            idLabel.style.flexShrink = 0;
            content.Add(idLabel);
        }

        public void SetWeb(WebGraph web, bool selected)
        {
            this.web = web;
            this.selected = selected;
            Refresh();
        }

        public void Refresh()
        {
            nameLabel.text = web.webName;
            nameLabel.style.color = selected ? Color.green : StyleKeyword.Null;
            idLabel.text = $"ID: {web.id}";
        }
    }
}
#endif