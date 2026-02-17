using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.SceneManagement;

namespace fwp.halpers
{

	static public class HalperScene
	{

		static public bool isRuntimeSceneLoaded(string sceneName)
        {
			var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
			return scene.isLoaded;
		}

#if UNITY_EDITOR
		static public bool isEditorSceneLoaded(string sceneName)
		{
			var scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName(sceneName);
			return scene.isLoaded;
		}

		static public string getPathOfSceneInProject(string sceneName)
		{
			string[] guids = AssetDatabase.FindAssets("t:Scene");

			for (int i = 0; i < guids.Length; i++)
			{
				// Assets/Modules/module-a-b.unity
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);

				string pathSceneName = path.Substring(0, path.LastIndexOf("."));
				pathSceneName = pathSceneName.Substring(pathSceneName.LastIndexOf("/")+1);

				// module-a-b
				//Debug.Log(pathSceneName);

				if (pathSceneName == sceneName) return path;
			}
			return string.Empty;
		}
#endif

		static public void setupObjectChildOfSceneOfObject(GameObject target, GameObject owner)
		{

			//https://stackoverflow.com/questions/45798666/move-transfer-gameobject-to-another-scene
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(target, owner.scene);

		}

		static public Scene[] getOpenedScenesOfPrefix(string prefix, string filter)
		{
			List<Scene> list = new List<Scene>();
			int count = SceneManager.sceneCount;
			for (int i = 0; i < count; i++)
			{
				Scene sc = SceneManager.GetSceneAt(i);

				if (sc.name.Contains(filter)) continue;
				if (!sc.name.StartsWith(prefix)) continue;

				if (sc.isLoaded) list.Add(sc);
			}
			return list.ToArray();
		}

		/// <summary>
		/// active scene
		/// </summary>
		/// <param name="partOfSceneName"></param>
		/// <returns></returns>
		static public bool isActiveScene(string partOfSceneName)
		{
			Scene sc = SceneManager.GetActiveScene();
			if (sc.name.Contains(partOfSceneName)) return true;
			return false;
		}

		static public bool isActiveScene(Scene sc)
		{
			return sc == SceneManager.GetActiveScene();
		}

