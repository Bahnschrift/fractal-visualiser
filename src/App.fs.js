import { replace, printf, toText } from "./.fable/fable-library.3.1.1/String.js";
import { findCookieValue } from "./Cookies.fs.js";
import { value as value_6 } from "./.fable/fable-library.3.1.1/Option.js";
import { createUniformLocation, createAttributeLocation, initBuffers, createShaderProgram, clear } from "./WebGLHelper.fs.js";
import { fsMandel, vsMandel } from "./Shaders.fs.js";
import { parse } from "./.fable/fable-library.3.1.1/Double.js";
import { SplitDouble_ofFloat_5E38073B, SplitDouble_toUniform_189C8C6F } from "./DoublePrecision.fs.js";
import { parse as parse_1 } from "./.fable/fable-library.3.1.1/Int32.js";
import { int32ToString, comparePrimitives, createAtom } from "./.fable/fable-library.3.1.1/Util.js";
import { FSharpSet__Remove, FSharpSet__Add, empty } from "./.fable/fable-library.3.1.1/Set.js";
import { forAll, getEnumerator } from "./.fable/fable-library.3.1.1/Seq.js";
import { isDigit } from "./.fable/fable-library.3.1.1/Char.js";

export const WIDTH = 1280;

export const HEIGHT = 720;

export const juliaPresets = [[0, 0.8], [0.37, 0.1], [0.355, 0.355], [-0.54, 0.54], [-0.4, -0.59], [0.34, -0.05], [-0.687, 0.312], [-0.673, 0.312], [0.355534, 0.337292]];

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

export const fieldPalatteOffset = getInputElement("palatteoffset");

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

export const cookiePalatteOffset = findCookieValue("palatteoffset");

export const cookieGenerator = findCookieValue("generator");

export const cookieMandelboxScale = findCookieValue("mandelboxscale");

export const cookieJuliaX = findCookieValue("jx");

export const cookieJuliaY = findCookieValue("jy");

export const cookieUseDoub = findCookieValue("usedoub");

export const cookies = [cookieX, cookieY, cookieZoom, cookiePalatteOffset, cookieGenerator, cookieMandelboxScale, cookieJuliaX, cookieJuliaY, cookieUseDoub];

if (cookies.every((c) => (!(c == null)))) {
    try {
        fieldX.value = value_6(cookieX);
        fieldY.value = value_6(cookieY);
        fieldZoom.value = value_6(cookieZoom);
        fieldPalatteOffset.value = value_6(cookiePalatteOffset);
        const matchValue = value_6(cookieGenerator);
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
        fieldMandelboxScale.value = value_6(cookieMandelboxScale);
        fieldJuliaX.value = value_6(cookieJuliaX);
        fieldJuliaY.value = value_6(cookieJuliaY);
        fieldUseDoub.checked = ((value_6(cookieJuliaY) === "true") ? true : false);
    }
    catch (matchValue_1) {
    }
}

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
    const zoom = parse(fieldZoom.value);
    let x = parse(fieldX.value);
    let y = parse(fieldY.value);
    const palatteOffset = parse(fieldPalatteOffset.value);
    const generator = (fieldMandelbrot.checked ? 1 : (fieldJulia.checked ? 2 : (fieldMandelbox.checked ? 3 : -1))) | 0;
    const mandelboxScale = parse(fieldMandelboxScale.value);
    const juliaX = parse(fieldJuliaX.value);
    const juliaY = parse(fieldJuliaY.value);
    const useDoub = fieldUseDoub.checked;
    document.cookie = toText(printf("zoom=%f;"))(zoom);
    const arg10_1 = x;
    document.cookie = toText(printf("x=%f;"))(arg10_1);
    const arg10_2 = y;
    document.cookie = toText(printf("y=%f;"))(arg10_2);
    document.cookie = toText(printf("palatteoffset=%f"))(palatteOffset);
    document.cookie = toText(printf("generator=%i"))(generator);
    document.cookie = toText(printf("mandelboxscale=%f"))(mandelboxScale);
    document.cookie = toText(printf("jx=%f"))(juliaX);
    document.cookie = toText(printf("jy=%f"))(juliaY);
    document.cookie = toText(printf("usedoub=%b"))(useDoub);
    fieldX.step = (0.1 * zoom).toString();
    fieldY.step = (0.1 * zoom).toString();
    fieldZoom.step = (0.1 * zoom).toString();
    const patternInput = initBuffers(gl);
    const positionBuffer = patternInput[0];
    const colourBuffer = patternInput[1];
    const vertexPositionAttr = createAttributeLocation(gl, shaderProgram, "aVertexPosition");
    const textureCoordAttr = createAttributeLocation(gl, shaderProgram, "aTextureCoord");
    const uLoc = (loc) => createUniformLocation(gl, shaderProgram, loc);
    const zoomUniform = uLoc("uZoom");
    const xcUniform = uLoc("xc");
    const ycUniform = uLoc("yc");
    const palatteOffsetUniform = uLoc("uPalatteOffset");
    const ratioUniform = uLoc("uRatio");
    const generatorUniform = uLoc("uGenerator");
    const mandelboxScaleUniform = uLoc("uMandelboxScale");
    const juliaXUniform = uLoc("uJuliaX");
    const juliaYUniform = uLoc("uJuliaY");
    const zoomDoubUniform = uLoc("uZoomDoub");
    const xcDoubUniform = uLoc("xcDoub");
    const ycDoubUniform = uLoc("ycDoub");
    const useDoubUniform = uLoc("uUseDoub");
    const draw = (zoom_1, x_1, y_1, ratio) => {
        resizeCanvas(gl.canvas);
        gl.viewport(0, 0, gl.canvas.width, gl.canvas.height);
        gl.useProgram(shaderProgram);
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.vertexAttribPointer(vertexPositionAttr, 2, gl.FLOAT, false, 0, 0);
        gl.bindBuffer(gl.ARRAY_BUFFER, colourBuffer);
        gl.vertexAttribPointer(textureCoordAttr, 2, gl.FLOAT, false, 0, 0);
        gl.uniform1f(zoomUniform, zoom_1);
        gl.uniform1f(xcUniform, x_1);
        gl.uniform1f(ycUniform, y_1);
        gl.uniform1f(palatteOffsetUniform, palatteOffset);
        gl.uniform1f(ratioUniform, ratio);
        gl.uniform1f(generatorUniform, generator);
        gl.uniform1f(mandelboxScaleUniform, mandelboxScale);
        gl.uniform1f(juliaXUniform, juliaX);
        gl.uniform1f(juliaYUniform, juliaY);
        gl.uniform2fv(zoomDoubUniform, SplitDouble_toUniform_189C8C6F(SplitDouble_ofFloat_5E38073B(zoom_1)));
        gl.uniform2fv(xcDoubUniform, SplitDouble_toUniform_189C8C6F(SplitDouble_ofFloat_5E38073B(x_1)));
        gl.uniform2fv(ycDoubUniform, SplitDouble_toUniform_189C8C6F(SplitDouble_ofFloat_5E38073B(y_1)));
        gl.uniform1i(useDoubUniform, useDoub ? 1 : 0);
        gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);
    };
    draw(zoom, x, y, gl.canvas.width / gl.canvas.height);
    gl.useProgram(shaderProgram);
}

