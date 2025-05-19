using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    [SerializeField] float CamSpeed=10f;
    [SerializeField] float ZoomSpeed=10f;
    [SerializeField] float minZoom = 4;
    [SerializeField] float maxZoom = 6;

    // Update is called once per frame
    void Update()
    {
         if (Input.GetMouseButton(1) && Input.mouseScrollDelta.magnitude > 0)
        {
            mainCam.orthographicSize += Input.mouseScrollDelta.y * Time.deltaTime * ZoomSpeed;
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minZoom, maxZoom);
        }

        else if (Input.GetMouseButton(1))
        {
            Vector3 deltaPos = Input.mousePositionDelta;
            deltaPos.Normalize();
            Debug.LogWarning(deltaPos);
            Vector3 cPos = mainCam.transform.position;
            cPos.x += -deltaPos.x * CamSpeed * Time.deltaTime;
            cPos.y += -deltaPos.y * CamSpeed * Time.deltaTime;
            mainCam.transform.position = cPos;
        }
       
    }
    public void ResetCam()
    {
        Vector3 cPos = mainCam.transform.position;
        cPos.x = 0;
        cPos.y = 0;
        mainCam.transform.position = cPos;
    }
}
