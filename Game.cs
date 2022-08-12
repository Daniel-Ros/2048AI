using Godot;
using System;

public class Game : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    private float offset = 0.11f;
    [Export]
    private float size = 0.18f;

    public int[,] table;
    private DynamicFont font;

    enum moves
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private moves DecideBestMove(int lvl)
    {
        moves bestMove;
        int score = -1;
        int tmpScore = -1;


        var orig = makeDup();
        moveLeft();
        score = ScoreMove(lvl - 1);
        bestMove = moves.LEFT;

        setTable(orig);
        moveRight();
        tmpScore = ScoreMove(lvl - 1);
        if (tmpScore > score)
        {
            score = tmpScore;
            bestMove = moves.RIGHT;
        }
        setTable(orig);
        moveUp();
        tmpScore = ScoreMove(lvl - 1);
        if (tmpScore > score)
        {
            score = tmpScore;
            bestMove = moves.UP;
        }
        setTable(orig);
        moveDown();
        tmpScore = ScoreMove(lvl - 1);
        if (tmpScore > score)
        {
            score = tmpScore;
            bestMove = moves.DOWN;
        }
        setTable(orig);
        return bestMove;
    }

    private int ScoreMove(int lvl)
    {
        if (lvl == 0)
        {
            int maxTile = -1;
            int ret = 0;
            float multi = 1;
            for (int i = 0; i < 4; i++) // should make the tiles go left.
            {
                for (int j = 0; j < 4; j++)
                {
                    ret += table[i, j] * (int)Math.Log(table[i, j], 2);
                    if (table[i, j] > maxTile) maxTile = table[i, j];

                    for (int x = Math.Max(0, i - 1); x < Math.Min(j + 1, 4); x++) // should make the tiles go left.
                    {
                        for (int y = Math.Max(0, i - 1); y < Math.Min(j + 1, 4); y++)
                        {
                            if (table[x, y] == table[i, j]) multi += 0.5f;
                            else if (table[x, y] * 2 == table[i, j]) multi += 0.25f;
                            else
                            {
                                multi -= 0.5f;
                            }
                        }
                    }
                }
            }
            if (multi < 0.5f) multi = 0.5f;
            ret = (int)Math.Floor(ret * multi);
            if (table[0, 0] == maxTile || table[3, 0] == maxTile || table[0, 3] == maxTile || table[3, 3] == maxTile)
                ret *= 100;
            else
                ret = (int)(ret / 2);

            return ret;
        }
        if (testLose()) return 0;
        int score = -1;
        int tmpScore = -1;


        var orig = makeDup();
        moveLeft();
        score = ScoreMove(lvl - 1);
        setTable(orig);
        moveRight();
        tmpScore = ScoreMove(lvl - 1);
        if (tmpScore > score)
        {
            score = tmpScore;
        }
        setTable(orig);
        moveUp();
        tmpScore = ScoreMove(lvl - 1);
        if (tmpScore > score)
        {
            score = tmpScore;
        }
        setTable(orig);
        moveDown();
        tmpScore = ScoreMove(lvl - 1);
        if (tmpScore > score)
        {
            score = tmpScore;
        }
        setTable(orig);
        return score;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        font = new DynamicFont();
        font.FontData = (DynamicFontData)GD.Load("res://Hack.ttf");
        font.Size = 20;
        table = new int[4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                table[i, j] = 0;
            }
        }

        Random rng = new Random();
        int x = rng.Next(4);
        int y = rng.Next(4);
        table[x, y] = 2;
        x = rng.Next(4);
        y = rng.Next(4);
        table[x, y] = 2;
    }

    public override void _Draw()
    {
        var vp = GetViewport();
        float width = vp.Size.x;
        float height = vp.Size.y;
        Rect2 rect = new Rect2(0.1f * width, 0.1f * height, width - 0.2f * width,
                               height - 0.2f * height);
        DrawRect(rect, Colors.Gray);
        DrawCells();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previousframe
    public override void _Process(float delta)
    {
        Update();
        var nextMove = DecideBestMove(6);
        switch (nextMove)
        {
            case moves.LEFT:
                moveLeft();
                break;
            case moves.RIGHT:
                moveRight();
                break;
            case moves.UP:
                moveUp();
                break;
            case moves.DOWN:
                moveDown();
                break;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            moveLeft();
            if (testLose()) GD.Print("lost");
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            moveRight();
            if (testLose()) GD.Print("lost");
        }
        else if (Input.IsActionJustPressed("ui_up"))
        {
            moveUp();
            if (testLose()) GD.Print("lost");
        }
        else if (Input.IsActionJustPressed("ui_down"))
        {
            moveDown();
            if (testLose()) GD.Print("lost");
        }
        else if (Input.IsActionJustPressed("ui_accept"))
        {
            _Ready();
        }
    }

    public void moveLeft()
    {

        MakeMoveLeft();

    }

    public void moveRight()
    {
        mirror();
        MakeMoveLeft();
        mirror();
    }
    public void moveUp()
    {
        transpose();
        MakeMoveLeft();
        transpose();
    }
    public void moveDown()
    {
        transpose();
        mirror();
        MakeMoveLeft();
        mirror();
        transpose();
    }

    private void MakeMoveLeft()
    {

        var dup = makeDup();
        shiftLeft();
        if (mergeLeft() != 0)
        {
            shiftLeft();
        };
        if (testDup(dup))
        {
            return;
        }
        while (!addTile()) ;


    }


    private bool addTile()
    {
        Random rng = new Random();
        for (int j = 3; j > 0; j--)
        {
            for (int i = 0; i < 4; i++) // should make the tiles go left.
            {
                if (table[i, j] != 0) continue;
                if (rng.Next(101) > 75)
                {
                    if (rng.Next(101) > 90)
                    {
                        table[i, j] = 4;
                    }
                    else
                    {
                        table[i, j] = 2;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private void shiftLeft()
    {
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                int x = j;
                while (x < 4 && table[i, x] == 0) x++;
                if (x >= 4 || x == j) continue;
                // GD.Print($"swap {i},{j} - {i},{x}");
                (table[i, j], table[i, x]) = (table[i, x], table[i, j]);
            }
        }
    }

    private int mergeLeft()
    {
        int counter = 0;
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 3; j++)
            {
                if (table[i, j] == table[i, j + 1] && table[i, j] != 0)
                {
                    table[i, j] *= 2;
                    table[i, j + 1] = 0;
                    counter++;
                }
            }
        }
        // GD.Print(counter);
        return counter;
    }

    private void mirror()
    {
        int[,] tmp = new int[4, 4];
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                tmp[i, j] = table[i, 3 - j];
            }
        }

        table = tmp;
    }

    private void transpose()
    {
        int[,] tmp = new int[4, 4];
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                tmp[i, j] = table[j, i];
            }
        }
        table = tmp;
    }


    private int[,] makeDup()
    {
        int[,] tmp = new int[4, 4];
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                tmp[i, j] = table[i, j];
            }
        }
        return tmp;
    }

    private bool testDup(int[,] dup)
    {
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                if (dup[i, j] != table[i, j]) return false;
            }
        }
        return true; ;
    }

    private void setTable(int[,] tmp)
    {
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                table[i, j] = tmp[i, j];
            }
        }
    }

    private bool testLose()
    {
        int availableSides = 0;
        // printTable(table);
        var orig = makeDup();
        moveLeft();
        if (testDup(orig)) availableSides++;
        setTable(orig);
        moveRight();
        if (testDup(orig)) availableSides++;
        setTable(orig);
        moveUp();
        if (testDup(orig)) availableSides++;
        setTable(orig);
        moveDown();
        if (testDup(orig)) availableSides++;
        setTable(orig);
        return availableSides == 4;
    }

    private void printTable(int[,] t)
    {
        string str = "";
        for (int i = 0; i < 4; i++) // should make the tiles go left.
        {
            for (int j = 0; j < 4; j++)
            {
                str += t[i, j].ToString() + " ";
            }
            str += "\n";
        }
        GD.Print(str);
    }

    private void DrawCells()
    {
        var vp = GetViewport();
        float width = vp.Size.x;
        float height = vp.Size.y;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float x = offset * width + j * 0.2f * width;
                float y = offset * height + i * 0.2f * height;

                Rect2 rect = new Rect2(x, y, size * width, size * height);
                Color c = Colors.Lavender;
                switch (table[i, j])
                {
                    case 0:
                        c = new Color(205.0f / 255.0f, 193 / 255.0f, 180.0f / 255.0f);
                        break;
                    case 2:
                        c = new Color(238.0f / 255.0f, 228.0f / 255.0f, 218.0f / 255.0f);
                        break;
                    case 4:
                        c = new Color(237.0f / 255.0f, 224.0f / 255.0f, 200.0f / 255.0f);
                        break;
                    case 8:
                        c = new Color(242.0f / 255.0f, 177.0f / 255.0f, 121.0f / 255.0f);
                        break;
                    case 16:
                        c = new Color(245.0f / 255.0f, 149.0f / 255.0f, 99.0f / 255.0f);
                        break;
                    case 32:
                        c = new Color(246.0f / 255.0f, 124.0f / 255.0f, 95.0f / 255.0f);
                        break;
                    case 64:
                        c = new Color(246.0f / 255.0f, 94.0f / 255.0f, 59.0f / 255.0f);
                        break;
                    case 128:
                        c = new Color(237.0f / 255.0f, 207.0f / 255.0f, 114.0f / 255.0f);
                        break;
                    case 256:
                        c = new Color(237.0f / 255.0f, 204.0f / 255.0f, 97.0f / 255.0f);
                        break;
                    case 512:
                        c = new Color(237.0f / 255.0f, 200.0f / 255.0f, 80.0f / 255.0f);
                        break;
                    case 1024:
                        c = new Color(237.0f / 255.0f, 197.0f / 255.0f, 63.0f / 255.0f);
                        break;
                    default:
                        c = new Color(237.0f / 255.0f, 194.0f / 255.0f, 46.0f / 255.0f);
                        break;
                }
                DrawRect(rect, c);
                if (table[i, j] != 0)
                    DrawString(font, new Vector2(x + size * width / 2, y + size * height / 2), table[i, j].ToString(), Colors.Black);
            }
        }
    }
}
