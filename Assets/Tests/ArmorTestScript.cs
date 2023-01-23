using System.Collections;
using System.Reflection;
using Nebuloic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ArmorTestScript
{

    private Armor armor;

    [SetUp] 
    public void Init()
    {
        var go = new GameObject();
        armor = go.AddComponent<Armor>();
        armor.GetType()
            .GetField("_gearHPs", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(armor, new int[] { 1, 1, 2, 3, 5 });
        armor.GetType().GetMethod("GenerateCumulativeHPs", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(armor, null);
    }

    [Test]
    public void IsVulnerable_AtStart()
    {
        Assert.IsTrue(armor.IsVulnerable);
    }

    [Test]
    public void HP_IsEqualToMax_AtStart() {
        Assert.AreEqual(12, armor.HP);
    }
    [Test]
    public void IsVulnerabilityEventFired() {
        bool eventFired = false;

        armor.OnVulnerabilityChanged += () => eventFired = true;

        armor.IsVulnerable = true;
        
        Assert.IsTrue(eventFired);
    }

    

}
