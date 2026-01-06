using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class ExtList
{

	/// <summary>
	/// shuffle list of Object
	/// using random.range
	/// https://medium.com/@occasoftware/how-to-shuffle-a-list-in-unity-267940dba432
	/// </summary>
	static public List<Object> shuffle<Object>(this List<Object> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Object temp = list[i];
			int randomIndex = Random.Range(i, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
		return list;
	}

}
