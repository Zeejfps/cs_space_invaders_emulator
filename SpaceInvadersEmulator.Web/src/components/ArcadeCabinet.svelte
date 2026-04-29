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
  // bezelEl now binds the entire cabinet-face (the gradient surface holding
  // both the screen recess and the joystick deck). The SVG perspective
  // overlay covers the whole face so its lines extend from the screen
  // corners all the way to the face's outer corners — i.e. to the seam
  // with the marquee at the top and with the footer at the bottom.
  let bezelEl: HTMLDivElement;
  // On touch, the deck (joystick + FIRE) sits at the bottom of the cabinet
  // face. Its height is subtracted from the screen's available vertical
  // space and used to position the screen for line endpoint calculations.
  let deckEl: HTMLDivElement | undefined = $state();
  let screenW = $state(0);
  let screenH = $state(0);
  let bezelW = $state(0);
  let bezelH = $state(0);
  let dh = $state(0);

  onMount(() => {
    function recompute(): void {
      const hostW = host.clientWidth;
      const hostH = host.clientHeight;
      const mh = marqueeEl.offsetHeight;
      const ch = controlsEl.offsetHeight;
      dh = deckEl ? deckEl.offsetHeight : 0;
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
    <!-- Container-typed wrapper so the title's font-size scales with the
         actual space available between the flanks, not the full marquee. -->
    <div class="flex-1 min-w-0 marquee-title-host overflow-hidden">
      <div class="text-center font-mono text-amber-300 [text-shadow:0_0_10px_rgba(252,211,77,0.6)] uppercase whitespace-nowrap leading-none marquee-title">
        ★ {game.title} ★
      </div>
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
    <div
      bind:this={bezelEl}
      class="cabinet-face relative flex-1 min-h-0 flex flex-col px-3 py-3"
    >
      <!--
        Perspective lines: from each screen corner to the matching cabinet-face
        corner (= the seam with the marquee at top and with the footer at
        bottom). Reads as 4 sloped walls receding from the chassis edge to the
        screen at the back of the recess.
      -->
      {#if bezelW > 0 && bezelH > 0 && screenW > 0 && screenH > 0}
        {@const sx = 12 + (bezelW - 24 - screenW) / 2}
        {@const sy = 12 + (bezelH - 24 - dh - screenH) / 2}
        {@const sxR = sx + screenW}
        {@const syB = sy + screenH}
        <svg
          class="perspective absolute inset-0 pointer-events-none"
          width={bezelW}
          height={bezelH}
          viewBox="0 0 {bezelW} {bezelH}"
          aria-hidden="true"
        >
          <!--
            4 trapezoidal "walls" sloping from the chassis edges (front face,
            lighter) to the screen edges (back of recess, darker). Each
            gradient's direction encodes the implied slope axis.
          -->
          <defs>
            <linearGradient id="wall-top" x1="0" y1="0" x2="0" y2="1">
              <stop offset="0%" stop-color="rgb(50, 50, 56)" />
              <stop offset="100%" stop-color="rgb(22, 22, 25)" />
            </linearGradient>
            <linearGradient id="wall-bot" x1="0" y1="1" x2="0" y2="0">
              <stop offset="0%" stop-color="rgb(50, 50, 56)" />
              <stop offset="100%" stop-color="rgb(22, 22, 25)" />
            </linearGradient>
            <linearGradient id="wall-left" x1="0" y1="0" x2="1" y2="0">
              <stop offset="0%" stop-color="rgb(50, 50, 56)" />
              <stop offset="100%" stop-color="rgb(22, 22, 25)" />
            </linearGradient>
            <linearGradient id="wall-right" x1="1" y1="0" x2="0" y2="0">
              <stop offset="0%" stop-color="rgb(50, 50, 56)" />
              <stop offset="100%" stop-color="rgb(22, 22, 25)" />
            </linearGradient>
          </defs>
          <polygon points="0,0 {bezelW},0 {sxR},{sy} {sx},{sy}" fill="url(#wall-top)" />
          <polygon points="0,{bezelH} {bezelW},{bezelH} {sxR},{syB} {sx},{syB}" fill="url(#wall-bot)" />
          <polygon points="0,0 {sx},{sy} {sx},{syB} 0,{bezelH}" fill="url(#wall-left)" />
          <polygon points="{bezelW},0 {bezelW},{bezelH} {sxR},{syB} {sxR},{sy}" fill="url(#wall-right)" />
          <!-- Edge highlights at the wall seams. -->
          <line x1="0" y1="0" x2={sx} y2={sy} />
          <line x1={bezelW} y1="0" x2={sxR} y2={sy} />
          <line x1="0" y1={bezelH} x2={sx} y2={syB} />
          <line x1={bezelW} y1={bezelH} x2={sxR} y2={syB} />
        </svg>
      {/if}

      <!-- Screen recess: takes available height, centers screen. -->
      <div class="screen-recess flex-1 min-h-0 flex items-center justify-center">
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
          <div class="flex justify-end">
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
  /* Title scales with the title-host's inline width (which excludes the
     flanks reserved for the help "?" + balancer). The cqi factor is tuned
     for the longest title we have ("Space Invaders Part II" = 26 chars
     including stars). With monospace char width ~0.6em and letter-spacing
     ≤ 0.1em, the worst case fits when font-size ≤ host_width / 18 ≈ 5.5%,
     so we use 5cqi and a low max letter-spacing. */
  .marquee-title-host {
    container-type: inline-size;
  }
  .marquee-title {
    font-size: clamp(0.55rem, 5cqi, 1.75rem);
    letter-spacing: clamp(0.02em, 0.4cqi, 0.1em);
  }
  @supports not (container-type: inline-size) {
    .marquee-title {
      font-size: clamp(0.55rem, 3vw, 1.5rem);
      letter-spacing: clamp(0.02em, 0.4vw, 0.1em);
    }
  }

  /* Cabinet face: flat fallback color. The visible "depth" comes from 4
     trapezoidal SVG polygons rendered on top, each filled with a directional
     gradient (light at the chassis edge, dark at the screen edge) so the
     screen reads as inset into rectangular sloped walls instead of a soft
     elliptical bowl. */
  .cabinet-face {
    background: rgb(28, 28, 32);
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
