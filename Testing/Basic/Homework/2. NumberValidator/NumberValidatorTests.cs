
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [Test]
    public void Test()
    {
        Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
        Assert.DoesNotThrow(() => new NumberValidator(1, 0, true)); // Дублирует 2 проверку

        ClassicAssert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0")); // Дублирует 3 проверку
        ClassicAssert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
        ClassicAssert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
        ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
        ClassicAssert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0")); // Дублирует 3 проверку
        ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
        ClassicAssert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
        ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
        ClassicAssert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
        ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
        ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
    }
}


// Было изменено:
// 1) Тесты были разделены на логические группы для лучшей читаемости
// 2) Были добалены сообщения к каждому тесту чтобы из стек-трейса было понятно на каких данных тест не работает
// 3) Использован Assert.Multiple() для того чтобы одна упавшая проверка не блокировала прохождение остальных проверок
// 4) Удалены дублирующиеся тесты
// 5) Добавлены тесты проверяющие: пограничные значения на scale и precision, параметр onlyPositive=false, пустую строку и null, запятую в качетве разделителя,
// некоректные форматы чисел (2 точки; число начинается с точки; без дробной части, но с точкой), отрицательный ноль.

[TestFixture]
public class NumberValidatorTestsFix
{
    [Test]
    public void КонструкторКорректноВалидируетПараметры()
    {
        // Некорректные параметры
        Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
        Assert.Throws<ArgumentException>(() => new NumberValidator(5, -1));
        Assert.Throws<ArgumentException>(() => new NumberValidator(5, 6));
        
        // Корректные параметры
        Assert.DoesNotThrow(() => new NumberValidator(1, 0));
        Assert.DoesNotThrow(() => new NumberValidator(5, 2));
        Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
    }

    [Test]
    public void IsValidNumberКорректноОбрабатываетОсновныеСценарии()
    {
        var validator = new NumberValidator(17, 2, true);
        
        Assert.Multiple(() =>
        {
            // Корректные числа
            Assert.That(validator.IsValidNumber("0"), Is.True, "Целый ноль");
            Assert.That(validator.IsValidNumber("0.0"), Is.True, "Десятичный ноль");
            Assert.That(validator.IsValidNumber("123.45"), Is.True, "Корректное десятичное");
            Assert.That(validator.IsValidNumber("+1.23"), Is.True, "С явным плюсом");
            
            // Некорректные числа
            Assert.That(validator.IsValidNumber("-1.23"), Is.False, "Отрицательное при onlyPositive=true");
            Assert.That(validator.IsValidNumber("00.00"), Is.False, "Превышение precision");
            Assert.That(validator.IsValidNumber("0.000"), Is.False, "Превышение scale");
            Assert.That(validator.IsValidNumber("a.sd"), Is.False, "Нечисловой формат");
            Assert.That(validator.IsValidNumber(""), Is.False, "Пустая строка"); 
            Assert.That(validator.IsValidNumber(null), Is.False, "Null");
        });
    }

    [Test]
    public void IsValidNumberКорректноРаботаетСPrecisionИScale()
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
    public void IsValidNumberКорректноРаботаетСРежимомOnlyPositive()
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
    public void IsValidNumberКорректноВалидируетФорматы() 
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