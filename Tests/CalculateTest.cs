using NUnit.Framework;

namespace ShareResource.Tests
{
    [TestFixture]
    public class CalculateTest
    {
        private  ICalculate _calculateService;
        [SetUp]
        public void Setup()
        {
            this._calculateService = new CalculateService();
        }
        [Test]
        public void AddAssert()
        {
            int a=3; int b=4;
            var result = _calculateService.Add(a, b);
            Assert.That(result, Is.EqualTo(7));
        }
        [Test]
        public void SubtractAsser()
        {
            int a = 3; int b = 4;
            var result = _calculateService.Subtract(a, b);
            Assert.That(result,Is.EqualTo(-1));
        }

    }
}
