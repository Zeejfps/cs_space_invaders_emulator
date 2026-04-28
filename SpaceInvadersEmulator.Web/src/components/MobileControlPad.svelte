<script lang="ts">
  import { setInput, type InputKey } from '../lib/inputState';
  import ArcadeButton from '../lib/ui/ArcadeButton.svelte';
  import CoinSlot from '../lib/ui/CoinSlot.svelte';

  function press(e: PointerEvent, key: InputKey, pressed: boolean): void {
    e.preventDefault();
    if (pressed) (e.currentTarget as HTMLElement).setPointerCapture(e.pointerId);
    setInput(key, pressed);
  }
</script>

<div
  class="pad w-full grid grid-cols-[1fr_auto] items-end gap-4 px-4 pt-3 pb-[max(env(safe-area-inset-bottom),0.75rem)] bg-zinc-950 border-t-2 border-zinc-800"
>
  <!-- Movement cluster -->
  <div class="flex gap-3 justify-start">
    <button
      class="dpad-btn bg-zinc-900 border-2 border-zinc-700 text-zinc-100 active:bg-[var(--color-crt-green)]/20 active:border-[var(--color-crt-green)] active:text-[var(--color-crt-green)]"
      aria-label="Move left"
      onpointerdown={(e) => press(e, 'left', true)}
      onpointerup={(e) => press(e, 'left', false)}
      onpointercancel={(e) => press(e, 'left', false)}
    >
      <span aria-hidden="true">◀</span>
    </button>
    <button
      class="dpad-btn bg-zinc-900 border-2 border-zinc-700 text-zinc-100 active:bg-[var(--color-crt-green)]/20 active:border-[var(--color-crt-green)] active:text-[var(--color-crt-green)]"
      aria-label="Move right"
      onpointerdown={(e) => press(e, 'right', true)}
      onpointerup={(e) => press(e, 'right', false)}
      onpointercancel={(e) => press(e, 'right', false)}
    >
      <span aria-hidden="true">▶</span>
    </button>
  </div>

  <!-- Fire button -->
  <button
    class="fire-btn rounded-full bg-red-950 border-4 border-red-700 text-red-100 active:bg-red-800 active:border-red-500 active:translate-y-[2px] [box-shadow:inset_0_-4px_0_rgba(127,29,29,0.9),0_0_18px_rgba(220,38,38,0.35)]"
    aria-label="Fire"
    onpointerdown={(e) => press(e, 'fire', true)}
    onpointerup={(e) => press(e, 'fire', false)}
    onpointercancel={(e) => press(e, 'fire', false)}
  >
    <span class="font-mono tracking-widest text-sm">FIRE</span>
  </button>

  <!-- Coin / P1 row (P2 omitted on touch — physical 2P sharing a phone screen isn't feasible). -->
  <div class="col-span-2 flex gap-4 justify-center items-center">
    <CoinSlot size={44} />
    <ArcadeButton inputKey="p1Start" label="P1" tone="red" size={44} ariaLabel="Player 1 start" />
  </div>
</div>

<style>
  .pad { user-select: none; -webkit-user-select: none; }
  .dpad-btn, .fire-btn {
    touch-action: none;
    -webkit-tap-highlight-color: transparent;
  }
  .dpad-btn {
    width: 4rem; height: 4rem;
    display: inline-flex; align-items: center; justify-content: center;
    font-size: 1.4rem;
    border-radius: 0.5rem;
  }
  .fire-btn {
    width: 5.5rem; height: 5.5rem;
    display: inline-flex; align-items: center; justify-content: center;
  }
</style>
