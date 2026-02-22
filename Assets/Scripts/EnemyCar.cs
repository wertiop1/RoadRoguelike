using UnityEngine;

public enum CarType { Faster, Slower }

[RequireComponent(typeof(SpriteRenderer))]
public class TrafficCar : MonoBehaviour
{
    [Header("Identity")]
    public CarType carType = CarType.Faster;
    public Sprite[] sprites;           

    [Header("Speed Offsets (relative to player speed)")]
    [Tooltip("How much FASTER than the player this car goes (Faster type)")]
    public float overtakeOffset = 3f;  

    [Tooltip("How much SLOWER than the player this car goes (Slower type)")]
    public float blockerOffset  = 3f;  

    [Header("Despawn")]
    [Tooltip("Destroy when this many units behind the camera's bottom edge")]
    public float despawnBehindDistance = 5f;
    [Tooltip("Destroy when this many units ahead of the camera's top edge")]
    public float despawnAheadDistance  = 5f;
    [HideInInspector] public float playerSpeed;

    private SpriteRenderer sr;
    private Camera cam;

    void Awake()
    {
        sr  = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        if (sprites != null && sprites.Length > 0)
            sr.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    void Update()
    {
        Move();
        CheckDespawn();
    }

    void Move()
    {
        float speed = carType == CarType.Faster
            ? playerSpeed + overtakeOffset
            : Mathf.Max(0f, playerSpeed - blockerOffset);

        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.World);
    }

    void CheckDespawn()
    {
        if (cam == null) return;

        float camHalfH  = cam.orthographicSize;
        float camCenterY = cam.transform.position.y;
        float topEdge    = camCenterY + camHalfH + despawnAheadDistance;
        float botEdge    = camCenterY - camHalfH - despawnBehindDistance;

        float y = transform.position.y;
        if (y > topEdge || y < botEdge)
            Destroy(gameObject);
    }
}