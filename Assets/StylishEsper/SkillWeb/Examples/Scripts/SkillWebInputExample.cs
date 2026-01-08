//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.UI.UGUI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Esper.SkillWeb.Examples
{
    public class SkillWebInputExample : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private float moveSpeed = 5;

        [SerializeField, Min(0)]
        private float zoomSpeed = 10;

        private void OnMove(InputValue value)
        {
            var move = value.Get<Vector2>();
            move *= moveSpeed;
            WebViewUGUI.Active.Move(move);
        }

        private void OnFocus(InputValue value)
        {
            var move = value.Get<Vector2>();

            if (move != Vector2.zero)
            {
                WebViewUGUI.Active.Focus(move);
            }
        }

        private void OnResetView(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.ResetView();
            }
        }

        private void OnResetSkills(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.ResetAllSkills();
            }
        }

        private void OnZoomIn(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.SetConstantZoom(zoomSpeed);
            }
            else
            {
                WebViewUGUI.Active.SetConstantZoom(0);
            }
        }

        private void OnZoomOut(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.SetConstantZoom(-zoomSpeed);
            }
            else
            {
                WebViewUGUI.Active.SetConstantZoom(0);
            }
        }

        private void OnRemoveFocus(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.RemoveFocus();
            }
        }

        private void OnUpgrade(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.UpgradeFocused();
            }
        }

        private void OnDowngrade(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.DowngradeFocused();
            }
        }

        private void OnToggle(InputValue value)
        {
            if (value.isPressed)
            {
                WebViewUGUI.Active.Toggle();
            }
        }
    }
}