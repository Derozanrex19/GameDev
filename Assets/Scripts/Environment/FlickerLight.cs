using System.Collections;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minIntensity = 0.3f;
    [SerializeField] private float maxIntensity = 1.1f;
    [SerializeField] private float minDelay = 0.03f;
    [SerializeField] private float maxDelay = 0.15f;

    private void Reset()
    {
        targetLight = GetComponent<Light>();
    }

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            if (targetLight != null)
                targetLight.intensity = Random.Range(minIntensity, maxIntensity);

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }
}
