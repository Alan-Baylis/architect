using UnityEngine;
using System;
using System.Collections;
using Architect.Pooling;
using Testing;
using NUnit.Framework;

[TestFixture]
public class ObjectPoolTests {

    [Test]
    public void BaseType() {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 1");
        PoolableObject poolableCompontent = newPoolableObject.AddComponent<PoolableObject>();
        newPool.Initialize<PoolableObject>(newPoolableObject);
        
        PoolableObject testComponent = PoolManager.Instance.Get(newPoolableObject).GetObjectComponent<PoolableObject>();

        Assert.AreSame(poolableCompontent.GetType(), testComponent.GetType());
    }

    [Test]
    public void ExtendedType() {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 2");
        PoolableObjectTestObject testA = newPoolableObject.AddComponent<PoolableObjectTestObject>();

        testA.intValue = 5;

        newPool.Initialize<PoolableObject>(newPoolableObject);
        
        PoolableObjectTestObject testComponent = PoolManager.Instance.Get(newPoolableObject).GetObjectComponent<PoolableObjectTestObject>();

        Assert.AreEqual(5, testComponent.intValue);
    }

    [Test]
    public void UnrelatedType() {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 3");
        newPoolableObject.AddComponent<PoolableObjectTestObject>();

        TestObjectClass testA = newPoolableObject.AddComponent<TestObjectClass>();
        testA.intValue = 5;

        newPool.Initialize<TestObjectClass>(newPoolableObject);
        
        TestObjectClass testComponent = PoolManager.Instance.Get(newPoolableObject).GetObjectComponent<TestObjectClass>();

        Assert.AreEqual(5, testComponent.intValue);
    }

    [TestCase(5)]
    [TestCase(25)]
    public void AreAddedObjectsInitialized(int aObjectsToGetFromPool) {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 4");
        newPoolableObject.AddComponent<PoolableObjectTestObject>();

        TestObjectClass testA = newPoolableObject.AddComponent<TestObjectClass>();
        testA.intValue = 5;

        newPool.Initialize<TestObjectClass>(newPoolableObject);

        TestObjectClass testObject = null;
        for (int i = 0; i < aObjectsToGetFromPool; i++) {
            testObject = PoolManager.Instance.Get(newPoolableObject).GetObjectComponent<TestObjectClass>();
            testObject.intValue = i;
        }

        // The loop starts at 0 so we have to offset otherwise the values will be 1 off
        Assert.AreEqual(aObjectsToGetFromPool, testObject.intValue + 1);
    }

    [TestCase(5)]
    [TestCase(25)]
    public void PooledObjectCount(int aObjectCount) {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 5");
        newPoolableObject.AddComponent<PoolableObject>();

        newPool.Initialize<PoolableObject>(newPoolableObject, aObjectCount);

        // The loop starts at 0 so we have to offset otherwise the values will be 1 off
        Assert.AreEqual(aObjectCount, PoolManager.Instance.Get(newPoolableObject).Count);
    }

}
