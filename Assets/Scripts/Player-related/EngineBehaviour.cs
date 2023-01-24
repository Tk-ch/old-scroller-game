using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component to manage ship's gears - the main point is to calculate acceleration and speed here and then use it in Ship class. 
/// </summary>
namespace Nebuloic
{
    public class EngineBehaviour : MonoBehaviour
    {
        [SerializeField] private Engine _engine;

        public Engine Engine { get => _engine; }

        public UnityEvent<int, int> EngineGearChanged;
        public UnityEvent EnginePerfectSwitched;
        public UnityEvent<bool> EnginePerfectSwitchChanged;

        private EventHandler<int> GearHandler;
        private Action PerfectSwitchHandler;
        private EventHandler<bool> PerfectSwitchChangeHandler;



        private void Awake()
        {
            Engine.Init(GetComponent<ArmorBehaviour>().Armor);
            Engine.PerfectSwitchChanged += ResetPerfectSwitch;

            GearHandler = (object eng, int gear) => EngineGearChanged?.Invoke(((Engine) eng).CurrentGear, gear);
            PerfectSwitchHandler = () => EnginePerfectSwitched?.Invoke();
            PerfectSwitchChangeHandler = (object _, bool ps) => EnginePerfectSwitchChanged?.Invoke(ps);

            Engine.GearChanged += GearHandler;
            Engine.PerfectSwitched += PerfectSwitchHandler;
            Engine.PerfectSwitchChanged += PerfectSwitchChangeHandler;

        }

        private void Start()
        {
            EngineGearChanged?.Invoke(0, 0);
        }

        private void FixedUpdate()
        {
            Engine.UpdateEngine(Time.fixedDeltaTime);
        }



        private void ResetPerfectSwitch(object _, bool ps)
        {
            if (!ps) return; 
            StartCoroutine(Utility.ExecuteAfterTime (() => Engine.PerfectSwitch = false, Engine.PerfectSwitchTiming));
        }
    }
}