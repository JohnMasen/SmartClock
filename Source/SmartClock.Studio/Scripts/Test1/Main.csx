var img = Loader.LoadImage("pic1.jpg");

Rotate(img, System.Random.Shared.Next(0, 180));
DrawImage(img,0,0,400,400);