<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import { runFrame, getVRam } from '../lib/emulator';
  import { render, SCREEN_W, SCREEN_H } from '../lib/renderer';
  import { setInput, clearAllInputs, type InputKey } from '../lib/inputState';

  let { maxHeight = 0, maxWidth = 0 }: { maxHeight?: number; maxWidth?: number } = $props();

  let canvas: HTMLCanvasElement;
  let rafId = 0;

  // Compute the largest scale that fits the parent's bounds. Allow non-integer
  // scales — black bars on mobile are worse than slight pixel shimmer.
  const scale = $derived.by(() => {
    if (!maxWidth && !maxHeight) return 2;
    const sH = maxHeight ? maxHeight / SCREEN_H : Infinity;
    const sW = maxWidth ? maxWidth / SCREEN_W : Infinity;
    return Math.max(1, Math.min(sH, sW));
  });

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

<canvas
  bind:this={canvas}
  width={SCREEN_W}
  height={SCREEN_H}
  style="width: {SCREEN_W * scale}px; height: {SCREEN_H * scale}px; image-rendering: pixelated; display: block;"
></canvas>

<style>
  canvas { cursor: none; }
</style>
