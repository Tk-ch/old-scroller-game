using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{   
    //Контекст: 
    //shift - передача
    //speed - швидкість
    [Header("Shifts")]

    [SerializeField] [Range(0, 30)] List<float> cumulativeShiftSpeeds; //швидкості що відповідають передачам

    [SerializeField] [Range(1, 10)] public List<int> shiftHps; //ХП що має кожна окрема передача

    [SerializeField] public List<Color> shiftColors; //кольори для відповідних передач

    List<int> cumulativeShiftHPs = new List<int>(); //сумарне ХП для кожної передачі

    int hp; // теперішні хітпоінти
    public int HP { get => hp; set {
            hp = Mathf.Clamp(value, 0, cumulativeShiftHPs[shiftNumber - 1]);
            HPChangeEvent.Invoke();
        } }

    public delegate void EmptyEventListener();
    
    // Is invoked when HP changes
    public event EmptyEventListener HPChangeEvent;
    
    // Maximum shift depending on HP
    public int MaxShift {
        get {
            for (int i = 0; i < shiftNumber; i++) {
                if (cumulativeShiftHPs[i] >= HP) {
                    return i;
                }   
            }
            return 0;
        }
    }

    [SerializeField] float minimumSpeed; //мінімальна вертикальна швидкість

    [SerializeField] public int shiftNumber = 1;

    int currentShift = 0;

    // Is called when the shift is changed

    public event EmptyEventListener ShiftChangeEvent;
    public int CurrentShift { get => currentShift; set {
            currentShift = Mathf.Clamp(value, 0, MaxShift);
            ShiftChangeEvent.Invoke();

        } }


    // Calculates the correct shift depending on the current speed. The correct shift will have the fastest acceleration
    public int CorrectShift { get {
            if (currentSpeed < cumulativeShiftSpeeds[0]) return 0;
            for (int i = 0; i < cumulativeShiftSpeeds.Count - 2; i++) 
            {
                if (cumulativeShiftSpeeds[i] < currentSpeed && cumulativeShiftSpeeds[i + 1] > currentSpeed) {
                    return i + 1;
                }
            }
            return CurrentShift;
        } 
    }
    
    float currentSpeed;
    
    public float CurrentSpeed { get => currentSpeed; set {
            currentSpeed = Mathf.Clamp(value, minimumSpeed, MaxSpeed);
        } }

    // Returns the maximum speed of the current shift
    public float MaxSpeed { get { return cumulativeShiftSpeeds[CurrentShift]; } }

    [SerializeField] [Range(0,10)] public float accelerationModifier;

    // Calculates acceleration depending on the modifier and the difference between the current shift and the correct one
    public float Acceleration { get { return (CurrentShift == CorrectShift) ? accelerationModifier : accelerationModifier / (1 + Mathf.Abs(CurrentShift - CorrectShift)); } }


    [Header("Physics")]

    [SerializeField] float horizontalSpeed; //горизонтальна швидкість для переміщення

    float horizontalMovement = 0; //горизонтальне переміщення для цього кадру

    [SerializeField] float horizontalLimits = 1; //горизонтальні ліміти дороги

    [SerializeField] [Range(0, 1)] float limitLerpSpeed; //швидкість повернення до "дороги" біля горизонтальних лімітів

    [Header("Roll")]
    [SerializeField] float horizontalRollLimits = 1.5f; //ліміти дороги під час бочки
    [SerializeField] float rollDurationInSeconds = 1; //тривалість бочки

    public bool isRolling = false; //статус бочки

    [SerializeField] SimpleAnimator myAnimator;

    public float speedTValue {
        get
        {
            float speedDifference = CorrectShift > 0 ? cumulativeShiftSpeeds[CorrectShift - 1] : minimumSpeed;
            return (CurrentSpeed - speedDifference) / (cumulativeShiftSpeeds[CurrentShift] - speedDifference);
        }
    }


    void Start()
    {
        int sum = 0; 
        foreach (int _hp in shiftHps) { //заповнює масив сумарних ХП
            sum += _hp;
            cumulativeShiftHPs.Add(sum);
        }
        HP = sum;
        currentSpeed = minimumSpeed;
        CurrentShift = 0;
    }

    public void TakeDamage(int hpDamage, int shiftDamage) {
        HP -= hpDamage;
        if (hpDamage > 0)myAnimator.ShakeCamera();
        if (CurrentShift > 0 && shiftDamage > 0) myAnimator.ShowDeceleration();
        CurrentShift -= shiftDamage;
        } 

    void Update()
    {
        GetInputs();
    }

    

    /// <summary>
    /// Gets all the needed inputs from the Input class and updates the variables
    /// </summary>
    private void GetInputs()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * horizontalSpeed;
        if (Input.GetButtonDown("Roll") & !isRolling)
        {
            isRolling = true;
            StartCoroutine(ResetRoll());
        }
        if (Input.GetButtonDown("ShiftDown"))
        {
            CurrentShift -= 1;
            CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
        }
        if (Input.GetButtonDown("ShiftUp"))
        {
            CurrentShift += 1;
        }
    }

    /// <summary>
    /// Resets the roll after a period of time
    /// </summary>
    IEnumerator ResetRoll() {
        yield return new WaitForSeconds(rollDurationInSeconds);
        isRolling = false;
        transform.rotation = Quaternion.identity;
    }


    /// <summary>
    /// Moves the player and updates the thruster, also updates the speed
    /// </summary>
    void FixedUpdate()
    {
        transform.Translate(new Vector2(horizontalMovement * Time.fixedDeltaTime, 0), Space.World);
        
        float diff = Mathf.Abs(transform.position.x) - horizontalLimits;
        if (diff > 0) {
            if (!isRolling)
            {
                transform.Translate(new Vector2(-diff * limitLerpSpeed * Mathf.Sign(transform.position.x), 0), Space.World);
            }
            else if (Mathf.Abs(transform.position.x) > horizontalRollLimits)
            {
                transform.position = new Vector2(-(horizontalRollLimits - 0.1f) * Mathf.Sign(transform.position.x), transform.position.y);
            }
        }

        
        CurrentSpeed += Acceleration * Time.fixedDeltaTime;



    }
}
