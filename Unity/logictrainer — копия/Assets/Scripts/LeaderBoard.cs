[System.Serializable]
public class LeaderBoard
{
    public int score;
    public string name;

    public LeaderBoard(int score, string name)
    {
        this.score = score;
        this.name = name;
    }
    public int Score { get => score; set => score = value; }
    public string Name { get => name; set => name = value; }
}
