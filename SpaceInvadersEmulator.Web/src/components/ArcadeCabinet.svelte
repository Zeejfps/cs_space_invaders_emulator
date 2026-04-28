<script lang="ts">
  import { onMount, tick } from 'svelte';
  import type { GameDef } from '../lib/games';
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
  const BEZEL_HORIZ_PADDING = 24;  // px-3 on each side
  const ASPECT_W = 7;
  const ASPECT_H = 8;

  let host: HTMLDivElement;
  let marqueeEl: HTMLDivElement;
  let controlsEl: HTMLDivElement;
  let bezelEl: HTMLDivElement;
  let screenW = $state(0);
  let screenH = $state(0);
  // Bezel dimensions feed the SVG perspective overlay (lines from screen
  // corners to bezel corners). Tracked separately from screenW/H because
  // the bezel grows with vertical slack while the screen is aspect-locked.
  let bezelW = $state(0);
  let bezelH = $state(0);

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
      bezelW = bezelEl.clientWidth;
      bezelH = bezelEl.clientHeight;
    }
    const ro = new ResizeObserver(recompute);
    ro.observe(host);
    ro.observe(marqueeEl);
    ro.observe(controlsEl);
    ro.observe(bezelEl);
    recompute();
    return () => ro.disconnect();
  });
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
  Outer host fills the viewport. Inside it, a vertical column holds the
  marquee and cabinet as siblings, both sized to the same cabinet width.
  Marquee is no longer a child of the cabinet (so it can have its own
  visual identity as a header) but sits in the same column, aligned to
  cabinet width — not spanning the full viewport. The cabinet stretches
  to fill the remaining vertical space; its bezel grows to absorb any
  slack so there's no gap between the marquee and the controls.
-->
<div bind:this={host} class="cabinet-host h-full w-full flex flex-col items-center">
  <!-- Marquee: sibling of cabinet, sized to cabinet width. -->
  <div
    bind:this={marqueeEl}
    class="marquee shrink-0 w-full flex items-center gap-2 px-3 py-5 bg-zinc-950 border-b-2 border-amber-900/50"
    style="max-width: {screenW ? screenW + BEZEL_HORIZ_PADDING + 'px' : '100%'};"
  >
    {#if !touch.isTouch}
      <!-- Invisible left flank balances the help "?" on the right so the title stays centered. -->
      <div class="shrink-0 w-[3.5rem]" aria-hidden="true"></div>
    {/if}
    <div class="flex-1 min-w-0 text-center font-mono text-amber-300 [text-shadow:0_0_10px_rgba(252,211,77,0.6)] uppercase whitespace-nowrap overflow-hidden leading-none marquee-title">
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
    {/if}
  </div>

  <!-- Cabinet: stretches to fill remaining vertical space; bezel absorbs slack. -->
  <div
    class="cabinet flex-1 min-h-0 flex flex-col w-full"
    style="max-width: {screenW ? screenW + BEZEL_HORIZ_PADDING + 'px' : '100%'};"
  >
    <!-- Bezel + screen -->
    <div bind:this={bezelEl} class="bezel relative flex-1 flex items-center justify-center px-3 py-3">
      <!--
        Perspective lines: 4 thin diagonals from each screen corner to the
        matching bezel corner. Reads as a recessed cutout — the lines are
        the visible edges of the (implicit) sloped walls between the cabinet
        face and the screen plane.
      -->
      {#if bezelW > 0 && bezelH > 0 && screenW > 0 && screenH > 0}
        {@const sx = (bezelW - screenW) / 2}
        {@const sy = (bezelH - screenH) / 2}
        <svg
          class="perspective absolute inset-0 pointer-events-none"
          width={bezelW}
          height={bezelH}
          viewBox="0 0 {bezelW} {bezelH}"
          aria-hidden="true"
        >
          <line x1="0" y1="0" x2={sx} y2={sy} />
          <line x1={bezelW} y1="0" x2={sx + screenW} y2={sy} />
          <line x1="0" y1={bezelH} x2={sx} y2={sy + screenH} />
          <line x1={bezelW} y1={bezelH} x2={sx + screenW} y2={sy + screenH} />
        </svg>
      {/if}
      <div
        class="screen bg-black"
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
    font-size: clamp(0.95rem, 5cqw, 1.75rem);
    letter-spacing: clamp(0.1em, 2.5cqw, 0.35em);
  }
  @supports not (container-type: inline-size) {
    .marquee-title {
      font-size: clamp(0.95rem, 4vw, 1.625rem);
      letter-spacing: clamp(0.1em, 2vw, 0.3em);
    }
  }

  /* Bezel: simulates the cabinet face sloping inward toward the screen.
     - Radial gradient: darkest at the screen (center), lighter at outer
       edges — reads as "the surface dips back" toward the screen.
     - Linear overlay: subtle top-left highlight, as if light from above-left
       is catching the front-facing rim of the bezel.
     - Inset box-shadow: vignette around the edges, deepening the bowl. */
  .bezel {
    background:
      linear-gradient(135deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0) 45%),
      radial-gradient(ellipse at center,
        rgb(8, 8, 10) 0%,
        rgb(18, 18, 21) 35%,
        rgb(32, 32, 36) 75%,
        rgb(42, 42, 47) 100%);
    box-shadow:
      inset 0 0 80px rgba(0, 0, 0, 0.55),
      inset 0 12px 24px rgba(0, 0, 0, 0.35),
      inset 0 -12px 24px rgba(0, 0, 0, 0.45);
  }

  /* Screen: just the inset CRT vignette. No outer halo, rim, or border —
     the perspective lines + bezel gradient communicate the recess instead. */
  .screen {
    box-shadow: inset 0 0 30px #000;
  }

  /* Perspective overlay: thin diagonals from screen corners to bezel
     corners. Subtle white-ish stroke reads as a beveled rim catching light. */
  .perspective line {
    stroke: rgba(255, 255, 255, 0.09);
    stroke-width: 1;
    vector-effect: non-scaling-stroke;
  }
</style>
