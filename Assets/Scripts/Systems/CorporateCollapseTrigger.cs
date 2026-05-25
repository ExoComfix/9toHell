using UnityEngine;
using System.Collections;
public class CorporateCollapseTrigger : MonoBehaviour
{
    [Header("🚨 Light Settings")]
    public UnityEngine.Rendering.Universal.Light2D globalLight;
    public Color collapseColor = new Color(0.8f, 0f, 0f, 1f);
    public float flickerSpeed = 0.15f;

    [Header("🎵 Audio Settings")]
    public AudioSource ambientAudioSource;
    public AudioClip sirenAlarmClip;

    private bool isCollapsing;
    private Color originalColor;

    private void OnEnable()
    {
        FloorManager.OnFloorStateChanged += HandleFloorStateChanged;
    }

    private void OnDisable()
    {
        FloorManager.OnFloorStateChanged -= HandleFloorStateChanged;
    }

    private void HandleFloorStateChanged(FloorManager.FloorState newState)
    {
        if (newState == FloorManager.FloorState.CorporateCollapse && !isCollapsing)
        {
            StartCollapsePresentation();
        }
        else if (newState == FloorManager.FloorState.Exploration)
        {
            StopCollapsePresentation();
        }
    }

    private void StartCollapsePresentation()
    {
        isCollapsing = true;
        Debug.LogError("[WORLD] Şirket iflasın eşiğinde! Çöküş sekansı başladı.");

        if (ambientAudioSource != null && sirenAlarmClip != null)
        {
            ambientAudioSource.clip = sirenAlarmClip;
            ambientAudioSource.loop = true;
            ambientAudioSource.volume = 0.6f;
            ambientAudioSource.Play();
        }

        if (globalLight != null)
        {
            originalColor = globalLight.color;
            StartCoroutine(LightFlickerRoutine());
        }
    }

    private void StopCollapsePresentation()
    {
        isCollapsing = false;
        StopAllCoroutines();
    }

    private IEnumerator LightFlickerRoutine()
    {
        while (isCollapsing)
        {
            globalLight.color = collapseColor;
            globalLight.intensity = 0.3f;
            yield return new WaitForSeconds(flickerSpeed);

            globalLight.intensity = 0.8f;
            yield return new WaitForSeconds(flickerSpeed * 2f);
        }
    }
}
