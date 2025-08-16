#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Plastic.Newtonsoft.Json;

public class EnemySkillsSheetSyncWindow : EditorWindow
{
    // ---------- DTOs ----------
    [Serializable]
    private class SheetPayload
    {
        public string secret;
        public string sheetName;
        public List<SkillRow> rows = new List<SkillRow>();
    }

    [Serializable]
    private class SkillRow
    {
        public string Key;
        public string AnimationID;
        public float Cooldown;
        public float Damage;
        public string AreaType;    // string in sheet
        public string Rarity;      // string in sheet
        public bool TargetSelf;
        public string Buffs;       // semicolon-joined
        public string Debuffs;     // semicolon-joined
    }

    [Serializable]
    private class PullResponse
    {
        public string status; // "ok" or "error"
        public string message;
        public List<SkillRow> rows;
    }

    // ---------- UI State ----------
    [Header("Google Apps Script Web App")]
    [SerializeField] private string _webAppUrl = "https://script.google.com/macros/s/AKfycbwtQGMgwAzF8d2r2odtKanxkRl8-dEGuPOaOIB9dp0YiCvHB1MmoDkiBjcz44M2kleRFA/exec"; // e.g. https://script.google.com/macros/s/AKfycb.../exec
    [SerializeField] private string _sharedSecret = "BYTHEBLADE";
    [SerializeField] private string _sheetName = "EnemySkills"; // tab name inside the Google Sheet

    [Header("Sources (ScriptableObjects)")]
    [SerializeField] private List<EnemySkillsSO> _sources = new List<EnemySkillsSO>();

    [Header("Options")]
    [SerializeField] private bool _dryRunOnPush = false; // true = Apps Script won't write

    private Vector2 _scroll;

