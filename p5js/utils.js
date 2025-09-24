export function hslForIndex(i, a) {
    step = 5

    angle = baseline + step * i
    while (angle > 360) angle -= 360
    angle = Math.floor(angle)
    return 'hsla(' + angle + ', 100%, 50%, ' + a + ') '
}