update();

fieldZoom.oninput = ((_arg1) => {
    update();
});

fieldX.oninput = ((_arg2) => {
    update();
});

fieldY.oninput = ((_arg3) => {
    update();
});

fieldMandelboxScale.oninput = ((_arg4) => {
    update();
});

fieldPalatteOffset.oninput = ((_arg5) => {
    update();
});

fieldJuliaX.oninput = ((_arg6) => {
    update();
});

fieldJuliaY.oninput = ((_arg7) => {
    update();
});

fieldMandelbrot.oninput = ((_arg8) => {
    divMandelbox.hidden = true;
    divJulia.hidden = true;
    update();
});

fieldJulia.oninput = ((_arg9) => {
    divJulia.hidden = false;
    divMandelbox.hidden = true;
    update();
});

fieldMandelbox.oninput = ((_arg10) => {
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
    }
    update();
});

fieldUseDoub.oninput = ((_arg12) => {
    update();
});

export const keysDown = createAtom(empty({
    Compare: comparePrimitives,
}));

document.onkeydown = ((e) => {
    const scale = 0.075;
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
                    fieldY.value = (parse(fieldY.value) + (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "s": {
                    fieldY.value = (parse(fieldY.value) - (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "a": {
                    fieldX.value = (parse(fieldX.value) - (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "d": {
                    fieldX.value = (parse(fieldX.value) + (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "q": {
                    fieldZoom.value = (parse(fieldZoom.value) + (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "e": {
                    fieldZoom.value = (parse(fieldZoom.value) - (parse(fieldZoom.value) * scale)).toString();
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
                    fieldY.value = (parse(fieldY.value) + (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "s": {
                    fieldY.value = (parse(fieldY.value) - (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "a": {
                    fieldX.value = (parse(fieldX.value) - (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "d": {
                    fieldX.value = (parse(fieldX.value) + (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "q": {
                    fieldZoom.value = (parse(fieldZoom.value) + (parse(fieldZoom.value) * scale)).toString();
                    update();
                    break;
                }
                case "e": {
                    fieldZoom.value = (parse(fieldZoom.value) - (parse(fieldZoom.value) * scale)).toString();
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
    fieldX.value = int32ToString(0);
    fieldY.value = int32ToString(0);
    update();
});

buttonReset.onclick = ((_arg3) => {
    fieldX.value = int32ToString(0);
    fieldY.value = int32ToString(0);
    fieldZoom.value = (2.5).toString();
    fieldPalatteOffset.value = int32ToString(0);
    fieldMandelboxScale.value = int32ToString(3);
    fieldJuliaX.value = int32ToString(0);
    fieldJuliaY.value = int32ToString(0);
    fieldJuliaPresets.value = int32ToString(-1);
    fieldMandelbrot.checked = true;
    fieldUseDoub.checked = false;
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
                const arg50 = fieldPalatteOffset.value;
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

