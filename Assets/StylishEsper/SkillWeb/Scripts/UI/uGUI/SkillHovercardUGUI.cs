//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A pop-up UI object that displays skill information.
    /// </summary>
    public class SkillHovercardUGUI : HovercardUGUI
    {
        /// <summary>
        /// The layout group used by labels.
        /// </summary>
        [SerializeField]
        protected VerticalLayoutGroup labelLayout;

        /// <summary>
        /// The label that displays the title.
        /// </summary>
        [SerializeField]
        protected TextMeshProUGUI titleLabel;

        /// <summary>
        /// The label that displays the subtitle.
        /// </summary>
        [SerializeField]
        protected TextMeshProUGUI subtitleLabel;

        /// <summary>
        /// The label that displays the description.
        /// </summary>
        [SerializeField]
        protected TextMeshProUGUI descriptionLabel;

        public override void Refresh()
        {
            if (Target.skillNode.dataset)
            {
                titleLabel.text = Target.skillNode.dataset.GetName();
                subtitleLabel.text = Target.skillNode.dataset.GetSubtext();
                descriptionLabel.text = Target.skillNode.dataset.GetDescription();

                if (SkillWeb.Settings.enablePlayerLevelRequirement && !Target.skillNode.IsUnlocked)
                {
                    descriptionLabel.text += $"<color=grey>\n\nRequired Level: {Target.skillNode.skill.levelRequirement}</color>";
                }
            }
            else
            {
                SkillWebLogger.LogWarning("Skill Hovercard: the dataset seems to be missing.");

                if (SkillWeb.Settings.enablePlayerLevelRequirement && !Target.skillNode.IsUnlocked)
                {
                    descriptionLabel.text = $"<color=grey>\n\nRequired Level: {Target.skillNode.skill.levelRequirement}</color>";
                }
            }
        }

        public override void Open(RectTransform target)
        {
            // Update the size of the description label, as it's the only label with a dynamic height
            descriptionLabel.rectTransform.sizeDelta = new Vector2(descriptionLabel.rectTransform.sizeDelta.x, descriptionLabel.preferredHeight);

            // Call base open method
            base.Open(target);
        }

        protected override Vector2 CalculateSize()
        {
            // Get the base size
            var size = base.CalculateSize();

            // Add the size of the labels through the layout (+ some extra bottom spacing)
            size.y += labelLayout.preferredHeight + 4;

            return size;
        }
    }
}