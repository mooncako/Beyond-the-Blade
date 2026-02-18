using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public static class DerivedComponentGenerator
{
    /// <summary>
    /// Creates a new C# script that derives from baseType.
    /// </summary>
    public static UnityEngine.Object CreateDerivedScriptOnly(
        Type baseType,
        string className,
        string outputFolder,
        string @namespace = null,
        string script = "",
        bool pingAndSelect = true)
    {
        ValidateInputs(baseType, className, outputFolder);

        Directory.CreateDirectory(outputFolder);

        string filePath = Path.Combine(outputFolder, $"{className}.cs");
        if (File.Exists(filePath))
            throw new IOException($"Script already exists: {filePath}");

        string scriptText = BuildScriptText(@namespace, className, baseType.FullName, script);
        File.WriteAllText(filePath, scriptText);
        AssetDatabase.Refresh();

        var assetPath = ToAssetPath(filePath);
        var scriptAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

        if (pingAndSelect && scriptAsset != null)
        {
            Selection.activeObject = scriptAsset;
            EditorGUIUtility.PingObject(scriptAsset);
        }

        return scriptAsset;
    }


    /// <summary>
    /// Generates a new C# script that inherits from baseType (must be a Monobehaviour),
    /// triggers compilation, then adds the resulting component type to target gameobject.
    /// </summary>
    public static void CreateDerivedAndAddComponent(
        GameObject target, 
        Type baseType,
        string className,
        string outputFolder,
        string @namespace = null,
        string script = ""
        )
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (baseType == null) throw new ArgumentNullException(nameof(baseType));

        if (!typeof(MonoBehaviour).IsAssignableFrom(baseType))
            throw new ArgumentException($"Base type must derive from MonoBehaviour. Type {baseType.FullName} does not.", nameof(baseType));

        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("className is empty or whitespace", nameof(className));
        
        if (string.IsNullOrEmpty(outputFolder))
            throw new ArgumentException("outputFolder is null or empty", nameof(outputFolder));
        
        Directory.CreateDirectory(outputFolder);

        string filePath = Path.Combine(outputFolder, $"{className}.cs");
        if(File.Exists(filePath))
            throw new IOException($"File already exists at path: {filePath}");
        
        string baseTypeName = baseType.FullName;
        string scriptText = BuildScriptText(@namespace, className, baseTypeName, script);

        File.WriteAllText(filePath, scriptText);
        AssetDatabase.Refresh();

        void TryAdd()
        {
            var derivedType = FindTypeByName(className, @namespace);
            if(derivedType == null)
            {
                Debug.LogWarning($"Compiled, but could not find type '{FormatFullName(@namespace, className)}'." +
                                 $"Check class name/namespace match the file.");
                return;
            }

            if(!typeof(MonoBehaviour).IsAssignableFrom(derivedType))
            {
                Debug.LogWarning($"Type '{derivedType.FullName}' is not a MonoBehaviour; cannot add to GameObject.");
                return;
            }

            Undo.AddComponent(target, derivedType);
            EditorUtility.SetDirty(target);
            Selection.activeGameObject = target;
        }

        CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;
        CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;

        void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            if (messages != null && messages.Any(m => m.type == CompilerMessageType.Error))
                return;
            
            CompilationPipeline.assemblyCompilationFinished -= OnAssemblyCompilationFinished;

            EditorApplication.delayCall += TryAdd;
        }

        CompilationPipeline.RequestScriptCompilation();
    }

    private static void ValidateInputs(Type baseType, string className, string outputFolder)
    {
        if (baseType == null) throw new ArgumentNullException(nameof(baseType));
        if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("className is null/empty", nameof(className));
        if (!IsValidIdentifier(className)) throw new ArgumentException($"'{className}' is not a valid C# identifier", nameof(className));
        if(string.IsNullOrWhiteSpace(outputFolder)) throw new ArgumentException("outputFolder is null/empty", nameof(outputFolder));
    }

    private static string BuildScriptText(string ns, string className, string baseTypeFullName, string script)
    {
        string baseType = $"{baseTypeFullName}";

        if(string.IsNullOrEmpty(ns))
        {
            if(string.IsNullOrEmpty(script))
            {
                return
$@"using UnityEngine;
public class {className} : {baseType}
{{
    // TODO: add overrides / fields here
}}
";
            }
            else
            {
                return script;
            }
            
        }
        if(string.IsNullOrEmpty(script))
        {
        return
$@"using UnityEngine;
namespace {ns}
{{
    public class {className} : {baseType}
    {{
        // TODO: add overrides / fields here
    }}
}}
";
        }
        else
        {
            return script;
        }
    }

    private static Type FindTypeByName(string className, string ns)
    {
        string fullname = FormatFullName(ns, className);

        var t = Type.GetType(fullname);
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = asm.GetType(fullname);
            if(t != null) return t;
        }

        return null;
    }

    private static string FormatFullName(string ns, string className)
        => string.IsNullOrWhiteSpace(ns) ? className : $"{ns}.{className}";
    
    private static bool IsValidIdentifier(string s)
    {
        if(string.IsNullOrEmpty(s))
            return false;
        
        if (!(char.IsLetter(s[0]) || s[0] == '_'))
            return false;
        for (int i = 1; i < s.Length; i++)
        {
            if(!(char.IsLetterOrDigit(s[i]) || s[i] == '_'))
                return false;
        }
        return true;
    }

    private static string ToAssetPath(string absoluteOrRelativePath)
    {
        // If already "Assets/..", keep it
        if (absoluteOrRelativePath.Replace('\\','/').StartsWith("Assets/"))
            return absoluteOrRelativePath.Replace('\\','/');

        // Convert absolute path under project to "Assets/.."
        var projectPath = Directory.GetParent(Application.dataPath)?.FullName?.Replace('\\','/');
        var p = absoluteOrRelativePath.Replace('\\','/');

        if (!string.IsNullOrEmpty(projectPath) && p.StartsWith(projectPath))
            return p.Substring(projectPath.Length + 1);

        return p;
    }
}
