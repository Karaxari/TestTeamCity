using FullSerializer;
[System.Serializable]
public class GameSessionData
{
    [fsProperty] public int result;
    [fsProperty] public string mode;
    [fsProperty] public int level;

    public GameSessionData(int result, string mode, int level)
    {
        this.result = result;
        this.mode = mode;
        this.level = level;
    }
}

[System.Serializable]
public class LevelPoints
{
    [fsProperty] public int formula;
    [fsProperty] public int labyrinth;
    [fsProperty] public int merge;
    [fsProperty] public int uniqueDrawing;

    public LevelPoints(int formula, int labyrinth, int merge, int uniqueDrawing)
    {
        this.formula = formula;
        this.labyrinth = labyrinth;
        this.merge = merge;
        this.uniqueDrawing = uniqueDrawing;
    }
}


public class GameLevelData
{
    public int level;

    public GameLevelData()
    {
        level = -1;
    }
    public GameLevelData(int level)
    {
        this.level = level;
    }
}