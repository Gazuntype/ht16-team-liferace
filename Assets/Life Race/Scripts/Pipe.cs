using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour
{
    public float pipeRadius = 1f;
    public int pipeSegment = 10;

    public float ringDistance = 1f;

    public float minCurveRadius = 4f, maxCurveRadius = 20f;
    public int minCurveSegment = 4, maxCurveSegment = 10;

    public PipeItem ovum;
    public PipeItemGenerator[] obstacleGenerators;
    public PipeItemGenerator[] ciliaGenerators;

    int curveSegment = 20;
    public int CurveSegment
    {
        get { return curveSegment; }
    }

    float curveRadius = 4f;
    public float CurveRadius
    {
        get { return curveRadius; }
    }

    float curveAngle;
    public float CurveAngle
    {
        get { return curveAngle; }
    }

    float relativeRotation;
    public float RelativeRotation
    {
        get { return relativeRotation; }
    }


    Mesh mesh;
    MeshFilter meshFilter;

    Mesh colliderMesh;
    MeshFilter colliderMeshFilter;
    MeshCollider colliderMeshCollider;
    MeshCollider meshCollider;

    Vector3[] vertices, colliderVertices;
    Vector2[] uv;
    int[] triangles, colliderTrinagles;

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Pipe";
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshCollider = GetComponent<MeshCollider>();
    }

    public void Generate(bool with_obstacles, bool with_egg)
    {
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegment = Random.Range(minCurveSegment, maxCurveSegment + 1);

        mesh.Clear();

        SetupVertices();
        SetupUV();
        SetupTriangles();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
        //meshCollider.convex = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }


		if (with_obstacles && ciliaGenerators.Length > 0)
        {
            if (ciliaGenerators.Length > 0)
            {
                ciliaGenerators[Random.Range(0, ciliaGenerators.Length)].GenerateItem(this);
            }

            if (obstacleGenerators.Length > 0)
            {
                obstacleGenerators[Random.Range(0, obstacleGenerators.Length)].GenerateItem(this);
            }

        }

        if (with_egg)
        {
            float angleStep = curveAngle / curveSegment;
            PipeItem egg = Instantiate(ovum);
            int segment = Random.Range(0, curveSegment);
            egg.Position(this, segment * angleStep, new Vector3(360f/pipeSegment, 0f, 0f));
        }
    }

    void SetupVertices()
    {
        vertices = new Vector3[pipeSegment * curveSegment * 4];
        //<--float uStep = (2f * Mathf.PI) / curveSegment;
        float uStep = ringDistance / curveRadius;
        curveAngle = uStep * curveSegment * (360f / (Mathf.PI * 2f));
        CreateFirstQuadRing(uStep);

        int iDelta = pipeSegment * 4;
        for (int u = 2, i = iDelta; u <= curveSegment; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }

        mesh.vertices = vertices;
    }

    void SetupUV()
    {
        uv = new Vector2[vertices.Length];

        for (int i = 0; i < uv.Length; i += 4)
        {
            uv[i] = Vector2.zero;
            uv[i + 1] = Vector2.right;
            uv[i + 2] = Vector2.up;
            uv[i + 3] = Vector2.one;
        }

        mesh.uv = uv;
    }

    void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegment;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);

        //<--for (int v = 1; v <= pipeSegment; v++)
        for (int v = 1, i = 0; v <= pipeSegment; v++, i += 4)
        {
            //<--vertexA = GetPointOnTorus(0f, v * vStep);
            //<--vertexB = GetPointOnTorus(u, v * vStep);

            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / pipeSegment;

        int ringOffset = pipeSegment * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f);

        for (int v = 1; v <= pipeSegment; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    void SetupTriangles()
    {
        triangles = new int[pipeSegment * curveSegment * 6];

        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 2;
            triangles[t + 2] = triangles[t + 3] = i + 1;
            triangles[t + 5] = i + 3;
        }

        mesh.triangles = triangles;
    }

    Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 position;

        float radius = (curveRadius + pipeRadius * Mathf.Cos(v));
        position.x = radius * Mathf.Sin(u);
        position.y = radius * Mathf.Cos(u);
        position.z = pipeRadius * Mathf.Sin(v);

        return position;
    }

    public void AlignWith(Pipe p)
    {
        transform.localScale = Vector3.one;
        
        //<--relativeRotation = Random.Range(0f, 360f);
        //relativeRotation = Random.Range(0f, curveSegment) * 360f / pipeSegment;
        relativeRotation = Random.Range(0, curveSegment) * (360f / pipeSegment);

        transform.SetParent(p.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -p.curveAngle);

        transform.Translate(0f, p.curveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -curveRadius, 0f);

        transform.SetParent(p.transform.parent);
        transform.localScale = Vector3.one;
    }
}
