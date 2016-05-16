using UnityEngine;
using System;
using System.Collections;
using Resources.Pooling;
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
        newPool.InitializeWithComponent<PoolableObject>(newPoolableObject);
        
        PoolableObject testComponent = ObjectPoolManager.Instance.Get(newPoolableObject).GetObjectComponent<PoolableObject>();

        Assert.AreSame(poolableCompontent.GetType(), testComponent.GetType());
    }

    [Test]
    public void ExtendedType() {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 2");
        PoolableObjectTestObject testA = newPoolableObject.AddComponent<PoolableObjectTestObject>();

        testA.intValue = 5;

        newPool.InitializeWithComponent<PoolableObject>(newPoolableObject);
        
        PoolableObjectTestObject testComponent = ObjectPoolManager.Instance.Get(newPoolableObject).GetObjectComponent<PoolableObjectTestObject>();

        Assert.AreEqual(5, testComponent.intValue);
    }

    [Test]
    public void UnrelatedType() {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 3");
        newPoolableObject.AddComponent<PoolableObjectTestObject>();

        TestObject testA = newPoolableObject.AddComponent<TestObject>();
        testA.intValue = 5;

        newPool.InitializeWithComponent<TestObject>(newPoolableObject);
        
        TestObject testComponent = ObjectPoolManager.Instance.Get(newPoolableObject).GetObjectComponent<TestObject>();

        Assert.AreEqual(5, testComponent.intValue);
    }

    [TestCase(5)]
    [TestCase(25)]
    public void AreAddedObjectsInitialized(int aObjectsToGetFromPool) {
        GameObject newObject = new GameObject();
        ObjectPool newPool = newObject.AddComponent<ObjectPool>();

        GameObject newPoolableObject = new GameObject("Pooled Object 4");
        newPoolableObject.AddComponent<PoolableObjectTestObject>();

        TestObject testA = newPoolableObject.AddComponent<TestObject>();
        testA.intValue = 5;

        newPool.InitializeWithComponent<TestObject>(newPoolableObject);

        TestObject testObject = null;
        for (int i = 0; i < aObjectsToGetFromPool; i++) {
            testObject = ObjectPoolManager.Instance.Get(newPoolableObject).GetObjectComponent<TestObject>();
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

        newPool.InitializeWithComponent<PoolableObject>(newPoolableObject, aObjectCount);

        // The loop starts at 0 so we have to offset otherwise the values will be 1 off
        Assert.AreEqual(aObjectCount, ObjectPoolManager.Instance.Get(newPoolableObject).Count);
    }

}
