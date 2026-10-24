// Canvas renderer for the visual dice roller.
//
// This module is deliberately small: it owns drawing and browser integration only.
// It contains no notation parsing, no random result generation, and no physics.
// Every value it draws arrives in the scene payload produced by C#.

const contexts = new Map();
let nextId = 1;

/**
 * Initializes a canvas and returns a handle used by the other exported functions.
 * @param {HTMLCanvasElement} canvas The canvas element to draw on.
 * @returns {number} The handle identifying this canvas.
 */
export function initialize(canvas) {
    if (!canvas) {
        throw new Error("A canvas element is required.");
    }

    const handle = nextId++;
    const context = canvas.getContext("2d");
    const state = { canvas, context, scene: null };

    // Redraw on container resize so the scene survives layout changes.
    if (typeof ResizeObserver !== "undefined") {
        state.observer = new ResizeObserver(() => {
            resizeBackingBuffer(state);
            draw(state);
        });
        state.observer.observe(canvas);
    }

    contexts.set(handle, state);
    resizeBackingBuffer(state);

    return handle;
}

/**
 * Draws a complete scene from a snapshot.
 * @param {number} handle The canvas handle.
 * @param {object} scene The scene payload supplied by C#.
 */
export function render(handle, scene) {
    const state = contexts.get(handle);
    if (!state) {
        return;
    }

    state.scene = scene;
    draw(state);
}

/**
 * Recomputes the backing buffer and redraws the current scene. Called when the
 * element size or device pixel ratio changes.
 * @param {number} handle The canvas handle.
 */
export function resize(handle) {
    const state = contexts.get(handle);
    if (!state) {
        return;
    }

    resizeBackingBuffer(state);
    draw(state);
}

/**
 * Releases the observer and cached state for a canvas.
 * @param {number} handle The canvas handle.
 */
export function dispose(handle) {
    const state = contexts.get(handle);
    if (!state) {
        return;
    }

    state.observer?.disconnect();
    contexts.delete(handle);
}

/**
 * Scales the backing buffer for high-DPI displays while keeping drawing
 * operations in logical CSS pixels.
 */
function resizeBackingBuffer(state) {
    const { canvas } = state;
    const ratio = window.devicePixelRatio || 1;
    const rect = canvas.getBoundingClientRect();
    const width = Math.max(1, Math.round(rect.width));
    const height = Math.max(1, Math.round(rect.height));

    canvas.width = Math.round(width * ratio);
    canvas.height = Math.round(height * ratio);

    state.cssWidth = width;
    state.cssHeight = height;
    state.ratio = ratio;
}

function draw(state) {
    const { context, scene, cssWidth, cssHeight, ratio } = state;
    if (!context) {
        return;
    }

    // Reset the transform before scaling so repeated draws do not compound it.
    context.setTransform(ratio, 0, 0, ratio, 0, 0);
    context.clearRect(0, 0, cssWidth, cssHeight);

    if (!scene) {
        return;
    }

    // Map logical simulation units to CSS pixels, preserving aspect ratio and
    // centering the tray within the element.
    const scale = Math.min(cssWidth / scene.tray.width, cssHeight / scene.tray.height);
    const originX = cssWidth / 2;
    const originY = cssHeight / 2;

    drawTray(context, scene, scale, originX, originY);

    for (const die of scene.dice) {
        drawDie(context, scene.theme, die, scale, originX, originY);
    }
}

function drawTray(context, scene, scale, originX, originY) {
    const { theme, tray } = scene;
    const width = tray.width * scale;
    const height = tray.height * scale;
    const radius = Math.max(0, theme.trayCornerRadius * scale);

    context.save();
    context.beginPath();
    roundedRect(context, originX - width / 2, originY - height / 2, width, height, radius);
    context.fillStyle = theme.trayColor;
    context.fill();
    context.lineWidth = Math.max(1, scale * 0.06);
    context.strokeStyle = theme.trayBorderColor;
    context.stroke();
    context.restore();
}

function drawDie(context, theme, die, scale, originX, originY) {
    // The simulation places the origin at the tray center. Canvas Y grows downward,
    // so the Y axis is inverted here rather than in the physics world.
    const centerX = originX + die.x * scale;
    const centerY = originY - die.y * scale;
    const width = die.width * scale;
    const height = die.height * scale;
    const radius = Math.max(0, theme.dieCornerRadius * scale);

    context.save();
    context.translate(centerX, centerY);
    context.rotate(-die.rotation);

    context.shadowColor = theme.dieShadowColor;
    context.shadowBlur = Math.max(2, scale * 0.12);
    context.shadowOffsetY = Math.max(1, scale * 0.05);

    context.beginPath();
    roundedRect(context, -width / 2, -height / 2, width, height, radius);
    context.fillStyle = theme.dieColor;
    context.fill();

    context.shadowColor = "transparent";
    context.shadowBlur = 0;
    context.shadowOffsetY = 0;
    context.lineWidth = Math.max(1, scale * 0.03);
    context.strokeStyle = theme.dieBorderColor;
    context.stroke();
    context.restore();

    drawValue(context, theme, die, centerX, centerY, Math.min(width, height));
}

/**
 * Draws the die's fixed value upright at the die center. The label is intentionally
 * not rotated with the body so it stays legible at any orientation.
 */
function drawValue(context, theme, die, centerX, centerY, size) {
    context.save();
    context.translate(centerX, centerY);
    context.fillStyle = theme.dieTextColor;
    context.font = `600 ${Math.max(8, size * 0.5)}px system-ui, sans-serif`;
    context.textAlign = "center";
    context.textBaseline = "middle";
    context.fillText(String(die.value), 0, 0);
    context.restore();
}

function roundedRect(context, x, y, width, height, radius) {
    const limit = Math.min(radius, width / 2, height / 2);
    if (typeof context.roundRect === "function") {
        context.roundRect(x, y, width, height, limit);
        return;
    }

    context.moveTo(x + limit, y);
    context.arcTo(x + width, y, x + width, y + height, limit);
    context.arcTo(x + width, y + height, x, y + height, limit);
    context.arcTo(x, y + height, x, y, limit);
    context.arcTo(x, y, x + width, y, limit);
    context.closePath();
}
