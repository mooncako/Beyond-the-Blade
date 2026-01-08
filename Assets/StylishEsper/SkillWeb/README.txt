Full documentation is available at the link below:

https://stylishesper.gitbook.io/skill-web

Quick Start Guide:

Create Skills from the Skill Bank
---------------------------------
Skills can be created from the Skill Bank window which can be accessed from Window > Skill Web > Skill Bank.


Create Webs (Skill Trees) from the Web Creator
----------------------------------------------
Webs can be created from the Web Creator window which can be accessed from Window > Skill Web > Web Creator.


Add Initializer
---------------
Before you can work with Skill Web at runtime, it may need to be initialized. You can initialize Skill Web whenever through code, or use the Skill Web Initializer component. The initializer component can be added to your scene by right-clicking in the hierarchy and navigating to Skill Web > Initializer.


Add and Load UI
---------------
1. The UI is required to view webs. You can add the UI by right-clicking in the hierarchy and navigating to Skill Web > Web View UGUI.
2. The hovercard is required to view skill information. You can add it by right-clicking in the hierarchy and navigating to Skill Web > Hovercard UGUI.
3. You can load a web by clicking on the Web View UGUI component, enabling testing, and setting a test web graph. You can also load a web through code:

// Get the web graph by name
var web = SkillWeb.GetWebGraph("My Web Graph");

// Load it
WebViewUGUI.Active.Load(web);