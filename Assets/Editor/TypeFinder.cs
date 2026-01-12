using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

public static class TypeFinder
{
    public static void GetTypeAfterCompile(string fullTypeName, Action<Type> onFound)
    {
        void Handler(string asm, CompilerMessage[] msgs)
        {
            // Abort if compile errors
            if (msgs.Any(m => m.type == CompilerMessageType.Error))
                return;

            CompilationPipeline.assemblyCompilationFinished -= Handler;

            EditorApplication.delayCall += () =>
            {
                var t = FindType(fullTypeName);
                onFound?.Invoke(t);
            };
        }

        CompilationPipeline.assemblyCompilationFinished += Handler;
    }

    public static Type FindType(string fullName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetType(fullName);
            if (t != null)
                return t;
        }
        return null;
    }
}
