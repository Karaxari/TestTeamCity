using FullSerializer;
using System.Collections.Generic;

[System.Serializable]
public class GameLevelConfiguration
{
    public int level = -1;
    //#refactor. У меня обморок.
    //Сделать для каждого уровня свой класс Configuration и при парсинге раскидать в объекты. GameLevelConfiguration можно оставить и внутрь него загнать эти объекты
    [fsProperty] public Dictionary<string, Dictionary<string, Dictionary<string, int>>> gameLevelConfig = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
}
