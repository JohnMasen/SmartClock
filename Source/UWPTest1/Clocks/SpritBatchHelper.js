import { Color, MeasureTextBound } from "sdk@Plugin.Drawing";
export class UIControl {
    constructor(drawingArea) {
        this.Children = new Array();
        this.BoarderWidth = 1;
        this.BoarderColor = new Color("#ffffff");
        this.DrawBoarder = false;
        this.DrawingArea = drawingArea;
    }
    Draw(batch) {
        batch.PushMatrix();
        if (this.DrawBoarder) {
            batch.DrawRectangle(this.DrawingArea, this.BoarderColor, this.BoarderWidth);
        }
        batch.Translate(this.DrawingArea);
        this.onDraw(batch);
        this.Children.forEach(element => {
            element.Draw(batch);
        });
        batch.PopMatrix();
    }
}
export class CenteredText extends UIControl {
    onDraw(batch) {
        let rect = MeasureTextBound(this.Text, this.Font);
        batch.PushMatrix();
        let trans = {
            X: (this.DrawingArea.Width - (rect.Width + rect.X * 2)) / 2,
            Y: (this.DrawingArea.Height - (rect.Height + rect.Y * 2)) / 2
        };
        batch.Translate(trans);
        batch.DrawText({ X: 0, Y: 0 }, this.Text, this.Font, this.Color);
        batch.PopMatrix();
    }
    constructor(drawingArea, text, font, color) {
        super(drawingArea);
        this.Text = text;
        this.Font = font;
        this.Color = color;
    }
}
export class CenteredImage extends UIControl {
    onDraw(batch) {
        batch.PushMatrix();
        batch.Translate({
            X: (this.DrawingArea.Width - this.TargetSize.Width) / 2,
            Y: (this.DrawingArea.Height - this.TargetSize.Height) / 2
        });
        batch.DrawImage({ X: 0, Y: 0 }, this.TargetSize, this.Image, 1);
        batch.PopMatrix();
    }
    constructor(drawingArea, image) {
        super(drawingArea);
        this.Image = image;
        this.TargetSize = image.GetSize();
    }
}
export class Page extends UIControl {
    constructor(drawingArea) {
        super(drawingArea);
        this.BackGround = new Color("#000000");
    }
    onDraw(batch) {
        batch.Fill(this.BackGround, this.DrawingArea);
    }
}
