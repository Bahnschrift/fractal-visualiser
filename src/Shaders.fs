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
    uniform float uPalletteOffset;
    uniform float uRatio;
    uniform float uGenerator;
    uniform float uMandelboxScale;
    uniform float uJuliaX;
    uniform float uJuliaY;

    uniform int uUseDoub;
    uniform vec2 uZoomDoub;
    uniform vec2 xcDoub;
    uniform vec2 ycDoub;

    const float MAX = 1000.;

    vec3 getPallette(int index) {
        if (index == 0) { return vec3(0.00004, 0.02816, 0.39805); }
        else if (index == 1) { return vec3(0.00166, 0.03771, 0.43408); }
        else if (index == 2) { return vec3(0.00555, 0.05555, 0.47167); }
        else if (index == 3) { return vec3(0.01152, 0.08046, 0.51019); }
        else if (index == 4) { return vec3(0.01937, 0.11120, 0.54901); }
        else if (index == 5) { return vec3(0.02888, 0.14654, 0.58753); }
        else if (index == 6) { return vec3(0.03987, 0.18527, 0.62511); }
        else if (index == 7) { return vec3(0.05214, 0.22615, 0.66114); }
        else if (index == 8) { return vec3(0.06548, 0.26795, 0.69499); }
        else if (index == 9) { return vec3(0.07970, 0.30945, 0.72605); }
        else if (index == 10) { return vec3(0.09459, 0.34942, 0.75368); }
        else if (index == 11) { return vec3(0.10996, 0.38663, 0.77727); }
        else if (index == 12) { return vec3(0.12561, 0.41984, 0.79620); }
        else if (index == 13) { return vec3(0.14545, 0.45198, 0.81253); }
        else if (index == 14) { return vec3(0.17281, 0.48636, 0.82859); }
        else if (index == 15) { return vec3(0.20679, 0.52255, 0.84433); }
        else if (index == 16) { return vec3(0.24650, 0.56012, 0.85969); }
        else if (index == 17) { return vec3(0.29104, 0.59865, 0.87459); }
        else if (index == 18) { return vec3(0.33953, 0.63770, 0.88898); }
        else if (index == 19) { return vec3(0.39105, 0.67686, 0.90278); }
        else if (index == 20) { return vec3(0.44473, 0.71568, 0.91594); }
        else if (index == 21) { return vec3(0.49967, 0.75374, 0.92839); }
        else if (index == 22) { return vec3(0.55496, 0.79061, 0.94006); }
        else if (index == 23) { return vec3(0.60972, 0.82587, 0.95090); }
        else if (index == 24) { return vec3(0.66305, 0.85909, 0.96083); }
        else if (index == 25) { return vec3(0.71406, 0.88983, 0.96979); }
        else if (index == 26) { return vec3(0.76185, 0.91767, 0.97771); }
        else if (index == 27) { return vec3(0.80553, 0.94219, 0.98454); }
        else if (index == 28) { return vec3(0.84421, 0.96294, 0.99021); }
        else if (index == 29) { return vec3(0.87698, 0.97951, 0.99465); }
        else if (index == 30) { return vec3(0.90296, 0.99147, 0.99780); }
        else if (index == 31) { return vec3(0.92125, 0.99838, 0.99959); }
        else if (index == 32) { return vec3(0.93123, 0.99988, 0.99937); }
        else if (index == 33) { return vec3(0.93843, 0.99710, 0.98452); }
        else if (index == 34) { return vec3(0.94534, 0.99079, 0.95178); }
        else if (index == 35) { return vec3(0.95193, 0.98123, 0.90364); }
        else if (index == 36) { return vec3(0.95818, 0.96870, 0.84257); }
        else if (index == 37) { return vec3(0.96408, 0.95346, 0.77105); }
        else if (index == 38) { return vec3(0.96960, 0.93578, 0.69157); }
        else if (index == 39) { return vec3(0.97473, 0.91594, 0.60660); }
        else if (index == 40) { return vec3(0.97944, 0.89420, 0.51864); }
        else if (index == 41) { return vec3(0.98372, 0.87083, 0.43015); }
        else if (index == 42) { return vec3(0.98755, 0.84611, 0.34362); }
        else if (index == 43) { return vec3(0.99090, 0.82031, 0.26153); }
        else if (index == 44) { return vec3(0.99376, 0.79369, 0.18637); }
        else if (index == 45) { return vec3(0.99610, 0.76653, 0.12060); }
        else if (index == 46) { return vec3(0.99792, 0.73909, 0.06672); }
        else if (index == 47) { return vec3(0.99918, 0.71165, 0.02720); }
        else if (index == 48) { return vec3(0.99987, 0.68448, 0.00453); }
        else if (index == 49) { return vec3(0.99876, 0.65735, 0.00000); }
        else if (index == 50) { return vec3(0.98110, 0.62437, 0.00000); }
        else if (index == 51) { return vec3(0.94463, 0.58470, 0.00000); }
        else if (index == 52) { return vec3(0.89211, 0.53955, 0.00000); }
        else if (index == 53) { return vec3(0.82630, 0.49015, 0.00000); }
        else if (index == 54) { return vec3(0.74993, 0.43771, 0.00000); }
        else if (index == 55) { return vec3(0.66576, 0.38345, 0.00000); }
        else if (index == 56) { return vec3(0.57655, 0.32857, 0.00000); }
        else if (index == 57) { return vec3(0.48503, 0.27430, 0.00000); }
        else if (index == 58) { return vec3(0.39396, 0.22186, 0.00000); }
        else if (index == 59) { return vec3(0.30608, 0.17245, 0.00000); }
        else if (index == 60) { return vec3(0.22416, 0.12729, 0.00000); }
        else if (index == 61) { return vec3(0.15093, 0.08760, 0.00000); }
        else if (index == 62) { return vec3(0.08915, 0.05460, 0.00000); }
        else if (index == 63) { return vec3(0.04157, 0.02949, 0.00000); }
        else if (index == 64) { return vec3(0.01094, 0.01350, 0.00000); }
        else if (index == 65) { return vec3(0.00000, 0.00784, 0.00000); }
        else if (index == 66) { return vec3(0.00000, 0.00803, 0.00641); }
        else if (index == 67) { return vec3(0.00000, 0.00858, 0.02456); }
        else if (index == 68) { return vec3(0.00000, 0.00949, 0.05251); }
        else if (index == 69) { return vec3(0.00000, 0.01074, 0.08835); }
        else if (index == 70) { return vec3(0.00000, 0.01232, 0.13013); }
        else if (index == 71) { return vec3(0.00000, 0.01422, 0.17593); }
        else if (index == 72) { return vec3(0.00000, 0.01642, 0.22382); }
        else if (index == 73) { return vec3(0.00000, 0.01891, 0.27186); }
        else if (index == 74) { return vec3(0.00000, 0.02168, 0.31814); }
        else if (index == 75) { return vec3(0.00000, 0.02471, 0.36071); }
    }

    vec3 getPalletteSmall(int index) {
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

    float mandelbrot(float x, float y) {
        if (pow(x + 1.0, 2.0) + y*y <= 0.0625) {
            return MAX;
        }

        vec2 c = vec2(x, y);
        vec2 z = c;

        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 4096.) {
                return float(i) - log2(log2(dot(z, z))) + 4.0;
            }
            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);
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
            }  else if (uGenerator == 3.0) {
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
            }  else if (uGenerator == 3.0) {
                m = mandelbox(x, y);
            } else {
                gl_FragColor = vec4(vTextureCoord.x, vTextureCoord.y, 0.0, 1.);
                return;
            }
        }

        // Colouring
        vec3 col = vec3(0., 0., 0.);
        if (m != MAX) {
            if (uGenerator == 3.0) {  // Mandelbox
                m = mod(m + uPalletteOffset, 16.0);
                col = getPalletteSmall(int(m));
            } else {
                m = mod(m + uPalletteOffset, 75.0);
                vec3 col1 = getPallette(int(m));
                vec3 col2 = getPallette(int(m) + 1);
                float f = mod(m, 1.0);
                col = col1 * (1.0 - f) + col2 * f;
            }
        }
        gl_FragColor = vec4(col.xyz, 1.0);
    }
    """