using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldOfViewMeshGenerate : MonoBehaviour
{
    // Start is called before the first frame update
    public float sightAngle = 60f;
    public float sightRange = 8;
    private Mesh mesh;
    void Start()
    {
        GetComponent<MeshFilter>().mesh = Genrate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    Vector3 GetVectorFromAngle(float angle)
    {

        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.x, dir.z)*Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    Mesh Genrate()
    {
        Mesh mesh = new Mesh();

        Vector3 origin = Vector3.zero;
        float fov = sightAngle;
        int rayCount = 50;
        float angle = fov;
        float angleIncrease = fov / rayCount;
        float viewDistance = sightRange;


        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            //RaycastHit raycastHit;
            //Physics.Raycast(origin, GetVectorFromAngle(angle), out raycastHit, viewDistance);

            vertex = origin + GetVectorFromAngle(angle).normalized * viewDistance;
            //if (raycastHit.collider == null)
            //{
            //    vertex = origin + GetVectorFromAngle(angle).normalized * viewDistance;
            //}
            //else
            //{
            //    vertex = raycastHit.point;
            //}
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

#if UNITY_EDITOR
        var savePath = "Assets/" + "EnemySightMesh" + ".asset";
        Debug.Log("Saved Mesh to:" + savePath);
        AssetDatabase.CreateAsset(mesh, savePath);
#endif

        return mesh;
    }
        
}
