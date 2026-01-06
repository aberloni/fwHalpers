using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

static public class ExtScriptable
{

#if UNITY_EDITOR

	static public void renameScriptable(this ScriptableObject candidate, string newName)
	{
		if (candidate.name == newName)
		{
			//Debug.LogWarning("same name : " + newName);
			return;
		}

		Debug.Log("renaming scriptable : " + newName);
		string path = AssetDatabase.GetAssetPath(candidate.GetInstanceID());
		AssetDatabase.RenameAsset(path, newName);
		AssetDatabase.SaveAssets();
	}

	static public void flagAsDirty(this ScriptableObject candidate)
	{
		UnityEditor.EditorUtility.SetDirty(candidate);
	}

#endif
}
