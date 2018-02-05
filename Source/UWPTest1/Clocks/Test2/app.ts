import { ClockApp, GetInfo } from "sdk@JSClock";
import { SpritBatch, GetDrawingSurface, BlendModeEnum, Color, Size, Rectangle, Font, LoadFont } from "sdk@Plugin.Drawing";
import { GetCurrent } from "xinzhi";
import { UIControl, ControlCollection, CenteredText } from "SpritBatchHelper";

export class App implements ClockApp
{
    sb:SpritBatch;
    drawRegion:Rectangle={X:0,Y:0,Width:400,Height:300};
    simhei:Font;
    Draw(): void {
        let sb=this.sb;
        let white=new Color("#ffffff");
        let black=new Color("#000000");
        let red=new Color("#FF0000");
        let current=GetCurrent();
        let controls:ControlCollection=new ControlCollection();
        let simhei60=this.simhei;
        sb.ResetMatrix();
        simhei60.Size=60;
        let test1=new CenteredText({X:0,Y:0},{Width:200,Height:200},"Hello",simhei60,white);
        test1.DrawBoarder=true;
        
         controls.Add(test1);
        

        sb.Begin(BlendModeEnum.Normal);
        sb.Fill(black,this.drawRegion);
        // sb.DrawText({"X":27.578125,"Y":79.7265625},"Hello",simhei60,red);
        // sb.DrawRectangle({"X":27.578125,"Y":79.7265625,Width:144.84375,"Height":40.546875},red);
        // test1.Draw(sb);
        controls.Draw(sb);
        sb.End();
    }
    Init(): void {
        let surface=GetDrawingSurface(this.drawRegion,"0.1");
        this.sb=surface.CreateSpritBatch();
        this.simhei=LoadFont("simhei.ttf");
    }
    
}