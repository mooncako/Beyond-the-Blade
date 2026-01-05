#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Plastic.Newtonsoft.Json;

public class EnemySkillsSheetSyncWindow : EditorWindow
{
    // ---------- Rows expected from the Sheet ----------
    // Columns expected (names must match your Apps Script JSON):
    // Key, AnimationID, Cooldown, Damage, DamageTickTime, DamageType,
    // WeaponType,
    // AreaType, RangeX, RangeY, RangeZ,
    // Rarity, TargetSelf, IsTargetedGroundAOE,
    // Buffs, Debuffs, Name, Description
    [Serializable]
    private class SkillRow
    {
        public string Key;
        public string AnimationID;
        public float Cooldown;
        public float Damage;

        public float DamageTickTime;
        public string DamageType;

        public string WeaponType;

        public string AreaType;
        public float RangeX;
        public float RangeY;
        public float RangeZ;

        public string Rarity;
        public bool TargetSelf;
        public bool IsTargetedGroundAOE;

        public string Buffs;   // semicolon-joined
        public string Debuffs; // semicolon-joined

        public string Name;
        public string Description;
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
    [SerializeField] private string _webAppUrl = "https://script.google.com/macros/s/AKfycbw-fe8xucRwRdbBlrM8r5yLFZGbHe7WZNIKMH-F_a2Dv9iiw7B3uNG_p04U3g6FeudR9w/exec";
    [SerializeField] private string _sharedSecret = "BYTHEBLADE";
    [SerializeField] private string _sheetName = "EnemySkills"; // tab name inside the Google Sheet

    [Header("Sources (ScriptableObjects)")]
    [SerializeField] private List<EnemySkillsSO> _sources = new List<EnemySkillsSO>();

    private Vector2 _scroll;

    [MenuItem("By the Blade/Enemy Skills ← Google Sheet")]
    public static void Open()
    {
        var win = GetWindow<EnemySkillsSheetSyncWindow>("Skills Sheet Pull");
        win.minSize = new Vector2(560, 440);
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
        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = ValidConfig();
            if (GUILayout.Button("PULL from Google Sheet", GUILayout.Height(32)))
            {
                PullFromSheet();
            }
            GUI.enabled = true;
        }

        EditorGUILayout.HelpBox(
            "This tool pulls data from the Google Sheet and updates the selected ScriptableObjects.\n\n" +
            "Expected columns: Key, AnimationID, Cooldown, Damage, WeaponType, AreaType, RangeX, RangeY, RangeZ, Rarity, TargetSelf, IsTargetedGroundAOE, Buffs, Debuffs, Name, Description.\n" +
            "Lists are semicolon-separated; escape literal semicolons as \\;.",
            MessageType.Info);
    }

