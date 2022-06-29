

if (IsFirstRun)
{
    LoadFont("Arvo-Regular.ttf");
}

var img = Loader.LoadImage("pic1.jpg");

//Rotate(img, System.Random.Shared.Next(0, 180));
DrawImage(img,0,0,400,400);
DrawText(ClockTime.ToString(), "Arvo", 16, 0, 0, "#FCF6FA");
DrawText(IsFirstRun.ToString(), "Arvo", 16, 0, 150, "#FCF6FA");