//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using Esper.SkillWeb.Graph;
using Esper.SkillWeb.UI;
using Esper.SkillWeb.UI.UGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Esper.SkillWeb
{
    /// <summary>
    /// Links a web to a GameObject.
    /// </summary>
    public class PlayerWebLink : MonoBehaviour
    {
        /// <summary>
        /// The name of the web to link with.
        /// </summary>
        public string webName;

        /// <summary>
        /// If the component should automatically link to the web. Automatic linking requires a Web to be loaded by a
        /// web view component. Disable this if you'd like to handle linking yourself.
        /// </summary>
        public bool linkAutomatically = true;

        /// <summary>
        /// The linked web. This will be null if there was no link made.
        /// </summary>
        [NonSerialized]
        public Web linkedWeb = null;

        /// <summary>
        /// A list of obtained skills. This is updated automatically as long as there is a link set.
        /// </summary>
        public List<SkillNode> obtainedSkills = new();

        /// <summary>
        /// If linked with a web.
        /// </summary>
        public bool IsLinked { get => linkedWeb != null; }

        /// <summary>
        /// A callback for when the PlayerWebLink becomes unlinked.
        /// </summary>
        [NonSerialized]
        public UnityEvent onUnlinked = new();

        /// <summary>
        /// A callback for when the PlayerWebLink is linked.
        /// </summary>
        [NonSerialized]
        public UnityEvent onLinked = new();

        private void Awake()
        {
            if (linkAutomatically)
            {
                RuntimeWebViewer.onLoadCompleted.AddListener(x => UpdateLinkStatus());
            }

            SkillNode.onSkillObtained.AddListener(AddToObtainedSkills);
            SkillNode.onSkillUnobtained.AddListener(RemoveFromObtainedSkills);
        }

        private void OnEnable()
        {
            if (linkAutomatically && !IsLinked)
            {
                StartCoroutine(AutoLinkCoroutine());
            }
        }

        /// <summary>
        /// Adds a skill to the list of obtained skills. The skill is only added if the web of the skill matches.
        /// </summary>
        /// <param name="skillNode">The skill node to add.</param>
        protected void AddToObtainedSkills(SkillNode skillNode)
        {
            if (skillNode.web != linkedWeb || obtainedSkills.Contains(skillNode))
            {
                return;
            }

            obtainedSkills.Add(skillNode);
        }

        /// <summary>
        /// Removes a skill to the list of obtained skills. The skill is only added if the web of the skill matches.
        /// </summary>
        /// <param name="skillNode">The skill node to remove.</param>
        protected void RemoveFromObtainedSkills(SkillNode skillNode)
        {
            if (skillNode.web != linkedWeb || !obtainedSkills.Contains(skillNode))
            {
                return;
            }

            obtainedSkills.Remove(skillNode);
        }

        /// <summary>
        /// Updates the link status. If a web view is found in the scene with a matching web name, a link to it will
        /// be set. Otherwise, there will be no link.
        /// 
        /// If already linked to a web, and the relevant web view is removed, this method will unlink from the web.
        /// </summary>
        public void UpdateLinkStatus()
        {
            // Find the relevant web view
            var webView = WebViewUGUI.Find(webName);

            if (webView)
            {
                // Link if found and not already linked
                if (!IsLinked)
                {
                    Link(webView.web);
                }
            }
            // Unlink if linked and not found
            else if (IsLinked)
            {
                Unlink();
            }
        }

        /// <summary>
        /// Tries to link with a web using the webName field. No link will be made if the relevant web view component
        /// is not found.
        /// </summary>
        public void Link()
        {
            var webView = WebViewUGUI.Find(webName, false);

            if (!webView)
            {
                SkillWebLogger.LogWarning("Player Web Link: unable to find a web to link with! Is the web name correct?");
                return;
            }

            Link(webView.web);
        }

        /// <summary>
        /// Links with a web. This may change the webName field.
        /// </summary>
        /// <param name="web">The web to link with.</param>
        public void Link(Web web)
        {
            if (web == null)
            {
                SkillWebLogger.LogWarning("Player Web Link: cannot link to a null web.");
                return;
            }

            linkedWeb = web;
            webName = web.graph.webName;
            obtainedSkills = GetAllObtainedSkills();
            onLinked.Invoke();
        }

        /// <summary>
        /// Unlinks from a web.
        /// </summary>
        public void Unlink()
        {
            if (!IsLinked)
            {
                SkillWebLogger.LogWarning("Player Web Link: cannot unlink when there is no link set.");
                return;
            }

            linkedWeb = null;
            obtainedSkills.Clear();

            if (linkAutomatically)
            {
                StartCoroutine(AutoLinkCoroutine());
            }

            onUnlinked.Invoke();
        }

        /// <summary>
        /// Gets the list of obtained skills. This requires a link.
        /// </summary>
        /// <returns>A list of obtained skills. Null is returned if no link is set.</returns>
        public List<SkillNode> GetAllObtainedSkills()
        {
            if (!IsLinked)
            {
                return null;
            }

            return linkedWeb.GetObtainedSkillNodes();
        }

        /// <summary>
        /// Gets skill nodes by their skill ID. A list is returned because multiple instances of the same skill are allowed
        /// in a web. This requires a link.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>A list of skills with a matching skill ID. Null is returned if no link is set.</returns>
        public List<SkillNode> GetSkillNodes(int id)
        {
            if (!IsLinked)
            {
                return null;
            }

            return linkedWeb.GetSkillNodes(id);
        }

        /// <summary>
        /// Gets skill nodes by their name. A list is returned because multiple instances of the same skill are allowed
        /// in a web. This requires a link.
        /// </summary>
        /// <param name="name">The skill name.</param>
        /// <returns>A list of skills with a matching name. Null is returned if no link is set.</returns>
        public List<SkillNode> GetSkillNodes(string name)
        {
            if (!IsLinked)
            {
                return null;
            }

            return linkedWeb.GetSkillNodes(name);
        }

        /// <summary>
        /// Gets the first skill node by skill ID. This requires a link.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>The skill node with the specific ID. Null is returned if no link is set.</returns>
        public SkillNode GetSkillNode(int id)
        {
            if (!IsLinked)
            {
                return null;
            }

            return linkedWeb.GetSkillNode(id);
        }

        /// <summary>
        /// Gets the first skill node by name. This requires a link.
        /// </summary>
        /// <param name="name">The name of the skill.</param>
        /// <returns>The skill node with the specific name. Null is returned if no link is set.</returns>
        public SkillNode GetSkillNode(string name)
        {
            if (!IsLinked)
            {
                return null;
            }

            return linkedWeb.GetSkillNode(name);
        }

        /// <summary>
        /// Checks if a skill with the specific skill ID is obtained. This requires a link.
        /// </summary>
        /// <param name="id">The skill ID.</param>
        /// <returns>True if the skill node with the skill ID is obtained. Otherwise, false.</returns>
        public bool IsObtained(int id)
        {
            if (!IsLinked)
            {
                return false;
            }

            return linkedWeb.IsObtained(id);
        }

        /// <summary>
        /// Checks if a skill with the specific skill name is obtained. This requires a link.
        /// </summary>
        /// <param name="name">The skill name.</param>
        /// <returns>True if the skill node with the name is obtained. Otherwise, false.</returns>
        public bool IsObtained(string name)
        {
            if (!IsLinked)
            {
                return false;
            }

            return linkedWeb.IsObtained(name);
        }

        /// <summary>
        /// A coroutine that runs indefinitely until a link is made.
        /// </summary>
        /// <returns>Yields for a second before each link attempt.</returns>
        public IEnumerator AutoLinkCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                UpdateLinkStatus();

                if (IsLinked || !linkAutomatically)
                {
                    break;
                }
            }
        }
    }
}