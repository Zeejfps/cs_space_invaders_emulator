<script lang="ts">
  import { onMount } from 'svelte';
  import type { GameDef } from '../lib/games';
  import { router } from '../lib/router.svelte';
  import { touch } from '../lib/touch.svelte';
  import GameCanvas from './GameCanvas.svelte';
  import ControlPanel from './ControlPanel.svelte';
  import MobileControlPad from './MobileControlPad.svelte';
  import KeyboardHints from './KeyboardHints.svelte';

  let { game }: { game: GameDef } = $props();

  let screenEl: HTMLDivElement;
  let maxW = $state(0);
  let maxH = $state(0);

  onMount(() => {
    const ro = new ResizeObserver(() => {
      const r = screenEl.getBoundingClientRect();
      maxW = r.width;
      maxH = r.height;
    });
    ro.observe(screenEl);
    return () => ro.disconnect();
  });

  function back(): void { router.navigate(null); }
</script>

<div class="cabinet h-full w-full flex flex-col">
  <!-- Marquee -->
  <div class="marquee shrink-0 flex items-center justify-between px-3 sm:px-6 py-2 bg-zinc-950 border-b-2 border-amber-900/50">
    <button
      class="font-mono text-xs text-zinc-400 hover:text-[var(--color-crt-green)] tracking-widest uppercase px-2 py-1 rounded border border-zinc-800 hover:border-zinc-600 transition-colors"
      onclick={back}
      aria-label="Back to launcher"
    >‹ BACK</button>
    <div class="font-mono text-amber-300 [text-shadow:0_0_10px_rgba(252,211,77,0.6)] tracking-[0.25em] uppercase text-sm sm:text-lg">
      ★ {game.title} ★
    </div>
    <div class="w-[3.5rem]"><!-- spacer to balance back button --></div>
  </div>

  <!-- Bezel + screen -->
  <div class="bezel flex-1 min-h-0 flex items-center justify-center bg-zinc-900 border-x-4 border-zinc-700 px-2 sm:px-4 py-2 sm:py-4">
    <div
      bind:this={screenEl}
      class="screen relative w-full h-full bg-black border border-zinc-800 [box-shadow:inset_0_0_30px_#000,0_0_25px_rgba(0,200,0,0.18)] flex items-center justify-center"
    >
      <GameCanvas maxWidth={maxW} maxHeight={maxH} />
    </div>
  </div>

  <!-- Controls -->
  {#if touch.isTouch}
    <MobileControlPad />
  {:else}
    <ControlPanel />
    <KeyboardHints />
  {/if}
</div>

<style>
  .cabinet { font-family: ui-monospace, "SF Mono", monospace; }
</style>
