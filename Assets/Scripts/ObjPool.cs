using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ObjPool : ScriptableObject 
{
	public string objectName;
	private GameObject objectPrefab;
	private GameObject poolParent;
	public int totalObjects;
	public List<GameObject> activeObjects;
	public List<GameObject> inactiveObjects;
	private bool initialized;

	public void Init(int objCount, string prefabName)
	{
		if (!initialized)
		{
			objectName = prefabName;
			totalObjects = objCount;

			objectPrefab = Resources.Load<GameObject>(objectName);

			activeObjects = new List<GameObject>();
			inactiveObjects = new List<GameObject>();

			GameObject newObject;
			poolParent = new GameObject("[Object Pool]: " + prefabName);
			for (int i = 0; i < totalObjects; i++)
			{
				newObject = GameObject.Instantiate(objectPrefab) as GameObject;

				newObject.transform.SetParent(poolParent.transform);

				newObject.SetActive(false);

				inactiveObjects.Add(newObject);
			}

			initialized = true;

		}
	}

	public GameObject GetObject()
	{
		if (inactiveObjects.Count > 0)
		{
			activeObjects.Add(inactiveObjects[0]);
			GameObject next = inactiveObjects[0];

			inactiveObjects.RemoveAt(0);

			next.SetActive(true);
			return next;
		}
		else
		{
			return null;
		}
	}

	public void DeactivateObject(int index)
	{
		GameObject newlyInactive = activeObjects[index];

		newlyInactive.transform.SetParent(poolParent.transform);

		newlyInactive.SetActive(false);
		activeObjects.RemoveAt(index);
		inactiveObjects.Add(newlyInactive);
	}

	public void DeactivateAll()
	{
		for (int i = 0; i < activeObjects.Count; i++)
		{
			DeactivateObject(i);
		}
		
	}

}
