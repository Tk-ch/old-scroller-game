using Nebuloic;
using System.Collections;
using UnityEngine;

namespace Nebuloic
{
    [ExecuteInEditMode]
    public class UIHandler : MonoBehaviour
    {

        public static UIHandler instance;
         
        [SerializeField] ParticleSystem Stars;
        [SerializeField] ParticleSystem Thruster;
        [SerializeField] public GUIHandler guiHandler;
        [SerializeField] public Color[] GearColors;
        [SerializeField] public Color[] GearColorsSelected;

        private void OnEnable()
        {
            instance = this;
        }

#if !(UNITY_EDITOR)
        private void Update()
        {
            var velocity = Stars.velocityOverLifetime;
            velocity.speedModifierMultiplier = Player.instance.Ship.Engine.CurrentSpeed;

            velocity = Thruster.velocityOverLifetime;
            velocity.speedModifierMultiplier = Player.instance.Ship.Engine.CurrentSpeed;
            var m = Thruster.main;
            m.startColor = GearColorsSelected[Player.instance.Ship.Engine.CurrentGear];
            var emission = Thruster.emission;
            emission.rateOverTimeMultiplier = Player.instance.Ship.Engine.SpeedPercentage * 20;

            if (Player.instance.Ship.IsRolling) emission.rateOverTimeMultiplier *= 3;
        }
#endif
    }
}