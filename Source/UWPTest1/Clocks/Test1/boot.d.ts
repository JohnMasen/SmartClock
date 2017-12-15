declare const _api: any;
declare abstract class idObject {
    readonly id: number;
    constructor(id: number);
}
declare abstract class DrawWith extends idObject {
    protected _thickness: number;
    protected _type: DrawWithType;
    constructor(id: number, thickness: number, type: DrawWithType);
}
declare enum DrawWithType {
    brush = 0,
    pen = 1,
}
declare enum BlendMode {
    Normal = 0,
    Multiply = 1,
    Add = 2,
    Substract = 3,
    Screen = 4,
    Darken = 5,
    Lighten = 6,
    Overlay = 7,
    HardLight = 8,
    Src = 9,
    Atop = 10,
    Over = 11,
    In = 12,
    Out = 13,
    Dest = 14,
    DestAtop = 15,
    DestOver = 16,
    DestIn = 17,
    DestOut = 18,
    Clear = 19,
    Xor = 20,
}
interface Point {
    x: number;
    y: number;
}
interface Size {
    width: number;
    height: number;
}
interface Rectangle {
    x: number;
    y: number;
    width: number;
    height: number;
}
declare function setAA(isEnabled: boolean): void;
declare class JSColor {
    readonly hexString: string;
    constructor(hex: string);
}
declare abstract class JSBrush extends DrawWith {
    Thickness: number;
    constructor(id: number, thickness?: number);
    static createSolid(color: JSColor): JSSolidBrush;
}
declare class JSSolidBrush extends JSBrush {
    readonly color: JSColor;
    constructor(color: JSColor);
}
declare class JSFont extends idObject {
    readonly family: string;
    readonly size: number;
    static Install(path: string): void;
    constructor(family: string, size: number);
    MeasureText(text: string): Size;
}
declare class JSMatrix extends idObject {
    constructor();
    push(): void;
    pop(): void;
    translate(position: Point): void;
    scale(size: Point): void;
    rotate(degrees: number, center?: Point): void;
    reset(): void;
}
declare class JSImage extends idObject {
    static Load(path: string, isPersistent?: boolean): JSImage;
    static Create(width: number, height: number, isPersistent?: boolean): JSImage;
    readonly size: Size;
    readonly matrix: JSMatrix;
    private constructor();
    DrawLines(drawWith: DrawWith, points: Point[]): void;
    Fill(brush: JSBrush): void;
    SetOutput(name?: string): void;
    DrawImage(texture: JSImage, blend?: BlendMode, percent?: number, size?: Size, location?: Point): void;
    DrawText(text: string, font: JSFont, drawWith: DrawWith, location: Point): void;
    DrawEclipse(drawWith: DrawWith, location: Point, size: Size): void;
    FillEclipse(brush: JSBrush, location: Point, size: Size): void;
    DrawPolygon(drawWith: DrawWith, points: Point[]): void;
    FillPolygon(brush: JSBrush, points: Point[]): void;
}
