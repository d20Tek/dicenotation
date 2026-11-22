using d20Tek.DiceNotation.Blazor.Interop;
using System.Reflection;

namespace d20Tek.DiceNotation.Blazor.UnitTests.Interop;

[TestClass]
public class CanvasRendererTrimmingTests
{
    private static readonly Assembly PayloadAssembly = typeof(RenderScene).Assembly;

    private static List<Type> FindPayloadTypes() => [.. PayloadTypes(typeof(RenderScene)).Distinct()];

    private static IEnumerable<Type> PayloadTypes(Type type) =>
        type.Assembly != PayloadAssembly
            ? []
            : [type, .. type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(p => PayloadTypes(ElementType(p.PropertyType)))];

    private static Type ElementType(Type type) =>
        type.GetInterfaces().Append(type).FirstOrDefault(IsGenericEnumerable)?.GetGenericArguments()[0] ?? type;

    private static bool IsGenericEnumerable(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);

    private static List<DynamicDependencyAttribute> GetRenderAsyncDependencies() =>
        [.. typeof(CanvasRenderer).GetMethod(nameof(CanvasRenderer.RenderAsync))!
            .GetCustomAttributes<DynamicDependencyAttribute>()];

    [TestMethod]
    public void RenderAsync_PayloadTypes_AllHaveDynamicDependency()
    {
        // Arrange
        var payloadTypes = FindPayloadTypes();

        // Act
        var dependencies = GetRenderAsyncDependencies().Select(d => d.Type).ToList();

        // Assert
        Assert.IsNotEmpty(payloadTypes);
        foreach (var type in payloadTypes)
        {
            Assert.Contains(type, dependencies);
        }
    }

    [TestMethod]
    public void RenderAsync_DynamicDependencies_OnlyTargetPayloadTypes()
    {
        // Arrange
        var payloadTypes = FindPayloadTypes();

        // Act
        var dependencies = GetRenderAsyncDependencies().Select(d => d.Type).ToList();

        // Assert
        Assert.IsNotEmpty(dependencies);
        foreach (var type in dependencies)
        {
            Assert.Contains(type!, payloadTypes);
        }
    }

    [TestMethod]
    public void RenderAsync_DynamicDependencies_KeepPublicConstructorsAndProperties()
    {
        // Arrange
        const DynamicallyAccessedMemberTypes expected =
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties;

        // Act
        var dependencies = GetRenderAsyncDependencies();

        // Assert
        Assert.IsNotEmpty(dependencies);
        foreach (var dependency in dependencies)
        {
            Assert.AreEqual(expected, dependency.MemberTypes);
        }
    }
}
