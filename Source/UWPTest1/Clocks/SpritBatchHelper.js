import { Color, MeasureTextBound } from "sdk@Plugin.Drawing";
import { Echo } from "sdk@JSClock";
export class UIControl {
    constructor(position, size) {
        this.BoarderWidth = 1;
        this.BoarderColor = new Color("#ffffff");
        this.DrawBoarder = false;
        this.Position = position;
        this.Size = size;
    }
    Draw(batch) {
        batch.PushMatrix();
        if (this.DrawBoarder) {
            let rect = { X: this.Position.X, Y: this.Position.Y, Width: this.Size.Width, Height: this.Size.Height };
            // Echo("boarder="+JSON.stringify(rect));
            batch.DrawRectangle(rect, this.BoarderColor, this.BoarderWidth);
        }
        batch.Translate(this.Position);
        this.onDraw(batch);
        batch.PopMatrix();
    }
}
export class CenteredText extends UIControl {
    onDraw(batch) {
        let rect = MeasureTextBound(this.Text, this.Font);
        batch.PushMatrix();
        // Echo("rect="+JSON.stringify(rect));
        let trans = {
            X: (this.Size.Width - (rect.Width + rect.X * 2)) / 2,
            Y: (this.Size.Height - (rect.Height + rect.Y * 2)) / 2
        };
        batch.Translate(trans);
        Echo("translate=" + JSON.stringify(trans));
        // Echo("size="+JSON.stringify(this.Size));
        batch.DrawRectangle(rect, this.Color);
        batch.DrawText({ X: 0, Y: 0 }, this.Text, this.Font, this.Color);
        batch.PopMatrix();
    }
    constructor(position, size, text, font, color) {
        super(position, size);
        this.Text = text;
        this.Font = font;
        this.Color = color;
    }
}
export class ControlCollection {
    constructor() {
        this.items = new Array();
    }
    Add(control) {
        this.items.push(control);
    }
    Draw(batch) {
        this.items.forEach(element => {
            element.Draw(batch);
        });
    }
}
