using UnityEditor;
using UnityEngine;

public class UnlitLighting : MonoBehaviour
{
    [MenuItem("GameObject/Light/Unlit SpotLight", false, 0)]
    private static void CreateUnitSpotLight(MenuCommand command)
    {
        GameObject obj = new GameObject("SpotLight");
        int layer = LayerMask.NameToLayer("Lighting");
        obj.layer = layer;

        Light light = obj.AddComponent<Light>();
        light.type = LightType.Spot;
        obj.AddComponent<UnlitLightingApplier>();

        GameObjectUtility.SetParentAndAlign(obj, command.context as GameObject);

        Undo.RegisterCreatedObjectUndo(obj, "Create SpotLight");
        Selection.activeObject = obj;

    }

    [MenuItem("GameObject/Light/Unlit PointLight", false, 1)]
    private static void CreateUnitPointLight(MenuCommand command)
    {
        GameObject obj = new GameObject("PointLight");
        int layer = LayerMask.NameToLayer("Lighting");
        obj.layer = layer;

        Light light = obj.AddComponent<Light>();
        light.type = LightType.Point;
        obj.AddComponent<UnlitLightingApplier>();

        GameObjectUtility.SetParentAndAlign(obj, command.context as GameObject);

        Undo.RegisterCreatedObjectUndo(obj, "Create PointLight");
        Selection.activeObject = obj;

    }

    [MenuItem("GameObject/Light/Unlit AreaLight", false, 2)]
    private static void CreateUnitAreaLight(MenuCommand command)
    {
        GameObject obj = new GameObject("AreaLight");
        int layer = LayerMask.NameToLayer("Lighting");
        obj.layer = layer;

        Light light = obj.AddComponent<Light>();
        light.type = LightType.Rectangle;
        obj.AddComponent<UnlitLightingApplier>();

        GameObjectUtility.SetParentAndAlign(obj, command.context as GameObject);

        Undo.RegisterCreatedObjectUndo(obj, "Create AreaLight");
        Selection.activeObject = obj;

    }

    [MenuItem("Tools/Add Unlit Lighting")]
    private static void AddLighting()
    {
        foreach(GameObject obj in Selection.gameObjects)
        {
            if(obj.GetComponent<UnlitLightingApplier>() == null)
            {
                obj.AddComponent<UnlitLightingApplier>();
            }

            int layer = LayerMask.NameToLayer("Lighting");
            obj.layer = layer;
        }
    }

    [MenuItem("Tools/AddUnlitLighting", true)]
    private static bool ValidateAddLighting()
    {
        return Selection.gameObjects.Length > 0;
    }
}
