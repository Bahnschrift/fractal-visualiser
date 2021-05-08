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
    uniform float uRatio;
    uniform float uGenerator;
    uniform float uMandelboxScale;
    uniform float uJuliaX;
    uniform float uJuliaY;

    const float MAX = 1000.;

    vec3 getPalatte(int index) {
        if (index == 0) { return vec3(0.00005, 0.02816, 0.39805); }
        else if (index == 1) { return vec3(0.00166, 0.03771, 0.43408); }
        else if (index == 2) { return vec3(0.00555, 0.05555, 0.47167); }
        else if (index == 3) { return vec3(0.01152, 0.08046, 0.51019); }
        else if (index == 4) { return vec3(0.01937, 0.1112, 0.54901); }
        else if (index == 5) { return vec3(0.02888, 0.14654, 0.58753); }
        else if (index == 6) { return vec3(0.03987, 0.18527, 0.62511); }
        else if (index == 7) { return vec3(0.05214, 0.22615, 0.66114); }
        else if (index == 8) { return vec3(0.06548, 0.26795, 0.69499); }
        else if (index == 9) { return vec3(0.0797, 0.30945, 0.72605); }
        else if (index == 10) { return vec3(0.09459, 0.34942, 0.75368); }
        else if (index == 11) { return vec3(0.10996, 0.38663, 0.77727); }
        else if (index == 12) { return vec3(0.12561, 0.41984, 0.7962); }
        else if (index == 13) { return vec3(0.14545, 0.45198, 0.81253); }
        else if (index == 14) { return vec3(0.17281, 0.48636, 0.82859); }
        else if (index == 15) { return vec3(0.20679, 0.52255, 0.84433); }
        else if (index == 16) { return vec3(0.2465, 0.56012, 0.85969); }
        else if (index == 17) { return vec3(0.29104, 0.59865, 0.87459); }
        else if (index == 18) { return vec3(0.33953, 0.6377, 0.88898); }
        else if (index == 19) { return vec3(0.39105, 0.67686, 0.90278); }
        else if (index == 20) { return vec3(0.44473, 0.71568, 0.91594); }
        else if (index == 21) { return vec3(0.49967, 0.75374, 0.92839); }
        else if (index == 22) { return vec3(0.55496, 0.79061, 0.94006); }
        else if (index == 23) { return vec3(0.60972, 0.82587, 0.9509); }
        else if (index == 24) { return vec3(0.66305, 0.85909, 0.96083); }
        else if (index == 25) { return vec3(0.71406, 0.88983, 0.96979); }
        else if (index == 26) { return vec3(0.76185, 0.91767, 0.97771); }
        else if (index == 27) { return vec3(0.80553, 0.94219, 0.98454); }
        else if (index == 28) { return vec3(0.84421, 0.96294, 0.99021); }
        else if (index == 29) { return vec3(0.87698, 0.97951, 0.99465); }
        else if (index == 30) { return vec3(0.90296, 0.99147, 0.9978); }
        else if (index == 31) { return vec3(0.92125, 0.99838, 0.99959); }
        else if (index == 32) { return vec3(0.93123, 0.99988, 0.99937); }
        else if (index == 33) { return vec3(0.93843, 0.9971, 0.98452); }
        else if (index == 34) { return vec3(0.94534, 0.99079, 0.95178); }
        else if (index == 35) { return vec3(0.95193, 0.98123, 0.90364); }
        else if (index == 36) { return vec3(0.95818, 0.9687, 0.84257); }
        else if (index == 37) { return vec3(0.96408, 0.95346, 0.77105); }
        else if (index == 38) { return vec3(0.9696, 0.93578, 0.69157); }
        else if (index == 39) { return vec3(0.97473, 0.91594, 0.6066); }
        else if (index == 40) { return vec3(0.97944, 0.8942, 0.51864); }
        else if (index == 41) { return vec3(0.98372, 0.87083, 0.43015); }
        else if (index == 42) { return vec3(0.98755, 0.84611, 0.34362); }
        else if (index == 43) { return vec3(0.9909, 0.82031, 0.26153); }
        else if (index == 44) { return vec3(0.99376, 0.79369, 0.18637); }
        else if (index == 45) { return vec3(0.9961, 0.76653, 0.1206); }
        else if (index == 46) { return vec3(0.99792, 0.73909, 0.06672); }
        else if (index == 47) { return vec3(0.99918, 0.71165, 0.0272); }
        else if (index == 48) { return vec3(0.99987, 0.68448, 0.00453); }
        else if (index == 49) { return vec3(0.99876, 0.65735, 0.0); }
        else if (index == 50) { return vec3(0.9811, 0.62437, 0.0); }
        else if (index == 51) { return vec3(0.94463, 0.5847, 0.0); }
        else if (index == 52) { return vec3(0.89211, 0.53955, 0.0); }
        else if (index == 53) { return vec3(0.8263, 0.49015, 0.0); }
        else if (index == 54) { return vec3(0.74993, 0.43771, 0.0); }
        else if (index == 55) { return vec3(0.66576, 0.38345, 0.0); }
        else if (index == 56) { return vec3(0.57655, 0.32857, 0.0); }
        else if (index == 57) { return vec3(0.48503, 0.2743, 0.0); }
        else if (index == 58) { return vec3(0.39396, 0.22186, 0.0); }
        else if (index == 59) { return vec3(0.30608, 0.17245, 0.0); }
        else if (index == 60) { return vec3(0.22416, 0.12729, 0.0); }
        else if (index == 61) { return vec3(0.15093, 0.0876, 0.0); }
        else if (index == 62) { return vec3(0.08915, 0.0546, 0.0); }
        else if (index == 63) { return vec3(0.04157, 0.02949, 0.0); }
        else if (index == 64) { return vec3(0.01094, 0.0135, 0.0); }
        else if (index == 65) { return vec3(0.0, 0.00784, 0.0); }
        else if (index == 66) { return vec3(0.0, 0.00803, 0.00641); }
        else if (index == 67) { return vec3(0.0, 0.00858, 0.02456); }
        else if (index == 68) { return vec3(0.0, 0.00949, 0.05251); }
        else if (index == 69) { return vec3(0.0, 0.01074, 0.08835); }
        else if (index == 70) { return vec3(0.0, 0.01232, 0.13013); }
        else if (index == 71) { return vec3(0.0, 0.01422, 0.17593); }
        else if (index == 72) { return vec3(0.0, 0.01642, 0.22382); }
        else if (index == 73) { return vec3(0.0, 0.01891, 0.27186); }
        else if (index == 74) { return vec3(0.0, 0.02168, 0.31814); }
        else if (index == 75) { return vec3(0.0, 0.02471, 0.36071); }
    }

    vec3 getPalatteOld(int index) {
        if (index == 0) { return vec3(66, 30, 15) / 255.; }
        else if (index == 1) { return vec3(25, 7, 26) / 255.; }
        else if (index == 2) { return vec3(9, 1, 47) / 255.; }
        else if (index == 3) { return vec3(4, 4, 73) / 255.; }
        else if (index == 4) { return vec3(0, 7, 100) / 255.; }
        else if (index == 5) { return vec3(12, 44, 138) / 255.; }
        else if (index == 6) { return vec3(24, 82, 177) / 255.; }
        else if (index == 7) { return vec3(57, 125, 209) / 255.; }
        else if (index == 8) { return vec3(134, 181, 229) / 255.; }
        else if (index == 9) { return vec3(211, 236, 248) / 255.; }
        else if (index == 10) { return vec3(241, 233, 191) / 255.; }
        else if (index == 11) { return vec3(248, 201, 95) / 255.; }
        else if (index == 12) { return vec3(255, 170, 0) / 255.; }
        else if (index == 13) { return vec3(204, 128, 0) / 255.; }
        else if (index == 14) { return vec3(153, 87, 0) / 255.; }
        else if (index == 15) { return vec3(106, 52, 3) / 255.; }
    }

    vec3 getPalatteBW(int index) {
        if (index == 0) {return vec3(0.4, 0.4, 0.4); }
        else if (index == 1) { return vec3(0.7, 0.7, 0.7); }
        else if (index == 1) { return vec3(0.8, 0.8, 0.8); }
        else if (index == 1) { return vec3(0.9, 0.9, 0.9); }
        else if (index == 1) { return vec3(1.0, 1.0, 1.0); }
    }

    vec3 linearInterpolate(vec3 v1, vec3 v2, float f) {
        return v1 * (1.0 - f) + v2 * f;
    }

    float mandelbrot(float x, float y) {
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

    float julia(float x, float y) {
        vec2 c = vec2(uJuliaX, uJuliaY);
        vec2 z = vec2(x, y);

        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 4096.) {
                return float(i) - log2(log(dot(z, z)) / log(10.)) + 2.0;
            }
            z = vec2(z.x*z.x - z.y*z.y + c.x, 2.0*z.x*z.y + c.y);
        }
        return MAX;
    }

    float mandelbox(float x, float y) {
        vec2 c = vec2(x, y);
        vec2 z = c;
        
        for (int i = 0; i <= int(MAX); i++) {
            if (length(z) > 4096.) {
                return float(i) - log2(log2(dot(z, z))) + 4.0;
                // return float(i);
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
        float x = xc + (vTextureCoord.x - 0.5) * uZoom * uRatio;
        float y = yc + (vTextureCoord.y - 0.5) * uZoom;
        float m = 0.0;
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

        vec3 col = vec3(0., 0., 0.);
        if (m != MAX) {
            if (uGenerator == 3.0) {
                m = mod(m, 16.0);
                vec3 col1 = getPalatteOld(int(m));
                vec3 col2 = getPalatteOld(int(m) + 1);
                col = linearInterpolate(col1, col2, mod(m, 1.0));
                // col = getPalatteOld(int(mod(m, 16.)));
            } else {
                // col = getPalatte(int(mod(m, 75.)));
                m = mod(m, 75.0);
                vec3 col1 = getPalatte(int(m));
                vec3 col2 = getPalatte(int(m) + 1);
                col = col1 * (1.0 - mod(m, 1.0)) + col2 * (mod(m, 1.0));

                // m = mod(m, 5.0);
                // vec3 col1 = getPalatteBW(int(m));
                // vec3 col2 = getPalatteBW(int(m) + 1);
                // col = col1 * (1.0 - mod(m, 1.0)) + col2 * (mod(m, 1.0));
                // col = getPalatte(int(mod(m, 75.)));
            }
        }
        gl_FragColor = vec4(col.x, col.y, col.z, 1.);
    }
    """