public class GameModes
{
    //public const string FORMULA = "Formula";
    //public const string LABYRINTH = "Labyrinth";
    //public const string MERGE = "Merge";
    //public const string UNIQUE_DRAWING = "UniqueDrawing";

    public const string formula = "formula";
    public const string labyrinth = "labyrinth";
    public const string merge = "merge";
    public const string uniqueDrawing = "uniqueDrawing";
}

public class GameConfig
{
    public const string level = "level"; // уровень
    public const string colAnswer = "colAnswer"; // количество ответов
    public const string correctAnswer = "correctAnswer"; // очки за правильный ответ
    public const string numberValue = "numberValue"; // числа используемые в операциях
    public const string sign = "sign"; // кроичесвто арифметических операций
    public const string numberTerms = "numberTerms"; // количество операндов
    public const string time = "time"; // время раунда на уровне
    public const string wrongAnswer = "wrongAnswer"; // штраф очков за неправильный ответ
    public const string easyPoints = "easyPoints"; // легкое прохождение
    public const string normalPoints = "normalPoints"; // среднее прохождение
    public const string hardPoints = "hardPoints"; // сложное прохождение
    public const string diversitySprite = "diversitySprite"; // коэффициент разнообразия спрайтов (не более 9) (может сочитаться с 3 параметрами в самой игре: "Вид фигуры, форма фигур, цвет фигур") ///!!!Согласовать использование этого парамента на уровнях!!!
    public const string heightMesh = "heightMesh"; // высота таблицы
    public const string sizeMesh = "sizeMesh"; // размер матриц используемых в операциях (не более 6)
    public const string widthMesh = "widthMesh"; // ширина таблицы
    public const string numberSprite = "numberSprite"; // Вариативность условий поворота (не более 28) (1-4 - "Одинарные перемещения", 5-12 - "С двумя поворотами", 13-28 - "Три поворота")
    public const string difficultyLevel = "difficultyLevel"; // Уроыень разнообразия критериев для фигур в игре uniqueDrawing
}