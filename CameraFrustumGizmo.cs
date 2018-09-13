using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFrustumGizmo : MonoBehaviour
{
    [ColorUsage(false)]
    public Color sphereColor;
    public float spheresRadius = 0.5f;

    Camera camera;

    GameObject sphereBottomLeft;
    GameObject sphereBottomRight;
    GameObject sphereTopLeft;
    GameObject sphereTopRight;
    GameObject sphereCenter;

    private Vector3 bottomLeftPosition_Screen;
    private Vector3 bottomRightPosition_Screen;
    private Vector3 topRightPosition_Screen;
    private Vector3 topLeftPosition_Screen;
    private Vector3 centerPosition_Screen;
       
    public void Start()
    {
        camera = GetComponent<Camera>();
        CreatePrimitives();
    }

    private void Update()
    {
        DrawGameViewGizmos();
        DrawVisibleArea();
    }

    public virtual void OnDrawGizmos()
    {
        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        camera = GetComponent<Camera>();
        if (camera.orthographic)
        {
            float spread = camera.farClipPlane - camera.nearClipPlane;
            float center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
            Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
        }
        else
        {
            Gizmos.DrawFrustum(new Vector3(0, 0, (camera.nearClipPlane)), camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
        }
        Gizmos.matrix = temp;
    }

    void CreatePrimitives()
    {
        Vector3 size = new Vector3(spheresRadius, spheresRadius, spheresRadius);
        Material color = new Material(Shader.Find("Unlit/Color"));
        color.color = sphereColor;

        sphereBottomLeft = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereBottomLeft.transform.localScale = size;
        sphereBottomLeft.GetComponent<Renderer>().material = color;

        sphereBottomRight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereBottomRight.transform.localScale = size;
        sphereBottomRight.GetComponent<Renderer>().material = color;

        sphereTopLeft = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereTopLeft.transform.localScale = size;
        sphereTopLeft.GetComponent<Renderer>().material = color;

        sphereTopRight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereTopRight.transform.localScale = size;
        sphereTopRight.GetComponent<Renderer>().material = color;

        sphereCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereCenter.transform.localScale = size;
        sphereCenter.GetComponent<Renderer>().material = color;

    }

    private static Vector3 GetPointAtHeight(Ray ray, float height)
    {
        if (ray.direction.y != 0)
        {
            return ray.origin + (((ray.origin.y - height) / -ray.direction.y) * ray.direction);
        }
        else
        {
            print("Invalid division by zero in GetPointAtHeight");
            return Vector3.zero;
        }
    }

    void DrawGameViewGizmos()
    {
        //create vector3 pos based on rays intersection with ground (0)
        Ray bottomLeft = camera.ViewportPointToRay(new Vector3(0, 0, 0));
        bottomLeftPosition_Screen = GetPointAtHeight(bottomLeft, 0);
        sphereBottomLeft.transform.position = bottomLeftPosition_Screen;

        Ray bottomRight = camera.ViewportPointToRay(new Vector3(1, 0, 0));
        bottomRightPosition_Screen = GetPointAtHeight(bottomRight, 0);
        sphereBottomRight.transform.position = bottomRightPosition_Screen;

        Ray topRight = camera.ViewportPointToRay(new Vector3(1, 1, 0));
        topRightPosition_Screen = GetPointAtHeight(topRight, 0);
        sphereTopRight.transform.position = topRightPosition_Screen;

        Ray topLeft = camera.ViewportPointToRay(new Vector3(0, 1, 0));
        topLeftPosition_Screen = GetPointAtHeight(topLeft, 0);
        sphereTopLeft.transform.position = topLeftPosition_Screen;

        Ray center = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        centerPosition_Screen = GetPointAtHeight(center, 0);
        sphereCenter.transform.position = centerPosition_Screen;
    }

    void DrawVisibleArea()
    {
        Debug.DrawRay(bottomLeftPosition_Screen, bottomRightPosition_Screen - bottomLeftPosition_Screen, Color.red); // BL -> BR
        Debug.DrawRay(bottomRightPosition_Screen, topRightPosition_Screen - bottomRightPosition_Screen, Color.red); // BR -> TR
        Debug.DrawRay(topRightPosition_Screen, topLeftPosition_Screen - topRightPosition_Screen, Color.red); // TR -> TL
        Debug.DrawRay(topLeftPosition_Screen, bottomLeftPosition_Screen - topLeftPosition_Screen, Color.red); //TL -> BL
    }

}