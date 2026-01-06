using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.halpers
{

	static public class HalperTransform
	{
		static public string getFullTransformHierarchyPathToString(this Transform tr)
		{
			string path = "";
			while (tr != null)
			{
				path = tr.name + "/" + path;
				tr = tr.parent;
			}
			return path;
		}

		/// <summary>
		/// Permet de virer tout enfants et component (sauf le transform)
		/// </summary>
		public static void cleanTransform(Transform tr)
		{

			//HierarchyAnimatorHighlighter.ShowIcon(!HierarchyAnimatorHighlighter.ShowIcon());
			if (tr == null) return;

			Debug.Log("cleaning " + tr.name);

			//remove all children
			while (tr.childCount > 0)
			{
				GameObject.Destroy(tr.GetChild(0).gameObject);
			}

			SpriteRenderer[] renders = tr.GetComponents<SpriteRenderer>();
			foreach (SpriteRenderer render in renders) { GameObject.Destroy(render); }

			Collider[] colliders = tr.GetComponents<Collider>();
			foreach (Collider collider in colliders) { GameObject.Destroy(collider); }


		}


		static public Transform[] getAllTransform(GameObject[] list)
		{
			List<Transform> tmp = new List<Transform>();
			for (int i = 0; i < list.Length; i++)
			{
				tmp.AddRange(getAllTransform(list[i].transform));
			}
			return tmp.ToArray();
		}

		static public Transform[] getAllTransform(Transform t)
		{
			List<Transform> trs = new List<Transform>();
			trs.Add(t);
			foreach (Transform child in t)
			{
				if (child.childCount > 0)
				{
					Transform[] children = getAllTransform(child);
					//for (int i = 0; i < children.Length; i++) Debug.Log(child.name+" >> "+children[i].name);
					trs.AddRange(children);
				}
				else
				{
					trs.Add(child);
				}
			}
			return trs.ToArray();
		}

		static public Transform fetchInChildren(Transform parent, string partName, bool strict = false, bool toLowercase = false)
		{
			foreach (Transform t in parent)
			{
				string nm = t.name;
				if (toLowercase) nm = nm.ToLower();

				if (strict)
				{
					if (nm == partName) return t;
				}
				else
				{
					if (nm.IndexOf(partName) > -1) return t;
				}

				Transform child = fetchInChildren(t, partName, strict, toLowercase);
				if (child != null) return child;
			}
			return null;
		}


		static public bool isInChildren(Transform parent, Transform target)
		{
			bool isIn = false;

			if (parent == target) isIn = true;

			if (!isIn)
			{
				foreach (Transform child in parent)
				{
					if (isIn) continue;

					if (child == target) isIn = true;

					if (child.childCount > 0) isIn = isInChildren(child, target);
				}
			}

			return isIn;
		}



		static public Transform[] findSameChildren(Transform transformOrigin, Transform transformFilter)
		{
			List<Transform> sameChildrenOrigin = new List<Transform>();

			foreach (Transform childOrigin in transformOrigin)
			{
				foreach (Transform childFilter in transformFilter)
				{
					if (string.Compare(childOrigin.name, childFilter.name) == 0)
					{
						sameChildrenOrigin.Add(childOrigin);
					}
				}
			}

			return sameChildrenOrigin.ToArray();

		}// findSameChildren()

		static public Transform findChild(Transform parent, string endName)
		{
			if (parent.name.EndsWith(endName)) return parent;

			if (parent.childCount > 0)
			{
				for (int i = 0; i < parent.childCount; i++)
				{
					Transform child = findChild(parent.GetChild(i), endName);
					if (child != null) return child;
				}
			}

			return null;
		}

	}

}