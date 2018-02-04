import { ClockApp } from "sdk@JSClock";
import { SpritBatch, GetDrawingSurface, BlendModeEnum, Color, Size, Rectangle } from "sdk@Plugin.Drawing";

export class App implements ClockApp
{
    sb:SpritBatch;
    drawRegion:Rectangle={X:0,Y:0,Width:400,Height:300};
    Draw(): void {
        let sb=this.sb;
        let white=new Color("#ffffff");
        let black=new Color("#000000");
        sb.Begin(BlendModeEnum.Normal);
        sb.Fill(black,this.drawRegion);
        sb.DrawEclipse({X:100,Y:100},{Width:100,Height:100},white);
        sb.End();
    }
    Init(): void {
        let surface=GetDrawingSurface(this.drawRegion,"0.1");
        this.sb=surface.CreateSpritBatch();
    }
    
}