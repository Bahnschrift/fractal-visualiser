import { ofArray, tryFind } from "./.fable/fable-library.3.1.1/Map.js";
import { equalsWith, map } from "./.fable/fable-library.3.1.1/Array.js";
import { comparePrimitives } from "./.fable/fable-library.3.1.1/Util.js";

export function findCookieValue(name) {
    return tryFind(name, ofArray(map((s) => {
        const kvArr = s.trim().split("=");
        if ((!equalsWith(comparePrimitives, kvArr, null)) ? (kvArr.length === 2) : false) {
            return [kvArr[0], kvArr[1]];
        }
        else {
            return ["", ""];
        }
    }, document.cookie.split(";"))));
}

