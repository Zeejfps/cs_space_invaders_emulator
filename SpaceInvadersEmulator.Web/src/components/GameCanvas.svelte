<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import { runFrame, getVRam } from '../lib/emulator';
  import { render, SCREEN_W, SCREEN_H } from '../lib/renderer';
  import { setInput, clearAllInputs, type InputKey } from '../lib/inputState';

  let canvas: HTMLCanvasElement;
  let rafId = 0;

  const KEY_MAP: Record<string, InputKey> = {
    ArrowLeft:  'left',
    ArrowRight: 'right',
    Space:      'fire',
    KeyC:       'coin',
    Digit1:     'p1Start',
    Digit2:     'p2Start',
  };

  function onKeyDown(e: KeyboardEvent): void {
    const k = KEY_MAP[e.code];
    if (!k) return;
    if (k === 'left' || k === 'right' || k === 'fire') e.preventDefault();
    setInput(k, true);
  }

  function onKeyUp(e: KeyboardEvent): void {
    const k = KEY_MAP[e.code];
    if (k) setInput(k, false);
  }

  function loop(): void {
    runFrame();
    const vram = getVRam();
    const ctx = canvas?.getContext('2d');
    if (ctx && vram) render(ctx, vram);
    rafId = requestAnimationFrame(loop);
  }

  onMount(() => {
    window.addEventListener('keydown', onKeyDown);
    window.addEventListener('keyup', onKeyUp);
    rafId = requestAnimationFrame(loop);
  });

  onDestroy(() => {
    cancelAnimationFrame(rafId);
    window.removeEventListener('keydown', onKeyDown);
    window.removeEventListener('keyup', onKeyUp);
    clearAllInputs();
  });
</script>

<!--
  The drawing buffer is fixed at the native 224×256 resolution. CSS sizes
  the displayed element to fill its 7:8-aspect-locked parent — so the canvas
  always renders at correct aspect, scaled crisply via image-rendering:
  pixelated. Browser does the (potentially fractional) nearest-neighbor scale.
-->
<canvas
  bind:this={canvas}
  width={SCREEN_W}
  height={SCREEN_H}
  class="block w-full h-full [image-rendering:pixelated]"
></canvas>
