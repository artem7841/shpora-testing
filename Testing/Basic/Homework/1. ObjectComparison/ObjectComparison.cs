using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Перепишите код на использование Fluent Assertions.
        ClassicAssert.AreEqual(actualTsar.Name, expectedTsar.Name);
        ClassicAssert.AreEqual(actualTsar.Age, expectedTsar.Age);
        ClassicAssert.AreEqual(actualTsar.Height, expectedTsar.Height);
        ClassicAssert.AreEqual(actualTsar.Weight, expectedTsar.Weight);
        
        ClassicAssert.AreEqual(expectedTsar.Parent!.Name, actualTsar.Parent!.Name);
        ClassicAssert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
        ClassicAssert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
        ClassicAssert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
        
        

    }
    
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsarFluentAssertions()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));


        actualTsar.Should().BeEquivalentTo(expectedTsar, options => 
                options.Excluding((IMemberInfo p) => p.Name == nameof(Person.Id))
        );
        
        // Тест с FluentAssertions имеет следующие приемущества: 
        // 1) Автоматически расширяется при добавлении новых свойств в класс Person
        // 2) Лучшая читаемость кода
        // 3) При  ошибке видно сразу каке свойтво не совпало, например если изменить возраст в expectedTsar вывод будет содержать "Expected field actualTsar.Age to be 53, but found 54." 
        // а при Assert "Assert.That(actual, Is.EqualTo(expected)) Expected: 54 But was:  53" - не понятно какое именно свойтво провалило тест 

    }

    
    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
        
        // Недостатки такого подхода следущие: 
        // 1) Есть отдельный метод AreEqual и при изменении класса Person нужно будет идти в него менять т.е. сложно поддреживать
        // 2) В стек-трейсе нет информации о том, какое именно свойство не совпало
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
