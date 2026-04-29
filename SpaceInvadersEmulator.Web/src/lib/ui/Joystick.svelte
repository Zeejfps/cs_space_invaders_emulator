<script lang="ts">
  import { setInput } from '../inputState';

  interface Props {
    /** Outer base diameter in px. Default 112. */
    size?: number;
    /** Deadzone as a fraction of max knob travel (0..1). Default 0.25. */
    deadzone?: number;
  }

  let { size = 112, deadzone = 0.25 }: Props = $props();

  // Knob is 70% of base — leaves a thin rim of socket visible. Max travel
  // = (base - knob) / 2 so the knob stays inside that rim.
  const KNOB_FRAC = 0.7;
  const knobSize = $derived(Math.round(size * KNOB_FRAC));
  const maxDist = $derived((size - knobSize) / 2);
  const deadzonePx = $derived(maxDist * deadzone);

  let baseEl: HTMLDivElement | undefined = $state();
  let dragging = $state(false);
  let dx = $state(0);
  let dy = $state(0);
  // Track current digital direction so we only fire setInput on transitions.
  let active = $state<'left' | 'right' | null>(null);

  function down(e: PointerEvent): void {
    e.preventDefault();
    (e.currentTarget as HTMLElement).setPointerCapture(e.pointerId);
    dragging = true;
    update(e);
  }

  function move(e: PointerEvent): void {
    if (!dragging) return;
    update(e);
  }

  function update(e: PointerEvent): void {
    if (!baseEl) return;
    const rect = baseEl.getBoundingClientRect();
    const cx = rect.left + rect.width / 2;
    const cy = rect.top + rect.height / 2;
    let nx = e.clientX - cx;
    let ny = e.clientY - cy;
    // Clamp knob inside the base.
    const dist = Math.hypot(nx, ny);
    if (dist > maxDist) {
      const k = maxDist / dist;
      nx *= k;
      ny *= k;
    }
    dx = nx;
    dy = ny;

    // Map x to digital input. Y is rendered but not consumed.
    const next: 'left' | 'right' | null =
      nx >  deadzonePx ? 'right' :
      nx < -deadzonePx ? 'left'  : null;
    if (next !== active) {
      if (active === 'left')  setInput('left',  false);
      if (active === 'right') setInput('right', false);
      if (next === 'left')    setInput('left',  true);
      if (next === 'right')   setInput('right', true);
      active = next;
    }
  }

  function up(): void {
    if (!dragging) return;
    dragging = false;
    dx = 0;
    dy = 0;
    if (active === 'left')  setInput('left',  false);
    if (active === 'right') setInput('right', false);
    active = null;
  }
</script>

<div
  bind:this={baseEl}
  class="base"
  class:dragging
  style="--size: {size}px; --knob: {knobSize}px;"
  role="slider"
  tabindex="0"
  aria-label="Move left or right"
  aria-valuemin={-1}
  aria-valuemax={1}
  aria-valuenow={active === 'left' ? -1 : active === 'right' ? 1 : 0}
  onpointerdown={down}
  onpointermove={move}
  onpointerup={up}
  onpointercancel={up}
>
  <span
    class="knob"
    aria-hidden="true"
    style="transform: translate3d({dx}px, {dy}px, 0);"
  ></span>
</div>

<style>
  .base {
    position: relative;
    width: var(--size);
    height: var(--size);
    /* Enforce 1:1 so a parent flex/grid container can't squash the base
       into an ellipse when border-radius: 50% is applied. */
    aspect-ratio: 1 / 1;
    flex-shrink: 0;
    border-radius: 50%;
    background: radial-gradient(circle at 50% 40%, #1c1c20 0%, #0a0a0c 80%);
    box-shadow:
      inset 0 3px 6px rgba(0, 0, 0, 0.85),
      inset 0 -1px 2px rgba(255, 255, 255, 0.05),
      0 1px 0 rgba(255, 255, 255, 0.04);
    touch-action: none;
    -webkit-tap-highlight-color: transparent;
    user-select: none;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: grab;
  }
  .base.dragging { cursor: grabbing; }

  .knob {
    width: var(--knob);
    height: var(--knob);
    border-radius: 50%;
    background: radial-gradient(circle at 50% 30%, #6e6e76 0%, #3f3f46 60%, #1f1f23 100%);
    box-shadow:
      inset 0 -2px 2px rgba(0, 0, 0, 0.45),
      inset 0 2px 2px rgba(255, 255, 255, 0.18),
      0 5px 0 rgba(0, 0, 0, 0.7),
      0 6px 8px rgba(0, 0, 0, 0.55);
    /* Default: smooth snap-back animation when not dragging. */
    transition: transform 180ms cubic-bezier(0.2, 0.7, 0.2, 1);
    will-change: transform;
  }
  /* While dragging, knob tracks the finger 1:1 with no easing. */
  .dragging .knob {
    transition: transform 0ms;
  }
</style>
