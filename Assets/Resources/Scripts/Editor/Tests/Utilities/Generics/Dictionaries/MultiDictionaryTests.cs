using UnityEngine;
using System.Collections;
using Resource.Utils;
using NUnit.Framework;

[TestFixture]
public class MultiDictionaryTests {

    [Test]
    public void Adding_SingleKey_Individually() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test", 1);
        testDictionary.Add("test", 2);
        testDictionary.Add("test", 3);
        testDictionary.Add("test", 4);
        testDictionary.Add("test", 5);

        Assert.AreEqual(1, testDictionary.Count);
    }

    [Test]
    public void Adding_SingleKey_Params() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test", 1, 2, 3, 4, 5);

        Assert.AreEqual(1, testDictionary.Count);
    }

    [Test]
    public void Adding_MultiKey_Individually() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test 1", 1);
        testDictionary.Add("test 2", 2);
        testDictionary.Add("test 3", 3);
        testDictionary.Add("test 4", 4);
        testDictionary.Add("test 5", 5);

        Assert.AreEqual(5, testDictionary.Count);
    }

    [Test]
    public void Adding_MultiKey_Params() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test 1", 1, 2, 3, 4, 5);
        testDictionary.Add("test 2", 1, 2, 3, 4, 5);

        Assert.AreEqual(2, testDictionary.Count);
    }

    [Test]
    public void Removing_EntireKeyPair() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test", 1);

        Assert.AreEqual(1, testDictionary.Count);

        testDictionary.Remove("test");

        Assert.AreEqual(0, testDictionary.Count);
    }

    [Test]
    public void Removing_SingleValue() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test", 1);

        Assert.AreEqual(1, testDictionary.Count);

        testDictionary.Remove("test", 1);

        Assert.AreEqual(1, testDictionary.Count);
    }

    [Test]
    public void Removing_ParamsValues() {
        MultiDictionary<string, int> testDictionary = new MultiDictionary<string, int>(5);
        testDictionary.Add("test", 1, 2, 3, 4, 5);

        Assert.AreEqual(5, testDictionary.ValueCount("test"));

        testDictionary.Remove("test", 1, 2, 3);

        Assert.AreEqual(2, testDictionary.ValueCount("test"));
    }

}