    [MenuItem("By the Blade/Enemy Skills ↔ Google Sheet Sync")]
    public static void Open()
    {
        var win = GetWindow<EnemySkillsSheetSyncWindow>("Skills Sheet Sync");
        win.minSize = new Vector2(560, 460);
        win.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Google Apps Script Web App", EditorStyles.boldLabel);
        _webAppUrl = EditorGUILayout.TextField("Web App URL", _webAppUrl);
        _sharedSecret = EditorGUILayout.TextField("Shared Secret", _sharedSecret);
        _sheetName = EditorGUILayout.TextField("Sheet (tab) Name", _sheetName);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sources (ScriptableObjects)", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Selected"))
            {
                foreach (var obj in Selection.objects)
                {
                    if (obj is EnemySkillsSO so && !_sources.Contains(so))
                        _sources.Add(so);
                }
            }
            if (GUILayout.Button("Clear"))
            {
                _sources.Clear();
            }
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));
        for (int i = 0; i < _sources.Count; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _sources[i] = (EnemySkillsSO)EditorGUILayout.ObjectField(_sources[i], typeof(EnemySkillsSO), false);
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    _sources.RemoveAt(i);
                    break;
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        _dryRunOnPush = EditorGUILayout.ToggleLeft("Dry-Run (don’t write to the sheet)", _dryRunOnPush);

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = ValidConfig();
            // if (GUILayout.Button("PUSH to Google Sheet", GUILayout.Height(32)))
            // {
            //     PushToSheet();
            // }

            if (GUILayout.Button("PULL from Google Sheet", GUILayout.Height(32)))
            {
                PullFromSheet();
            }
            GUI.enabled = true;
        }

        EditorGUILayout.HelpBox(
            "Workflow:\n" +
            "1) Edit cells in the Sheet (numbers/enums/bools/lists).\n" +
            "2) PULL to update your ScriptableObjects.\n\n" +
            "Lists are semicolon-separated. Literal semicolons in items are escaped as \\; .",
            MessageType.Info);
    }

    private bool ValidConfig()
    {
        return !string.IsNullOrWhiteSpace(_webAppUrl)
               && !string.IsNullOrWhiteSpace(_sharedSecret)
               && _sources.Count > 0;
    }

    // ---------- PUSH ----------
    private void PushToSheet()
    {
        var payload = new SheetPayload
        {
            secret = _sharedSecret,
            sheetName = _sheetName,
            rows = new List<SkillRow>()
        };

        foreach (var so in _sources.Where(s => s != null))
        {
            foreach (var kv in so.EnemySkillDict)
            {
                var key = kv.Key;
                var s = kv.Value;
                var row = new SkillRow
                {
                    Key = key,
                    AnimationID = s.AnimationID ?? "",
                    Cooldown = s.Cooldown,
                    Damage = s.Damage,
                    AreaType = s.AreaType.ToString(),
                    Rarity = s.Rarity.ToString(),
                    TargetSelf = s.TargetSelf,
                    Buffs = JoinList(s.Buffs),
                    Debuffs = JoinList(s.Debuffs)
                };
                payload.rows.Add(row);
            }
        }

        var json = JsonConvert.SerializeObject(payload);
        string url = _webAppUrl + "?mode=push" + (_dryRunOnPush ? "&dryrun=1" : "");

        EditorHttp.HttpPost(url, json, (ok, detail) =>
        {
            Debug.Log($"[SheetSync] PUSH result:\n{detail}");
            EditorUtility.DisplayDialog(ok ? "PUSH OK" : "PUSH Error", detail, "OK");
        });
    }

    // ---------- PULL ----------
    private void PullFromSheet()
    {
        string url = _webAppUrl +
                     $"?mode=pull&sheetName={UnityWebRequest.EscapeURL(_sheetName)}&secret={UnityWebRequest.EscapeURL(_sharedSecret)}";

        EditorHttp.HttpGet(url, (ok, detail) =>
        {
            if (!ok)
            {
                Debug.LogError($"[SheetSync] PULL ERROR:\n{detail}");
                EditorUtility.DisplayDialog("PULL Error", detail, "OK");
                return;
            }

            // detail contains header line then body; extract the JSON body
            var idx = detail.IndexOf('\n');
            var body = idx >= 0 ? detail.Substring(idx + 1) : detail;

            Debug.Log($"[SheetSync] PULL raw body:\n{body}");
            try
            {
                var resp = JsonConvert.DeserializeObject<PullResponse>(body);
                if (resp == null || !string.Equals(resp.status, "ok", StringComparison.OrdinalIgnoreCase) || resp.rows == null)
                    throw new Exception(resp?.message ?? "Malformed response.");

                // Merge rows back into SOs
                var map = resp.rows.ToDictionary(r => r.Key, r => r);

                int updated = 0, created = 0;
                foreach (var so in _sources.Where(s => s != null))
                {
                    bool dirty = false;

                    foreach (var kv in map)
                    {
                        if (string.IsNullOrWhiteSpace(kv.Key)) continue;

                        if (!so.EnemySkillDict.TryGetValue(kv.Key, out var skill))
                        {
                            skill = new Skill();
                            so.EnemySkillDict[kv.Key] = skill;
                            created++;
                        }

                        var r = kv.Value;

                        skill.AnimationID = r.AnimationID ?? "";
                        skill.Cooldown = r.Cooldown;
                        skill.Damage = r.Damage;

                        // Enums with Trim + case-insensitive parsing
                        var areaStr = (r.AreaType ?? "").Trim();
                        var rarityStr = (r.Rarity ?? "").Trim();

                        if (Enum.TryParse<SkillAreaType>(areaStr, true, out var area)) skill.AreaType = area;
                        else Debug.LogWarning($"[SheetSync] Unknown AreaType '{r.AreaType}' for Key '{kv.Key}'");

                        if (Enum.TryParse<SkillRarity>(rarityStr, true, out var rarity)) skill.Rarity = rarity;
                        else Debug.LogWarning($"[SheetSync] Unknown Rarity '{r.Rarity}' for Key '{kv.Key}'");

                        skill.TargetSelf = r.TargetSelf;

                        skill.Buffs = SplitList(r.Buffs);
                        skill.Debuffs = SplitList(r.Debuffs);

                        dirty = true;
                        updated++;
                    }

                    if (dirty)
                    {
                        EditorUtility.SetDirty(so);
                    }
                }

                AssetDatabase.SaveAssets();
                Debug.Log($"[SheetSync] PULL OK. Updated: {updated}, Created: {created}");
                EditorUtility.DisplayDialog("PULL", $"Updated: {updated}\nCreated: {created}", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SheetSync] Parse error: {ex}");
                EditorUtility.DisplayDialog("PULL Error", ex.Message, "OK");
            }
        });
    }

    // ---------- Helpers ----------
    private static string JoinList(List<string> list)
    {
        if (list == null || list.Count == 0) return "";
        return string.Join(";", list.Select(s => (s ?? "").Replace(";", "\\;")));
    }

    private static List<string> SplitList(string s)
    {
        if (string.IsNullOrEmpty(s)) return new List<string>();
        // Basic split, then unescape \; back to ;
        var parts = s.Split(new[] { ';' }, StringSplitOptions.None).ToList();
        for (int i = 0; i < parts.Count; i++)
            parts[i] = parts[i].Replace("\\;", ";");
        return parts;
    }
}

