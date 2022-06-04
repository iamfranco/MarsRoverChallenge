﻿namespace MarsRover.AppUI.Helpers;
public class InputReader
{
    public virtual string GetUserInput(string prompt)
    {
        Console.WriteLine();
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (input is null)
            throw new Exception($"Input cannot be null");

        return input;
    }
}