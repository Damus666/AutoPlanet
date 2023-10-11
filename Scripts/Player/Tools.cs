using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    [SerializeField] StateManager stateManager;
    [SerializeField] Inventory inventory;

    [SerializeField] float minDistance = 1f;
    [SerializeField] float minXDistance;
    [SerializeField] float rightCorrectionAngle = 45;
    [SerializeField] float leftCorrectionAngle = 135;
    [SerializeField] float runningOffset = 0.5f;

    [SerializeField] AudioSource clickSound;
    [SerializeField] CharacterController2D cc;
    [SerializeField] Player player;

    [SerializeField] Transform mainPivot;
    [SerializeField] Transform center;
    [SerializeField] GameObject toolSelection;

    [SerializeField] List<Transform> tools = new();
    [SerializeField] List<Transform> pivots = new();

    [SerializeField] List<float> rightCorrectionAngles = new();
    [SerializeField] List<float> leftCorrectionAngles = new();

    [SerializeField] Item laserSwordItem;
    [SerializeField] Item laserRayItem;

    public Transform currentTool = null;
    public Transform currentPivot = null;
    public int toolIndex = 0;
    public bool isSelecting = false;

    Camera mainCamera;
    Vector3 normalScaleGun = new(1.5f, 1.5f, 1);
    Vector3 invertedScaleGun = new(1.5f, -1.5f, 1);
    Vector3 normalScaleS = new(2f, 2f, 1);
    Vector3 invertedScaleS = new(2f, -2f, 1);

    bool cannotEquipWeapon { get { 
            return 
                (toolIndex == 4 && inventory.CountItem(laserSwordItem) <= 0) || 
                (toolIndex == 5 && inventory.CountItem(laserRayItem) <= 0); 
        } }

    void Start()
    {
        mainCamera = Camera.main;
        currentTool = tools[0];
        currentPivot = pivots[0];
        currentTool.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toolSelection.SetActive(!toolSelection.activeSelf);
            isSelecting = !isSelecting;
            clickSound.Play();
            if (isSelecting)
            {
                if (stateManager.inventoryOpen)
                {
                    inventory.Close();
                }
            }
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSelecting)
            {
                toolSelection.SetActive(false);
                isSelecting = false;
                clickSound.Play();
            }
        }
        if (isSelecting)
        {
            toolSelection.transform.position = mainCamera.WorldToScreenPoint(center.position);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectTool(-1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectTool(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectTool(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectTool(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectTool(3);
        } else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectTool(4);
        } else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectTool(5);
        }
        if (cannotEquipWeapon)
        {
            SelectTool(-1);
        }
        if (currentTool != null)
        {
            if (player.isRunning)
            {
                transform.localPosition = new Vector3(-runningOffset, 0, 0);
            } else
            {
                transform.localPosition = Vector3.zero;
            }
            Vector3 mousePos = Input.mousePosition;
            Vector3 toolPos = mainCamera.WorldToScreenPoint(currentTool.position);
            toolPos.z = 0;
            float angleAddition = 0;
            Vector3 mouseInWorld = mainCamera.ScreenToWorldPoint(mousePos);
            if (Vector3.Distance(mouseInWorld, currentTool.position) > minDistance && 
                Mathf.Abs(mouseInWorld.x-currentTool.position.x) > minXDistance)
            {
                Vector3 dir = mousePos - toolPos;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (cc.m_FacingRight)
                {
                    if (toolIndex == 2 || toolIndex == 1 || toolIndex == 5)
                    {
                        if (Mathf.Sign(mousePos.x - toolPos.x) == 1)
                        {
                            if (toolIndex == 1)
                            {
                                currentTool.localScale = normalScaleS;
                            }
                            else { currentTool.localScale = normalScaleGun; }
                        }
                        else
                        {

                            if (toolIndex == 1)
                            {
                                currentTool.localScale = invertedScaleS;
                                angleAddition = 90;
                            }
                            else
                            {
                                currentTool.localScale = invertedScaleGun;
                            }
                        }
                    }
                    currentTool.rotation = Quaternion.AngleAxis(angle - rightCorrectionAngles[toolIndex] + angleAddition, Vector3.forward);
                }
                else
                {
                    if (toolIndex == 2 || toolIndex == 1 || toolIndex == 5)
                    {
                        if (Mathf.Sign(mousePos.x - toolPos.x) == 1)
                        {

                            if (toolIndex == 1)
                            {
                                angleAddition = -90;
                                currentTool.localScale = invertedScaleS;
                            }
                            else
                            {
                                currentTool.localScale = invertedScaleGun;
                            }
                        }
                        else
                        {
                            if (toolIndex == 1)
                            {
                                currentTool.localScale = normalScaleS;
                            }
                            else { currentTool.localScale = normalScaleGun; }
                        }
                    }
                    currentTool.rotation = Quaternion.AngleAxis(angle - leftCorrectionAngles[toolIndex] + angleAddition, Vector3.forward);
                }
                Vector3 diff = currentPivot.position - mainPivot.position;
                if (diff.magnitude > 0)
                {
                    currentTool.position -= diff;
                }
            }
        }
    }

    public void SelectTool(int index)
    {
        if (index == -1 || cannotEquipWeapon)
        {
            foreach (Transform t in tools)
            {
                t.gameObject.SetActive(false);
            }
            currentPivot = null;
            currentTool = null;
        }
        else
        {
            foreach (Transform t in tools)
            {
                t.gameObject.SetActive(false);
            }
            currentPivot = pivots[index];
            currentTool = tools[index];
            currentTool.gameObject.SetActive(true);
        }
        clickSound.Play();
        toolIndex = index;
        toolSelection.SetActive(false);
        isSelecting = false;
    }
}
