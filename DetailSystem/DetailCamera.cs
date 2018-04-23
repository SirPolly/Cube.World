using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cube.World {
    public class DetailCamera : MonoBehaviour {
        public static List<DetailCamera> all = new List<DetailCamera>();
        public DetailLayer[] detailLayers;
        public new Camera camera {
            get;
            private set;
        }

        void Start() {
            camera = GetComponent<Camera>();
        }

        void OnEnable() {
            all.Add(this);
        }

        void OnDisable() {
            all.Remove(this);
        }
    }
}