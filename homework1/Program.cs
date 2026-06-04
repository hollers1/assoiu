using System;

while (true)
{
    Console.Write("Введите первую строку (или 'exit' для выхода): ");
    var first = Console.ReadLine();

    if (string.Equals(first, "exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Завершение программы.");
        break;
    }

    Console.Write("Введите вторую строку: ");
    var second = Console.ReadLine();

    var distance = LevenshteinDistance(first ?? string.Empty, second ?? string.Empty);
    Console.WriteLine($"Расстояние Левенштейна: {distance}");
    Console.WriteLine();
}

static int LevenshteinDistance(string source, string target)
{
    var rows = source.Length + 1;
    var cols = target.Length + 1;

    var matrix = new int[rows, cols];

    for (var i = 0; i < rows; i++)
    {
        matrix[i, 0] = i;
    }

    for (var j = 0; j < cols; j++)
    {
        matrix[0, j] = j;
    }

    for (var i = 1; i < rows; i++)
    {
        for (var j = 1; j < cols; j++)
        {
            var cost = source[i - 1] == target[j - 1] ? 0 : 1;

            matrix[i, j] = Math.Min(
                Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                matrix[i - 1, j - 1] + cost);
        }
    }

    return matrix[rows - 1, cols - 1];
}
