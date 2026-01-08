#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Plastic.Newtonsoft.Json;

public class PlayerSkillsSheetSyncWindow : EditorWindow
{
    // --- rows expected from the Sheet ---
    [Serializable]
    private class SkillRow
    {
        public string Key;
        public string AnimationID;
        public float Cooldown;
        public float Damage;

        public string WeaponType;   // NEW

        public string AreaType;
        public float RangeX;
        public float RangeY;
        public float RangeZ;

        public string Rarity;
        public bool TargetSelf;
        public bool IsTargetedGroundAOE;

        public string Buffs;
        public string Debuffs;

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

    // --- UI state (adjust URL/secret to your values) ---
    [Header("Google Apps Script Web App")]
    [SerializeField] private string _webAppUrl = "https://script.google.com/macros/s/AKfycbw-fe8xucRwRdbBlrM8r5yLFZGbHe7WZNIKMH-F_a2Dv9iiw7B3uNG_p04U3g6FeudR9w/exec";
    [SerializeField] private string _sharedSecret = "BYTHEBLADE";
    [SerializeField] private string _sheetName = "PlayerSkills"; // <- new tab name

    [Header("Sources (ScriptableObjects)")]
    [SerializeField] private List<PlayerSkillsSO> _sources = new List<PlayerSkillsSO>();

    private Vector2 _scroll;

    [MenuItem("By the Blade/Player Skills ← Google Sheet")]
    public static void Open()
    {
        var win = GetWindow<PlayerSkillsSheetSyncWindow>("Player Skills Sheet Pull");
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
                    if (obj is PlayerSkillsSO so && !_sources.Contains(so))
                        _sources.Add(so);
                }
            }
            if (GUILayout.Button("Clear")) _sources.Clear();
        }

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));
        for (int i = 0; i < _sources.Count; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _sources[i] = (PlayerSkillsSO)EditorGUILayout.ObjectField(_sources[i], typeof(PlayerSkillsSO), false);
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
                PullFromSheet();
            GUI.enabled = true;
        }

        EditorGUILayout.HelpBox(
            "Pulls data from the 'PlayerSkills' tab and updates selected PlayerSkillsSO assets.\n" +
            "Expected columns: Key, AnimationID, Cooldown, Damage, WeaponType, AreaType, RangeX, RangeY, RangeZ, Rarity, TargetSelf, IsTargetedGroundAOE, Buffs, Debuffs, Name, Description.\n" +
            "Lists use semicolons; escape literal semicolons as \\;.",
            MessageType.Info);
    }

    private bool ValidConfig()
    {
        return !string.IsNullOrWhiteSpace(_webAppUrl)
               && !string.IsNullOrWhiteSpace(_sharedSecret)
               && _sources.Count > 0;
    }

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

            // detail has an info header then the JSON body on next line
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

                    // Track which keys already exist in the SO.
                    // Anything left in here after applying the sheet will be deleted.
                    var existingKeys = new HashSet<string>(so.SkillDict.Keys);

                    foreach (var kv in map)
                    {
                        var key = kv.Key;
                        var r = kv.Value;

                        // If this key exists already, we keep it by removing from existingKeys set.
                        existingKeys.Remove(key);

                        // Ensure skill exists
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
                        skill.TargetSelf = r.TargetSelf;
                        skill.IsTargetedGroundAOE = r.IsTargetedGroundAOE;

                        // WeaponType enum ------------- NEW
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
                            skill.SkillRange.AreaType = area;
                        else if (!string.IsNullOrEmpty(areaStr))
                            Debug.LogWarning($"[SheetSync] Unknown AreaType '{r.AreaType}' for Key '{key}'");

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

                    // Delete any skills that are NOT present in the sheet
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
                Debug.Log($"[SheetSync] PULL OK (Player). Updated: {updated}, Created: {created}, Removed: {removed}");
                EditorUtility.DisplayDialog("PULL",
                    $"Updated: {updated}\nCreated: {created}\nRemoved: {removed}", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SheetSync] Parse error: {ex}");
                EditorUtility.DisplayDialog("PULL Error", ex.Message, "OK");
            }
        });
    }


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
#endif
