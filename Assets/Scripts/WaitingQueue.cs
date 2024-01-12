using System;
using UnityEngine;

public class WaitingQueue<T>
{
    private readonly Vector3[] _waitingPositions;
    private readonly StackList<T> _waitingEntities;
    
    public int Count => _waitingEntities.Count;
    public StackList<T> Entities => _waitingEntities;

    public WaitingQueue(int queueSize, Vector3 queueOffset, Transform queueCheckoutPosition)
    {
        _waitingEntities = new StackList<T>();
        _waitingPositions = new Vector3[queueSize];
        for (int queuePosition = 0; queuePosition < queueSize; queuePosition++)
        {
            float rotation = queueCheckoutPosition.rotation.eulerAngles.y;
            Vector3 position = queueCheckoutPosition.position + (Quaternion.AngleAxis(rotation, Vector3.up) * queueOffset * queuePosition);
            _waitingPositions[queuePosition] = position;
        }
    }
    
    public void Add(T entity)
    {
        _waitingEntities.Push(entity);
    }
    
    public virtual void Remove(T entity)
    {
        _waitingEntities.Remove(entity);
    }
    
    public virtual T Shift()
    {
        T entity = _waitingEntities.Shift();
        return entity;
    }

    public Vector3 GetPosition(T entity)
    {
        int index = _waitingEntities.IndexOf(entity);
        return _waitingPositions[index];
    }

    public int GetPositionIndex(T entity)
    {
        return _waitingEntities.IndexOf(entity);
    }
}