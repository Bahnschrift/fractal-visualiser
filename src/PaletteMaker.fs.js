import { createAtom } from "./.fable/fable-library.3.1.1/Util.js";
import { parse } from "./.fable/fable-library.3.1.1/Int32.js";
import { findCookieValue } from "./Cookies.fs.js";
import { parse as parse_1 } from "./.fable/fable-library.3.1.1/Double.js";
import { append, map } from "./.fable/fable-library.3.1.1/Array.js";
import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { item, ofArray } from "./.fable/fable-library.3.1.1/List.js";
import { rangeNumber, getEnumerator } from "./.fable/fable-library.3.1.1/Seq.js";

export const WIDTH = 1250;

export const HEIGHT = 150;

export const points = createAtom([]);

export const scaledPoints = createAtom([]);

export function interpolateLinear(y1, y2, mu) {
    return (y1 * (1 - mu)) + (y2 * mu);
}

export function interpolateCosine(y1, y2, mu) {
    const mu2 = (1 - Math.cos(mu * 3.141592653589793)) / 2;
    return (y1 * (1 - mu2)) + (y2 * mu2);
}

export function interpolateCubic(y0, y1, y2, y3, mu) {
    const mu2 = (mu * mu) | 0;
    const a0 = (((y3 - y2) - y0) + y1) | 0;
    return (((((a0 * mu) * mu2) + (((y0 - y1) - a0) * mu2)) + ((y2 - y0) * mu)) + y1) | 0;
}

export function interpolateCatmull(y0, y1, y2, y3, mu) {
    const mu2 = mu * mu;
    return ((((((((-0.5 * y0) + (1.5 * y1)) - (1.5 * y2)) + (0.5 * y3)) * mu) * mu2) + ((((y0 - (2.5 * y1)) + (2 * y2)) - (0.5 * y3)) * mu2)) + (((-0.5 * y0) + (0.5 * y2)) * mu)) + y1;
}

const TENSION = 1;

const BIAS = -0;

export function interpolateHermite(y0, y1, y2, y3, mu) {
    const mu2 = mu * mu;
    const mu3 = mu2 * mu;
    return ((((((2 * mu3) - (3 * mu2)) + 1) * y1) + (((mu3 - (2 * mu2)) + mu) * (((((y1 - y0) * (1 + BIAS)) * (1 - TENSION)) / 2) + ((((y2 - y1) * (1 - BIAS)) * (1 - TENSION)) / 2)))) + ((mu3 - mu2) * (((((y2 - y1) * (1 + BIAS)) * (1 - TENSION)) / 2) + ((((y3 - y2) * (1 - BIAS)) * (1 - TENSION)) / 2)))) + (((-2 * mu3) + (3 * mu2)) * y2);
}

export function hexToRGB(hex) {
    let hex_1 = hex;
    if (hex_1[0] === "#") {
        hex_1 = hex_1.slice(1, hex_1.length);
    }
    return [parse(hex_1.slice(0, 1 + 1), 511, false, 32, 16) / 255, parse(hex_1.slice(2, 3 + 1), 511, false, 32, 16) / 255, parse(hex_1.slice(4, 5 + 1), 511, false, 32, 16) / 255];
}

export const fieldP1X = document.getElementById("p1x");

export const fieldP1C = document.getElementById("p1c");

export const fieldP2X = document.getElementById("p2x");

export const fieldP2C = document.getElementById("p2c");

export const fieldP3X = document.getElementById("p3x");

export const fieldP3C = document.getElementById("p3c");

export const fieldP4X = document.getElementById("p4x");

export const fieldP4C = document.getElementById("p4c");

export const fieldP5X = document.getElementById("p5x");

export const fieldP5C = document.getElementById("p5c");

export const buttonResetpalette = document.getElementById("resetpalette");

export const cookieP1X = findCookieValue("p1x");

export const cookieP1C = findCookieValue("p1c");

export const cookieP2X = findCookieValue("p2x");

export const cookieP2C = findCookieValue("p2c");

export const cookieP3X = findCookieValue("p3x");

export const cookieP3C = findCookieValue("p3c");

export const cookieP4X = findCookieValue("p4x");

export const cookieP4C = findCookieValue("p4c");

export const cookieP5X = findCookieValue("p5x");

export const cookieP5C = findCookieValue("p5c");

export function updatePoints() {
    const patternInput = hexToRGB(fieldP1C.value);
    const p1R = patternInput[0];
    const p1G = patternInput[1];
    const p1B = patternInput[2];
    const patternInput_1 = hexToRGB(fieldP2C.value);
    const patternInput_2 = hexToRGB(fieldP3C.value);
    const patternInput_3 = hexToRGB(fieldP4C.value);
    const patternInput_4 = hexToRGB(fieldP5C.value);
    points([[parse_1(fieldP1X.value), p1R, p1G, p1B], [parse_1(fieldP2X.value), patternInput_1[0], patternInput_1[1], patternInput_1[2]], [parse_1(fieldP3X.value), patternInput_2[0], patternInput_2[1], patternInput_2[2]], [parse_1(fieldP4X.value), patternInput_3[0], patternInput_3[1], patternInput_3[2]], [parse_1(fieldP5X.value), patternInput_4[0], patternInput_4[1], patternInput_4[2]], [1, p1R, p1G, p1B]], true);
    scaledPoints(map((tupledArg) => [tupledArg[0] * WIDTH, tupledArg[1] * HEIGHT, tupledArg[2] * HEIGHT, tupledArg[3] * HEIGHT], points()), true);
    fieldP2X.min = fieldP1X.value;
    fieldP2X.max = fieldP3X.value;
    fieldP3X.min = fieldP2X.value;
    fieldP3X.max = fieldP4X.value;
    fieldP4X.min = fieldP3X.value;
    fieldP4X.max = fieldP5X.value;
    fieldP5X.min = fieldP4X.value;
    const arg10 = fieldP1X.value;
    document.cookie = toText(printf("p1x=%s"))(arg10);
    const arg10_1 = fieldP1C.value;
    document.cookie = toText(printf("p1c=%s"))(arg10_1);
    const arg10_2 = fieldP2X.value;
    document.cookie = toText(printf("p2x=%s"))(arg10_2);
    const arg10_3 = fieldP2C.value;
    document.cookie = toText(printf("p2c=%s"))(arg10_3);
    const arg10_4 = fieldP3X.value;
    document.cookie = toText(printf("p3x=%s"))(arg10_4);
    const arg10_5 = fieldP3C.value;
    document.cookie = toText(printf("p3c=%s"))(arg10_5);
    const arg10_6 = fieldP4X.value;
    document.cookie = toText(printf("p4x=%s"))(arg10_6);
    const arg10_7 = fieldP4C.value;
    document.cookie = toText(printf("p4c=%s"))(arg10_7);
    const arg10_8 = fieldP5X.value;
    document.cookie = toText(printf("p5x=%s"))(arg10_8);
    const arg10_9 = fieldP5C.value;
    document.cookie = toText(printf("p5c=%s"))(arg10_9);
}

