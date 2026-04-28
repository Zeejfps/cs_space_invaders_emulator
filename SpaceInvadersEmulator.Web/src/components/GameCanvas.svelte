<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import {
    runFrame, getVRam,
    writeP1Left, writeP1Right, writeP1Fire,
    writeCoin, writeP1Start, writeP2Start,
  } from '../lib/emulator';
  import { render, SCREEN_W, SCREEN_H } from '../lib/renderer';

  let canvas: HTMLCanvasElement;
  let rafId = 0;
  let scale = $state(1);

  function computeScale(): void {
    // Largest integer scale that fits the viewport height (with some margin for the cabinet chrome).
    const available = window.innerHeight * 0.85;
    scale = Math.max(1, Math.floor(available / SCREEN_H));
  }

  function loop(): void {
    runFrame();
    const vram = getVRam();
    const ctx = canvas?.getContext('2d');
    if (ctx && vram) render(ctx, vram);
    rafId = requestAnimationFrame(loop);
  }

  function onKeyDown(e: KeyboardEvent): void {
    switch (e.code) {
      case 'ArrowLeft':  e.preventDefault(); writeP1Left(true);  break;
      case 'ArrowRight': e.preventDefault(); writeP1Right(true); break;
      case 'Space':      e.preventDefault(); writeP1Fire(true);  break;
      case 'KeyC':                           writeCoin(true);    break;
      case 'Digit1':                         writeP1Start(true); break;
      case 'Digit2':                         writeP2Start(true); break;
    }
  }

  function onKeyUp(e: KeyboardEvent): void {
    switch (e.code) {
      case 'ArrowLeft':  writeP1Left(false);  break;
      case 'ArrowRight': writeP1Right(false); break;
      case 'Space':      writeP1Fire(false);  break;
      case 'KeyC':       writeCoin(false);    break;
      case 'Digit1':     writeP1Start(false); break;
      case 'Digit2':     writeP2Start(false); break;
    }
  }

  onMount(() => {
    computeScale();
    window.addEventListener('keydown', onKeyDown);
    window.addEventListener('keyup', onKeyUp);
    window.addEventListener('resize', computeScale);
    rafId = requestAnimationFrame(loop);
  });

  onDestroy(() => {
    cancelAnimationFrame(rafId);
    window.removeEventListener('keydown', onKeyDown);
    window.removeEventListener('keyup', onKeyUp);
    window.removeEventListener('resize', computeScale);
  });
</script>

<!--
  The canvas drawing buffer is always the native resolution (224×256).
  CSS scaling via width/height + image-rendering: pixelated gives crisp integer scaling.
-->
<canvas
  bind:this={canvas}
  width={SCREEN_W}
  height={SCREEN_H}
  style="width: {SCREEN_W * scale}px; height: {SCREEN_H * scale}px; image-rendering: pixelated; display: block;"
></canvas>

<style>
  canvas {
    cursor: none;
  }
</style>
