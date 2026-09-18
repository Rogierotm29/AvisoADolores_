using System.Collections.Generic;
using UnityEngine;


public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private float spawnX = 12f;
    [Tooltip("Altura del borde superior del suelo")]
    [SerializeField] private float groundY = -3.5f;
    [Tooltip("Segundos sin obstáculos al iniciar la partida")]
    [SerializeField] private float safeStartTime = 3f;
    [Tooltip("Segundos antes de la meta en los que ya no se generan obstáculos")]
    [SerializeField] private float finalStraightTime = 5f;
    [Tooltip("Pausa sin obstáculos después de cada segmento especial")]
    [SerializeField] private float breatherTime = 1f;

    private readonly Dictionary<GameObject, List<Chunk>> pool = new();
    private readonly HashSet<GameObject> invalidPrefabs = new();
    private float timer;
    private int lastSectionIndex = -1;
    private bool relaySpawned;
    private bool introPending;

    private void Start()
    {
        timer = safeStartTime;
    }

    private void Update()
    {
        if (!SectionManager.IsRunning) return;

        SectionManager sections = SectionManager.Instance;
        if (sections == null) return;
        if (sections.SecondsToGoal <= finalStraightTime) return;

        SectionData current = sections.Current;
        if (current == null) return;

        if (sections.CurrentIndex != lastSectionIndex)
        {
            lastSectionIndex = sections.CurrentIndex;
            relaySpawned = false;
            introPending = true;
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        GameObject prefab;
        bool special = false;
        if (introPending && current.introChunk == null) introPending = false;

        if (introPending && current.introChunk != null)
        {
            prefab = current.introChunk;
            introPending = false;
            special = true;
        }
        else if (!relaySpawned && current.relayChunk != null && sections.SectionProgress >= current.relayAt)
        {
            prefab = current.relayChunk;
            relaySpawned = true;
            special = true;
        }
        else if (current.chunks != null && current.chunks.Length > 0)
        {
            prefab = PickRandomChunk(current);
        }
        else
        {
            timer = 1f;
            return;
        }

        if (prefab == null)
        {
            timer = 1f;
            return;
        }

        Chunk chunk = GetFromPool(prefab);
        if (chunk == null)
        {
            timer = 1f;
            return;
        }

        chunk.transform.position = new Vector3(spawnX, groundY, 0f);
        chunk.gameObject.SetActive(true);

        float interval = Random.Range(current.minSpawnInterval, current.maxSpawnInterval);
        if (special) interval += breatherTime;
        float chunkTime = SectionManager.Speed > 0f ? chunk.Width / SectionManager.Speed : 0f;
        timer = interval + chunkTime;
    }

    private GameObject PickRandomChunk(SectionData section)
    {
  
        for (int i = 0; i < 5; i++)
        {
            GameObject candidate = section.chunks[Random.Range(0, section.chunks.Length)];
            if (candidate != null) return candidate;
        }
        return null;
    }

    private Chunk GetFromPool(GameObject prefab)
    {
        if (!pool.TryGetValue(prefab, out List<Chunk> list))
        {
            list = new List<Chunk>();
            pool[prefab] = list;
        }

        foreach (Chunk c in list)
        {
            if (!c.gameObject.activeSelf) return c;
        }

        GameObject go = Instantiate(prefab, transform);
        go.SetActive(false);
        Chunk chunk = go.GetComponent<Chunk>();

        if (chunk == null)
        {
            if (invalidPrefabs.Add(prefab))
            {
                Debug.LogError(
                    $"[ObstacleSpawner] El prefab '{prefab.name}' no tiene el componente Chunk en su raíz. " +
                    "Agrégaselo o quítalo de la lista del SectionData.", prefab);
            }
            Destroy(go);
            return null;
        }

        list.Add(chunk);
        return chunk;
    }
}