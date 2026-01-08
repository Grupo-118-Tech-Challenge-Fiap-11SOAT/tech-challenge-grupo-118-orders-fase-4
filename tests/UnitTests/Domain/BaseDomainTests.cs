using Domain;
using NUnit.Framework;

namespace UnitTests.Domain;

[TestFixture]
public class BaseDomainTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties_WhenArgumentsAreProvided()
    {
        // Arrange
        var id = 1;
        var createdAt = DateTimeOffset.Now.AddDays(-1);
        var updatedAt = DateTimeOffset.Now;

        // Act
        var baseDomain = new BaseDomain(id, createdAt, updatedAt);

        // Assert
        Assert.AreEqual(id, baseDomain.Id);
        Assert.AreEqual(createdAt, baseDomain.CreatedAt);
        Assert.AreEqual(updatedAt, baseDomain.UpdatedAt);
    }

    [Test]
    public void Constructor_ShouldNotSetId_WhenIdIsZero()
    {
        // Arrange
        var id = 0;

        // Act
        var baseDomain = new BaseDomain(id);

        // Assert
        Assert.AreEqual(0, baseDomain.Id);
    }

    [Test]
    public void Constructor_ShouldNotSetDates_WhenDatesAreNull()
    {
        // Arrange
        // Act
        var baseDomain = new BaseDomain();

        // Assert
        Assert.AreEqual(default(DateTimeOffset), baseDomain.CreatedAt);
        Assert.AreEqual(default(DateTimeOffset), baseDomain.UpdatedAt);
    }
    
    [Test]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var baseDomain = new BaseDomain();
        var id = 10;
        var createdAt = DateTimeOffset.Now;
        var updatedAt = DateTimeOffset.Now.AddHours(1);
        var isActive = true;

        // Act
        baseDomain.Id = id;
        baseDomain.CreatedAt = createdAt;
        baseDomain.UpdatedAt = updatedAt;
        baseDomain.IsActive = isActive;

        // Assert
        Assert.AreEqual(id, baseDomain.Id);
        Assert.AreEqual(createdAt, baseDomain.CreatedAt);
        Assert.AreEqual(updatedAt, baseDomain.UpdatedAt);
        Assert.AreEqual(isActive, baseDomain.IsActive);
    }
}