// ================== Robust Editor HTTP helper ==================
public static class EditorHttp
{
    // Polling loop that survives common Editor coroutine quirks
    private static void RunEditorAsync(UnityWebRequest req, Action<bool, string, long, UnityWebRequest.Result> onDone, int timeoutSeconds = 30)
    {
        UnityWebRequestAsyncOperation op = req.SendWebRequest();
        var startTime = EditorApplication.timeSinceStartup;

        void Tick()
        {
            // Timeout
            if (EditorApplication.timeSinceStartup - startTime > timeoutSeconds)
            {
                EditorApplication.update -= Tick;
                try { req.Abort(); } catch { }
                string body = SafeText(req);
                onDone?.Invoke(false,
                    $"Timeout after {timeoutSeconds}s\nHTTP {req.responseCode} ({req.result})\n{(string.IsNullOrEmpty(body) ? "<no body>" : body)}",
                    req.responseCode, req.result);
                req.Dispose();
                return;
            }

            if (!op.isDone) return;

            EditorApplication.update -= Tick;

            string text = SafeText(req);
            bool httpOk = req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300;
            bool bodyOk = false;
            try
            {
                var obj = JsonConvert.DeserializeObject<Unity.Plastic.Newtonsoft.Json.Linq.JObject>(string.IsNullOrEmpty(text) ? "{}" : text);
                bodyOk = obj?["status"]?.ToString().Equals("ok", StringComparison.OrdinalIgnoreCase) == true;
            }
            catch { /* ignore */ }

            bool finalOk = httpOk || bodyOk;
            onDone?.Invoke(finalOk,
                $"HTTP {req.responseCode} ({req.result})\n{(string.IsNullOrEmpty(text) ? "<no body>" : text)}",
                req.responseCode, req.result);

            req.Dispose();
        }

        EditorApplication.update += Tick;
    }

    private static string SafeText(UnityWebRequest req)
    {
        try { return req.downloadHandler?.text; } catch { return null; }
    }

    public static void HttpPost(string url, string json, Action<bool, string> onDone, int timeoutSeconds = 30)
    {
        var req = new UnityWebRequest(url, "POST");
        var bodyRaw = Encoding.UTF8.GetBytes(json ?? "");
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.redirectLimit = 4;
        req.timeout = timeoutSeconds;

        RunEditorAsync(req, (ok, detail, code, result) => onDone?.Invoke(ok, detail), timeoutSeconds);
    }

    public static void HttpGet(string url, Action<bool, string> onDone, int timeoutSeconds = 30)
    {
        var req = UnityWebRequest.Get(url);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.redirectLimit = 4;
        req.timeout = timeoutSeconds;

        RunEditorAsync(req, (ok, detail, code, result) => onDone?.Invoke(ok, detail), timeoutSeconds);
    }
}
#endif
