using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling<T> where T : MonoBehaviour
{
    private readonly int _initialSize = 15;
    private T _prefab;
    private Transform _parent;
    private List<T> _objects;

    public ObjectPooling(T prefab, int initialSize = 15, Transform parent = null)
    {
        _prefab = prefab;
        _initialSize = initialSize;
        _parent = parent;
        _objects = new List<T>(_initialSize);

        for(int i = 0; i < _initialSize; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject()
    {
        var obj = GameObject.Instantiate(_prefab, _parent); 
        obj.gameObject.SetActive(false);
        _objects.Add(obj);
        return obj;
    }

    public T Get()
    {
        foreach(var obj in _objects)
        {
            if(!obj.isActiveAndEnabled)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        var created = CreateObject();
        created.gameObject.SetActive(true);
        return created;
    }

    public void Release(T obj)
    {
        obj.transform.parent = _parent;
        obj.gameObject.SetActive(false);
    }
}
