using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    private float cameraShakeDuration = 0.25f;
    private float cameraShakeDecreaseFactor = 1f;
    private float cameraShakeAmount = .2f;

    public void StartCameraShake()
    {
        StartCoroutine(ShakeCamera());
    }

    IEnumerator ShakeCamera()
    {
        var originalPos = mainCamera.transform.localPosition;
        var duration = cameraShakeDuration;
        while (duration > 0)
        {
            mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * cameraShakeAmount;
            duration -= Time.deltaTime * cameraShakeDecreaseFactor;
            yield return null;
        }
        mainCamera.transform.localPosition = originalPos;
    }
}