import { Size, Point, SpritBatch, Color, Font, MeasureTextBound, ITexture, Rectangle } from "sdk@Plugin.Drawing";
import { Echo } from "sdk@JSClock";

export abstract class UIControl {
    Children:Array<UIControl>=new Array<UIControl>();
    DrawingArea:Rectangle;
    constructor(drawingArea:Rectangle) {
        this.DrawingArea=drawingArea;
    }
    Draw(batch:SpritBatch){
        batch.PushMatrix();
        if(this.DrawBoarder){
            batch.DrawRectangle(
                this.DrawingArea
                ,this.BoarderColor
                ,this.BoarderWidth);
        }
        batch.Translate(this.DrawingArea);
        this.onDraw(batch);
        this.Children.forEach(element => {
            element.Draw(batch);
        });
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
        let trans={
            X:(this.DrawingArea.Width-(rect.Width+rect.X*2))/2,
            Y:(this.DrawingArea.Height-(rect.Height+rect.Y*2))/2
        };
        batch.Translate(trans);
        batch.DrawText({X:0,Y:0},this.Text,this.Font,this.Color);
        batch.PopMatrix();
    }
    constructor(drawingArea:Rectangle,text:string,font:Font,color:Color) {
        super(drawingArea);
        this.Text=text;
        this.Font=font;
        this.Color=color;
    }
}

export class CenteredImage extends UIControl {
    Image:ITexture;
    TargetSize:Size;
    onDraw(batch: SpritBatch): void {
        batch.PushMatrix();
        batch.Translate({
            X:(this.DrawingArea.Width-this.TargetSize.Width)/2,
            Y:(this.DrawingArea.Height-this.TargetSize.Height)/2
        });
        batch.DrawImage({X:0,Y:0},this.TargetSize,this.Image,1);
        batch.PopMatrix();
    }   
    constructor(drawingArea:Rectangle,image:ITexture) {
        super(drawingArea);
        this.Image=image;
        this.TargetSize=image.GetSize();
    }
}




export class Page extends UIControl {
    BackGround:Color=new Color("#000000");
    onDraw(batch: SpritBatch): void {
        batch.Fill(this.BackGround,this.DrawingArea);
    }
    constructor(drawingArea:Rectangle) {
        super(drawingArea);
    }
}