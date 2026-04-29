<script lang="ts">
  import { setInput, type InputKey } from '../inputState';

  type Tone = 'red' | 'blue' | 'neutral';
  interface Props {
    inputKey: InputKey;
    label: string;
    tone: Tone;
    /** Outer ring diameter in px. Inner cap is ~85% of this. Default 52. */
    size?: number;
    ariaLabel?: string;
  }

  let { inputKey, label, tone, size = 52, ariaLabel }: Props = $props();

  let pressed = $state(false);

  function down(e: PointerEvent): void {
    e.preventDefault();
    (e.currentTarget as HTMLElement).setPointerCapture(e.pointerId);
    pressed = true;
    setInput(inputKey, true);
  }
  function up(): void {
    if (!pressed) return;
    pressed = false;
    setInput(inputKey, false);
  }
</script>

<button
  class="arcade-btn tone-{tone}"
  class:is-pressed={pressed}
  style="--ring-size: {size}px; --cap-size: {Math.round(size * 0.85)}px;"
  aria-label={ariaLabel ?? label}
  aria-pressed={pressed}
  onpointerdown={down}
  onpointerup={up}
  onpointercancel={up}
>
  <span class="ring" aria-hidden="true">
    <span class="cap">
      <span class="label">{label}</span>
    </span>
  </span>
</button>

<style>
  .arcade-btn {
    /* Reset native button look. */
    background: transparent;
    border: 0;
    padding: 0;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: var(--ring-size);
    height: var(--ring-size);
    /* Enforce 1:1 so the ring stays circular even when a parent flex/grid
       container would otherwise shrink one axis (border-radius: 50% on a
       non-square box renders as an ellipse). */
    aspect-ratio: 1 / 1;
    flex-shrink: 0;
    -webkit-tap-highlight-color: transparent;
    touch-action: none;
    user-select: none;
  }
  .arcade-btn:focus-visible {
    outline: 2px solid var(--color-crt-green);
    outline-offset: 3px;
    border-radius: 50%;
  }

  /* Outer socket: gunmetal ring with inner shadow so the cap looks recessed
     into a fitting. Doesn't move on press. */
  .ring {
    position: relative;
    width: 100%;
    height: 100%;
    border-radius: 50%;
    background: radial-gradient(circle at 50% 40%, #2a2a2e 0%, #131316 80%);
    box-shadow:
      inset 0 2px 3px rgba(0, 0, 0, 0.7),
      inset 0 -1px 1px rgba(255, 255, 255, 0.05),
      0 1px 0 rgba(255, 255, 255, 0.04);
    display: flex;
    align-items: center;
    justify-content: center;
  }

  /* Inner cap: the part that depresses. Sits 5px above the ring with a
     drop-shadow underneath. Slight gradient highlight = molded plastic feel. */
  .cap {
    position: relative;
    width: var(--cap-size);
    height: var(--cap-size);
    border-radius: 50%;
    transform: translateY(-5px);
    transition:
      transform 130ms cubic-bezier(0.2, 0.7, 0.2, 1),
      filter 130ms ease-out,
      box-shadow 130ms ease-out;
    display: flex;
    align-items: center;
    justify-content: center;
  }
  .is-pressed .cap {
    transform: translateY(0);
    transition-duration: 70ms;
    transition-timing-function: ease-out;
    filter: brightness(0.85);
    box-shadow: 0 0 0 transparent !important;
  }

  /* Tone variants — gradient on the cap, colored drop-shadow under it. */
  .tone-red .cap {
    background: radial-gradient(circle at 50% 30%, #ff5b5b 0%, #d62121 60%, #8a0e0e 100%);
    box-shadow:
      inset 0 -2px 2px rgba(60, 0, 0, 0.5),
      inset 0 2px 2px rgba(255, 255, 255, 0.25),
      0 5px 0 rgba(70, 0, 0, 0.85),
      0 6px 8px rgba(0, 0, 0, 0.6);
  }
  .tone-blue .cap {
    background: radial-gradient(circle at 50% 30%, #6db1ff 0%, #1e6fd9 60%, #0d3a85 100%);
    box-shadow:
      inset 0 -2px 2px rgba(0, 10, 60, 0.5),
      inset 0 2px 2px rgba(255, 255, 255, 0.25),
      0 5px 0 rgba(0, 20, 70, 0.85),
      0 6px 8px rgba(0, 0, 0, 0.6);
  }
  /* Neutral / zinc — for direction inputs. Reads as "control, not action." */
  .tone-neutral .cap {
    background: radial-gradient(circle at 50% 30%, #6e6e76 0%, #3f3f46 60%, #1f1f23 100%);
    box-shadow:
      inset 0 -2px 2px rgba(0, 0, 0, 0.45),
      inset 0 2px 2px rgba(255, 255, 255, 0.18),
      0 5px 0 rgba(0, 0, 0, 0.7),
      0 6px 8px rgba(0, 0, 0, 0.55);
  }

  .label {
    font-family: ui-monospace, "SF Mono", monospace;
    font-weight: 700;
    font-size: calc(var(--cap-size) * 0.32);
    letter-spacing: 0.05em;
    color: rgba(255, 255, 255, 0.95);
    text-shadow: 0 1px 1px rgba(0, 0, 0, 0.4);
    pointer-events: none;
  }
</style>
