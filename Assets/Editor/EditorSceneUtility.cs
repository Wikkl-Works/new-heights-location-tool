using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EditorSceneUtility
{
    static EditorSceneUtility()
    {
        EditorApplication.update += Update;
    }

    /// <summary>
    /// Saved active scene name, because it's not accessible outside the main thread.
    /// </summary>
    public static string ActiveSceneName { get; private set; }

    public static void Update()
    {
        ActiveSceneName = SceneManager.GetActiveScene().name;
    }
}
