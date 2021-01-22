using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SkillT;

namespace Tests
{

    class Persion{
        public string name;
        public int age;
    }

    class PersionContainer{
        public Persion persion{get;set;}
        public PersionContainer(string name, int age){
            persion = new Persion();
            persion.name = name;
            persion.age = age;
        }
    }

    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            PersionContainer container = new PersionContainer("a",1);
            Debug.Log(container.persion.name+" "+container.persion.age);
            container.persion.name = "b";
            container.persion.age = 2;
            Debug.Log(container.persion.name+" "+container.persion.age);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
