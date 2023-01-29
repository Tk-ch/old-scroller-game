using System.Collections;
using System.Collections.Generic;
using Nebuloic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TimerTest
{

    [Test]
    public void TimerWorks()
    {
        bool isDone = false;
        Timer t = new Timer(1);
        t.OnTimerEnd += () => isDone = true;
        Assert.IsFalse(isDone);
        t.Update(2);
        Assert.IsTrue(isDone);
    }

}
