using System;
using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    public class ShipLogic
    {
        private readonly ArmorLogic _armorLogic;
        private readonly EngineLogic _engineLogic;

        public EngineLogic Engine { get => _engineLogic;}
        public ArmorLogic Armor { get => _armorLogic; }

        public event Action<bool> OnRollChanged;

        private readonly float _rollDurationInSeconds;
        private Timer rollingTimer;

        private bool _isRolling;
        public bool IsRolling
        {
            get { return _isRolling; }
            set
            {
                if (_isRolling == value) return;
                _isRolling = value;
                OnRollChanged?.Invoke(value);
            }
        }

        public void Roll()
        {
            if (!IsRolling)
            {
                IsRolling = true;
                rollingTimer = new Timer(_rollDurationInSeconds);
                rollingTimer.OnTimerEnd += ResetRoll;
            }
        }
        void ResetRoll() => IsRolling = false;

        public ShipLogic( ShipData _data, float rollDurationInSeconds, float perfectSwitchTiming) {
            _rollDurationInSeconds = rollDurationInSeconds;
            _armorLogic = new ArmorLogic(_data.ArmorData);
            _engineLogic = new EngineLogic(_data, _armorLogic, perfectSwitchTiming);
        }


        public void Update(float deltaTime) { 
            if (rollingTimer != null) rollingTimer.Update(deltaTime); 
            _armorLogic.Update(deltaTime);
            _engineLogic.Update(deltaTime);
        }

    }
}