export const canv = document.getElementById("palettecanv");

canv.width = WIDTH;

canv.height = HEIGHT;

export const ctx = canv.getContext('2d');

const set$ = [ofArray([4, 0, 1, 2]), ofArray([0, 1, 2, 3]), ofArray([1, 2, 3, 4]), ofArray([2, 3, 4, 0]), ofArray([3, 4, 0, 1])];

function getNeighbouring(x) {
    let tupledArg_1;
    let minpoint = 0;
    while (x > scaledPoints()[minpoint + 1][0]) {
        minpoint = (minpoint + 1);
    }
    if (minpoint === (scaledPoints().length - 1)) {
        minpoint = (minpoint - 1);
    }
    const p = set$[minpoint];
    return [scaledPoints()[item(0, p)], scaledPoints()[item(1, p)], (minpoint < 4) ? scaledPoints()[item(2, p)] : (tupledArg_1 = scaledPoints()[item(2, p)], [tupledArg_1[0] + WIDTH, tupledArg_1[1], tupledArg_1[2], tupledArg_1[3]]), scaledPoints()[item(3, p)]];
}

export function drawpalette() {
    const enumerator = getEnumerator(rangeNumber(0, 1, WIDTH));
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const x = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const patternInput = getNeighbouring(x);
            const p1x = patternInput[1][0];
            const mul = (x - p1x) / (patternInput[2][0] - p1x);
            const yInterpR = interpolateHermite(patternInput[0][1], patternInput[1][1], patternInput[2][1], patternInput[3][1], mul);
            const yInterpG = interpolateHermite(patternInput[0][2], patternInput[1][2], patternInput[2][2], patternInput[3][2], mul);
            const yInterpB = interpolateHermite(patternInput[0][3], patternInput[1][3], patternInput[2][3], patternInput[3][3], mul);
            const arg30 = (yInterpB / HEIGHT) * 255;
            const arg20 = (yInterpG / HEIGHT) * 255;
            const arg10 = (yInterpR / HEIGHT) * 255;
            ctx.fillStyle = toText(printf("rgb(%f, %f, %f)"))(arg10)(arg20)(arg30);
            ctx.fillRect(x, 0, 1, HEIGHT);
            ctx.fillStyle = "rgb(255, 0, 0)";
            ctx.fillRect(x, (HEIGHT - yInterpR) - 1, 1, 1);
            ctx.fillStyle = "rgb(0, 255, 0)";
            ctx.fillRect(x, (HEIGHT - yInterpG) - 1, 1, 1);
            ctx.fillStyle = "rgb(0, 0, 255)";
            ctx.fillRect(x, (HEIGHT - yInterpB) - 1, 1, 1);
        }
    }
    finally {
        enumerator.Dispose();
    }
    for (let idx = 0; idx <= (scaledPoints().length - 1); idx++) {
        const point = scaledPoints()[idx];
        const x_1 = point[0];
        ctx.fillStyle = "rgb(255, 0, 0)";
        ctx.fillRect(x_1 - 3, (HEIGHT - point[1]) - 3, 5, 5);
        ctx.fillStyle = "rgb(0, 255, 0)";
        ctx.fillRect(x_1 - 3, (HEIGHT - point[2]) - 3, 5, 5);
        ctx.fillStyle = "rgb(0, 0, 255)";
        ctx.fillRect(x_1 - 3, (HEIGHT - point[3]) - 3, 5, 5);
    }
}

export function getColour(x) {
    const patternInput = getNeighbouring(x);
    const p1x = patternInput[1][0];
    const mul = (x - p1x) / (patternInput[2][0] - p1x);
    return [(interpolateHermite(patternInput[0][1], patternInput[1][1], patternInput[2][1], patternInput[3][1], mul) / HEIGHT) * 255, (interpolateHermite(patternInput[0][2], patternInput[1][2], patternInput[2][2], patternInput[3][2], mul) / HEIGHT) * 255, (interpolateHermite(patternInput[0][3], patternInput[1][3], patternInput[2][3], patternInput[3][3], mul) / HEIGHT) * 255];
}

export function getColours(n) {
    let cols = [];
    const enumerator = getEnumerator(rangeNumber(0, WIDTH / n, WIDTH - 1));
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const x = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            cols = append(cols, [getColour(x)]);
        }
    }
    finally {
        enumerator.Dispose();
    }
    return cols;
}

export function init() {
    updatePoints();
    drawpalette();
}

