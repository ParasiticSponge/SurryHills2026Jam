using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class GameManager : MonoBehaviour
{
    public GameObject entity;
    public List<Sprite> head;
    public List<Sprite> mask;
    public List<Sprite> top;
    public List<Sprite> bottom;
    public List<Sprite> hand;
    List<List<Sprite>> parts;

    GameObject player;

    float[] laneXPos = new float[] {-4.45f, -0.95f, 2.65f, 6.3f};
    float laneYPos = 6.6f;
    string[] partNames = new string[] { "head", "mask", "top", "bottom", "hand" };
    float[] partPositions = new float[] { 1.2f, 0.75f, -0.15f, -0.75f, 0 };

    public bool waveStart = false;
    int waveNum = 0;
    string waveSerial = "";
    int time = 120;
    int currentTime = 120;
    public float speed = 1f;

    private void Awake()
    {
        //Create a reference to the parts
        parts = new List<List<Sprite>> { head, mask, top, bottom, hand };
    }
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerBehaviour>().gameObject;
        StartCoroutine(GameLogic());
    }

    // Update is called once per frame
    void Update()
    {
        // Check if left mouse button was clicked down this frame
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse position from screen space to world space
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Cast a ray from the mouse position in any direction (Vector2.zero for a single point)
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

            // Check if the ray hit any collider
            if (hit.collider != null)
            {
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                player.transform.position = hit.collider.transform.position;
            }
        }
    }

    IEnumerator GameLogic()
    {
        yield return StartCoroutine(BeginWave());
        yield return StartCoroutine(SpawnLoop());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(3);
        waveStart = true;
    }

    IEnumerator BeginWave()
    {
        waveNum++;
        waveSerial = "";
        waveStart = true;
        for (int i = 0; i < partNames.Length; i++)
        {
            int randomiser = Random.Range(0, parts[i].Count);
            waveSerial += randomiser.ToString();
        }
        yield return 0;
    }

    IEnumerator SpawnLoop()
    {
        while (waveStart)
        {
            yield return StartCoroutine(SpawnEntity());
        }
    }
    IEnumerator SpawnEntity()
    {
        CreateEntity();

        int waitTime = Random.Range(2, 6);
        // Pause for the allotted time
        yield return new WaitForSeconds(waitTime);
    }

    void CreateEntity()
    {
        //Choose a random lane
        int randomLane = Random.Range(0, 4);
        //Create the main entity body
        GameObject entityObject = Instantiate(entity, new Vector3(laneXPos[randomLane], 6.6f, -1), Quaternion.identity);
        EntityBehaviour behaviour = entityObject.GetComponent<EntityBehaviour>();
        behaviour.speed = speed;
        //Create the parts for the entity, choosing random clothes, using the reference list
        for (int i = 0; i < partNames.Length; i++)
        {
            GameObject part = new GameObject();
            part.name = partNames[i];

            part.transform.parent = entityObject.transform;
            part.transform.localPosition = new Vector3(0, partPositions[i], -1.1f);
            SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();

            int randomiser = Random.Range(0, parts[i].Count);
            behaviour.serial += randomiser.ToString();
            renderer.sprite = parts[i][randomiser];
        }
        print(behaviour.serial);
    }
}
