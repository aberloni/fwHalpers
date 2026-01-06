using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace fwp.halpers.editor
{
	public class HalperEditor
	{

		/// <summary>
		/// % = ctrl
		/// # = shit
		/// & = alt
		/// </summary>

		[MenuItem("Tools/Clear console #&c")]
		public static void ClearConsole()
		{
			var assembly = Assembly.GetAssembly(typeof(SceneView));
			var type = assembly.GetType("UnityEditor.LogEntries");
			var method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
		}

		[MenuItem("Tools/pause %#&w")]
		public static void PauseEditor()
		{
			Debug.Log("PAUSE EDITOR");
			Debug.Break();
			//UnityEditor.EditorApplication.isPlaying = false;

		}

		/// <summary>
		/// shortcut to EditorGUIUtility.PingObject
		/// </summary>
		static public void pingObject(Object asset)
		{
			// Also flash the folder yellow to highlight it
			EditorGUIUtility.PingObject(asset);
		}

		/// <summary>
		/// use : EditorGUIUtility.PingObject
		/// </summary>
		static public void pingFolder(string assetsPath)
		{
			string path = "Assets/" + assetsPath;

			// Load object
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

			// Select the object in the project folder
			Selection.activeObject = obj;

			// Also flash the folder yellow to highlight it
			EditorGUIUtility.PingObject(obj);
		}

		static public T editor_draw_selectObject<T>(T instance = null, string overrideSelectLabel = "") where T : Component
		{
			if (instance == null)
			{
				instance = qh.gc<T>();
			}

			if (instance == null)
			{
				GUILayout.Label("can't find " + typeof(T) + " in scene");
				return null;
			}

			EditorGUILayout.ObjectField(typeof(T).ToString(), instance, typeof(T), true);
			if (GUILayout.Button(overrideSelectLabel + " " + typeof(T)))
			{
				Selection.activeGameObject = instance.gameObject;
			}

			return instance;
		}

		static public void editorCenterCameraToObject(GameObject obj)
		{
			GameObject tmp = Selection.activeGameObject;

			Selection.activeGameObject = obj;

			if (SceneView.lastActiveSceneView != null)
			{
				SceneView.lastActiveSceneView.FrameSelected();
			}

			if (tmp != null) Selection.activeGameObject = tmp;
		}


		/// <summary>
		/// get the sorting layer names<para/>
		/// from : http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
		/// </summary>
		/// <returns>The Sorting Layers (as string[])</returns>
		static public string[] getSortingLayerNames()
		{
			System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);

			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

			return (string[])sortingLayersProperty.GetValue(null, new object[0]);

		}// getSortingLayerNames()

		/// <summary>
		/// get the unique sorting layer IDs<para/>
		/// from : http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
		/// </summary>
		/// <returns>The sorting layer unique IDs (as int[])</returns>
		static public int[] getSortingLayerUniqueIDs()
		{
			System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);

			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);

			return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);

		}// getSortingLayerUniqueIDs()



		/// <summary>
		/// Retourne l'ID local d'un UnityEngine.Object dans une scène<para/>
		/// Viens de https://forum.unity3d.com/threads/how-to-get-the-local-identifier-in-file-for-scene-objects.265686/
		/// </summary>
		/// <param name="obj">L'objet cible.</param>
		/// <returns>L'ID de l'objet. Retourne 0 ou -1 si pas sauvegardé.</returns>
		static public int getLocalIdInFile(Object obj)
		{
			if (obj == null) return -1;

			PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

			SerializedObject serializeObject = new SerializedObject(obj);

			inspectorModeInfo.SetValue(serializeObject, InspectorMode.Debug, null);

			SerializedProperty propertyLocalID = serializeObject.FindProperty("m_LocalIdentfierInFile");

			int localID = propertyLocalID.intValue;

			inspectorModeInfo.SetValue(serializeObject, InspectorMode.Normal, null);

			return localID;

		}// getLocalIdInFile()

		/// <summary>
		/// meant to send left arrow event to hierarchy window
		/// </summary>
		/// <param name="count"></param>
		static public void upfoldNodeHierarchy(int count = 6)
		{

			if (!HalperPrefsEditor.isToggled(HalperPrefsEditor.ppref_editor_lock_upfold))
			{
				//Debug.Log("HalperEditor:upfolding NOK");
				return;
			}

			Debug.Log("HalperEditor:upfolding");

			//Debug.Log(EditorWindow.focusedWindow);

			EditorWindow targetWindow = null;
			SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));

			foreach (SearchableEditorWindow window in windows)
			{
				if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
				{
					targetWindow = window;
					break;
				}
			}

			//Debug.Log(targetWindow);

			if (targetWindow == null) Debug.LogWarning("no Hierarchy window ?");
			else
			{
				//targetWindow.Focus();

				//Debug.Log(targetWindow.titleContent.text);
				//Debug.Log(targetWindow + " = " + Selection.activeGameObject);

				/*
                // down arrow to select last of list
                Object candidate = GameObject.FindObjectOfType<GameObject>();
                Selection.activeObject = candidate;
                candidate = null;

                int safe = 999;
                while (Selection.activeObject != candidate && safe > 0)
                {
                    candidate = Selection.activeGameObject; // buff
                    Debug.Log("swap to " + candidate, candidate);

                    targetWindow.SendEvent(new Event { keyCode = KeyCode.DownArrow, type = EventType.KeyDown });
                    safe--;
                }
                Debug.Assert(safe > 0);

                Debug.Log(candidate);
                */

				// left arrows
				//Debug.Log($"sending x{count} left arrow to {targetWindow}");

				for (int i = 0; i < count; i++)
				{
					targetWindow.SendEvent(new Event { keyCode = KeyCode.LeftArrow, type = EventType.KeyDown });
				}
			}

			Selection.activeGameObject = null;
		}

	}

}
