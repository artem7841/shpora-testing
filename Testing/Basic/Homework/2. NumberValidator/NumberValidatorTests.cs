
using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;


[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1, 2,  "Исключение, когда precision отрицательная")]
    [TestCase(5, -1, "Исключение, когда scale отрицательный")]
    [TestCase(5, 6, "Исключение, когда scale больше precision")]
    public void Constructor_WithInvalidParameters_ThrowsArgumentException(int precision, int scale, string description)
    {
        Action action = () => new NumberValidator(precision, scale);

        action.Should().Throw<ArgumentException>(description);
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
    [TestCase("00.000", "Превышение precision в дробной части")]
    [TestCase("1234.56", "Превышение precision в целой части")]
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
    [TestCase("1,23", "Запятая в качестве разделителя")]
    public void IsValidNumber_WithValidPrecisionAndScale_ReturnsTrue(string number, string description)
    {
        var strictValidator = new NumberValidator(3, 2);

        strictValidator.IsValidNumber(number).Should().BeTrue(description);
    }
    
    
    
    [TestCase("-1.23", "Отрицательное число")]
    [TestCase("-0", "Отрицательный ноль")]
    public void IsValidNumber_WithOnlyPositiveFalse_AcceptsNegativeNumbers(string number, string description)
    {
        var anyNum = new NumberValidator(5, 2, false);
    
        anyNum.IsValidNumber(number).Should().BeTrue(description);
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