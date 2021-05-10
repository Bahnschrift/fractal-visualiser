import { createAtom } from "./.fable/fable-library.3.1.1/Util.js";
import { parse } from "./.fable/fable-library.3.1.1/Int32.js";
import { findCookieValue } from "./Cookies.fs.js";
import { parse as parse_1 } from "./.fable/fable-library.3.1.1/Double.js";
import { append, pairwise, map } from "./.fable/fable-library.3.1.1/Array.js";
import { printf, toText } from "./.fable/fable-library.3.1.1/String.js";
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

export function hexToRGB(hex) {
    let hex_1 = hex;
    if (hex_1[0] === "#") {
        hex_1 = hex_1.slice(1, hex_1.length);
    }
    const r = parse(hex_1.slice(0, 1 + 1), 511, false, 32, 16) / 255;
    const g = parse(hex_1.slice(2, 3 + 1), 511, false, 32, 16) / 255;
    const b = parse(hex_1.slice(4, 5 + 1), 511, false, 32, 16) / 255;
    return [r, g, b];
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

export const buttonResetPallette = document.getElementById("resetpallette");

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
    const p2R = patternInput_1[0];
    const p2G = patternInput_1[1];
    const p2B = patternInput_1[2];
    const patternInput_2 = hexToRGB(fieldP3C.value);
    const p3R = patternInput_2[0];
    const p3G = patternInput_2[1];
    const p3B = patternInput_2[2];
    const patternInput_3 = hexToRGB(fieldP4C.value);
    const p4R = patternInput_3[0];
    const p4G = patternInput_3[1];
    const p4B = patternInput_3[2];
    const patternInput_4 = hexToRGB(fieldP5C.value);
    const p5R = patternInput_4[0];
    const p5G = patternInput_4[1];
    const p5B = patternInput_4[2];
    points([[parse_1(fieldP1X.value), p1R, p1G, p1B], [parse_1(fieldP2X.value), p2R, p2G, p2B], [parse_1(fieldP3X.value), p3R, p3G, p3B], [parse_1(fieldP4X.value), p4R, p4G, p4B], [parse_1(fieldP5X.value), p5R, p5G, p5B], [1, p1R, p1G, p1B]], true);
    scaledPoints(map((tupledArg) => {
        const x = tupledArg[0];
        const r = tupledArg[1];
        const g = tupledArg[2];
        const b = tupledArg[3];
        return [x * WIDTH, r * HEIGHT, g * HEIGHT, b * HEIGHT];
    }, points()), true);
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

export const canv = document.getElementById("pallettecanv");

canv.width = WIDTH;

canv.height = HEIGHT;

export const ctx = canv.getContext('2d');

export function drawPallette() {
    ctx.fillStyle = "#FFFFFF";
    ctx.fillRect(0, 0, WIDTH, HEIGHT);
    const arr = pairwise(scaledPoints());
    for (let idx = 0; idx <= (arr.length - 1); idx++) {
        const pair = arr[idx];
        const p2x = pair[1][0];
        const p2r = pair[1][1];
        const p2g = pair[1][2];
        const p2b = pair[1][3];
        const p1x = pair[0][0];
        const p1r = pair[0][1];
        const p1g = pair[0][2];
        const p1b = pair[0][3];
        const enumerator = getEnumerator(rangeNumber(p1x, 1, p2x));
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const x = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const mul = (x - p1x) / (p2x - p1x);
                const yInterpR = interpolateCosine(p1r, p2r, mul);
                const yInterpG = interpolateCosine(p1g, p2g, mul);
                const yInterpB = interpolateCosine(p1b, p2b, mul);
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
    }
    for (let idx_1 = 0; idx_1 <= (scaledPoints().length - 1); idx_1++) {
        const point = scaledPoints()[idx_1];
        const x_1 = point[0];
        const r = point[1];
        const g = point[2];
        const b = point[3];
        ctx.fillStyle = "rgb(255, 0, 0)";
        ctx.fillRect(x_1 - 3, (HEIGHT - r) - 3, 5, 5);
        ctx.fillStyle = "rgb(0, 255, 0)";
        ctx.fillRect(x_1 - 3, (HEIGHT - g) - 3, 5, 5);
        ctx.fillStyle = "rgb(0, 0, 255)";
        ctx.fillRect(x_1 - 3, (HEIGHT - b) - 3, 5, 5);
    }
}

export function getColour(x) {
    let x_1;
    let minpoint = 0;
    while (x > (x_1 = scaledPoints()[minpoint + 1][0], x_1)) {
        minpoint = (minpoint + 1);
    }
    if (minpoint === (scaledPoints().length - 1)) {
        minpoint = (minpoint - 1);
    }
    const patternInput = scaledPoints()[minpoint];
    const p1x = patternInput[0];
    const p1r = patternInput[1];
    const p1g = patternInput[2];
    const p1b = patternInput[3];
    const patternInput_1 = scaledPoints()[minpoint + 1];
    const p2x = patternInput_1[0];
    const p2r = patternInput_1[1];
    const p2g = patternInput_1[2];
    const p2b = patternInput_1[3];
    const mul = (x - p1x) / (p2x - p1x);
    const yInterpR = (interpolateCosine(p1r, p2r, mul) / HEIGHT) * 255;
    const yInterpG = (interpolateCosine(p1g, p2g, mul) / HEIGHT) * 255;
    const yInterpB = (interpolateCosine(p1b, p2b, mul) / HEIGHT) * 255;
    return [yInterpR, yInterpG, yInterpB];
}

export function getColours(n) {
    const inc = WIDTH / n;
    let cols = [];
    const enumerator = getEnumerator(rangeNumber(0, inc, WIDTH - 1));
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
    drawPallette();
}
