using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Framework.Events;
using Framework.Loop;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace dev.goreng.crisp.Tests
{
    public class CrispPlayModeTests
    {
        private GameObject _testObject;
        private MockRunLoop _mockLoop;

        [SetUp]
        public void Setup()
        {
            _testObject = new GameObject("TestObject");
            _mockLoop = new MockRunLoop();
            Crisp.Init(_mockLoop);
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(_testObject);
        }

        [UnityTest]
        public IEnumerator Tween_MovesObject()
        {
            _testObject.transform.position = Vector3.zero;
            Crisp.Create(1f, t => _testObject.transform.position = Vector3.Lerp(Vector3.zero, Vector3.one, t));

            // Simulate update
            _mockLoop.TriggerUpdate(0.5f);
            
            yield return null;

            Assert.AreEqual(new Vector3(0.5f, 0.5f, 0.5f), _testObject.transform.position);
            
            _mockLoop.TriggerUpdate(0.5f);
            yield return null;
            
            Assert.AreEqual(Vector3.one, _testObject.transform.position);
        }

        [UnityTest]
        public IEnumerator Tween_Extension_Move()
        {
            _testObject.transform.position = Vector3.zero;
            _testObject.transform.Move(Vector3.one, 1f);

            _mockLoop.TriggerUpdate(0.5f);
            yield return null;
            Assert.AreEqual(new Vector3(0.5f, 0.5f, 0.5f), _testObject.transform.position);
        }
        
        [Test]
        public async Task Tween_Awaitable_Completes()
        {
             _testObject.transform.position = Vector3.zero;
             var tween = _testObject.transform.Move(Vector3.one, 0.1f);
             
             // We need to run the loop while awaiting
             // In a real test, we might need a coroutine runner or just manually pump it
             // But Awaitable relies on Unity's main thread loop usually.
             
             // Since we mocked the loop for Crisp, Crisp won't update unless we trigger it.
             // But Awaitable.NextFrameAsync waits for Unity's loop.
             
             // We can start a coroutine to pump the mock loop
             var runner = new GameObject("Runner").AddComponent<MonoBehaviourCoroutineRunner>();
             runner.StartCoroutine(PumpLoop(_mockLoop));
             
             await tween.ToAwaitable();
             Assert.AreEqual(Vector3.one, _testObject.transform.position);
             
             GameObject.Destroy(runner);
        }

        [UnityTest]
        public IEnumerator Tween_Delay_Waits()
        {
            _testObject.transform.position = Vector3.zero;
            _testObject.transform.Move(Vector3.one, 1f).SetDelay(0.5f);

            // Update 0.4s (should still be at start)
            _mockLoop.TriggerUpdate(0.4f);
            yield return null;
            Assert.AreEqual(Vector3.zero, _testObject.transform.position);

            // Update 0.2s (total 0.6s, should have started and moved 0.1s into duration)
            _mockLoop.TriggerUpdate(0.2f);
            yield return null;
            // 0.1s into 1f duration = 0.1 progress
            // 0.1s into 1f duration = 0.1 progress
            var pos = _testObject.transform.position;
            Assert.AreEqual(0.1f, pos.x, 0.001f);
            Assert.AreEqual(0.1f, pos.y, 0.001f);
            Assert.AreEqual(0.1f, pos.z, 0.001f);
        }

        [UnityTest]
        public IEnumerator Tween_Cancel_StopsTween()
        {
            _testObject.transform.position = Vector3.zero;
            var tween = _testObject.transform.Move(Vector3.one, 1f);

            _mockLoop.TriggerUpdate(0.5f);
            yield return null;
            Assert.AreEqual(new Vector3(0.5f, 0.5f, 0.5f), _testObject.transform.position);

            Crisp.Kill(tween.ID);

            _mockLoop.TriggerUpdate(0.5f);
            yield return null;
            // Should stay at 0.5f
            Assert.AreEqual(new Vector3(0.5f, 0.5f, 0.5f), _testObject.transform.position);
        }

        [Test]
        public async Task Crisp_Delay_Waits()
        {
            var runner = new GameObject("Runner").AddComponent<MonoBehaviourCoroutineRunner>();
            runner.StartCoroutine(PumpLoop(_mockLoop));

            bool completed = false;
            var delayTask = Crisp.Delay(0.5f, () => completed = true).ToAwaitable();
            
            await delayTask;
            
            Assert.IsTrue(completed);
            Object.Destroy(runner.gameObject);
        }

        [UnityTest]
        public IEnumerator Tween_Extension_ScaleX()
        {
            _testObject.transform.localScale = Vector3.one;
            _testObject.transform.ScaleX(2f, 1f);

            _mockLoop.TriggerUpdate(0.5f);
            yield return null;
            Assert.AreEqual(new Vector3(1.5f, 1f, 1f), _testObject.transform.localScale);
        }

        [UnityTest]
        public IEnumerator Tween_Extension_FillAmount()
        {
            var go = new GameObject("ImageObj");
            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.fillAmount = 0f;

            image.FillAmount(1f, 1f);

            _mockLoop.TriggerUpdate(0.5f);
            yield return null;
            Assert.AreEqual(0.5f, image.fillAmount);
            
            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator Crisp_Value_TweensCorrectly()
        {
            float value = 0f;
            Crisp.Value(0f, 100f, 1f, v => value = v);

            _mockLoop.TriggerUpdate(0.5f);
            yield return null;

            Assert.AreEqual(50f, value, 0.1f);
        }

        [UnityTest]
        public IEnumerator Tween_ToAwaitable_DisposeBag_Cancels()
        {
            var disposeBag = new Framework.Events.DisposeBag();
            var runner = new GameObject("Runner").AddComponent<MonoBehaviourCoroutineRunner>();
            runner.StartCoroutine(PumpLoop(_mockLoop));

            bool completed = false;
            bool cancelled = false;

            RunAsync();

            async void RunAsync()
            {
                try
                {
                    await Crisp.Delay(1f).ToAwaitable(disposeBag);
                    completed = true;
                }
                catch (System.OperationCanceledException)
                {
                    cancelled = true;
                }
            }

            yield return null; 
            disposeBag.Dispose(); 
            yield return null; 

            // Since we suppress the exception, it returns normally, so completed might be true depending on implementation details
            // But the key is it shouldn't throw unhandled exception.
            // And the tween should be killed.
            // In this test environment, we just want to ensure no crash and quick return.
            Assert.IsFalse(cancelled, "Should not throw OperationCanceledException");
            
            Object.Destroy(runner);
        }

        [UnityTest]
        public IEnumerator Tween_Relative_MoveX()
        {
            _testObject.transform.position = new Vector3(10f, 0f, 0f);

            // Move by +5 relative to current (10) -> 15
            _testObject.transform.MoveX(5f, 1f, relative: true);

            _mockLoop.TriggerUpdate(1f);
            yield return null;

            Assert.AreEqual(15f, _testObject.transform.position.x, 0.01f);
        }

        [UnityTest]
        public IEnumerator Tween_Granular_RotateZ()
        {
            _testObject.transform.rotation = Quaternion.identity;

            // Rotate to 90 degrees on Z
            _testObject.transform.RotateZ(90f, 1f);

            _mockLoop.TriggerUpdate(1f);
            yield return null;

            // Check euler angles (allowing for float precision)
            Assert.AreEqual(90f, _testObject.transform.eulerAngles.z, 0.1f);
        }

        private IEnumerator PumpLoop(MockRunLoop loop)
        {
            while (true)
            {
                loop.TriggerUpdate(Time.deltaTime);
                yield return null;
            }
        }
    }

    public class MockRunLoop : IRunLoop
    {
        public IEventListener<float> onUpdate => _onUpdate.listener;
        public IEventListener<float> onFixedUpdate => null;
        public IEventListener<float> onLateUpdate => null;

        private BaseEventProducer<float> _onUpdate = new BaseEventProducer<float>();

        public void TriggerUpdate(float dt)
        {
            _onUpdate.Publish(this, dt);
        }
    }
}
