using UnityEngine;

public class Player : MonoBehaviour
{
    // Referência ao CharacterController da Unity
    private CharacterController controller;
    
    // Velocidade de movimento
    public float speed = 5f;

    // Velocidade de corrida
    public float runSpeed = 15f;

    // Velocidade de movimento no ar (geralmente menor que no chão)
    public float airSpeed = 2f;
    
    // Força do pulo
    public float jumpForce = 8f;
    
    // Gravidade aplicada ao personagem
    public float gravity = -9.81f;
    
    // Velocidade vertical (queda, pulo, etc.)
    private float verticalVelocity;
    
    // Referência ao Animator (para animações)
    private Animator anim;
    
    // Referência à câmera (para rotação com o mouse)
    public Transform cameraTransform;
    
    // Sensibilidade do mouse
    public float mouseSensitivity = 2f;
    
    // Acumulador para rotação vertical (câmera)
    private float xRotation = 0f;
    
    // Para controlar estado de pulo
    private bool wasGrounded = true;
    
    void Start()
    {
        // Pegando os componentes necessários na cena
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        
        // Bloqueia e esconde o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        Move();
        HandleJump();
        HandleMouseLook();
        UpdateAnimations();
    }
    
    void HandleJump()
    {
        // ----------- PULO ---------------------
        if (controller.isGrounded && verticalVelocity >= 0)
        {
            verticalVelocity = -2f; // "cola" no chão
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
            anim.SetTrigger("jump");
        }
        
        // Aplica gravidade
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 verticalMove = Vector3.up * verticalVelocity;
        controller.Move(verticalMove * Time.deltaTime);
    }
    
    void HandleMouseLook()
    {
        // ----------- ROTACIONA O PLAYER COM O MOUSE (CÂMERA) ----------------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Roda o player horizontalmente
        transform.Rotate(Vector3.up * mouseX);
        
        // Roda a câmera verticalmente (limitada para não girar demais)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = (moveX != 0 || moveZ != 0);
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (move != Vector3.zero)
        {
            float currentSpeed;

            if (controller.isGrounded)
            {
                // Se estiver segurando Shift, usa runSpeed
                bool isRunning = Input.GetKey(KeyCode.LeftShift);

                currentSpeed = isRunning ? runSpeed : speed;
                anim.SetBool("Running", isRunning && controller.isGrounded);
            }
            else
            {
                currentSpeed = airSpeed;
                anim.SetBool("Running", false);
            }

            controller.Move(currentSpeed * Time.deltaTime * move);
        }
        else 
        {
            anim.SetBool("Running", false);
        }
    }


    void UpdateAnimations()
    {
        // ----------- ANIMAÇÕES -------------
        // Verifica se está se movendo
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = (moveX != 0 || moveZ != 0);
        
        // Atualiza animação de caminhada
        anim.SetBool("IsWalking", isMoving && controller.isGrounded);

        if (anim.GetBool("Running"))
        {
            anim.SetBool("IsWalking", false);
        }
        else
        {
            anim.SetBool("IsWalking", isMoving && controller.isGrounded);
        }
        // Descobre automaticamente o nome do estado atual
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        
        string currentStateName = "Desconhecido";
        if (clipInfo.Length > 0)
        {
            currentStateName = clipInfo[0].clip.name;
        }
        

        
        // Força saída do Jump quando pousa no chão
        if (currentStateName.Contains("Jump") && controller.isGrounded && currentState.normalizedTime >= 0.8f)
        {
            Debug.Log("FORÇANDO SAÍDA DO JUMP!");
            if (isMoving)
            {
                // Tenta diferentes nomes possíveis para o estado de andar
                if (HasState("Walking"))
                    anim.Play("Walking", 0);
                else if (HasState("Walk"))
                    anim.Play("Walk", 0);
                else if (HasState("Run"))
                    anim.Play("Run", 0);
            }
            else
            {
                // Tenta diferentes nomes possíveis para o estado idle
                if (HasState("Idle"))
                    anim.Play("Idle", 0);
                else if (HasState("Idle_0"))
                    anim.Play("Idle_0", 0);
            }
        }
        
        wasGrounded = controller.isGrounded;
    }
    
    // Função auxiliar para verificar se um estado existe
    bool HasState(string stateName)
    {
        return anim.HasState(0, Animator.StringToHash(stateName));
    }
}