using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlatformDetails
{
    public string Name;
    public GameObject Prefab;
    public int Chanche;
}

public class PlatformGenerator : MonoBehaviour
{
    [Header("Level Size")]
    public float XAxis;    
    public int YLevelsMax;
    public int YLevelsMin;

    [Header("Platform Prefabs")]
    public List<PlatformDetails> NormalPlatformPrefabs;
    public List<PlatformDetails> SinglePlatformPrefabs;
    public List<PlatformDetails> BigPlatformPrefabs;

    [Header("Kode")]
    private int MakePlatformChancher;
    public List<GameObject> LastPlacedPlatforms;
    public GameObject WinScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var spriteRenderer = NormalPlatformPrefabs[0].Prefab.GetComponent<SpriteRenderer>();
        float platformWidth = spriteRenderer.bounds.size.x;


        MakePlatformChancher = 100;

        for (int y = YLevelsMin; y < YLevelsMax; y++)
        {
            int _rr = Random.Range(0, 100);

            if (_rr <= MakePlatformChancher)
            {
                if (MakePlatformChancher > 100)
                {
                    bool placed = false;
                    int attempts = 0;

                    List<GameObject> placedPlatforms = new List<GameObject>();

                    while (!placed && attempts < 10)
                    {
                        attempts++;

                        int _LastPlacedPlatformsIndex = Random.Range(0, LastPlacedPlatforms.Count);

                        float candidateX = Mathf.Clamp(LastPlacedPlatforms[_LastPlacedPlatformsIndex].transform.position.x + Random.Range(-10f, 11f), -XAxis, XAxis);

                        bool overlaps = false;

                        foreach (var platform in LastPlacedPlatforms)
                        {
                            if (Mathf.Abs(candidateX - platform.transform.position.x) < platformWidth - (platformWidth / 4))
                            {
                                overlaps = true;
                                break;
                            }
                        }

                        if (!overlaps)
                        {
                            PlatformDetails PD = GetRandomPlatform(BigPlatformPrefabs);

                            Vector2 spawnPos = new Vector2(candidateX, y);
                            GameObject Platform = Instantiate(PD.Prefab, spawnPos, Quaternion.identity);
                            placedPlatforms.Add(Platform);
                            placed = true;
                        }
                    }

                    UpdateLastPlacedPlatforms(placedPlatforms);

                }
                else
                {
                    int countThisRow = Random.Range(2, 3);

                    int single_rr = Random.Range(0, 10);

                    if (single_rr == 0)
                    {
                       countThisRow = 1;
                    }

                    List<GameObject> placedPlatforms = new List<GameObject>();

                    for (int i = 0; i < countThisRow; i++)
                    {
                        bool placed = false;
                        int attempts = 0;

                        while (!placed && attempts < 10)
                        {
                            attempts++;

                            int _LastPlacedPlatformsIndex = Random.Range(0, LastPlacedPlatforms.Count);

                            float candidateX = Mathf.Clamp(LastPlacedPlatforms[_LastPlacedPlatformsIndex].transform.position.x + Random.Range(-10f, 11f), -XAxis, XAxis);

                            bool overlaps = false;
                            foreach (var used in placedPlatforms)
                            {
                                if (Mathf.Abs(candidateX - used.transform.position.x) < platformWidth)
                                {
                                    overlaps = true;
                                    break;
                                }
                            }

                            foreach (var platform in LastPlacedPlatforms)
                            {
                                if (Mathf.Abs(candidateX - platform.transform.position.x) < platformWidth - (platformWidth / 4))
                                {
                                    overlaps = true;
                                    break;
                                }
                            }

                            if (!overlaps)
                            {
                                PlatformDetails PD = null;

                                if (countThisRow == 1)
                                {
                                    PD = GetRandomPlatform(SinglePlatformPrefabs);
                                }
                                else
                                {
                                    PD = GetRandomPlatform(NormalPlatformPrefabs);
                                }

                                Vector2 spawnPos = new Vector2(candidateX, y);
                                GameObject Platform = Instantiate(PD.Prefab, spawnPos, Quaternion.identity);
                                placedPlatforms.Add(Platform);
                                placed = true;
                            }
                        }
                    }

                    UpdateLastPlacedPlatforms(placedPlatforms);
                }

                MakePlatformChancher = 0;
            }
            else
            {
                MakePlatformChancher += 34;
            }
        }

        foreach (var item in LastPlacedPlatforms)
        {
            item.AddComponent<WinPlatform>();
        }
    }

    public PlatformDetails GetRandomPlatform(List<PlatformDetails> platforms)
    {
        int totalChance = 0;
        foreach (var platform in platforms)
        {
            totalChance += platform.Chanche;
        }

        int randomValue = Random.Range(0, totalChance);
        int cumulative = 0;

        foreach (var platform in platforms)
        {
            cumulative += platform.Chanche;
            if (randomValue < cumulative)
            {
                return platform;
            }
        }

        return platforms[0]; 
    }


    private void UpdateLastPlacedPlatforms(List<GameObject> _placedPlatforms)
    {
        if (_placedPlatforms.Count > 0)
        {
            LastPlacedPlatforms.Clear();
            LastPlacedPlatforms = new List<GameObject>(_placedPlatforms);
        }
    }

        

}
