﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.DI
{
    public class ProjectContext : MonoBehaviour
    {
        private static ProjectContext _instance;

        private Dictionary<Type, object> _registrations;
        private Dictionary<Type, Func<object>> _lazyRegistrations;

        private static ProjectContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("ProjectContext").AddComponent<ProjectContext>();
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        public static void Bind<T>(T obj)
        {
            Instance.BindInternal(obj);
        }

        public static void BindLazy<T>(Func<T> func)
        {
            Instance.BindLazyInternal(func);
        }

        public static T GetInstance<T>()
        {
            return Instance.GetInstanceInternal<T>();
        }

        private void BindInternal<T>(T obj)
        {
            var type = typeof(T);
            CheckContains(type);

            if (_registrations == null)
            {
                _registrations = new Dictionary<Type, object>();
            }

            _registrations.Add(type, obj);
        }

        private void BindLazyInternal<T>(Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var type = typeof(T);
            CheckContains(type);

            if (_lazyRegistrations == null)
            {
                _lazyRegistrations = new Dictionary<Type, Func<object>>();
            }

            _lazyRegistrations.Add(type, () => func());
        }

        private T GetInstanceInternal<T>()
        {
            var type = typeof(T);
            if (_lazyRegistrations != null && _lazyRegistrations.ContainsKey(type))
            {
                var result = _lazyRegistrations[type]();
                _registrations.Add(type, result);
                _lazyRegistrations.Remove(type);
                return (T) result;
            }

            if (_registrations != null && _registrations.ContainsKey(type))
            {
                return (T) _registrations[type];
            }

            throw new ContainerException("Container doesn't contain type " + type);
        }

        private bool CheckContains(Type type) 
        {
            if (_registrations != null && _registrations.ContainsKey(type) ||
                _lazyRegistrations != null && _lazyRegistrations.ContainsKey(type))
            {
#if DEBUG
                Debug.LogWarning("Re-registration of type " + type);
#endif
                return true;
            }

            return false;
        }
    }

    public class ContainerException : Exception
    {
        public ContainerException(string message) : base(message)
        {
        }
    }
}