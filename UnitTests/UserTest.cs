namespace UnitTests;


[TestFixture]
public class UserTest
{
    [Test]
    public void Template_equals_3()
    {
        // Arrange
        int one = 1;
        int two = 2;

        // Act
        var result = one + two;

        // Assert
        Assert.That(result, Is.EqualTo(3));
    }
}