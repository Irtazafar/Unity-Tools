using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneBookmarkWindow : EditorWindow
{
    private List<SceneBookmark> bookmarks = new List<SceneBookmark>();

    [MenuItem("Tools/Scene Bookmarking")]
    public static void ShowWindow()
    {
        GetWindow<SceneBookmarkWindow>("Scene Bookmarking");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Bookmarking Tool", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Current Position as Bookmark"))
        {
            AddBookmark();
        }

        foreach (var bookmark in bookmarks)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(bookmark.name);

            if (GUILayout.Button("Go to"))
            {
                GoToBookmark(bookmark);
            }

            if (GUILayout.Button("Remove"))
            {
                RemoveBookmark(bookmark);
            }

            GUILayout.EndHorizontal();
        }
    }

    private void AddBookmark()
    {
        Camera sceneCam = SceneView.lastActiveSceneView.camera;
        if (sceneCam != null)
        {
            // Get the selected object in the scene
            GameObject selectedObject = Selection.activeGameObject;
            string bookmarkName;
            Vector3 bookmarkPosition = sceneCam.transform.position;

            if (selectedObject != null && IsObjectInView(sceneCam, selectedObject))
            {
                bookmarkName = selectedObject.name;
                bookmarkPosition = selectedObject.transform.position; // Use object's position for focusing
            }
            else
            {
                bookmarkName = "No Object Selected";
            }

            // Get the current active scene path
            string scenePath = SceneManager.GetActiveScene().path;

            bookmarks.Add(new SceneBookmark
            {
                name = bookmarkName,
                position = bookmarkPosition, // Save the object's position or camera position
                rotation = sceneCam.transform.rotation,
                scenePath = scenePath // Store the scene path
            });
        }
    }

    private void GoToBookmark(SceneBookmark bookmark)
    {
        // Get the current active scene path
        string currentScenePath = SceneManager.GetActiveScene().path;

        // If the current scene is not the bookmarked scene, load the bookmarked scene
        if (currentScenePath != bookmark.scenePath)
        {
            // Check for unsaved changes and prompt the user to save them
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(bookmark.scenePath);
            }
        }

        // After loading the scene, move the camera to the bookmarked position and adjust zoom
        SceneView sceneView = SceneView.lastActiveSceneView;
        sceneView.LookAt(bookmark.position); // Focus on the bookmarked object or position
        sceneView.Repaint();
    }

    private void RemoveBookmark(SceneBookmark bookmark)
    {
        bookmarks.Remove(bookmark);
    }

    // Helper method to check if the object is within the camera's view
    private bool IsObjectInView(Camera camera, GameObject obj)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(obj.transform.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }
}

public class SceneBookmark
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string scenePath; // New field to store the scene path
}
