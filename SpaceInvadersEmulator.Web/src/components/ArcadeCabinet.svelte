<script lang="ts">
  import { onMount, tick } from 'svelte';
  import type { GameDef } from '../lib/games';
  import { router } from '../lib/router.svelte';
  import { touch } from '../lib/touch.svelte';
  import GameCanvas from './GameCanvas.svelte';
  import ControlPanel from './ControlPanel.svelte';
  import MobileControlPad from './MobileControlPad.svelte';
  import ControlsHelp from './ControlsHelp.svelte';

  let { game }: { game: GameDef } = $props();

  // Help popover state. Click outside / Esc / re-click trigger to close.
  let helpOpen = $state(false);
  let helpButton: HTMLButtonElement | undefined = $state();
  let helpPopover: HTMLDivElement | undefined = $state();

  async function toggleHelp(): Promise<void> {
    helpOpen = !helpOpen;
    if (helpOpen) {
      // Wait for the popover to render before attaching outside-click listener,
      // otherwise the same click that opened it would immediately close it.
      await tick();
      document.addEventListener('pointerdown', onOutsidePointer, true);
      window.addEventListener('keydown', onEscape);
    } else {
      document.removeEventListener('pointerdown', onOutsidePointer, true);
      window.removeEventListener('keydown', onEscape);
    }
  }
  function onOutsidePointer(e: PointerEvent): void {
    const t = e.target as Node;
    if (helpPopover?.contains(t) || helpButton?.contains(t)) return;
    helpOpen = false;
    document.removeEventListener('pointerdown', onOutsidePointer, true);
    window.removeEventListener('keydown', onEscape);
  }
  function onEscape(e: KeyboardEvent): void {
    if (e.key === 'Escape') {
      helpOpen = false;
      document.removeEventListener('pointerdown', onOutsidePointer, true);
      window.removeEventListener('keydown', onEscape);
    }
  }

  // Compute the screen's exact pixel dimensions (preserving 7:8 aspect) by
  // taking whichever axis is limiting:
  //   availH = hostH - marqueeH - controlsH - bezelVertPadding
  //   availW = hostW                         - bezelHorizPadding
  //   screenH = min(availH, availW * 8/7)
  //   screenW = screenH * 7/8
  //   cabinetW = screenW + bezelHorizPadding
  // The cabinet is content-sized (not flex-1) so when the height-derived path
  // wins it fills the host, and when the width-derived path wins (narrow
  // viewport) it shrinks vertically and centers — never stretching beyond aspect.
  const BEZEL_VERT_PADDING = 24;   // py-3 top + py-3 bottom
  const BEZEL_HORIZ_PADDING = 32;  // px-3 + border-x-4 on each side
  const ASPECT_W = 7;
  const ASPECT_H = 8;

  let host: HTMLDivElement;
  let marqueeEl: HTMLDivElement;
  let controlsEl: HTMLDivElement;
  let screenW = $state(0);
  let screenH = $state(0);

  onMount(() => {
    function recompute(): void {
      const hostW = host.clientWidth;
      const hostH = host.clientHeight;
      const mh = marqueeEl.offsetHeight;
      const ch = controlsEl.offsetHeight;
      const availH = Math.max(0, hostH - mh - ch - BEZEL_VERT_PADDING);
      const availW = Math.max(0, hostW - BEZEL_HORIZ_PADDING);
      const sH = Math.max(0, Math.min(availH, (availW * ASPECT_H) / ASPECT_W));
      screenH = Math.round(sH);
      screenW = Math.round((sH * ASPECT_W) / ASPECT_H);
    }
    const ro = new ResizeObserver(recompute);
    ro.observe(host);
    ro.observe(marqueeEl);
    ro.observe(controlsEl);
    recompute();
    return () => ro.disconnect();
  });

  function back(): void { router.navigate(null); }
</script>

<!--
  Layout strategy:

  - The cabinet is an inline-grid centered in the viewport. Its column width
    is determined by the widest child — which is the bezel, since the bezel
    contains the screen (aspect-locked to 7:8 with `h-full`, deriving its
    width from available vertical space). Marquee and controls inherit that
    same column width via grid stretch alignment.
  - On wide desktops: cabinet width is height-driven, leaving flanking space
    around the cabinet (handled by the radial backdrop on <main>).
  - On portrait phones: the screen's `max-w-full` clamps the height-derived
    width to viewport, so the cabinet shrinks to fit. Vertical slack inside
    the bezel becomes part of the chrome.
