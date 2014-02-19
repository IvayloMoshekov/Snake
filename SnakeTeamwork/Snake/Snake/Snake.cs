using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

class Snake
{
    private Queue<Element> snakeElements;
    private List<Element> directions;
    private static int direction = 1; //at first snake moves to the right
    private Element newSnakeElement; //the head of the snake

    public Snake()
    {
        SetDefaultSnakeSize();
        SetAvailableDirections();
        Display();
    }

    public Snake(Queue<Element> snakeElements)
    {
        this.snakeElements = snakeElements;
        SetAvailableDirections();
        Display();
    }

    private void SetDefaultSnakeSize()
    {
        int startX = (Console.WindowWidth / 2) - 3;
        int startY = Console.WindowHeight / 2;

        snakeElements = new Queue<Element>();
        for (int i = 0; i < 5; i++)
        {
            snakeElements.Enqueue(new Element(startY, startX + i, '#'));
        }
    }

    private void SetAvailableDirections()
    {
        directions = new List<Element>();
        directions.Add(new Element(0, -1)); //move left
        directions.Add(new Element(0, 1)); //move right
        directions.Add(new Element(-1, 0)); //move up 
        directions.Add(new Element(1, 0)); //move down
    }

    private void Display()
    {
        foreach (Element element in snakeElements)
        {
            Console.SetCursorPosition(element.col, element.row);
            Console.Write(element.symbol);
        }
    }

    public void Move()
    {
        try
        {
            //get the last element in the queue (the snake head)
            Element snakeHead = snakeElements.Last();

            //get the next direction requested by the user's input
            Element nextElement = directions[direction];

            //set new element's parameters
            newSnakeElement = new Element(snakeHead.row + nextElement.row, snakeHead.col + nextElement.col, snakeHead.symbol);
            snakeElements.Enqueue(newSnakeElement);

            //check if snake bite itself
            isBitten(newSnakeElement, snakeElements);

            //check if snake has hit the window frame
            HasHitTheFrame(newSnakeElement);

            //display snake if everything is ok
            Display();
        }
        catch (ArgumentException)
        {
            throw new GameException();
        }
    }

    //gets the snake elements without the head and check whether the head hits any of those elements.
    private static void isBitten(Element snakeHead, Queue<Element> snakeElements)
    {
        for (int i = 0; i < snakeElements.Count - 1; i++)
        {
            if (snakeElements.ElementAt(i).row == snakeHead.row && snakeElements.ElementAt(i).col == snakeHead.col)
            {
                throw new GameException();
            }
        }
    }

    //gets snake's head and checks whether it's coordinates are within the game field.
    private static void HasHitTheFrame(Element snakeHead)
    {
        if (snakeHead.row == 2 || snakeHead.row > Console.WindowHeight || snakeHead.col > Console.WindowWidth || snakeHead.col < 0)
        {
            throw new GameException();
        }
    }

    // get current direction of the snake
    public int GetCurrentDirection()
    {
        return direction;
    }

    // set the direction of the snake
    public void SetCurrentDirection(int dir)
    {
        direction = dir;
    }

    public Element GetSnakeHead()
    {
        return newSnakeElement;
    }

    public List<Element> GetSnakeElements()
    {
        List<Element> snakeParts = new List<Element>();
        foreach (Element element in snakeElements)
        {
            snakeParts.Add(element);
        }
        return snakeParts;
    }

    public void RemoveSnakeTail()
    {
        //empty space to avoid Console.Clear() method.
        Element elementToRemove = snakeElements.Dequeue();
        Console.SetCursorPosition(elementToRemove.col, elementToRemove.row);
        Console.Write(" ");
    }

}

