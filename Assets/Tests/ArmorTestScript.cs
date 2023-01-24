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
        armor = new Armor(new int[] { 1, 1, 2, 3, 5 });
        armor.GenerateCumulativeHPs();
    }

    // HP testing

    [Test]
    public void HP_IsEqualToMax_AtStart()
    {
        Assert.AreEqual(12, armor.HP);
    }
    [Test]
    public void HP_IsClamped_0Max()
    {
        armor.HP = 15;
        Assert.AreEqual(12, armor.HP);
        armor.HP = -5;
        Assert.AreEqual(0, armor.HP);
        armor.HP = 10;
        Assert.AreEqual(10, armor.HP);
    }
    
    // Invulnerability testing

    [Test]
    public void IsVulnerable_AtStart()
    {
        Assert.IsTrue(armor.IsVulnerable);
    }
    [Test]
    public void IsInvulnerable_AfterDecreasingHP() {
        armor.HP -= 1;
        Assert.IsTrue(!armor.IsVulnerable);
    }
    [Test]
    public void HP_IsNotDecreased_WhenInvulnerable() {
        armor.IsVulnerable = false;
        armor.HP -= 1;
        Assert.AreEqual(12, armor.HP);
    }
    [Test]
    public void HP_CanIncrease_WhenInvulnerable()
    {
        armor.HP = 10;
        armor.IsVulnerable = false;
        armor.HP += 1;
        Assert.AreEqual(11, armor.HP);
    }

    // CheckGearHP testing
    [Test]
    public void MaxGearIsAvailable_AtStart() {
        Assert.IsTrue(armor.CheckGearHP(4));  
    }
    [Test]
    public void GearIsAvailable_OnCumulativeHP() {
        armor.HP = armor.GearHPs[0] + armor.GearHPs[1] + armor.GearHPs[2];
        Assert.IsTrue(armor.CheckGearHP(2));
    }
    [Test]
    public void GearIsNotAvailable_OnIncorrectHP() {
        armor.HP = armor.GearHPs[0] + armor.GearHPs[1];
        Assert.IsFalse(armor.CheckGearHP(2));
    }
    [Test]
    public void GearIsAvailable_OnLowGearHP() {
        armor.HP = armor.GearHPs[0] + armor.GearHPs[1] + 1;
        Assert.IsTrue(armor.CheckGearHP(2));
    }


}
