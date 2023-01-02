using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
   
    //Контекст: 
    //shift - передача
    //speed - швидкість
    [Header("Shifts")]

    [SerializeField]
    [Range(0, 30)]
    List<float> cumulativeShiftSpeeds; //швидкості що відповідають передачам

    [SerializeField]
    [Range(1, 10)]
    public List<int> shiftHps; //ХП що має кожна окрема передача

    [SerializeField]
    public List<Color> shiftColors;

    List<int> cumulativeShiftHPs = new List<int>(); //сумарне ХП для кожної передачі

    int hp;
    public int HP { get => hp; set {
            hp = Mathf.Clamp(value, 0, cumulativeShiftHPs[shiftNumber - 1]);
            HPChangeEvent.Invoke();
        } }
    public delegate void EmptyEventListener();
    public event EmptyEventListener HPChangeEvent;

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

    [SerializeField]
    float minimumSpeed; //мінімальна вертикальна швидкість

    [SerializeField]
    int shiftNumber = 1;

    int currentShift = 0;
    public event EmptyEventListener ShiftChangeEvent;
    public int CurrentShift { get => currentShift; set {
            currentShift = Mathf.Clamp(value, 0, MaxShift);
            ShiftChangeEvent.Invoke();
            thruster.material.SetColor("_Color", shiftColors[currentShift]);
        } }


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

    public float MaxSpeed { get { return cumulativeShiftSpeeds[CurrentShift]; } }

    [SerializeField]
    [Range(0,10)]
    float accelerationModifier;

    public float Acceleration { get { return (CurrentShift == CorrectShift) ? accelerationModifier : accelerationModifier / (1 + Mathf.Abs(CurrentShift - CorrectShift)); } }


    [Header("Physics")]

    [SerializeField]
    float horizontalSpeed; //горизонтальна швидкість для переміщення

    float horizontalMovement = 0; //горизонтальне переміщення для цього кадру

    [SerializeField]
    float horizontalLimits = 1; //горизонтальні ліміти дороги

    [SerializeField]
    [Range(0, 1)]
    float limitLerpSpeed; //швидкість повернення до "дороги" біля горизонтальних лімітів

    [Header("Roll")]
    [SerializeField]
    float horizontalRollLimits = 1.5f; //ліміти дороги під час бочки
    [SerializeField]
    float rollDurationInSeconds = 1; //тривалість бочки

    bool isRolling = false; //статус бочки

    [Space(20)]
    [SerializeField]
    SpriteRenderer thruster;

    void Start()
    {
        int sum = 0; 
        foreach (int _hp in shiftHps) { //заповнюю масив сумарних ХП
            sum += _hp;
            cumulativeShiftHPs.Add(sum);
        }
        HP = sum;
        currentSpeed = minimumSpeed;
        CurrentShift = 0;
    }

    void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * horizontalSpeed;
        if (Input.GetButtonDown("Roll") & !isRolling) {
            isRolling = true;
            StartCoroutine(ResetRoll());
        }
        if (Input.GetButtonDown("ShiftDown")) {
            CurrentShift -= 1;
            CurrentSpeed = Mathf.Min(CurrentSpeed, MaxSpeed);
        } 
        if (Input.GetButtonDown("ShiftUp")) {
            CurrentShift += 1;
        }
    }

    IEnumerator ResetRoll() {
        yield return new WaitForSeconds(rollDurationInSeconds);
        isRolling = false;
    }



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



        float speedDifference = CorrectShift > 0 ? cumulativeShiftSpeeds[CorrectShift - 1] : minimumSpeed;
        thruster.material.SetFloat("_tValue", Mathf.Lerp(thruster.material.GetFloat("_tValue"), (CurrentSpeed - speedDifference) / (cumulativeShiftSpeeds[CurrentShift] - speedDifference), 0.5f));        
        

    }
}
