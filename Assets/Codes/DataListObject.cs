using UnityEngine;
using System.Collections.Generic;
public class DataListObject <T>
{
    private GameObject prefab;
    private Transform parent;
    private Dictionary<int, T> dictionary = new Dictionary<int, T>();
    private Queue<T> pool = new Queue<T>();
    
}