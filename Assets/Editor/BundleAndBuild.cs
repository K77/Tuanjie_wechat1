using UnityEngine;
using UnityEditor;

public class BundleAndBuild
{
    [MenuItem("Assets/Build with Bundle/Build")]
    static void Build()
    {
		AssetBundles.BuildScript.BuildStandalonePlayer();
	}

	[MenuItem("Assets/Build with Bundle/Build and Run")]
	static void BuildAndRun()
	{
		AssetBundles.BuildScript.BuildStandalonePlayer(true);
	}
}
