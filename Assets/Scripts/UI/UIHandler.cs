using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIHandler : MonoBehaviour
    {
        private Player _player;
        Player PlayerComponent => _player ?? (_player = gameObject.GetComponent<Player>());
        [SerializeField] ParticleSystem Stars;
        [SerializeField] ParticleSystem Thruster;
        [SerializeField] GUIHandler guiHandler;


        private void Update()
        {
            var velocity = Stars.velocityOverLifetime;
            velocity.speedModifierMultiplier = PlayerComponent.EngineComponent.CurrentSpeed;

            velocity = Thruster.velocityOverLifetime;
            velocity.speedModifierMultiplier = PlayerComponent.EngineComponent.CurrentSpeed;
            var m = Thruster.main;
            m.startColor = guiHandler.GearColorsSelected[PlayerComponent.EngineComponent.CurrentGear];
            var emission = Thruster.emission;
            emission.rateOverTimeMultiplier = PlayerComponent.EngineComponent.SpeedPercentage * 20;
        }

    }
}