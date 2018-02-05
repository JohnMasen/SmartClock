import { Size, Point, SpritBatch, Color, Font, MeasureTextBound } from "sdk@Plugin.Drawing";
import { Echo } from "sdk@JSClock";

export abstract class UIControl {
    Position:Point;
    Size:Size;
    constructor(position:Point, size:Size) {
        this.Position=position;
        this.Size=size;
    }
    Draw(batch:SpritBatch){
        batch.PushMatrix();
        if(this.DrawBoarder){
            let rect={X:this.Position.X,Y:this.Position.Y,Width:this.Size.Width,Height:this.Size.Height};
            // Echo("boarder="+JSON.stringify(rect));
            batch.DrawRectangle(
                rect
                ,this.BoarderColor
                ,this.BoarderWidth);
        }
        batch.Translate(this.Position);
        this.onDraw(batch);
        batch.PopMatrix();
    }
    abstract onDraw(batch:SpritBatch):void;
    BoarderWidth:number=1;
    BoarderColor:Color=new Color("#ffffff");
    DrawBoarder:boolean=false;
}

export class CenteredText extends UIControl {
    Text:string;
    Font:Font;
    Color:Color;
    onDraw(batch: SpritBatch): void {
        let rect=MeasureTextBound(this.Text,this.Font);
        batch.PushMatrix();
        // Echo("rect="+JSON.stringify(rect));
        let trans={
            X:(this.Size.Width-(rect.Width+rect.X*2))/2,
            Y:(this.Size.Height-(rect.Height+rect.Y*2))/2
        };
        batch.Translate(trans);
        Echo("translate="+JSON.stringify(trans));
        // Echo("size="+JSON.stringify(this.Size));
        batch.DrawRectangle(rect,this.Color);
        batch.DrawText({X:0,Y:0},this.Text,this.Font,this.Color);
        batch.PopMatrix();
    }
    constructor(position:Point, size:Size,text:string,font:Font,color:Color) {
        super(position,size);
        this.Text=text;
        this.Font=font;
        this.Color=color;
    }
}



export class ControlCollection {
    items:Array<UIControl>=new Array<UIControl>();
    Add(control:UIControl){
        this.items.push(control);
    }
    Draw(batch:SpritBatch){
       this.items.forEach(element => {
           element.Draw(batch);
       });
    }
}