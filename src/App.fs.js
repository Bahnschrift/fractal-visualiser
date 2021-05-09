import { replace, printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { findCookieValue } from "./Cookies.fs.js";
import { value as value_3 } from "./.fable/fable-library.3.1.1/Option.js";
import { int32ToString, comparePrimitives, createAtom } from "./.fable/fable-library.3.1.1/Util.js";
import { parse } from "./.fable/fable-library.3.1.1/Double.js";
import { createUniformLocation, createAttributeLocation, initBuffers, createShaderProgram, clear } from "./WebGLHelper.fs.js";
import { fsMandel, vsMandel } from "./Shaders.fs.js";
import { SplitDouble_ofFloat_5E38073B, SplitDouble_toUniform_189C8C6F } from "./DoublePrecision.fs.js";
import { parse as parse_1 } from "./.fable/fable-library.3.1.1/Int32.js";
import { FSharpSet__Remove, FSharpSet__Add, empty } from "./.fable/fable-library.3.1.1/Set.js";
import { forAll, getEnumerator } from "./.fable/fable-library.3.1.1/Seq.js";
import { isDigit } from "./.fable/fable-library.3.1.1/Char.js";

export const WIDTH = 1280;

export const HEIGHT = 720;

export const juliaPresets = [[0, 0.8], [0.37, 0.1], [0.355, 0.355], [-0.54, 0.54], [-0.4, -0.59], [0.34, -0.05], [-0.687, 0.312], [-0.673, 0.312], [-0.75, -0.15], [0.355534, 0.337292]];

export function getDivElement(id) {
    return document.querySelector(toText(printf("#%s"))(id));
}

export const divMandelbox = getDivElement("settingsmandelbox");

export const divJulia = getDivElement("settingsjulia");

export function getInputElement(id) {
    return document.querySelector(toText(printf("#%s"))(id));
}

export const fieldZoom = getInputElement("zoom");

export const fieldX = getInputElement("x");

export const fieldY = getInputElement("y");

export const fieldPalletteOffset = getInputElement("palletteoffset");

export const fieldFractal = getInputElement("fractal");

export const fieldMandelbrot = getInputElement("mandelbrot");

export const fieldMandelbox = getInputElement("mandelbox");

export const fieldMandelboxScale = getInputElement("mandelboxscale");

export const fieldJulia = getInputElement("julia");

export const fieldJuliaX = getInputElement("juliax");

export const fieldJuliaY = getInputElement("juliay");

export const fieldUseDoub = getInputElement("usedoub");

export const fieldJuliaPresets = document.querySelector("#juliapresets");

export function getButtonElement(id) {
    return document.querySelector(toText(printf("#%s"))(id));
}

export const buttonFullscreen = getButtonElement("fullscreen");

export const buttonCentre = getButtonElement("centre");

export const buttonReset = getButtonElement("reset");

export const buttonSaveImage = getButtonElement("saveimage");

export const cookieX = findCookieValue("x");

export const cookieY = findCookieValue("y");

export const cookieZoom = findCookieValue("zoom");

export const cookiePalletteOffset = findCookieValue("palletteoffset");

export const cookieGenerator = findCookieValue("generator");

export const cookieMandelboxScale = findCookieValue("mandelboxscale");

export const cookieJuliaX = findCookieValue("jx");

export const cookieJuliaY = findCookieValue("jy");

export const cookieUseDoub = findCookieValue("usedoub");

export function setupCookies() {
    const cookies = [cookieX, cookieY, cookieZoom, cookiePalletteOffset, cookieGenerator, cookieMandelboxScale, cookieJuliaX, cookieJuliaY, cookieUseDoub];
    if (cookies.every((c) => (!(c == null)))) {
        try {
            fieldX.value = value_3(cookieX);
            fieldY.value = value_3(cookieY);
            fieldZoom.value = value_3(cookieZoom);
            fieldPalletteOffset.value = value_3(cookiePalletteOffset);
            const matchValue = value_3(cookieGenerator);
            switch (matchValue) {
                case "1": {
                    fieldMandelbrot.checked = true;
                    break;
                }
                case "2": {
                    fieldJulia.checked = true;
                    divJulia.hidden = false;
                    break;
                }
                case "3": {
                    fieldMandelbox.checked = true;
                    divMandelbox.hidden = false;
                    break;
                }
                default: {
                }
            }
            fieldMandelboxScale.value = value_3(cookieMandelboxScale);
            fieldJuliaX.value = value_3(cookieJuliaX);
            fieldJuliaY.value = value_3(cookieJuliaY);
            fieldUseDoub.checked = ((value_3(cookieJuliaY) === "true") ? true : false);
        }
        catch (matchValue_1) {
        }
    }
}

setupCookies();

export const zoom = createAtom(parse(fieldZoom.value));

export const x = createAtom(parse(fieldX.value));

export const y = createAtom(parse(fieldY.value));

export const palletteOffset = createAtom(parse(fieldPalletteOffset.value));

export const generator = createAtom(fieldMandelbrot.checked ? 1 : (fieldJulia.checked ? 2 : (fieldMandelbox.checked ? 3 : -1)));

export const mandelboxScale = createAtom(parse(fieldMandelboxScale.value));

export const juliaX = createAtom(parse(fieldJuliaX.value));

export const juliaY = createAtom(parse(fieldJuliaY.value));

export const useDoub = createAtom(fieldUseDoub.checked);

export const canv = document.querySelector("#canv");

canv.width = WIDTH;

canv.height = HEIGHT;

export const gl = canv.getContext("webgl");

clear(gl);

export const shaderProgram = createShaderProgram(gl, vsMandel, fsMandel);

export function update() {
    const resizeCanvas = (canvas) => {
        const displayWidth = canvas.clientWidth;
        const displayHeight = canvas.clientHeight;
        const needResize = (canvas.width !== displayWidth) ? true : (canvas.height !== displayHeight);
        if (needResize) {
            canvas.width = (window.innerWidth * window.devicePixelRatio);
            canvas.height = (window.innerHeight * window.devicePixelRatio);
        }
    };
    const arg10 = zoom();
    document.cookie = toText(printf("zoom=%f;"))(arg10);
    const arg10_1 = x();
    document.cookie = toText(printf("x=%f;"))(arg10_1);
    const arg10_2 = y();
    document.cookie = toText(printf("y=%f;"))(arg10_2);
    const arg10_3 = palletteOffset();
    document.cookie = toText(printf("palletteoffset=%f"))(arg10_3);
    const arg10_4 = generator() | 0;
    document.cookie = toText(printf("generator=%i"))(arg10_4);
    const arg10_5 = mandelboxScale();
    document.cookie = toText(printf("mandelboxscale=%f"))(arg10_5);
    const arg10_6 = juliaX();
    document.cookie = toText(printf("jx=%f"))(arg10_6);
    const arg10_7 = juliaY();
    document.cookie = toText(printf("jy=%f"))(arg10_7);
    const arg10_8 = useDoub();
    document.cookie = toText(printf("usedoub=%b"))(arg10_8);
    fieldX.value = x().toString();
    fieldY.value = y().toString();
    fieldZoom.value = zoom().toString();
    fieldPalletteOffset.value = palletteOffset().toString();
    fieldUseDoub.checked = useDoub();
    fieldJuliaX.value = juliaX().toString();
    fieldJuliaY.value = juliaY().toString();
    fieldMandelboxScale.value = mandelboxScale().toString();
    fieldX.step = (0.1 * zoom()).toString();
    fieldY.step = (0.1 * zoom()).toString();
    fieldZoom.step = (0.1 * zoom()).toString();
    const patternInput = initBuffers(gl);
    const positionBuffer = patternInput[0];
    const colourBuffer = patternInput[1];
    const vertexPositionAttr = createAttributeLocation(gl, shaderProgram, "aVertexPosition");
    const textureCoordAttr = createAttributeLocation(gl, shaderProgram, "aTextureCoord");
    const uLoc = (loc) => createUniformLocation(gl, shaderProgram, loc);
    const zoomUniform = uLoc("uZoom");
    const xcUniform = uLoc("xc");
    const ycUniform = uLoc("yc");
    const palletteOffsetUniform = uLoc("uPalletteOffset");
    const ratioUniform = uLoc("uRatio");
    const generatorUniform = uLoc("uGenerator");
    const mandelboxScaleUniform = uLoc("uMandelboxScale");
    const juliaXUniform = uLoc("uJuliaX");
    const juliaYUniform = uLoc("uJuliaY");
    const zoomDoubUniform = uLoc("uZoomDoub");
    const xcDoubUniform = uLoc("xcDoub");
    const ycDoubUniform = uLoc("ycDoub");
    const useDoubUniform = uLoc("uUseDoub");
    resizeCanvas(gl.canvas);
    gl.viewport(0, 0, gl.canvas.width, gl.canvas.height);
    gl.useProgram(shaderProgram);
    gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
    gl.vertexAttribPointer(vertexPositionAttr, 2, gl.FLOAT, false, 0, 0);
    gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer);
    gl.vertexAttribPointer(textureCoordAttr, 2, gl.FLOAT, false, 0, 0);
    const ratio = gl.canvas.width / gl.canvas.height;
    gl.uniform1f(zoomUniform, zoom());
    gl.uniform1f(xcUniform, x());
    gl.uniform1f(ycUniform, y());
    gl.uniform1f(palletteOffsetUniform, palletteOffset());
    gl.uniform1f(ratioUniform, ratio);
    gl.uniform1f(generatorUniform, generator());
    gl.uniform1f(mandelboxScaleUniform, mandelboxScale());
    gl.uniform1f(juliaXUniform, juliaX());
    gl.uniform1f(juliaYUniform, juliaY());
    gl.uniform2fv(zoomDoubUniform, SplitDouble_toUniform_189C8C6F(SplitDouble_ofFloat_5E38073B(zoom())));
    gl.uniform2fv(xcDoubUniform, SplitDouble_toUniform_189C8C6F(SplitDouble_ofFloat_5E38073B(x())));
    gl.uniform2fv(ycDoubUniform, SplitDouble_toUniform_189C8C6F(SplitDouble_ofFloat_5E38073B(y())));
    gl.uniform1i(useDoubUniform, useDoub() ? 1 : 0);
    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
    gl.useProgram(shaderProgram);
}

