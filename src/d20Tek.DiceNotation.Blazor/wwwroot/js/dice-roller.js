// Canvas renderer for the visual dice roller.
//
// This module is deliberately small: it owns drawing and browser integration only.
// It contains no notation parsing, no random result generation, and no physics.
// Every value it draws arrives in the scene payload produced by C#.

const contexts = new Map();
let nextId = 1;

// Fallback tuning used only if a scene arrives without a visuals block.
const DEFAULT_VISUALS = {
    lightAngle: -Math.PI * 0.75,
    bevelThickness: 0.1,
    smearSteps: 3,
};

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
    const state = {
        canvas,
        context,
        scene: null,
        callback: null,
        frameId: undefined,
        lastFrameTime: 0,
        framePending: false,
    };

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
 * Starts the single animation scheduler for a canvas. The browser's
 * requestAnimationFrame is the only frame source: C# owns the physics and is
 * notified once per displayed frame with the elapsed time since the last frame.
 *
 * Starting a loop that is already running cancels the previous registration
 * first, so a canvas can never own more than one scheduler.
 * @param {number} handle The canvas handle.
 * @param {object} callback A DotNetObjectReference exposing OnFrameAsync.
 * @param {number} maxFrameSeconds The largest elapsed value reported to C#.
 */
export function startLoop(handle, callback, maxFrameSeconds) {
    const state = contexts.get(handle);
    if (!state || !callback) {
        return;
    }

    stopLoop(handle);

    const clamp = Number.isFinite(maxFrameSeconds) && maxFrameSeconds > 0
        ? maxFrameSeconds
        : 0.1;

    state.callback = callback;
    state.lastFrameTime = 0;
    state.framePending = false;

    const onFrame = (timestamp) => {
        // The loop may have been stopped or disposed between frames.
        if (state.frameId === undefined || state.callback !== callback) {
            return;
        }

        state.frameId = requestAnimationFrame(onFrame);

        // Skip overlapping frames so a slow C# frame cannot queue up work.
        if (state.framePending) {
            return;
        }

        // The first frame has no previous timestamp, and a backgrounded tab can
        // produce a very large gap, so the delta is always clamped.
        const previous = state.lastFrameTime || timestamp;
        state.lastFrameTime = timestamp;
        const elapsed = Math.min(Math.max((timestamp - previous) / 1000, 0), clamp);

        state.framePending = true;
        callback.invokeMethodAsync("OnFrameAsync", elapsed)
            .then(() => {
                state.framePending = false;
            })
            .catch(() => {
                // A failed frame callback usually means .NET disposed the loop.
                // Stop scheduling rather than spinning on a broken reference.
                state.framePending = false;
                stopLoop(handle);
            });
    };

    state.frameId = requestAnimationFrame(onFrame);
}

/**
 * Stops the animation scheduler for a canvas. Safe to call when no loop is running.
 * @param {number} handle The canvas handle.
 */
