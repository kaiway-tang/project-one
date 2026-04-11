using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapBuilder : MonoBehaviour
{
    [SerializeField] ChunkInfo[] chunkInfos;
    [SerializeField] Tilemap mainTileMap;
    [SerializeField] int chunkWidth, chunkHeight;

    [SerializeField] ProcGenSegment procGenSegment;
    public static int up = 1, down = 2, left = 4, right = 8;
    // Start is called before the first frame update
    void Awake()
    {
        BuildChunkInfoDict();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Run()
    {
        for (int i = 0; i < procGenSegment.directionIDs.Length; i++)
        {
            InsertChunk(GetChunk(procGenSegment.directionIDs[i]), i % 4, i / 4);
        }
    }
    public void Clear()
    {
        mainTileMap.ClearAllTiles();
    }

    void InsertChunk(ChunkInfo chunkInfo, int cx, int cy)
    {
        Tilemap chunkTileMap = chunkInfo.tileMap;
        for (int tx = -chunkWidth/2; tx < chunkWidth/2; tx++)
        {
            for (int ty = -chunkHeight/2; ty < chunkHeight/2; ty++)
            {
                mainTileMap.SetTile(new Vector3Int(cx * chunkWidth + tx, cy * chunkHeight + ty, 0), chunkTileMap.GetTile(new Vector3Int(tx, ty, 0)));
            }
        }

        GameObject chunkObj = Instantiate(chunkInfo.gameObject, new Vector3(cx * chunkWidth, cy * chunkHeight, 0), Quaternion.identity);
        
        while (chunkObj.transform.childCount > 0)
        {
            chunkObj.transform.GetChild(0).parent = null;
        }
        Destroy(chunkObj);
    }

    ChunkInfo GetChunk(int directionID)
    {        
        return chunkInfoDict[directionID][Random.Range(0, chunkInfoDict[directionID].Count)];
    }

    Dictionary<int, List<ChunkInfo>> chunkInfoDict = new Dictionary<int, List<ChunkInfo>>();
    void BuildChunkInfoDict()
    {
        for(int i = 0; i < chunkInfos.Length; i++)
        {
            if (chunkInfos[i].disabled) {  continue; }
            chunkInfos[i].SetDirectionID();
            if (!chunkInfoDict.ContainsKey(chunkInfos[i].directionID))
            {
                chunkInfoDict.Add(chunkInfos[i].directionID, new List<ChunkInfo>());
            }
            chunkInfoDict[chunkInfos[i].directionID].Add(chunkInfos[i]);
        }
    }
}
