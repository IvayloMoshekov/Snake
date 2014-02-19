using System;

class PlayList
{
    //static int C = 264;
    //static int D = 297;
    static int E = 330;
    static int F = 352;
    static int G = 396;
    static int A = 440;
    //    static int Bb = 466;
    static int B = 495;
    //    static int C2 = 528;

    static int note = 1000;
    static int half = note / 2;
    static int quarter = note / 4;
    static int eighth = note / 8;

    public void PlayGameOverSong()
    {
        Console.Beep(G, half);
        Console.Beep(F, half);
        Console.Beep(E, note);
    }

    public void PlayVictorySong()
    {
        Console.Beep(A, quarter);
        Console.Beep(A, quarter);
        Console.Beep(A, quarter);
        Console.Beep(B, half);
        Console.Beep(A, quarter);
        Console.Beep(B, note);
    }
}