update();

fieldZoom.oninput = ((_arg1) => {
    zoom(parse(fieldZoom.value), true);
    update();
});

fieldX.oninput = ((_arg2) => {
    x(parse(fieldX.value), true);
    update();
});

fieldY.oninput = ((_arg3) => {
    y(parse(fieldY.value), true);
    update();
});

fieldMandelboxScale.oninput = ((_arg4) => {
    mandelboxScale(parse(fieldMandelboxScale.value), true);
    update();
});

fieldPalletteOffset.oninput = ((_arg5) => {
    palletteOffset(parse(fieldPalletteOffset.value), true);
    update();
});

fieldJuliaX.oninput = ((_arg6) => {
    juliaX(parse(fieldJuliaX.value), true);
    update();
});

fieldJuliaY.oninput = ((_arg7) => {
    juliaY(parse(fieldJuliaY.value), true);
    update();
});

fieldMandelbrot.oninput = ((_arg8) => {
    generator(1, true);
    divMandelbox.hidden = true;
    divJulia.hidden = true;
    update();
});

fieldJulia.oninput = ((_arg9) => {
    generator(2, true);
    divJulia.hidden = false;
    divMandelbox.hidden = true;
    update();
});

fieldMandelbox.oninput = ((_arg10) => {
    generator(3, true);
    divJulia.hidden = true;
    divMandelbox.hidden = false;
    update();
});

