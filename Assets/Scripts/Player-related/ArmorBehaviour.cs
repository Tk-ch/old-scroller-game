using System;
using UnityEngine;
using UnityEngine.Events;

namespace Nebuloic
{
    /// <summary>
    /// Monobehaviour handler for the Armor class
    /// </summary>
    public class ArmorBehaviour : MonoBehaviour
    {
        [SerializeField] Armor armor;
        public Armor Armor { get => armor; }

        private EventHandler<int> HPListener;
        private EventHandler<bool> VulnerabilityListener;

        private void Awake()
        {
            armor.GenerateCumulativeHPs();

            // subscribing to armor events
            HPListener = (object a, int hp) => ArmorHPChanged?.Invoke(hp);
            VulnerabilityListener = (object a, bool vulnerability) => ArmorVulnerabilityChanged?.Invoke(vulnerability);
            armor.HPChanged += HPListener;
            armor.VulnerabilityChanged += VulnerabilityListener;
        }

        private void OnDestroy()
        {
            // unsubscribing from armor events
            armor.HPChanged -= HPListener;
            armor.VulnerabilityChanged -= VulnerabilityListener;
        }
        
        public UnityEvent<int> ArmorHPChanged;
        public UnityEvent<bool> ArmorVulnerabilityChanged;

    }
}