using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public GameObject terrainPrefab;
    Terrain terrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrain = terrainPrefab.GetComponent<Terrain>();
        // spawn literally 32x32 of these fricking things from -1600 in a grid pattern below us
        for (int i = -16; i < 16; i++)
        {
            for (int j = -16; j < 16; j++)
            {
                GameObject.Instantiate(terrainPrefab, new Vector3(i * terrain.terrainData.size.x, -1600, j * terrain.terrainData.size.z), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
