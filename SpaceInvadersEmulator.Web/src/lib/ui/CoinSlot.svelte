<script lang="ts">
  import { setInput } from '../inputState';
  import { playCoinInsert } from '../audio';

  interface Props {
    /** Bezel plate height. Width is ~70% of height (vertical orientation). Default 52. */
    size?: number;
  }

  let { size = 52 }: Props = $props();

  let pressed = $state(false);

  function down(e: PointerEvent): void {
    e.preventDefault();
    (e.currentTarget as HTMLElement).setPointerCapture(e.pointerId);
    pressed = true;
    setInput('coin', true);
    playCoinInsert();
  }
  function up(): void {
    if (!pressed) return;
    pressed = false;
    setInput('coin', false);
  }
</script>

<button
  class="coin-slot"
  class:is-pressed={pressed}
  style="--plate-h: {size}px; --plate-w: {Math.round(size * 0.7)}px;"
  aria-label="Insert coin"
  aria-pressed={pressed}
  onpointerdown={down}
  onpointerup={up}
  onpointercancel={up}
>
  <span class="plate" aria-hidden="true">
    <span class="slot"></span>
    <span class="hint">COIN</span>
  </span>
</button>

<style>
  .coin-slot {
    background: transparent;
    border: 0;
    padding: 0;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: var(--plate-w);
    height: var(--plate-h);
    -webkit-tap-highlight-color: transparent;
    touch-action: none;
    user-select: none;
  }
  .coin-slot:focus-visible {
    outline: 2px solid var(--color-crt-green);
    outline-offset: 3px;
    border-radius: 4px;
  }

  /* Bezel plate: beveled gunmetal rectangle, vertical. */
  .plate {
    position: relative;
    width: 100%;
    height: 100%;
    border-radius: 4px;
    background: linear-gradient(180deg, #2a2a2e 0%, #18181b 100%);
    box-shadow:
      inset 0 1px 0 rgba(255, 255, 255, 0.08),
      inset 0 -1px 0 rgba(0, 0, 0, 0.6),
      0 1px 0 rgba(0, 0, 0, 0.6);
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 4px;
    padding: 6px 4px;
  }

  /* The slot opening — narrow vertical cutout with a deep inset shadow
     so it reads as "a coin would slide in here". Pseudo-element is the
     amber flash that fires while pressed. */
  .slot {
    position: relative;
    width: 5px;
    height: 55%;
    background: #050507;
    border-radius: 1px;
    box-shadow:
      inset 0 2px 3px rgba(0, 0, 0, 0.9),
      inset 0 -1px 1px rgba(255, 255, 255, 0.05);
    overflow: hidden;
  }
  .slot::before {
    content: "";
    position: absolute;
    inset: 0;
    background: linear-gradient(180deg, rgba(255, 191, 0, 0.0) 0%, rgba(255, 191, 0, 0.85) 50%, rgba(255, 191, 0, 0) 100%);
    transform: translateY(-100%);
    transition: transform 0ms;
  }
  .is-pressed .slot::before {
    /* Coin "drops through" — a brief amber streak passes from top to bottom. */
    animation: coin-flash 220ms ease-out;
  }
  @keyframes coin-flash {
    0%   { transform: translateY(-100%); }
    100% { transform: translateY(100%); }
  }

  .hint {
    font-family: ui-monospace, "SF Mono", monospace;
    font-size: calc(var(--plate-h) * 0.16);
    letter-spacing: 0.1em;
    color: rgba(245, 158, 11, 0.7);
    pointer-events: none;
  }
</style>