-->
<!--
  Outer host: full viewport, used by ResizeObserver to measure available height.
  Inner cabinet: explicit JS-computed width, centered via mx-auto.
-->
<div bind:this={host} class="cabinet-host h-full w-full flex items-center justify-center">
  <div
    class="cabinet flex flex-col"
    style="width: {screenW ? screenW + BEZEL_HORIZ_PADDING + 'px' : '100%'};"
  >
    <!-- Marquee -->
    <div bind:this={marqueeEl} class="marquee flex items-center gap-2 px-3 py-2 bg-zinc-950 border-b-2 border-amber-900/50">
      <button
        class="shrink-0 font-mono text-xs text-zinc-400 hover:text-[var(--color-crt-green)] tracking-widest uppercase px-2 py-1 rounded border border-zinc-800 hover:border-zinc-600 transition-colors"
        onclick={back}
        aria-label="Back to launcher"
      >‹ BACK</button>
      <div class="flex-1 min-w-0 text-center font-mono text-amber-300 [text-shadow:0_0_10px_rgba(252,211,77,0.6)] uppercase whitespace-nowrap overflow-hidden marquee-title">
        ★ {game.title} ★
      </div>
      {#if !touch.isTouch}
        <div class="shrink-0 w-[3.5rem] flex justify-end relative">
          <button
            bind:this={helpButton}
            class="font-mono text-xs text-zinc-400 hover:text-[var(--color-crt-green)] uppercase w-7 h-7 rounded-full border border-zinc-800 hover:border-zinc-600 transition-colors flex items-center justify-center"
            onclick={toggleHelp}
            aria-label="Show controls"
            aria-expanded={helpOpen}
          >?</button>
          {#if helpOpen}
            <div
              bind:this={helpPopover}
              role="dialog"
              aria-label="Controls"
              class="help-popover absolute right-0 top-[calc(100%+0.5rem)] z-50 min-w-[14rem] bg-zinc-950 border border-zinc-700 rounded-md shadow-[0_10px_30px_rgba(0,0,0,0.8),0_0_20px_rgba(0,200,0,0.08)] p-3"
            >
              <ControlsHelp />
            </div>
          {/if}
        </div>
      {:else}
        <div class="shrink-0 w-[3.5rem]" aria-hidden="true"><!-- balance back button --></div>
      {/if}
    </div>

    <!-- Bezel + screen -->
    <div class="bezel flex items-center justify-center bg-zinc-900 border-x-4 border-zinc-700 px-3 py-3">
      <div
        class="screen bg-black border border-zinc-800 [box-shadow:inset_0_0_30px_#000,0_0_25px_rgba(0,200,0,0.18)]"
        style="width: {screenW}px; height: {screenH}px;"
      >
        <GameCanvas />
      </div>
    </div>

    <!-- Controls (wrap so ResizeObserver sees a single element) -->
    <div bind:this={controlsEl} class="controls">
      {#if touch.isTouch}
        <MobileControlPad />
      {:else}
        <ControlPanel />
      {/if}
    </div>
  </div>
</div>

<style>
  .cabinet { font-family: ui-monospace, "SF Mono", monospace; }
  .help-popover {
    animation: help-in 110ms ease-out;
    transform-origin: top right;
  }
  @keyframes help-in {
    from { opacity: 0; transform: scale(0.96) translateY(-2px); }
    to   { opacity: 1; transform: scale(1) translateY(0); }
  }
  /* Title scales with cabinet width (cqw = 1% of containing element's width
     when container-type is set on the marquee). Falls back to viewport-based
     clamp on browsers without container queries. */
  .marquee {
    container-type: inline-size;
  }
  .marquee-title {
    font-size: clamp(0.7rem, 4cqw, 1.25rem);
    letter-spacing: clamp(0.05em, 2cqw, 0.25em);
  }
  @supports not (container-type: inline-size) {
    .marquee-title {
      font-size: clamp(0.7rem, 3vw, 1.125rem);
      letter-spacing: clamp(0.05em, 1.5vw, 0.25em);
    }
  }
</style>
