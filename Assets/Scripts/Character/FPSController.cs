using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public static FPSController instance;
    [Header("Hareket Ayarları")]
    [SerializeField] float yürümeHızı = 2f;
    [SerializeField] float koşmaHızı = 3.5f;
    [SerializeField] float zıplamaGücü = 0.7f;
    [SerializeField] float yerçekimi = -9.81f;

    [Header("Zemin Ayarları")]
    [SerializeField] Transform zeminKontrol;
    [SerializeField] float zeminKontrolMesafesi = 0.4f;
    [SerializeField] LayerMask zeminMaskesi;

    [Header("Ayak Sesi Ayarları")]
    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private AudioClip[] runSounds;
    [SerializeField] AudioSource footstepSource;
    
    [Header("İniş Sesi")]
    [SerializeField] private AudioClip landSound;

    private CharacterController controller;
    private Animator animator;

    private Vector3 hız;
    private bool zeminde;
    private bool zıplıyor = false;
    public bool duvaraDegdi = false;
    private bool öncedenZemindeydi = true;
    private bool yereYeniİndi = false;
    private float onLandCooldown = 0.2f; // tekrar çağrılmasını engelleyen zaman
    private float onLandTimer = 0f;


    private int animIDSpeed;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDGrounded;
    private int animIDMotionSpeed;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        AssignAnimationIDs();
    }

    void Update()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        duvaraDegdi = Physics.Raycast(ray, 0.6f);

        ZeminiKontrolEt();
        HareketEt();
        Zıpla();
        YerçekiminiUygula();

        controller.Move(hız * Time.deltaTime);
        AnimasyonGüncelle();
    }

    void ZeminiKontrolEt()
    {
        zeminde = Physics.CheckSphere(zeminKontrol.position, zeminKontrolMesafesi, zeminMaskesi);
        animator.SetBool(animIDGrounded, zeminde);
        
        if (zeminde && !öncedenZemindeydi && !yereYeniİndi)
        {
            OnLand();
            yereYeniİndi = true;
            onLandTimer = onLandCooldown;
        }
        
        if (zeminde && hız.y < 0)
        {
            hız.y = -2f;
            animator.SetBool(animIDFreeFall, false);

            if (zıplıyor)
            {
                zıplıyor = false;
                animator.SetBool(animIDJump, false);
            }
        }
        else if (!zeminde)
        {
            animator.SetBool(animIDFreeFall, true);
            yereYeniİndi = false; 
        }
        
        if (yereYeniİndi)
        {
            onLandTimer -= Time.deltaTime;
            if (onLandTimer <= 0f)
            {
                yereYeniİndi = false;
            }
        }

        öncedenZemindeydi = zeminde;
    }


    void HareketEt()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float aktifHız = Input.GetKey(KeyCode.LeftShift) ? koşmaHızı : yürümeHızı;
        Vector3 yön = (transform.right * x + transform.forward * z).normalized;
        Vector3 yatayHareket = yön * aktifHız;
        hız.x = yatayHareket.x;
        hız.z = yatayHareket.z;
    }

    void Zıpla()
    {
        if (Input.GetButtonDown("Jump") && zeminde)
        {
            hız.y = Mathf.Sqrt(zıplamaGücü * -2f * yerçekimi);
            animator.SetBool(animIDJump, true);
            animator.SetBool(animIDFreeFall, false);
            zıplıyor = true;
        }
    }

    void YerçekiminiUygula()
    {
        hız.y += yerçekimi * Time.deltaTime;
    }

    void AnimasyonGüncelle()
    {
        Vector3 yatayHareket = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float hızMiktarı = yatayHareket.magnitude;

        animator.SetFloat(animIDSpeed, hızMiktarı);
        animator.SetFloat(animIDMotionSpeed, Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f);
    }

    void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public void FootstepWalk()
    {
        if (!zeminde || walkSounds.Length == 0) return;
        int index = Random.Range(0, walkSounds.Length);
        footstepSource.PlayOneShot(walkSounds[index]);
    }

    public void FootstepRun()
    {
        if (!zeminde || runSounds.Length == 0) return;
        int index = Random.Range(0, runSounds.Length);
        footstepSource.PlayOneShot(runSounds[index]);
    }
    
    public bool IsRunning
    {
        get
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool hareketVar = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
            return Input.GetKey(KeyCode.LeftShift) && hareketVar && zeminde;
        }
    }
    
    public void OnLand()
    {
        Debug.Log("OnLand eventi tetiklendi: Karakter yere indi.");
        
        if (landSound != null)
            footstepSource.PlayOneShot(landSound);
    }
}
