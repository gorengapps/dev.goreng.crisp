using NUnit.Framework;
using UnityEngine;

namespace dev.goreng.crisp.Tests
{
    public class CrispTests
    {
        [Test]
        public void Easing_Linear_ReturnsCorrectValue()
        {
            Assert.AreEqual(0.5f, Ease.Evaluate(0.5f, EaseType.Linear));
            Assert.AreEqual(0f, Ease.Evaluate(0f, EaseType.Linear));
            Assert.AreEqual(1f, Ease.Evaluate(1f, EaseType.Linear));
        }

        [Test]
        public void Easing_InQuad_ReturnsCorrectValue()
        {
            Assert.AreEqual(0.25f, Ease.Evaluate(0.5f, EaseType.InQuad));
        }

        [Test]
        public void Easing_OutQuad_ReturnsCorrectValue()
        {
            Assert.AreEqual(0.75f, Ease.Evaluate(0.5f, EaseType.OutQuad));
        }
    }
}
