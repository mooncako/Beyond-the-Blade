//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace Esper.SkillWeb.Editor
{
#if !UNITY_2022
    [UxmlElement]
    public partial class DraggableEditorElement : Button
    {
#endif
#if UNITY_2022
    public class DraggableEditorElement : Button
    {
        public new class UxmlFactory : UxmlFactory<DraggableEditorElement, UxmlTraits> { }
#endif
        protected Vector2 startingPosition;
        protected Vector2 startingMousePosition;
        protected bool isDragging = false;

        public DraggableEditorElement()
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
            RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
        }

        public void ClampOnScreen()
        {
            var position = new Vector2(resolvedStyle.left, resolvedStyle.top);

            if (position.x + resolvedStyle.width > parent.resolvedStyle.width)
            {
                position.x = parent.resolvedStyle.width - resolvedStyle.width;
            }
            else if (position.x < 0)
            {
                position.x = 0;
            }

            if (position.y + resolvedStyle.height > parent.resolvedStyle.height)
            {
                position.y = parent.resolvedStyle.height - resolvedStyle.height;
            }
            else if (position.y < 0)
            {
                position.y = 0;
            }

            style.left = position.x;
            style.top = position.y;
        }

        public void ForceStopDrag()
        {
            isDragging = false;
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button == 0)
            {
                isDragging = true;
                startingMousePosition = evt.position;
                startingPosition = new Vector2(resolvedStyle.left, resolvedStyle.top);
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (isDragging)
            {
                var mousePosition = (Vector2)evt.position;
                var diff = mousePosition - startingMousePosition;
                var position = startingPosition + diff;

                if (position.x + resolvedStyle.width > parent.resolvedStyle.width)
                {
                    position.x = parent.resolvedStyle.width - resolvedStyle.width;
                }
                else if (position.x < 0)
                {
                    position.x = 0;
                }

                if (position.y + resolvedStyle.height > parent.resolvedStyle.height)
                {
                    position.y = parent.resolvedStyle.height - resolvedStyle.height;
                }
                else if (position.y < 0)
                {
                    position.y = 0;
                }

                style.left = position.x;
                style.top = position.y;
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button == 0)
            {
                isDragging = false;
            }
        }
    }
}
#endif