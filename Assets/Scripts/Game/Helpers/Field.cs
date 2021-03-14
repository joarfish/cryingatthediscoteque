using System;
using UnityEngine;

namespace Game {
    
    [Serializable]
    public struct Field {
        public Field(int x, int z) {
            this.x = x;
            this.z = z;
        }

        public Field(Vector3 vector) {
            x = (int) vector.x;
            z = (int) vector.z;
        }

        public static Field Zero => Field.ZeroField;
        private static readonly Field ZeroField = new Field(0, 0);

        public Vector3 ToVector3(float y = 0.0f) {
            return new Vector3((float) this.x, y, (float) this.z);
        }

        public Field SetFromVector3(Vector3 vector) {
            x = (int) vector.x;
            z = (int) vector.z;
            return this;
        }

        public int x;
        public int z;
    }
}