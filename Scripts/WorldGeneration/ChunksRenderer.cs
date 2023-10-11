using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksRenderer : MonoBehaviour
{
    [Header("Setup:")]
    public Transform player;
    public float RenderDistance;
    public Color MapSpawnLoadGizmo = Color.green;
    public float RefreshTime;
    [Header("Object List")]
    public List<GameObject> ObjectsPooled = new List<GameObject>();
    void Start(){
        StartCoroutine(waitfordeload());

    }

    IEnumerator waitfordeload(){
        yield return new WaitForSeconds(RefreshTime);
        for (int i = 0; i < ObjectsPooled.Count; i++)
        {
            float dist = Vector3.Distance(ObjectsPooled[i].transform.position, player.position);
            if(dist > RenderDistance * 10){
                if (ObjectsPooled[i].activeSelf)
                {
                    Chunk c = ObjectsPooled[i].GetComponent<Chunk>();
                    if (c.canDisable)
                    {
                        c.gameObject.SetActive(false);
                        c.DISABLE();
                    }
                }
            }else{
                if (!ObjectsPooled[i].activeSelf)
                {
                    Chunk c = ObjectsPooled[i].GetComponent<Chunk>();
                    c.ENABLE();
                    c.gameObject.SetActive(true);
                }
            }
        }
        StartCoroutine(waitfordeload());
    }


    void OnDrawGizmos()
    {
        Gizmos.color = MapSpawnLoadGizmo;
        Gizmos.DrawWireSphere(player.position, RenderDistance * 10);
    }
}
