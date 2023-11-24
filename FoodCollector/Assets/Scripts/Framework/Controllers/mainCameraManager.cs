using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainCameraManager : MonoBehaviour
{

    public Camera camera;

    private Vector3 mainPositionShift = new Vector3(11.31f, 3.4f, -12.6f);
    private Quaternion mainRotationShift = Quaternion.Euler(-7f, -6.5f, -1.7f);

    private Vector3 cameraPositionSpace = new Vector3(20.06f, 18.57f, 9.65f);
    private Quaternion cameraRotationSpace = Quaternion.Euler(60.816f, -88.055f, 1.942f);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TransitionCameraView(camera.transform.position, camera.transform.rotation, cameraPositionSpace, cameraRotationSpace, 2.0f));
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
        {
            StartCoroutine(TransitionCameraView(camera.transform.position, camera.transform.rotation, mainPositionShift, mainRotationShift, 2.0f));
        }
    }

   /// <summary>
    /// Move the camera of position and rotation
    /// </summary>
    /// <param name="startCamPosition"></param>
    /// <param name="startCamRotation"></param>
    /// <param name="endCamPosition"></param>
    /// <param name="endCamRotation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator TransitionCameraView(Vector3 startCamPosition, Quaternion startCamRotation, Vector3 endCamPosition, Quaternion endCamRotation, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            camera.transform.position = Vector3.Lerp(startCamPosition, endCamPosition, timeElapsed / duration);
            camera.transform.rotation = Quaternion.Lerp(startCamRotation, endCamRotation, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final camera position and rotation are set exactly to the target values
        camera.transform.position = endCamPosition;
        camera.transform.rotation = endCamRotation;
    }
}
