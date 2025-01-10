namespace UnitTests
{
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }

    public class Tests
    {
        private Calculator _calculator;

        [SetUp]
        public void Setup()
        {
            // Arrange: Initialize objects needed for the tests
            _calculator = new Calculator();
        }

        [Test]
        public void Add_WhenCalledWithTwoIntegers_ReturnsCorrectSum()
        {
            // Act: Perform the operation to test
            var result = _calculator.Add(2, 3);

            // Assert: Verify the expected outcome
            Assert.That(result, Is.EqualTo(5));
        }
    }
}