fieldJuliaPresets.oninput = ((_arg11) => {
    const juliaPreset = parse_1(fieldJuliaPresets.value, 511, false, 32) | 0;
    if (juliaPreset !== -1) {
        const juliaPresetCoords = juliaPresets[juliaPreset];
        fieldJuliaX.value = juliaPresetCoords[0].toString();
        fieldJuliaY.value = juliaPresetCoords[1].toString();
        juliaX(juliaPresetCoords[0], true);
        juliaY(juliaPresetCoords[1], true);
    }
    update();
});

fieldUseDoub.oninput = ((_arg12) => {
    useDoub(fieldUseDoub.checked, true);
    update();
});

export const keysDown = createAtom(empty({
    Compare: comparePrimitives,
}));

document.onkeydown = ((e) => {
    let scale = 0.075;
    const matchValue = e.key;
    switch (matchValue) {
        case "w":
        case "s":
        case "a":
        case "d":
        case "q":
        case "e": {
            keysDown(FSharpSet__Add(keysDown(), e.key), true);
            break;
        }
        default: {
        }
    }
    const enumerator = getEnumerator(keysDown());
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const key = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            switch (key) {
                case "w": {
                    y(y() + (zoom() * scale), true);
                    update();
                    break;
                }
                case "s": {
                    y(y() - (zoom() * scale), true);
                    update();
                    break;
                }
                case "a": {
                    x(x() - (zoom() * scale), true);
                    update();
                    break;
                }
                case "d": {
                    x(x() + (zoom() * scale), true);
                    update();
                    break;
                }
                case "q": {
                    zoom(zoom() + (zoom() * scale), true);
                    update();
                    break;
                }
                case "e": {
                    zoom(zoom() - (zoom() * scale), true);
                    update();
                    break;
                }
                default: {
                }
            }
        }
    }
    finally {
        enumerator.Dispose();
    }
});

