using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceList", menuName = "Scriptable Objects/PieceList")]
public class PieceList : ScriptableObject, IEnumerable<GameObject>
{
    [SerializeField] private GameObject[] piecePrefabs;

    public GameObject this[int i]
    {
        get => piecePrefabs[i];
    }

    public IEnumerator<GameObject> GetEnumerator()
    {
        foreach (GameObject o in piecePrefabs)
        {
            yield return o;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
