using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    public bool isGrounded = true;
    public bool useFootsteps = true;
    private bool isCrouching = false;
    public bool isSprinting = false;

    public GameObject playerCamera;
    
    private Vector2 currentInput;
    [SerializeField] private Image colorStamina;
    public float NewAlpha = 0f;
    

    [Header("Stamina")]
    public float stamina;
    float maxStamina;
    public Slider staminaBar;
    public float dValue;
    public bool Tired;
    public GameObject Breathing;

   [Header("Footsteps")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler : isSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

    void Start()
    {
        NewAlpha = 0f;
        useFootsteps = true;
        maxStamina = stamina;
        staminaBar.maxValue = maxStamina;
        colorStamina.GetComponent<Image> ();
    }
    // Update is called once per frame
    void Update()
    {
        


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !Tired)
        {
           
            speed = 11;
            isSprinting= true;
            dValue = 4;
           
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Tired)
        {
            speed = 8;
            isSprinting= false;
            dValue = 1;
        }

        if (useFootsteps)
        {
            Handle_Footsteps();
        }
        if (isSprinting)
        {
            NewAlpha = NewAlpha + 0.008f;
            DecreaseEnergy();
        }
        else if (stamina != maxStamina)
            IncreaseEnergy();

        if (!isSprinting)
        {
            NewAlpha = NewAlpha - 0.0005f;
          
        }

        if (stamina <= 0)
        {
            stamina = stamina = 0.1f;
            IncreaseEnergy();
            Tired = true;
            dValue = 1;
            //sprint disabled
            Breathing.SetActive(true);

        }

        if (stamina >= 10)
        {
            stamina = stamina = 10f;
            Tired = false;
            dValue = 4;
           
            //sprint enabled
        }

        if (stamina >= 5)
        {        
            Tired = false;
            //sprint enabled
            NewAlpha = NewAlpha - 0.0008f;
            Breathing.SetActive(false);

        }
        if(NewAlpha >= 1)
        {
            NewAlpha = 1;
        }
        if(NewAlpha <= 0)
        {
            NewAlpha = 0;   
        }
        colorStamina.color = new Color(colorStamina.color.r, colorStamina.color.g, colorStamina.color.b, NewAlpha);
        staminaBar.value = stamina;
    }

    private void DecreaseEnergy()
    {
        if (stamina != 0)
            stamina -= dValue * Time.deltaTime;
    }

    private void IncreaseEnergy()
    {
        if (stamina != 0)
            stamina += dValue * Time.deltaTime;
    }

    private void Handle_Footsteps()
    {
       // if (!isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
           if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/WOOD":
                        footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                        break;
                    case "Footsteps/METAL":
                        footstepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                        break;
                    case "Footsteps/GRASS":
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
        }
    }
}

