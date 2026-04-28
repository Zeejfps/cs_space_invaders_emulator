// Converts the Space Invaders VRam (256×224 px, 1-bit packed, column-major)
// to a 224×256 canvas ImageData with arcade-accurate color zones.
//
// VRam layout:  256 columns, each 28 bytes (224 rows / 8 bits).
//   bit (col, row) where col ∈ [0,255], row ∈ [0,223]
//
// Screen mapping (90° CW rotation from VRam perspective):
//   screen pixel (sx, sy)  →  vram bit col=(255 - sy), row=sx
//
// Color zones (screen Y):
//   0–31:    red   — score & lives
//   32–191:  white — invaders & UFO
//   192–255: green — shields & player

export const SCREEN_W = 224;
export const SCREEN_H = 256;

const BYTES_PER_COL = 28; // 224 / 8

let imageData: ImageData | null = null;

export function render(ctx: CanvasRenderingContext2D, vram: Uint8Array): void {
  if (!imageData) {
    imageData = ctx.createImageData(SCREEN_W, SCREEN_H);
    // Pre-fill alpha channel to fully opaque.
    for (let i = 3; i < imageData.data.length; i += 4) imageData.data[i] = 255;
  }

  const data = imageData.data;

  for (let sy = 0; sy < SCREEN_H; sy++) {
    const vramCol = 255 - sy;
    const byteBase = vramCol * BYTES_PER_COL;
    const pixelRowBase = sy * SCREEN_W * 4;

    let r: number, g: number, b: number;
    if (sy < 32) {
      r = 255; g = 0;   b = 0;   // red
    } else if (sy < 192) {
      r = 255; g = 255; b = 255; // white
    } else {
      r = 0;   g = 255; b = 0;   // green
    }

    for (let sx = 0; sx < SCREEN_W; sx++) {
      const byteIdx = byteBase + (sx >> 3);
      const on = (vram[byteIdx] >> (sx & 7)) & 1;
      const i = pixelRowBase + sx * 4;
      data[i]     = on ? r : 0;
      data[i + 1] = on ? g : 0;
      data[i + 2] = on ? b : 0;
      // alpha already set to 255
    }
  }

  ctx.putImageData(imageData, 0, 0);
}