		static public Scene getSceneFromAdded(string sceneName)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sc = SceneManager.GetSceneAt(i);
				if (sc.isLoaded && sc.IsValid())
				{
					if (sc.name == sceneName) return sc;
				}
			}
			return default(Scene);
		}

		static public bool isSceneOpened(string sceneName)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sc = SceneManager.GetSceneAt(i);
				if (sc.name == sceneName) return true;
			}
			return false;
		}

		static public T getComponentInScene<T>(Scene sc, bool includeInactive = false) where T : Component
		{
			GameObject[] roots = sc.GetRootGameObjects();
			for (int i = 0; i < roots.Length; i++)
			{
				T output = roots[i].GetComponentInChildren<T>();
				if (output != null) return output;
			}
			return null;
		}

		static public T[] getComponentsInScene<T>(Scene sc, bool includeInactive = false) where T : Component
		{
			List<T> output = new List<T>();
			GameObject[] roots = sc.GetRootGameObjects();
			for (int i = 0; i < roots.Length; i++)
			{
				output.AddRange(roots[i].GetComponentsInChildren<T>(includeInactive));
			}
			return output.ToArray();
		}

		static public string[] getAllBuildSettingsScenes(bool removePath)
		{
			List<string> paths = new List<string>();

			//Debug.Log(SceneManager.sceneCountInBuildSettings);

			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string path = SceneUtility.GetScenePathByBuildIndex(i);

				if (removePath)
				{
					int slashIndex = path.LastIndexOf('/');

					if (slashIndex >= 0)
					{
						path = path.Substring(slashIndex + 1);
					}

					path = path.Remove(path.LastIndexOf(".unity"));

				}

				paths.Add(path);
			}

			return paths.ToArray();
		}

		static public bool checkIfCanBeLoaded(string sceneName)
		{
			string[] all = getAllBuildSettingsScenes(true);
			for (int i = 0; i < all.Length; i++)
			{
				if (all[i].Contains(sceneName)) return true;
			}
			return false;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Récupère un tableau des scènes/chemin d'accès qui sont présente dans les paramètres du build
		/// </summary>
		/// <param name="removePath">Juste le nom (myScene) ou tout le chemin d'accès (Assets/folder/myScene.unity) ?</param>
		/// <returns>Le tableau avec le nom ou chemin d'accès aux scènes.</returns>
		static public string[] getAllEditorBuildScenes(bool includeSceneOnly, bool removePath)
		{
			string[] scenes = new string[] { };

			if (includeSceneOnly)
			{
				scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
			}
			else
			{
				EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

				scenes = new string[buildScenes.Length];

				for (int i = 0; i < scenes.Length; i++)
				{
					scenes[i] = buildScenes[i].path;
				}
			}

			if (removePath)
			{
				for (int i = 0; i < scenes.Length; i++)
				{
					int slashIndex = scenes[i].LastIndexOf('/');

					if (slashIndex >= 0)
					{
						scenes[i] = scenes[i].Substring(slashIndex + 1);
					}

					scenes[i] = scenes[i].Remove(scenes[i].LastIndexOf(".unity"));
				}

				return scenes;
			}
			else return scenes;

		} // getAllBuildScenesNames()

		static public string getBuildSettingsSceneFullName(string partName)
		{
			if (partName.EndsWith(".unity")) partName = partName.Substring(0, partName.IndexOf(".unity"));

			string[] all = HalperScene.getAllBuildSettingsScenes(true); // no path
			for (int i = 0; i < all.Length; i++)
			{
				if (all[i].Contains(partName))
				{
					return all[i];
				}
			}
			return string.Empty;
		}

		static public string getBuildSettingsFullPathOfScene(string partName)
		{
			string fullName = getBuildSettingsSceneFullName(partName);
			string[] paths = getAllBuildSettingsScenes(false);
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i].Contains(fullName))
				{
					return paths[i];
				}
			}

			return string.Empty;
		}

		static public bool isSceneInBuildSettings(string partName, bool hardCheck = false)
		{

			string nm = getBuildSettingsSceneFullName(partName);
			if (nm.Length < 0) return false;

			if (hardCheck) return nm == partName;
			return true;
		}

		static public void addSceneToBuildSettings(string sceneName)
		{
			if (isSceneInBuildSettings(sceneName, true)) return;

			string assetPath = getSceneAssetFullPath(sceneName);

			//string fullName = getBuildSettingsSceneFullName(sceneName);

			List<EditorBuildSettingsScene> all = new List<EditorBuildSettingsScene>();
			all.AddRange(EditorBuildSettings.scenes);

			//string path = getBuildSettingsFullPathOfScene(sceneName);

			EditorBuildSettingsScene addScene = new EditorBuildSettingsScene(assetPath, true);
			all.Add(addScene);

			EditorBuildSettings.scenes = all.ToArray();
		}

		static public string getSceneAssetFullPath(string sceneName)
		{
			string fullName = getBuildSettingsSceneFullName(sceneName);

			string[] paths = getAssetScenesPaths();

			for (int i = 0; i < paths.Length; i++)
			{
				if (!paths[i].Contains(".unity")) continue;

				if (paths[i].Contains(sceneName)) return paths[i];
			}

			return string.Empty;
		}

		static public string[] getAssetScenesPaths()
		{
			string[] paths = AssetDatabase.FindAssets("t:Scene");

			if (paths.Length <= 0)
			{
				Debug.LogWarning("asking for scene but none ?");
			}

			//replace GUID by full path
			for (int i = 0; i < paths.Length; i++)
			{
				paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);
			}

			return paths;
		}

		static public string[] getAssetScenesNames(bool remExt = false)
		{
			string[] paths = getAssetScenesPaths();

			List<string> tmp = new List<string>();
			for (int i = 0; i < paths.Length; i++)
			{
				string scName = paths[i].Substring(paths[i].LastIndexOf("/") + 1);
				if (remExt && scName.IndexOf(".") > -1) scName = scName.Substring(0, scName.IndexOf("."));
				tmp.Add(scName);
			}

			return tmp.ToArray();
		}

#endif

	}

}
