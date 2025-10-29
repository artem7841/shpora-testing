
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;


[TestFixture]
public class NumberValidatorTestsFix
{
    [Test]
    public void Constructor_WithInvalidParameters_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
        Assert.Throws<ArgumentException>(() => new NumberValidator(5, -1));
        Assert.Throws<ArgumentException>(() => new NumberValidator(5, 6));
    }
    
    [Test]
    public void Constructor_WithValidParameters_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => new NumberValidator(1, 0));
        Assert.DoesNotThrow(() => new NumberValidator(5, 2));
        Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
    }

    [Test]
    public void IsValidNumber_WithValidNumberFormats_ReturnsTrue()
    {
        var validator = new NumberValidator(5, 2, true);
        
        Assert.Multiple(() =>
        {
            Assert.That(validator.IsValidNumber("0"), Is.True, "Целый ноль");
            Assert.That(validator.IsValidNumber("0.0"), Is.True, "Десятичный ноль");
            Assert.That(validator.IsValidNumber("123.45"), Is.True, "Корректное десятичное");
            Assert.That(validator.IsValidNumber("+1.23"), Is.True, "С явным плюсом");

        });
    }
    
    [Test]
    public void IsValidNumber_WithInvalidNumberFormats_ReturnsFalse()
    {
        var validator = new NumberValidator(5, 2, true);
        
        Assert.Multiple(() =>
        {
            Assert.That(validator.IsValidNumber("-1.23"), Is.False, "Отрицательное при onlyPositive=true");
            Assert.That(validator.IsValidNumber("00.000"), Is.False, "Превышение precision");
            Assert.That(validator.IsValidNumber("0.000"), Is.False, "Превышение scale");
            Assert.That(validator.IsValidNumber("a.sd"), Is.False, "Нечисловой формат");
            Assert.That(validator.IsValidNumber(""), Is.False, "Пустая строка"); 
            Assert.That(validator.IsValidNumber(null), Is.False, "Null");
        });
    }

    [Test]
    public void IsValidNumber_WithPrecisionAndScaleLimits_RespectsBoundaries()
    {
        var strictValidator = new NumberValidator(3, 2);
        
        Assert.Multiple(() =>
        {
            // Граничные случаи
            Assert.That(strictValidator.IsValidNumber("1.23"), Is.True, "Максимальная precision и scale");
            Assert.That(strictValidator.IsValidNumber("12.3"), Is.True, "Граница precision");
            Assert.That(strictValidator.IsValidNumber("0.12"), Is.True, "Граница scale");
            
            // Превышение лимитов
            Assert.That(strictValidator.IsValidNumber("12.34"), Is.False, "Превышение precision");
            Assert.That(strictValidator.IsValidNumber("123.4"), Is.False, "Превышение precision в целой части");
        });
    }

    [Test]
    public void IsValidNumber_WithOnlyPositiveMode_RespectsSignRestrictions()
    {
        var positiveOnly = new NumberValidator(5, 2, true);
        var anyNum = new NumberValidator(5, 2, false);
        
        Assert.Multiple(() =>
        {
            Assert.That(positiveOnly.IsValidNumber("-1.23"), Is.False, "Только положительные: отрицательное число");
            Assert.That(anyNum.IsValidNumber("-1.23"), Is.True, "Любые знаки: отрицательное число");
            Assert.That(anyNum.IsValidNumber("+1.23"), Is.True, "Любые знаки: положительное с плюсом");
            Assert.That(anyNum.IsValidNumber("-0"), Is.True, "Любые знаки: отрицательный ноль");
        });
    }
    
    [Test]
    public void IsValidNumber_WithInvalidFormats_ReturnsFalse() 
    {
        var validator = new NumberValidator(10, 2);
        
        Assert.Multiple(() =>
        {
            Assert.That(validator.IsValidNumber("12..34"), Is.False, "Две точки");
            Assert.That(validator.IsValidNumber(".123"), Is.False, "Начинается с точки");
            Assert.That(validator.IsValidNumber("123."), Is.False, "Оканчивается точкой");
        });
    }
}