using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksGenerator : MonoBehaviour
{
    [Header("Basic Setup:")]
    public GameObject Player;
    public ChunksRenderer MapLoadingManager;
    public int ObjectDistance;
    public GameObject ObjectToSpawn;
    [Header("Extra settings:")]
    public int RenderRadious = 10;
    public Color MapSpawnDistanceGizmo = Color.blue;
    public bool RandomOffset;
    public Vector3 Minimumoffset; 
    public Vector3 Maximumoffset;
    public bool RandomRotate;

    private int XPlayerMove => (int)(Player.transform.position.x - StartPosition.x);
    private int ZPlayerMove => (int)(Player.transform.position.y - StartPosition.y);
    private int XPlayerLocation => (int)Mathf.Floor(Player.transform.position.x/ObjectDistance) * ObjectDistance;
    private int ZPlayerLocation => (int)Mathf.Floor(Player.transform.position.y/ObjectDistance) * ObjectDistance;

    private Hashtable tileobjs = new Hashtable();
    private Vector3 StartPosition;

    void Start(){
        if(Player == null){
            Debug.LogError("No player assigned to the Map manager.");
        }
        for(int x = -RenderRadious; x < RenderRadious; x++){
                for (int z = -RenderRadious; z < RenderRadious; z++)
                {
                    
                    Vector3 pos = new Vector3((x * ObjectDistance + XPlayerLocation), (z * ObjectDistance + ZPlayerLocation), 0);

                    if(!tileobjs.Contains(pos)){
                        GameObject OBJ = Instantiate(ObjectToSpawn, pos, Quaternion.identity);
                        if(RandomRotate){
                           OBJ.transform.Rotate(0,Random.Range(-360, 360), 0); 
                        }
                        if(RandomOffset){
                           OBJ.transform.position += new Vector3(Random.Range(Minimumoffset.x, Maximumoffset.x), Random.Range(Minimumoffset.y, Maximumoffset.y), Random.Range(Minimumoffset.z, Maximumoffset.z)); 
                        }
                        
                        tileobjs.Add(pos, OBJ);
                        OBJ.transform.parent = this.transform;
                        MapLoadingManager.ObjectsPooled.Add(OBJ);
                    }
                }
            }
    }
    void Update()
    {

        if(HasPlayerMoved()){
            for(int x = -RenderRadious; x < RenderRadious; x++){
                for (int z = -RenderRadious; z < RenderRadious; z++)
                {
                    
                    Vector3 pos = new Vector3((x * ObjectDistance + XPlayerLocation), (z * ObjectDistance + ZPlayerLocation), 0);
                    if(!tileobjs.Contains(pos)){
                        GameObject OBJ = Instantiate(ObjectToSpawn, pos, Quaternion.identity);
                        if(RandomRotate){
                           OBJ.transform.Rotate(0,Random.Range(-360, 360), 0); 
                        }
                        if(RandomOffset){
                           OBJ.transform.position += new Vector3(Random.Range(Minimumoffset.x, Maximumoffset.x), Random.Range(Minimumoffset.y, Maximumoffset.y), Random.Range(Minimumoffset.z, Maximumoffset.z)); 
                        }
                        tileobjs.Add(pos, OBJ);
                        OBJ.transform.parent = this.transform;
                        MapLoadingManager.ObjectsPooled.Add(OBJ);
                    }
                }
            }
        }
    }

    bool HasPlayerMoved(){
        if(Mathf.Abs(XPlayerMove) >= ObjectDistance || Mathf.Abs(ZPlayerMove) >= ObjectDistance){
            return true;
        }else{
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = MapSpawnDistanceGizmo;
        Gizmos.DrawWireSphere(Player.transform.position, RenderRadious * ObjectDistance);
    }
}
