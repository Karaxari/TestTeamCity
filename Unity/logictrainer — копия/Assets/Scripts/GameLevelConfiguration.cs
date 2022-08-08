using FullSerializer;
using System.Collections.Generic;

[System.Serializable]
public class GameLevelConfiguration
{
    public int level = -1;
    //#refactor. � ���� �������.
    //������� ��� ������� ������ ���� ����� Configuration � ��� �������� ��������� � �������. GameLevelConfiguration ����� �������� � ������ ���� ������� ��� �������
    [fsProperty] public Dictionary<string, Dictionary<string, Dictionary<string, int>>> gameLevelConfig = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
}
