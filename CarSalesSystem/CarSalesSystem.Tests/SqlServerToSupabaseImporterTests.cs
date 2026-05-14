using System.Reflection;
using CarSalesSystem.Services;
using Xunit;

namespace CarSalesSystem.Tests;

public class SqlServerToSupabaseImporterTests
{
	[Theory]
	[InlineData("4111111111111111", "1111")]
	[InlineData("12", "0012")]
	[InlineData(null, "0000")]
	public void GetLast4_ReturnsMaskedSafeValue(string? input, string expected)
	{
		var result = (string)InvokePrivateStatic("GetLast4", input)!;

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData("9", "09")]
	[InlineData("13", "12")]
	[InlineData("abc", "01")]
	[InlineData(null, "01")]
	public void NormalizeMonth_ReturnsTwoDigitBoundedMonth(string? input, string expected)
	{
		var result = (string)InvokePrivateStatic("NormalizeMonth", input)!;

		Assert.Equal(expected, result);
	}

	private static object? InvokePrivateStatic(string methodName, params object?[]? args)
	{
		var type = typeof(SqlServerToSupabaseImporter);
		var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
		Assert.NotNull(method);
		return method!.Invoke(null, args);
	}
}
