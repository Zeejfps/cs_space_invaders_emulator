// Converts the Space Invaders VRam to a 224×256 canvas ImageData with
// arcade-accurate color zones.
//
// VRam layout (per Computer Archeology / hardware reference):
//   7168 bytes at $2400. Each byte = 8 vertical pixels on the rotated CRT.
//   Memory increases UP the rotated screen first, then RIGHT.
//     - 224 "columns" of 32 bytes each (224 * 32 = 7168)
//     - column index   = sx (rotated screen X, 0..223; left → right)
//     - within column  = bytes go bottom → top
//     - within a byte  = LSB is the bottom pixel of the 8-pixel vertical strip
//
// Rotated screen pixel (sx, sy) where sy=0 is the TOP of the screen:
//   r        = 255 - sy                       // row from bottom
//   byteIdx  = sx * 32 + (r >> 3)
//   bit      = (vram[byteIdx] >> (r & 7)) & 1
//
// Color zones (rotated screen Y, 0 = top):
//   0–31:    red   — score & lives (bottom of original CRT before rotation)
//   32–191:  white — invaders & UFO
//   192–255: green — shields & player

export const SCREEN_W = 224;
export const SCREEN_H = 256;

const BYTES_PER_COL = 32; // 256 / 8

let imageData: ImageData | null = null;

export function render(ctx: CanvasRenderingContext2D, vram: Uint8Array): void {
  if (!imageData) {
    imageData = ctx.createImageData(SCREEN_W, SCREEN_H);
    // Pre-fill alpha channel to fully opaque.
    for (let i = 3; i < imageData.data.length; i += 4) imageData.data[i] = 255;
  }

  const data = imageData.data;

  for (let sy = 0; sy < SCREEN_H; sy++) {
    const r = 255 - sy;
    const byteOffsetInCol = r >> 3;
    const bitMask = 1 << (r & 7);
    const pixelRowBase = sy * SCREEN_W * 4;

    let cr: number, cg: number, cb: number;
    if (sy < 32) {
      cr = 255; cg = 0;   cb = 0;   // red
    } else if (sy < 192) {
      cr = 255; cg = 255; cb = 255; // white
    } else {
      cr = 0;   cg = 255; cb = 0;   // green
    }

    for (let sx = 0; sx < SCREEN_W; sx++) {
      const byteIdx = sx * BYTES_PER_COL + byteOffsetInCol;
      const on = vram[byteIdx] & bitMask;
      const i = pixelRowBase + sx * 4;
      if (on) {
        data[i]     = cr;
        data[i + 1] = cg;
        data[i + 2] = cb;
      } else {
        data[i]     = 0;
        data[i + 1] = 0;
        data[i + 2] = 0;
      }
      // alpha already set to 255
    }
  }

  ctx.putImageData(imageData, 0, 0);
}
