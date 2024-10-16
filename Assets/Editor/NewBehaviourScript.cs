using UnityEditor;
using UnityEngine;

public class MyWindow : EditorWindow
{

    string myString = "CompleteProject";

    [MenuItem("Window / Export Entire Project")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Export complete Project", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Filename:", myString);

        if (GUILayout.Button("Export complete Project"))
            AssetDatabase.ExportPackage("Assets", myString + ". Unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies);
    }

}