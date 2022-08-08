using FullSerializer;
using System.Collections.Generic;

public class UserProgressData
{
    [fsProperty] private int currentLevel;
    [fsProperty] private string name;
    [fsProperty] private string surname;
    [fsProperty] private int totalPoints;
    [fsProperty] private Dictionary<string, LevelPoints> points;

    public UserProgressData(string name, string surname, int currentLevel)
    {
        this.currentLevel = currentLevel;
        this.name = name;
        this.surname = surname;
        //this.points = points;
    }

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public string Name { get => name; set => name = value; }
    public string Surname { get => surname; set => surname = value; }
    public int TotalPoints { get => totalPoints; set => totalPoints = value; }
    public Dictionary<string, LevelPoints> PointsDict { get => points; set => points = value; }

    public override string ToString()
    {
        return $"[name - {name}, currentLevel - {currentLevel}]";
    }
}
