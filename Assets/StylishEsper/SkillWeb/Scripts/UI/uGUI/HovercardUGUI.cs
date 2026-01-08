//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Esper.SkillWeb.UI.UGUI
{
    /// <summary>
    /// A pop-up UI object that displays skill information.
    /// </summary>
    public abstract class HovercardUGUI : MonoBehaviour
    {
        /// <summary>
        /// The content container.
        /// </summary>
        [SerializeField]
        protected RectTransform content;

        /// <summary>
        /// The canvas component.
        /// </summary>
        protected Canvas canvas;

        /// <summary>
        /// The animator component.
        /// </summary>
        [SerializeField]
        protected Animator animator;

        /// <summary>
        /// The object that runs a video.
        /// </summary>
        [SerializeField]
        protected VideoPlayer videoPlayer;

        /// <summary>
        /// The length of time the user must hover over a skill for the details to display.
        /// </summary>
        [Min(0)]
        public float appearanceDelay = 0.5f;

        /// <summary>
        /// The length of the opening animation.
        /// </summary>
        protected float openAnimLength = 0.25f;

        /// <summary>
        /// The length of the closing animation.
        /// </summary>
        protected float closeAnimLength = 0.25f;

        /// <summary>
        /// The open action.
        /// </summary>
        protected Action openAction;

        /// <summary>
        /// The current transition state.
        /// </summary>
        protected TransitionState transitionState;

        /// <summary>
        /// The open coroutine.
        /// </summary>
        protected Coroutine openCoroutine;

        /// <summary>
        /// The close coroutine.
        /// </summary>
        protected Coroutine closeCoroutine;

        /// <summary>
        /// The open with delay coroutine.
        /// </summary>
        protected Coroutine openDelayCoroutine;

        /// <summary>
        /// The SkillNodeUGUI currently being targeted.
        /// </summary>
        public SkillNodeUGUI Target { get; protected set; }

        /// <summary>
        /// If the hover details is currently open.
        /// </summary>
        public bool IsOpen { get => content.gameObject.activeInHierarchy && transitionState == TransitionState.Opened; }

        /// <summary>
        /// The active instance.
        /// </summary>
        public static HovercardUGUI Instance { get; protected set; }

        protected virtual void Awake()
        {
            // Singleton
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            canvas = GetComponent<Canvas>();
            transitionState = TransitionState.Closed;

            content.gameObject.SetActive(false);

            if (animator)
            {
                animator.Play("Close");
            }
        }

        /// <summary>
        /// Refreshes the content.
        /// </summary>
        public virtual void Refresh()
        {
            
        }

        /// <summary>
        /// Updates the hovercard position.
        /// </summary>
        public void UpdatePosition()
        {
            if (Target != null)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    var position = Target.rectTransform.position;
                    position.x += content.rect.width / 2 * canvas.scaleFactor;
                    position.y += content.rect.height / -2 * canvas.scaleFactor;
                    content.position = position;
                }
                else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    Vector3 worldPosition = Target.rectTransform.position;
                    Vector3 screenPosition = canvas.worldCamera.WorldToScreenPoint(worldPosition);
                    screenPosition.x += content.rect.width / 2 * canvas.scaleFactor;
                    screenPosition.y += content.rect.height / -2 * canvas.scaleFactor;
                    Vector3 targetWorldPosition = canvas.worldCamera.ScreenToWorldPoint(screenPosition);
                    content.position = targetWorldPosition;
                }
                else
                {
                    Vector3 baseWorldPosition = Target.rectTransform.position;
                    Vector2 uiOffset = new Vector2(content.rect.width / 2f, -content.rect.height / 2f);
                    Vector3 worldOffset = content.transform.TransformVector(uiOffset);
                    Vector3 finalWorldPosition = baseWorldPosition + worldOffset;
                    finalWorldPosition.z = content.position.z;
                    content.position = finalWorldPosition;
                }
            }
            else
            {
                Vector2 mousePosition = SkillWebUtility.GetMousePositionUGUI();

                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    var position = mousePosition;
                    position.x += content.rect.width / 2 * canvas.scaleFactor;
                    position.y += content.rect.height / -2 * canvas.scaleFactor;
                    content.position = position;
                }
                else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    mousePosition.x += content.rect.width / 2 * canvas.scaleFactor;
                    mousePosition.y += content.rect.height / -2 * canvas.scaleFactor;

                    Vector3 worldPoint;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        canvas.transform as RectTransform,
                        mousePosition,
                        canvas.worldCamera,
                        out worldPoint
                    );

                    content.position = worldPoint;
                }
                else
                {
                    Vector3 worldPoint;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        canvas.transform as RectTransform,
                        mousePosition,
                        canvas.worldCamera,
                        out worldPoint
                    );

                    Vector3 baseWorldPosition = worldPoint;
                    Vector2 uiOffset = new Vector2(content.rect.width / 2f, -content.rect.height / 2f);
                    Vector3 worldOffset = content.transform.TransformVector(uiOffset);
                    Vector3 finalWorldPosition = baseWorldPosition + worldOffset;
                    finalWorldPosition.z = content.position.z;
                    content.position = finalWorldPosition;
                }
            }

            SkillWebUtility.ForceInsideView(content, canvas);
        }

        /// <summary>
        /// Prepares the video and plays it after it is prepared.
        /// </summary>
        /// <param name="clip">The video clip.</param>
        /// <returns>Yields until the video is prepared.</returns>
        protected IEnumerator PrepareVideo(VideoClip clip)
        {
            yield return null;
            videoPlayer.clip = clip;
            videoPlayer.Prepare();
            yield return videoPlayer.isPrepared;
            videoPlayer.Play();
        }

        /// <summary>
        /// Sets the content position on the target and forces the content inside the game viewport if it's somewhat outside of it.
        /// </summary>
        /// <returns>Yields for fixed updates.</returns>
        protected IEnumerator OpenCoroutine()
        {
            yield return null;
            Resize();
            yield return null;
            UpdatePosition();

            if (animator)
            {
                animator.Play("Open");
                StartCoroutine(Invoke(Opened, openAnimLength));
            }
            else
            {
                Opened();
            }
        }

        /// <summary>
        /// Resizes the window based on its content.
        /// </summary>
        protected virtual void Resize()
        {
            var size = CalculateSize();
            content.sizeDelta = new Vector2(size.x, size.y);
        }

        /// <summary>
        /// Calculates the size.
        /// </summary>
        /// <returns>The size.</returns>
        protected virtual Vector2 CalculateSize()
        {
            float totalHeight = 0;
            float width = content.sizeDelta.x;

            if (videoPlayer.gameObject.activeSelf)
            {
                totalHeight += (videoPlayer.transform as RectTransform).sizeDelta.y;
            }

            return new Vector2(width, totalHeight);
        }

        /// <summary>
        /// Opens the hover details after a delay.
        /// </summary>
        /// <returns>Yields for appearanceDelay.</returns>
        protected IEnumerator OpenWithDelay()
        {
            if (SkillWeb.Settings.deltaTime == Settings.SkillWebSettings.DeltaTime.DeltaTime)
            {
                yield return new WaitForSeconds(appearanceDelay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(appearanceDelay);
            }

            if (videoPlayer.gameObject.activeSelf)
            {
                StartCoroutine(PrepareVideo(Target.skillNode.skill.demoClip));
            }

            Open(Target.rectTransform);
        }

        /// <summary>
        /// Opens the hovercard.
        /// </summary>
        /// <param name="target">The target visual element to use as a position reference.</param>
        /// <returns>True if the hovercard was successfully opened. Opening may fail if it's already opened or it's transitioning.</returns>
        public virtual bool Open(SkillNodeUGUI target)
        {
            if (transitionState == TransitionState.Closing)
            {
                openAction = () => Open(target);
                return false;
            }
            else if (transitionState != TransitionState.Closed)
            {
                return false;
            }

            if (!target)
            {
                Close();
                return false;
            }

            if (!target.skillNode.skill.demoClip)
            {
                videoPlayer.gameObject.SetActive(false);
            }
            else
            {
                videoPlayer.gameObject.SetActive(true);
            }

            Target = target;
            openDelayCoroutine = StartCoroutine(OpenWithDelay());
            Refresh();
            return true;
        }

        /// <summary>
        /// Opens the item details.
        /// </summary>
        /// <param name="target">The target RectTransform to use as a position reference.</param>
        public virtual void Open(RectTransform target)
        {
            content.gameObject.SetActive(true);
            transitionState = TransitionState.Opening;
            openCoroutine = StartCoroutine(OpenCoroutine());
        }

        /// <summary>
        /// Closes the hover details.
        /// </summary>
        public virtual void Close()
        {
            if (transitionState == TransitionState.Closing || transitionState == TransitionState.Closed)
            {
                StopOpenCoroutines();
                openAction = null;
                return;
            }

            transitionState = TransitionState.Closing;

            if (animator)
            {
                animator.Play("Close");
                StartCoroutine(Invoke(Closed, closeAnimLength));
            }
            else
            {
                Closed();
            }

            StopOpenCoroutines();
            StartCoroutine(OpenOnCloseIfStillHovering());
        }

        /// <summary>
        /// Stops open and close coroutines.
        /// </summary>
        protected void StopOpenCoroutines()
        {
            if (openCoroutine != null)
            {
                StopCoroutine(openCoroutine);
            }

            if (openDelayCoroutine != null)
            {
                StopCoroutine(openDelayCoroutine);
            }
        }

        /// <summary>
        /// Invokes an action after a delay.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="delay">The delay.</param>
        /// <returns>Yields for the delay.</returns>
        protected IEnumerator Invoke(Action action, float delay)
        {
            if (SkillWeb.Settings.deltaTime == Settings.SkillWebSettings.DeltaTime.DeltaTime)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            action();
        }

        /// <summary>
        /// Opens the hovercard if the user is still hovering a skill.
        /// </summary>
        /// <returns>Yields until transition state is closed.</returns>
        protected IEnumerator OpenOnCloseIfStillHovering()
        {
            var previousTarget = Target;

            yield return transitionState == TransitionState.Closed;

            if (previousTarget.hasPointerHover && previousTarget == Target)
            {
                Open(Target);
            }
        }

        /// <summary>
        /// Sets the transition state as opened.
        /// </summary>
        public void Opened()
        {
            transitionState = TransitionState.Opened;
        }

        /// <summary>
        /// Sets the transition state as closed.
        /// </summary>
        public void Closed()
        {
            transitionState = TransitionState.Closed;
            content.gameObject.SetActive(false);

            if (openAction != null)
            {
                openAction();
                openAction = null;
            }

            closeCoroutine = null;
        }

        /// <summary>
        /// The transition state of the hover details.
        /// </summary>
        protected enum TransitionState
        {
            Opened,
            Opening,
            Closed,
            Closing
        }
    }
}