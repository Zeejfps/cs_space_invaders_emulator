<script lang="ts">
  import { onMount, tick } from 'svelte';
  import type { GameDef } from '../lib/games';
  import { touch } from '../lib/touch.svelte';
  import GameCanvas from './GameCanvas.svelte';
  import ControlPanel from './ControlPanel.svelte';
  import MobileControlPad from './MobileControlPad.svelte';
  import ControlsHelp from './ControlsHelp.svelte';
  import Joystick from '../lib/ui/Joystick.svelte';
  import ArcadeButton from '../lib/ui/ArcadeButton.svelte';

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
  // Cabinet face padding (py-3/px-3 = 12 on each side) plus the 2px chassis
  // border that wraps marquee + cabinet on each side.
  const BEZEL_VERT_PADDING = 24 + 4;   // py-3 (12+12) + chassis border (2+2)
  const BEZEL_HORIZ_PADDING = 24 + 4;  // px-3 (12+12) + chassis border (2+2)
  const ASPECT_W = 7;
  const ASPECT_H = 8;

  let host: HTMLDivElement;
  let marqueeEl: HTMLDivElement;
  let controlsEl: HTMLDivElement;
  let bezelEl: HTMLDivElement;
  // On touch, the deck (joystick + FIRE) is a sibling of the bezel — a
  // dedicated section of the cabinet face below the screen recess. Measured
  // so its height is subtracted from the screen's available vertical space.
  let deckEl: HTMLDivElement | undefined = $state();
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
      const dh = deckEl ? deckEl.offsetHeight : 0;
      const availH = Math.max(0, hostH - mh - ch - dh - BEZEL_VERT_PADDING);
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
    if (deckEl) ro.observe(deckEl);
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
 <!--
   Chassis: outer wrapper with a visible trim border that frames the marquee
   + cabinet body as a single unit. Without this the dark cabinet face
   bleeds into the dark page backdrop on wide displays.
 -->
 <div
   class="chassis flex-1 min-h-0 w-full flex flex-col"
   style="max-width: {screenW ? screenW + BEZEL_HORIZ_PADDING + 'px' : '100%'};"
 >
  <!-- Marquee: sibling of cabinet, sized to chassis width. -->
  <div
    bind:this={marqueeEl}
    class="marquee shrink-0 w-full flex items-center gap-2 px-3 py-5 bg-zinc-950 border-b-2 border-amber-900/50"
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
  >
    <!--
      Cabinet face: continuous gray gradient surface holding both the screen
      recess (top, flex-1) and the joystick + FIRE deck (bottom, shrink-0).
      Both sit on the same surface — no visual seam — and the bezel above the
      screen + the gap between screen and joystick can shrink as needed.
    -->
    <div class="cabinet-face flex-1 min-h-0 flex flex-col px-3 py-3">
      <!-- Screen recess: perspective lines + screen, takes available height. -->
      <div
        bind:this={bezelEl}
        class="screen-recess relative flex-1 min-h-0 flex items-center justify-center"
      >
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

      {#if touch.isTouch}
        <!--
          Joystick (left), P1 (center), FIRE (right): primary gameplay
          controls mounted directly on the cabinet face. 3-column grid keeps
          P1 optically centered between the larger flanking controls.
        -->
        <div
          bind:this={deckEl}
          class="deck-row shrink-0 grid grid-cols-3 items-center pt-3"
        >
          <div class="flex justify-start">
            <Joystick size={112} />
          </div>
          <div class="flex justify-center pl-6">
            <ArcadeButton inputKey="p1Start" label="P1" tone="red" size={52} ariaLabel="Player 1 start" />
          </div>
          <div class="flex justify-end">
            <ArcadeButton inputKey="fire" label="FIRE" tone="red" size={88} ariaLabel="Fire" />
          </div>
        </div>
      {/if}
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
</div>

<style>
  .cabinet { font-family: ui-monospace, "SF Mono", monospace; }
  /* Chassis: dark gray trim around marquee + cabinet so the unit reads as a
     distinct object against the page backdrop. The inset top highlight gives
     the trim a bit of dimensionality (light catching the bevel from above);
     the outer drop shadow grounds the cabinet on the surroundings. */
  .chassis {
    border: 2px solid rgb(55, 55, 60);
    box-shadow:
      inset 0 1px 0 rgba(255, 255, 255, 0.08),
      0 12px 40px rgba(0, 0, 0, 0.7);
  }
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

  /* Cabinet face: the continuous gray surface holding both the screen recess
     and the joystick deck. Subtle vertical gradient (lighter at top, darker
     near the bottom) suggests light from above. The radial focus is centered
     on the upper portion where the screen sits, so the recess reads as the
     darkest point — the joystick area is on the lighter outer surface. */
  .cabinet-face {
    background:
      linear-gradient(135deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0) 45%),
      radial-gradient(ellipse 90% 70% at 50% 35%,
        rgb(8, 8, 10) 0%,
        rgb(20, 20, 23) 40%,
        rgb(34, 34, 38) 100%);
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
