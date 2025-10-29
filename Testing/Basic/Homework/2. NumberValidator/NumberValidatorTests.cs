
using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;


[TestFixture]
public class NumberValidatorTestsFix
{
    [TestCase(-1, 2)]
    [TestCase(5, -1)]
    [TestCase(5, 6)]
    public void Constructor_WithInvalidParameters_ThrowsArgumentException(int precision, int scale)
    {
        Action action = () => new NumberValidator(precision, scale);

        action.Should().Throw<ArgumentException>();
    }
    

    [TestCase("0", "Целый ноль")]
    [TestCase("0.0", "Десятичный ноль")]
    [TestCase("123.45", "Корректное десятичное")]
    [TestCase("+1.23", "С явным плюсом")]
    public void IsValidNumber_WithValidNumberFormats_ReturnsTrue(string number, string description)
    {
        var validator = new NumberValidator(5, 2, true);

        validator.IsValidNumber(number).Should().BeTrue(description);
    }
    
    [TestCase("-1.23", "Отрицательное при onlyPositive=true")]
    [TestCase("00.000", "Превышение precision")]
    [TestCase("0.000", "Превышение scale")]
    [TestCase("a.sd", "Нечисловой формат")]
    [TestCase("", "Пустая строка")]
    [TestCase(null, "Null")]
    public void IsValidNumber_WithInvalidNumberFormats_ReturnsFalse(string number, string description)
    {
        var validator = new NumberValidator(5, 2, true);

        validator.IsValidNumber(number).Should().BeFalse(description);
    }

    [TestCase("1.23", "Максимальная precision и scale")]
    [TestCase("12.3", "Граница precision")]
    [TestCase("0.12", "Граница scale")]
    public void IsValidNumber_WithValidPrecisionAndScale_ReturnsTrue(string number, string description)
    {
        var strictValidator = new NumberValidator(3, 2);

        strictValidator.IsValidNumber(number).Should().BeTrue(description);
    }


    [TestCase("123.4", "Превышение precision в целой части")]
    public void IsValidNumber_WithExceededPrecisionAndScale_ReturnsFalse(string number, string description)
    {
        var strictValidator = new NumberValidator(3, 2);

        strictValidator.IsValidNumber(number).Should().BeFalse(description);
    }

    [Test]
    public void IsValidNumber_WithOnlyPositiveMode_RespectsSignRestrictions()
    {
        var positiveOnly = new NumberValidator(5, 2, true);
        var anyNum = new NumberValidator(5, 2, false);
        
        
        positiveOnly.IsValidNumber("-1.23").Should().BeFalse("Только положительные: отрицательное число");
        anyNum.IsValidNumber("-1.23").Should().BeTrue("Любые знаки: отрицательное число");
        anyNum.IsValidNumber("+1.23").Should().BeTrue("Любые знаки: положительное с плюсом");
        anyNum.IsValidNumber("-0").Should().BeTrue("Любые знаки: отрицательный ноль");
    }
    
    [TestCase("12..34", "Две точки")]
    [TestCase(".123", "Начинается с точки")]
    [TestCase("123.", "Оканчивается точкой")]
    public void IsValidNumber_WithInvalidFormats_ReturnsFalse(string number, string description)
    {
        var validator = new NumberValidator(10, 2);

        validator.IsValidNumber(number).Should().BeFalse(description);
    }
}