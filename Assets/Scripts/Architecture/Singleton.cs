using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture {
    public class Singleton<T> where T : class {
        protected Singleton() { }

        private static T _instance = null;
        private static readonly object _instanceLock = new object();

        public static T getInstance() {
            if (_instance == null) {
                lock (_instanceLock) {
                    _instance = (T) System.Activator.CreateInstance(typeof(T), true);
                }
            }

            return _instance;
        }
    }
}