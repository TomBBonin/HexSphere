using System;
using UnityEngine;

public abstract class Service<TService> : MonoBehaviour where TService : Service<TService>
{
    private static TService _instance;

    public static TService I { get { return _instance; } }

    protected void Awake()
    {
        if (_instance != null)
            throw new InvalidOperationException();
        _instance = this as TService;

        InitService();
    }

    protected abstract void InitService();
}