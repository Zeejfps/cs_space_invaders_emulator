<script lang="ts">
  import { playCoinReturn } from '../audio';

  interface Props {
    /** Bezel plate height. Width is ~70% of height to match CoinSlot. Default 52. */
    size?: number;
    onclick: () => void;
  }

  let { size = 52, onclick }: Props = $props();
</script>

<button
  class="coin-return"
  style="--plate-h: {size}px; --plate-w: {Math.round(size * 0.7)}px;"
  aria-label="Return to launcher"
  onpointerdown={playCoinReturn}
  {onclick}
>
  <span class="plate" aria-hidden="true">
    <span class="chevron">‹</span>
    <span class="hint">RETURN</span>
  </span>
</button>

<style>
  .coin-return {
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
    user-select: none;
  }
  .coin-return:focus-visible {
    outline: 2px solid var(--color-crt-green);
    outline-offset: 3px;
    border-radius: 4px;
  }

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
    gap: 2px;
    padding: 6px 4px;
    transition: transform 60ms ease-out, box-shadow 60ms ease-out;
  }
  .coin-return:active .plate {
    transform: translateY(1px);
    box-shadow:
      inset 0 2px 3px rgba(0, 0, 0, 0.6),
      inset 0 -1px 0 rgba(255, 255, 255, 0.04);
  }

  /* Engraved chevron: stamped into the plate, not raised. Dim zinc tone
     keeps it subordinate to the gameplay-primary buttons (FIRE/P1/P2). */
  .chevron {
    font-family: ui-monospace, "SF Mono", monospace;
    font-size: calc(var(--plate-h) * 0.42);
    line-height: 1;
    color: rgba(161, 161, 170, 0.7);
    text-shadow:
      0 -1px 0 rgba(0, 0, 0, 0.7),
      0 1px 0 rgba(255, 255, 255, 0.04);
    pointer-events: none;
  }

  .hint {
    font-family: ui-monospace, "SF Mono", monospace;
    font-size: calc(var(--plate-h) * 0.14);
    letter-spacing: 0.1em;
    color: rgba(161, 161, 170, 0.7);
    text-shadow:
      0 -1px 0 rgba(0, 0, 0, 0.7),
      0 1px 0 rgba(255, 255, 255, 0.04);
    pointer-events: none;
  }
</style>
