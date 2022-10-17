

if (IsFirstRun)
{
    LoadFont("Arvo-Regular.ttf");
    LoadFont("TYPEWR.TTF");
}

var img = Loader.LoadImage("Pic1.jpg");

//Rotate(img, System.Random.Shared.Next(0, 180));
DrawImage(img,0,0,800,600);
DrawText(ClockTime.ToString("yyyy/MM/dd"), "Arvo", 64, 0, 0, "#FCF6FA");
DrawText(ClockTime.ToString("HH:mm:ss"), "Typewriter", 64, 0, 120, "#FCF6FA");
DrawText(IsFirstRun.ToString(), "Arvo", 16, 0, 150, "#FCF6FA");
bool isAM;
switch(ClockTime.Hour)
{
    case <= 12:
        isAM = true;
        break;
    default:
        isAM = false;
        break;
}
DrawText($"isAM {isAM.ToString()}", "Arvo", 16, 0, 200, "#FCF6FA");