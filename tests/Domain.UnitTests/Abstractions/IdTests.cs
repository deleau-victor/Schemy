using System.ComponentModel;

namespace Domain.UnitTests.Abstractions;

public class IdTests
{
    [Fact]
    public void New_ShouldGenerateDistinctNonEmptyIds()
    {
        Id first = Id.New();
        Id second = Id.New();

        first.IsEmpty.ShouldBeFalse();
        first.ShouldNotBe(second);
    }

    [Fact]
    public void Parse_ShouldRoundTripUlidString()
    {
        Id id = Id.New();

        Id.Parse(id.ToString(), null).ShouldBe(id);
    }

    [Fact]
    public void TryParse_ShouldAcceptGuid()
    {
        var guid = Guid.NewGuid();

        Id.TryParse(guid.ToString(), null, out Id id).ShouldBeTrue();
        id.ToGuid().ShouldBe(guid);
    }

    [Fact]
    public void TryParse_ShouldRejectGarbage()
    {
        Id.TryParse("pas-un-id", null, out _).ShouldBeFalse();
    }

    [Fact]
    public void TypeConverter_ShouldConvertFromString()
    {
        Id id = Id.New();

        TypeDescriptor.GetConverter(typeof(Id)).ConvertFromString(id.ToString()).ShouldBe(id);
    }
}