    private bool ValidConfig()
    {
        return !string.IsNullOrWhiteSpace(_webAppUrl)
               && !string.IsNullOrWhiteSpace(_sharedSecret)
               && _sources.Count > 0;
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

            // detail contains header line then body; extract JSON
            var idx = detail.IndexOf('\n');
            var body = idx >= 0 ? detail.Substring(idx + 1) : detail;

            Debug.Log($"[SheetSync] PULL raw body:\n{body}");
            try
            {
                var resp = JsonConvert.DeserializeObject<PullResponse>(body);
                if (resp == null || !string.Equals(resp.status, "ok", StringComparison.OrdinalIgnoreCase) || resp.rows == null)
                    throw new Exception(resp?.message ?? "Malformed response.");

                var map = resp.rows
                    .Where(r => !string.IsNullOrWhiteSpace(r.Key))
                    .ToDictionary(r => r.Key, r => r);

                int updated = 0, created = 0, removed = 0;

                foreach (var so in _sources.Where(s => s != null))
                {
                    bool dirty = false;

                    // Track existing keys in this SO; anything left after applying the sheet will be deleted.
                    var existingKeys = new HashSet<string>(so.SkillDict.Keys);

                    foreach (var (key, r) in map)
                    {
                        // Key is present in sheet, so we do NOT want to delete it.
                        existingKeys.Remove(key);

                        if (!so.SkillDict.TryGetValue(key, out var skill))
                        {
                            skill = new Skill();
                            so.SkillDict[key] = skill;
                            created++;
                        }

                        // Basic fields
                        skill.AnimationID = r.AnimationID ?? "";
                        skill.Cooldown = r.Cooldown;
                        skill.Damage = r.Damage;
                        skill.DamageTickTime = r.DamageTickTime;

                        var dmgTypeStr = (r.DamageType ?? "").Trim();
                        if(Enum.TryParse<DamageType>(dmgTypeStr, true, out var damageType))
                        {
                            skill.DamageType = damageType;
                        }
                        else if(!string.IsNullOrEmpty(dmgTypeStr))
                        {
                            Debug.LogWarning($"[SheetSync] Unknown DamageType '{r.DamageType}' for Key '{key}'");
                        }

                        skill.TargetSelf = r.TargetSelf;
                        skill.IsTargetedGroundAOE = r.IsTargetedGroundAOE;

                        // WeaponType enum (NEW)
                        var weaponStr = (r.WeaponType ?? "").Trim();
                        if (Enum.TryParse<WeaponType>(weaponStr, true, out var weaponType))
                        {
                            skill.WeaponType = weaponType;
                        }
                        else if (!string.IsNullOrEmpty(weaponStr))
                        {
                            Debug.LogWarning($"[SheetSync] Unknown WeaponType '{r.WeaponType}' for Key '{key}'");
                        }

                        // Rarity enum
                        var rarityStr = (r.Rarity ?? "").Trim();
                        if (Enum.TryParse<Rarity>(rarityStr, true, out var rarity))
                            skill.Rarity = rarity;
                        else if (!string.IsNullOrEmpty(rarityStr))
                            Debug.LogWarning($"[SheetSync] Unknown Rarity '{r.Rarity}' for Key '{key}'");

                        // Range & AreaType
                        if (skill.SkillRange == null)
                            skill.SkillRange = new SkillRange();

                        var areaStr = (r.AreaType ?? "").Trim();
                        if (Enum.TryParse<SkillAreaType>(areaStr, true, out var area))
                        {
                            skill.SkillRange.AreaType = area;
                        }
                        else if (!string.IsNullOrEmpty(areaStr))
                        {
                            Debug.LogWarning($"[SheetSync] Unknown AreaType '{r.AreaType}' for Key '{key}'");
                        }

                        skill.SkillRange.X = r.RangeX;
                        skill.SkillRange.Y = r.RangeY;
                        skill.SkillRange.Z = r.RangeZ;

                        // Lists
                        skill.Buffs = ParsePairs(r.Buffs);
                        skill.Debuffs = ParsePairs(r.Debuffs);

                        skill.Name = r.Name ?? "";
                        skill.Description = r.Description ?? "";

                        dirty = true;
                        updated++;
                    }

                    // Remove any skills that are not present on the sheet
                    if (existingKeys.Count > 0)
                    {
                        foreach (var keyToRemove in existingKeys)
                        {
                            so.SkillDict.Remove(keyToRemove);
                            removed++;
                        }

                        dirty = true;
                    }

                    if (dirty)
                        EditorUtility.SetDirty(so);
                }

                AssetDatabase.SaveAssets();
                Debug.Log($"[SheetSync] PULL OK (Enemy). Updated: {updated}, Created: {created}, Removed: {removed}");
                EditorUtility.DisplayDialog("PULL",
                    $"Updated: {updated}\nCreated: {created}\nRemoved: {removed}",
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SheetSync] Parse error: {ex}");
                EditorUtility.DisplayDialog("PULL Error", ex.Message, "OK");
            }
        });
    }


    // ---------- Helpers ----------
    private static List<string> SplitList(string s)
    {
        if (string.IsNullOrEmpty(s)) return new List<string>();
        var parts = s.Split(new[] { ';' }, StringSplitOptions.None).ToList();
        for (int i = 0; i < parts.Count; i++)
            parts[i] = parts[i].Replace("\\;", ";");
        return parts;
    }

    private static List<(string, float)> ParsePairs(string s)
    {
        var result = new List<(string, float)>();
        if (string.IsNullOrEmpty(s)) return result;

        // Tokenize by unescaped semicolons
        List<string> tokens = new List<string>();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        bool escape = false;
        foreach (char ch in s)
        {
            if (escape) { sb.Append(ch); escape = false; continue; }
            if (ch == '\\') { escape = true; continue; }
            if (ch == ';') { tokens.Add(sb.ToString()); sb.Clear(); continue; }
            sb.Append(ch);
        }
        if (sb.Length > 0) tokens.Add(sb.ToString());

        foreach (var token in tokens)
        {
            // Split by first unescaped colon
            string left = null, right = null;
            sb.Clear(); escape = false;
            bool split = false;
            foreach (char ch in token)
            {
                if (escape) { sb.Append(ch); escape = false; continue; }
                if (ch == '\\') { escape = true; continue; }
                if (ch == ':' && !split)
                {
                    left = sb.ToString();
                    sb.Clear();
                    split = true;
                    continue;
                }
                sb.Append(ch);
            }
            right = sb.ToString();

            string id = (left ?? "").Trim();
            string valStr = (right ?? "").Trim();
            if (string.IsNullOrEmpty(id)) continue;

            float value = 0f;
            float.TryParse(valStr, System.Globalization.NumberStyles.Float,
                           System.Globalization.CultureInfo.InvariantCulture, out value);

            result.Add((id, value));
        }

        return result;
    }

    // Optional (useful if you add PUSH later)
    private static string FormatPairs(IEnumerable<(string, float)> pairs)
    {
        if (pairs == null) return "";
        string Esc(string x) => x?.Replace("\\", "\\\\").Replace(";", "\\;").Replace(":", "\\:") ?? "";
        var parts = new List<string>();
        foreach (var (id, val) in pairs)
            parts.Add($"{Esc(id)}:{val.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        return string.Join(";", parts);
    }
}

// ================== Robust Editor HTTP helper (GET only) ==================
public static class EditorHttp
{
    private static void RunEditorAsync(UnityWebRequest req, Action<bool, string, long, UnityWebRequest.Result> onDone, int timeoutSeconds = 30)
    {
        UnityWebRequestAsyncOperation op = req.SendWebRequest();
        double startTime = EditorApplication.timeSinceStartup;

        void Tick()
        {
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
