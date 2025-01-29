using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPatchAndRunCommand
{
    // Add a new menu item under "Build" with a shortcut key
    [MenuItem("Tools/Patch And Run %g")] // %g is the shortcut key
    public static void PatchAndRun()
    {
        // Automatically include all scenes currently open in the build
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        // Set build settings
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = "YourBuildPath/YourGame.apk", // Adjust the path and file name as necessary
            target = BuildTarget.Android, // Change this depending on your target device
            options = BuildOptions.AutoRunPlayer // Automatically run on device after build
        };

        // Build and run on connected device
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Build completed: " + report.summary.result);
    }
}
