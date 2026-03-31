using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class FootstepAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;
    [SerializeField] private float minMoveDistancePerSecond = 0.1f;
    [SerializeField] private string resourcesPath = "Footsteps";
    [SerializeField] private float volume = 1f;
    [SerializeField] private float playOneShotVolumeScale = 3f;
    [SerializeField] private bool debugLogs = true;

    private CharacterController controller;
    private SimpleFirstPersonController playerController;
    private AudioSource audioSource;
    private float stepTimer;
    private Vector3 lastPosition;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerController = GetComponent<SimpleFirstPersonController>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = volume;

        if ((footstepClips == null || footstepClips.Length == 0) && !string.IsNullOrWhiteSpace(resourcesPath))
        {
            footstepClips = Resources.LoadAll<AudioClip>(resourcesPath)
                .OrderBy(clip => clip.name)
                .ToArray();
        }

        lastPosition = transform.position;

        if (debugLogs)
        {
            Debug.Log($"FootstepAudio loaded {footstepClips.Length} clips on {name}.", this);
        }
    }

    private void Update()
    {
        Vector3 horizontalDelta = transform.position - lastPosition;
        horizontalDelta.y = 0f;

        float horizontalSpeed = horizontalDelta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        bool isGrounded = playerController != null ? playerController.IsGrounded : controller.isGrounded;
        bool isMoving = horizontalSpeed > minMoveDistancePerSecond;

        if (!isGrounded || !isMoving || footstepClips == null || footstepClips.Length == 0)
        {
            stepTimer = 0f;
            lastPosition = transform.position;
            return;
        }

        bool isSprinting = playerController != null ? playerController.IsSprinting : IsSprinting();
        float interval = isSprinting ? sprintStepInterval : walkStepInterval;

        stepTimer += Time.deltaTime;

        if (stepTimer >= interval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clip, playOneShotVolumeScale);

        if (debugLogs)
        {
            Debug.Log($"Playing footstep clip: {clip.name}", this);
        }
    }

    private bool IsSprinting()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
#else
        return Input.GetKey(KeyCode.LeftShift);
#endif
    }
}
