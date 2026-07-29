using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AddUnityCLICube
{
    [MenuItem("Tools/Add UnityCLI Cube")]
    public static void Execute()
    {
        // Open the active scene (SampleScene as default)
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");

        // Create a cube named "UnityCLI"
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "UnityCLI";

        // Save the scene
        EditorSceneManager.SaveScene(scene);

        Debug.Log("Successfully added 'UnityCLI' cube to SampleScene.");
    }
}
