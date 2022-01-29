module Shaders


// For linting
let glsl str = str

let vsMandel = glsl """
    precision highp float;
    precision highp int;

    attribute vec4 aVertexPosition;
    attribute vec2 aTextureCoord;
    varying vec2 vTextureCoord;
    void main() {
        gl_Position = aVertexPosition;
        vTextureCoord = aTextureCoord;
    }
    """

let fsMandel = glsl """
    precision highp float;
    precision highp int;
    
    varying vec2 vTextureCoord;
    uniform float uZoom;
    uniform float xc;
    uniform float yc;
    uniform float upaletteOffset;
    uniform float uRatio;
    uniform float uGenerator;
    uniform float uMandelbrotPower;
    uniform float uMandelboxScale;
    uniform float uJuliaX;
    uniform float uJuliaY;

    uniform int uUseDoub;
    uniform vec2 uZoomDoub;
    uniform vec2 xcDoub;
    uniform vec2 ycDoub;

    uniform vec3 upalette[76];

    const float MAX = 1000.;

    vec3 getpalette(int index) {
        if (index == 0) { return upalette[0] / 255.0; }
        else if (index == 1) { return upalette[1] / 255.0; }
        else if (index == 2) { return upalette[2] / 255.0; }
        else if (index == 3) { return upalette[3] / 255.0; }
        else if (index == 4) { return upalette[4] / 255.0; }
        else if (index == 5) { return upalette[5] / 255.0; }
        else if (index == 6) { return upalette[6] / 255.0; }
        else if (index == 7) { return upalette[7] / 255.0; }
        else if (index == 8) { return upalette[8] / 255.0; }
        else if (index == 9) { return upalette[9] / 255.0; }
        else if (index == 10) { return upalette[10] / 255.0; }
        else if (index == 11) { return upalette[11] / 255.0; }
        else if (index == 12) { return upalette[12] / 255.0; }
        else if (index == 13) { return upalette[13] / 255.0; }
        else if (index == 14) { return upalette[14] / 255.0; }
        else if (index == 15) { return upalette[15] / 255.0; }
        else if (index == 16) { return upalette[16] / 255.0; }
        else if (index == 17) { return upalette[17] / 255.0; }
        else if (index == 18) { return upalette[18] / 255.0; }
        else if (index == 19) { return upalette[19] / 255.0; }
        else if (index == 20) { return upalette[20] / 255.0; }
        else if (index == 21) { return upalette[21] / 255.0; }
        else if (index == 22) { return upalette[22] / 255.0; }
        else if (index == 23) { return upalette[23] / 255.0; }
        else if (index == 24) { return upalette[24] / 255.0; }
        else if (index == 25) { return upalette[25] / 255.0; }
        else if (index == 26) { return upalette[26] / 255.0; }
        else if (index == 27) { return upalette[27] / 255.0; }
        else if (index == 28) { return upalette[28] / 255.0; }
        else if (index == 29) { return upalette[29] / 255.0; }
        else if (index == 30) { return upalette[30] / 255.0; }
        else if (index == 31) { return upalette[31] / 255.0; }
        else if (index == 32) { return upalette[32] / 255.0; }
        else if (index == 33) { return upalette[33] / 255.0; }
        else if (index == 34) { return upalette[34] / 255.0; }
        else if (index == 35) { return upalette[35] / 255.0; }
        else if (index == 36) { return upalette[36] / 255.0; }
        else if (index == 37) { return upalette[37] / 255.0; }
        else if (index == 38) { return upalette[38] / 255.0; }
        else if (index == 39) { return upalette[39] / 255.0; }
        else if (index == 40) { return upalette[40] / 255.0; }
        else if (index == 41) { return upalette[41] / 255.0; }
        else if (index == 42) { return upalette[42] / 255.0; }
        else if (index == 43) { return upalette[43] / 255.0; }
        else if (index == 44) { return upalette[44] / 255.0; }
        else if (index == 45) { return upalette[45] / 255.0; }
        else if (index == 46) { return upalette[46] / 255.0; }
        else if (index == 47) { return upalette[47] / 255.0; }
        else if (index == 48) { return upalette[48] / 255.0; }
        else if (index == 49) { return upalette[49] / 255.0; }
        else if (index == 50) { return upalette[50] / 255.0; }
        else if (index == 51) { return upalette[51] / 255.0; }
        else if (index == 52) { return upalette[52] / 255.0; }
        else if (index == 53) { return upalette[53] / 255.0; }
        else if (index == 54) { return upalette[54] / 255.0; }
        else if (index == 55) { return upalette[55] / 255.0; }
        else if (index == 56) { return upalette[56] / 255.0; }
        else if (index == 57) { return upalette[57] / 255.0; }
        else if (index == 58) { return upalette[58] / 255.0; }
        else if (index == 59) { return upalette[59] / 255.0; }
        else if (index == 60) { return upalette[60] / 255.0; }
        else if (index == 61) { return upalette[61] / 255.0; }
        else if (index == 62) { return upalette[62] / 255.0; }
        else if (index == 63) { return upalette[63] / 255.0; }
        else if (index == 64) { return upalette[64] / 255.0; }
        else if (index == 65) { return upalette[65] / 255.0; }
        else if (index == 66) { return upalette[66] / 255.0; }
        else if (index == 67) { return upalette[67] / 255.0; }
        else if (index == 68) { return upalette[68] / 255.0; }
        else if (index == 69) { return upalette[69] / 255.0; }
        else if (index == 70) { return upalette[70] / 255.0; }
        else if (index == 71) { return upalette[71] / 255.0; }
        else if (index == 72) { return upalette[72] / 255.0; }
        else if (index == 73) { return upalette[73] / 255.0; }
        else if (index == 74) { return upalette[74] / 255.0; }
        else if (index == 75) { return upalette[75] / 255.0; }
    }

    vec3 getpaletteSmall(int index) {
        if (index == 0) { return vec3(0.01253, 0.08452, 0.51573); }
        else if (index == 1) { return vec3(0.06352, 0.26197, 0.69032); }
        else if (index == 2) { return vec3(0.13598, 0.43791, 0.80556); }
        else if (index == 3) { return vec3(0.30453, 0.60977, 0.87876); }
        else if (index == 4) { return vec3(0.55496, 0.79061, 0.94006); }
        else if (index == 5) { return vec3(0.79353, 0.93555, 0.98271); }
        else if (index == 6) { return vec3(0.92651, 0.99970, 0.99992); }
        else if (index == 7) { return vec3(0.95905, 0.96668, 0.83293); }
        else if (index == 8) { return vec3(0.98314, 0.87426, 0.44273); }
        else if (index == 9) { return vec3(0.99720, 0.75086, 0.08820); }
        else if (index == 10) { return vec3(0.97248, 0.61366, 0.00000); }
        else if (index == 11) { return vec3(0.66576, 0.38345, 0.00000); }
        else if (index == 12) { return vec3(0.24680, 0.13969, 0.00000); }
        else if (index == 13) { return vec3(0.00368, 0.00974, 0.00000); }
        else if (index == 14) { return vec3(0.00000, 0.01095, 0.09399); }
        else if (index == 15) { return vec3(0.00000, 0.02126, 0.31171); }
    }

    vec3 linearInterpolate(vec3 v1, vec3 v2, float f) {
        return v1 * (1.0 - f) + v2 * f;
    }

    vec3 cosineInterpolate(vec3 v1, vec3 v2, float f) {
        float f2 = (1.0 - cos(f * 3.15159)) / 2.0;
        return v1 * (1.0 - f2) + v2 * f2;
    }

    vec2 doubOfFloat(float upper) {
        return vec2(upper, 0.0);
    }

    vec2 doubAddDoub(vec2 doubA, vec2 doubB) {
        vec2 doubC;
        float t1, t2, e;

        t1 = doubA.x + doubB.x;
        e = t1 - doubA.x;
        t2 = ((doubB.x - e) + (doubA.x - (t1 - e))) + doubA.y + doubB.y;

        doubC.x = t1 + t2;
        doubC.y = t2 - (doubC.x - t1);
        return doubC;
    }

    vec2 doubAddFloat(vec2 doub, float f) {
        return doubAddDoub(doub, doubOfFloat(f));
    }

    vec2 doubMulDoub(vec2 doubA, vec2 doubB) {
        vec2 doubC;
        float c11, c21, c2, e, t1, t2;
        float a1, a2, b1, b2, cona, conb, split = 8193.;

        cona = doubA.x * split;
        conb = doubB.x * split;
        a1 = cona - (cona - doubA.x);
        b1 = conb - (conb - doubB.x);
        a2 = doubA.x - a1;
        b2 = doubB.x - b1;

        c11 = doubA.x * doubB.x;
        c21 = a2 * b2 + (a2 * b1 + (a1 * b2 + (a1 * b1 - c11)));

        c2 = doubA.x * doubB.y + doubA.y * doubB.x;

        t1 = c11 + c2;
        e = t1 - c11;
        t2 = doubA.y * doubB.y + ((c2 - e) + (c11 - (t1 - e))) + c21;

        doubC.x = t1 + t2;
        doubC.y = t2 - (doubC.x - t1);
        return doubC;
    }

    vec2 doubMulFloat(vec2 doub, float f) {
        return doubMulDoub(doub, doubOfFloat(f));
    }

    vec2 doubDot(vec2 x, vec2 y) {
        return doubAddDoub(doubMulDoub(x, x), doubMulDoub(y, y));
    }

    vec2 compPower(vec2 z, float n) {
        return vec2(
            pow(z.x * z.x + z.y * z.y, n / 2.0) * cos(n * atan(z.y, z.x)),
            pow(z.x * z.x + z.y * z.y, n / 2.0) * sin(n * atan(z.y, z.x))
        );
    }

    float mandelbrot(float x, float y) {
        vec2 c = vec2(x, y);
        vec2 z = c;

        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > sqrt(8.0)) {
                float l = log(abs(uMandelbrotPower));
                return float(i) - log(log(dot(z, z)) / l) / l + 4.0;
            }
            if (uMandelbrotPower == 2.0) {
                z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);
            } else {
                z = compPower(z, uMandelbrotPower) + c;
            }
        }
        return MAX;
    }

    float mandelbrotDoub(vec2 x, vec2 y) {
        vec2 zx = x;
        vec2 zy = y;

        for (int i = 0; i <= int(MAX); i++) {
            if (doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x > 4096.0) {
                return float(i) - log2(log2(doubDot(zx, zy).x)) + 4.0;
            }
            vec2 zxTemp = doubAddDoub(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(doubOfFloat(-1.0), doubMulDoub(zy, zy))), x);
            zy = doubAddDoub(doubMulFloat(doubMulDoub(zx, zy), 2.0), y);
            zx = zxTemp;
        }
        return MAX;
    }

    float julia(float x, float y) {
        vec2 c = vec2(uJuliaX, uJuliaY);
        vec2 z = vec2(x, y);

        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 4096.) {
                return float(i) - log2(log(dot(z, z)) / log(10.0)) + 2.0;
            }
            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);
        }
        return MAX;
    }

    float juliaDoub(vec2 x, vec2 y) {
        vec2 zx = x;
        vec2 zy = y;
        vec2 cx = doubOfFloat(uJuliaX);
        vec2 cy = doubOfFloat(uJuliaY);

        for (int i = 0; i <= int(MAX); i++) {
            if (doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x > 4096.0) {
                return float(i) + 1.0 - log2(log(sqrt(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(zy, zy)).x)));
            }
            vec2 zxTemp = doubAddDoub(doubAddDoub(doubMulDoub(zx, zx), doubMulDoub(doubOfFloat(-1.0), doubMulDoub(zy, zy))), cx);
            zy = doubAddDoub(doubMulFloat(doubMulDoub(zx, zy), 2.0), cy);
            zx = zxTemp;
        }
        return MAX;
    }

    float burningShip(float x, float y) {
        vec2 c = vec2(x, -y);
        vec2 z = c;

        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 4096.) {
                return float(i) - log2(log2(dot(z, z))) + 4.0;
            }
            z = vec2(z.x*z.x - z.y*z.y + c.x, abs(2.0*z.x*z.y) + c.y);
        }
        return MAX;
    }

    float mandelbox(float x, float y) {
        vec2 c = vec2(x, y);
        vec2 z = c;
        
        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 4096.) {
                return float(i) - log2(log2(dot(z, z))) + 4.0;
            }

            if (z.x > 1.) {
                z.x = 2. - z.x;
            } else if (z.x < -1.) {
                z.x = -2. - z.x;
            }
            if (z.y > 1.) {
                z.y = 2. - z.y;
            } else if (z.y < -1.) {
                z.y = -2. - z.y;
            }

            float mag = length(z);
            if (mag < .5) {
                z *= 4.;
            } else if (mag < 1.) {
                z /= mag * mag;
            }

            z = uMandelboxScale * z + c;
        }
        return 0.;
    }

    void main() {
        bool useDoub = uUseDoub == 1 ? true : false;
        
        float m = 0.0;
        if (useDoub) {
            vec2 x = doubAddDoub(xcDoub, doubMulFloat(uZoomDoub, uRatio * (vTextureCoord.x - 0.5)));
            vec2 y = doubAddDoub(ycDoub, doubMulFloat(uZoomDoub, vTextureCoord.y - 0.5));
            if (uGenerator == 1.0) {
                m = mandelbrotDoub(x, y);
            } else if (uGenerator == 2.0) {
                m = juliaDoub(x, y);
            } else if (uGenerator == 3.0) {
                m = burningShip(x.x, y.x);
            } else if (uGenerator == 4.0) {
                m = mandelbox(x.x, y.x);
            } else {
                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);
                return;
            }
        } else {
            float x = xc + (vTextureCoord.x - 0.5) * uZoom * uRatio;
            float y = yc + (vTextureCoord.y - 0.5) * uZoom;
            if (uGenerator == 1.0) {
                m = mandelbrot(x, y);
            } else if (uGenerator == 2.0) {
                m = julia(x, y);
            } else if (uGenerator == 3.0) {
                m = burningShip(x, y);
            } else if (uGenerator == 4.0) {
                m = mandelbox(x, y);
            } else {
                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);
                return;
            }
        }

        // Colouring
        vec3 col = vec3(0., 0., 0.);
        if (m != MAX) {
            if (uGenerator == 4.0) {  // Mandelbox
                m = mod(m + upaletteOffset, 16.0);
                col = getpaletteSmall(int(m));
            } else {
                m = mod(m + upaletteOffset, 75.0);
                vec3 col1 = getpalette(int(m));
                vec3 col2 = getpalette(int(m) + 1);
                float f = mod(m, 1.0);
                col = linearInterpolate(col1, col2, f);
            }
        }
        gl_FragColor = vec4(col.xyz, 1.0);
    }
    """