document.onkeyup = ((e) => {
    const scale = 0.075;
    const matchValue = e.key;
    switch (matchValue) {
        case "w":
        case "s":
        case "a":
        case "d":
        case "q":
        case "e": {
            keysDown(FSharpSet__Remove(keysDown(), e.key), true);
            break;
        }
        default: {
        }
    }
    const enumerator = getEnumerator(keysDown());
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const key = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            switch (key) {
                case "w": {
                    y(y() + (zoom() * scale), true);
                    update();
                    break;
                }
                case "s": {
                    y(y() - (zoom() * scale), true);
                    update();
                    break;
                }
                case "a": {
                    x(x() - (zoom() * scale), true);
                    update();
                    break;
                }
                case "d": {
                    x(x() + (zoom() * scale), true);
                    update();
                    break;
                }
                case "q": {
                    zoom(zoom() + (zoom() * scale), true);
                    update();
                    break;
                }
                case "e": {
                    zoom(zoom() - (zoom() * scale), true);
                    update();
                    break;
                }
                default: {
                }
            }
        }
    }
    finally {
        enumerator.Dispose();
    }
});

buttonFullscreen.onclick = ((_arg1) => {
    canv.requestFullscreen();
});

buttonCentre.onclick = ((_arg2) => {
    x(0, true);
    y(0, true);
    update();
});

buttonReset.onclick = ((_arg3) => {
    x(0, true);
    y(0, true);
    zoom(2.5, true);
    palletteOffset(0, true);
    useDoub(false, true);
    juliaX(0, true);
    juliaY(0, true);
    mandelboxScale(3, true);
    generator(1, true);
    fieldJuliaPresets.value = int32ToString(-1);
    fieldMandelbrot.checked = true;
    divMandelbox.hidden = true;
    divJulia.hidden = true;
    update();
});

buttonSaveImage.onclick = ((_arg4) => {
    const saveResWidth = window.prompt("Save resolution width:");
    if (!(saveResWidth == null)) {
        const saveResHeight = window.prompt("Save resolution height:");
        if (!(saveResHeight == null)) {
            if (forAll(isDigit, saveResHeight.split("")) ? forAll(isDigit, saveResWidth.split("")) : false) {
                canv.width = parse(saveResWidth);
                canv.height = parse(saveResHeight);
                update();
                update();
                const mode = fieldMandelbrot.checked ? "Mandelbrot" : (fieldJulia.checked ? "Julia" : "Mandelbox");
                let fname;
                const arg50 = fieldPalletteOffset.value;
                const arg40 = fieldZoom.value;
                const arg30 = fieldY.value;
                const arg20 = fieldX.value;
                fname = toText(printf("%s x=%s,y=%s,zoom=%s,offset=%s %sx%s.png"))(mode)(arg20)(arg30)(arg40)(arg50)(saveResWidth)(saveResHeight);
                const link = document.querySelector("#link");
                link.setAttribute("download", fname);
                link.setAttribute("href", replace(canv.toDataURL("png"), "image/png", "image/octet-stream"));
                link.click();
                canv.width = WIDTH;
                canv.height = HEIGHT;
            }
        }
    }
});

document.onfullscreenchange = ((_arg5) => {
    if (document.fullscreenElement == null) {
        canv.width = WIDTH;
        canv.height = HEIGHT;
        update();
    }
});

window.onresize = ((_arg6) => {
    update();
    update();
});