export function stopLoop(handle) {
    const state = contexts.get(handle);
    if (!state) {
        return;
    }

    if (state.frameId !== undefined) {
        cancelAnimationFrame(state.frameId);
        state.frameId = undefined;
    }

    state.callback = null;
    state.lastFrameTime = 0;
    state.framePending = false;
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

    stopLoop(handle);
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

    const visuals = scene.visuals || DEFAULT_VISUALS;
    for (const die of scene.dice) {
        // Every presentation decision was already made in C#; this only draws.
        drawDie(context, scene.theme, visuals, die, scale, originX, originY);
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

function drawDie(context, theme, visuals, die, scale, originX, originY) {
    // The simulation places the origin at the tray center. Canvas Y grows downward,
    // so the Y axis is inverted here rather than in the physics world.
    const centerX = originX + die.x * scale;
    const centerY = originY - die.y * scale;

    // Lift, shadow, and smear all arrive already resolved from C#.
    const width = die.width * scale * die.lift;
    const height = die.height * scale * die.lift;
    const radius = Math.max(0, theme.dieCornerRadius * scale);

    if (die.smearStrength > 0) {
        drawMotionSmear(context, theme, visuals, die, scale, centerX, centerY, width, height, radius);
    }

    context.save();
    context.translate(centerX, centerY);
    context.rotate(-die.rotation);

    context.shadowColor = theme.dieShadowColor;
    context.shadowBlur = Math.max(2, die.shadowBlur * scale);
    context.shadowOffsetY = Math.max(1, die.shadowOffset * scale);

    context.beginPath();
    roundedRect(context, -width / 2, -height / 2, width, height, radius);
    context.fillStyle = buildDieFill(context, theme, visuals, width, height, -die.rotation);
    context.fill();

    context.shadowColor = "transparent";
    context.shadowBlur = 0;
    context.shadowOffsetY = 0;

    drawBevel(context, theme, visuals, width, height, radius, -die.rotation);

    context.lineWidth = Math.max(1, scale * 0.03);
    context.strokeStyle = theme.dieBorderColor;
    context.beginPath();
    roundedRect(context, -width / 2, -height / 2, width, height, radius);
    context.stroke();

    // The value is drawn inside the rotated transform so it tumbles with the body.
    drawValue(context, theme, die, Math.min(width, height));

    context.restore();
}

/**
 * Builds a soft gradient across the die face so it reads as a solid object rather
 * than a flat fill. Falls back to the plain color if gradients are unavailable.
 *
 * The gradient direction is counter-rotated by the die's own rotation so the light
 * appears to come from a fixed point in the scene rather than spinning with the die.
 */
function buildDieFill(context, theme, visuals, width, height, rotation) {
    if (typeof context.createLinearGradient !== "function") {
        return theme.dieColor;
    }

    // Light arrives from a fixed scene direction, expressed in the die's local space.
    const reach = Math.max(width, height) / 2;
    const angle = visuals.lightAngle + rotation;
    const lightX = Math.cos(angle) * reach;
    const lightY = Math.sin(angle) * reach;

    const gradient = context.createLinearGradient(lightX, lightY, -lightX, -lightY);
    gradient.addColorStop(0, theme.dieBevelLightColor);
    gradient.addColorStop(0.4, theme.dieColor);
    gradient.addColorStop(1, theme.dieColor);

    return gradient;
}

/**
 * Draws a lit and shaded inner edge so a square reads as a cube seen from above.
 *
 * The bevel follows the die's rounded outline and is clipped to it, so it can never
 * cut across a corner or bleed outside the silhouette. The lit and shaded halves are
 * selected by clipping to a half-plane whose angle is counter-rotated by the die's
 * rotation, which keeps the light fixed in the scene instead of spinning with the die.
 */
function drawBevel(context, theme, visuals, width, height, radius, rotation) {
    const thickness = Math.max(1, Math.min(width, height) * visuals.bevelThickness);
    const inset = thickness / 2;
    const innerWidth = Math.max(1, width - thickness);
    const innerHeight = Math.max(1, height - thickness);
    const innerRadius = Math.max(0, radius - inset);
    const reach = Math.max(width, height);
    const lightAngle = visuals.lightAngle + rotation;

    context.save();

    // Clipping to the body keeps the bevel inside the die at every rotation.
    context.beginPath();
    roundedRect(context, -width / 2, -height / 2, width, height, radius);
    context.clip();

    context.lineWidth = thickness;

    // The shaded edge covers the whole ring, then the lit half is drawn over it.
    context.beginPath();
    roundedRect(context, -innerWidth / 2, -innerHeight / 2, innerWidth, innerHeight, innerRadius);
    context.strokeStyle = theme.dieBevelDarkColor;
    context.stroke();

    context.save();
    context.rotate(lightAngle);
    context.beginPath();
    context.rect(-reach, -reach, reach * 2, reach);
    context.clip();
    context.rotate(-lightAngle);

    context.beginPath();
    roundedRect(context, -innerWidth / 2, -innerHeight / 2, innerWidth, innerHeight, innerRadius);
    context.strokeStyle = theme.dieBevelLightColor;
    context.stroke();
    context.restore();

    context.restore();
}

/**
 * Draws a short directional trail behind a fast-moving die. This is a cheap stand-in
 * for motion blur: a few fading copies offset along the travel direction. The strength
 * and reach of the trail are decided in C#.
 */
function drawMotionSmear(context, theme, visuals, die, scale, centerX, centerY, width, height, radius) {
    const steps = Math.max(1, visuals.smearSteps);
    const reach = die.smearReach * scale;
    const dirX = die.smearDirectionX;
    const dirY = die.smearDirectionY;

    context.save();
    context.fillStyle = theme.dieMotionColor;

    for (let i = steps; i >= 1; i--) {
        const fraction = i / steps;
        context.globalAlpha = (1 - fraction) * die.smearStrength * 0.5;

        context.save();
        // Trail behind the die along its actual direction of travel. Canvas Y grows
        // downward, so the vertical component is inverted to match the physics world.
        context.translate(centerX - dirX * reach * fraction,
                          centerY + dirY * reach * fraction);
        context.rotate(-die.rotation);
        context.beginPath();
        roundedRect(context, -width / 2, -height / 2, width, height, radius);
        context.fill();
        context.restore();
    }

    context.restore();
}

/**
 * Draws the die's face at the die center, rotated with the body so the numeral tumbles
 * like a real face. Which face to draw is decided in C# and arrives already resolved,
 * including any transient churn while the die is moving quickly.
 */
function drawValue(context, theme, die, size) {
    context.save();
    context.fillStyle = theme.dieTextColor;
    context.font = `600 ${Math.max(8, size * 0.5)}px system-ui, sans-serif`;
    context.textAlign = "center";
    context.textBaseline = "middle";

    // Keep the numeral upright relative to its own face, not the viewport.
    context.fillText(String(die.shownValue), 0, 0);
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
