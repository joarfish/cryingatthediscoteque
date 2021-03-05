using UnityEngine;

namespace Game.Entities
{
    public class MoveOptions : MonoBehaviour
    {
        private Mesh _mesh;
        private Material _material;
        private float _timer = 0.0f;
        private int _rhythmTimePropertyIndex;
        private AnimationCurve _animationCurve;
    
        void Awake()
        {
            _mesh = new Mesh { name = "OptionsMesh" };
            gameObject.AddComponent<MeshFilter>().sharedMesh = _mesh;
        
            _material = new Material(Shader.Find("Unlit/MovementOptions"));
            gameObject.AddComponent<MeshRenderer>().material = _material;
            
            _rhythmTimePropertyIndex = Shader.PropertyToID("_RhythmTime");

            _animationCurve = new AnimationCurve();
            _animationCurve.AddKey(0.0f, 0.0f);
            _animationCurve.AddKey(0.25f, 0.125f);
            _animationCurve.AddKey(0.5f, 0.4f);
            _animationCurve.AddKey(0.75f, 0.75f);
            _animationCurve.AddKey(1.0f, 1.0f);

            CreateMesh();
        }

        void Update()
        {
            _timer = (_timer + Time.deltaTime) % 1.0f;
            _material.SetFloat(_rhythmTimePropertyIndex,_animationCurve.Evaluate(_timer / 1.0f));
        }

        private void CreateMesh()
        {
            var vertices = new Vector3[]
            {
                new Vector3(-0.333333f, 0.0f, 0.333333f), // 0
                new Vector3(0.333333f, 0.0f, 0.333333f), // 1
                new Vector3(-0.333333f, 0.0f, -0.333333f), // 2
                new Vector3(0.333333f, 0.0f, -0.333333f), // 3
                new Vector3(-0.333333f, 0.0f, 1.0f), // 4
                new Vector3(0.333333f, 0.0f, 1.0f), // 5
                new Vector3(1.0f, 0.0f, 0.333333f), // 6
                new Vector3(1.0f, 0.0f, -0.333333f), // 7
                new Vector3(0.333333f, 0.0f, -1.0f), // 8
                new Vector3(-0.333333f, 0.0f, -1.0f), // 9
                new Vector3(-1.0f, 0.0f, -0.333333f), // 10
                new Vector3(-1.0f, 0.0f, 0.333333f) // 11
            };


            var indices = new[]
            {
                4, 1, 0,
                4, 5, 1,
                1, 6, 7,
                1, 7, 3,
                2, 8, 9,
                2, 3, 8,
                11, 0, 2,
                11, 2, 10
            };
            _mesh.SetVertices(vertices);
            _mesh.triangles = indices;
        }
    }
}
