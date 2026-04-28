<script lang="ts">
  import { setInput, type InputKey } from '../lib/inputState';
  import { cn } from '../lib/ui/cn';

  function press(e: PointerEvent, key: InputKey, pressed: boolean): void {
    if (pressed) (e.currentTarget as HTMLElement).setPointerCapture(e.pointerId);
    setInput(key, pressed);
  }

  const buttons: { key: InputKey; label: string; tone: 'red' | 'amber' | 'green' }[] = [
    { key: 'coin',    label: 'COIN',     tone: 'amber' },
    { key: 'p1Start', label: '1P START', tone: 'green' },
    { key: 'p2Start', label: '2P START', tone: 'red' },
  ];

  const toneClass: Record<'red' | 'amber' | 'green', string> = {
    red:   'text-red-300   border-red-900   shadow-[inset_0_-3px_0_rgba(127,29,29,0.9),0_0_8px_rgba(220,38,38,0.25)] hover:border-red-700',
    amber: 'text-amber-200 border-amber-900 shadow-[inset_0_-3px_0_rgba(146,64,14,0.9),0_0_8px_rgba(245,158,11,0.25)] hover:border-amber-700',
    green: 'text-green-200 border-green-900 shadow-[inset_0_-3px_0_rgba(20,83,45,0.9),0_0_8px_rgba(34,197,94,0.25)] hover:border-green-700',
  };
</script>

<div class="control-panel flex items-center justify-center gap-3 px-4 py-3 bg-zinc-950 border-x-4 border-b-4 border-zinc-700 rounded-b-md w-full">
  {#each buttons as b (b.key)}
    <button
      class={cn(
        'arcade-btn font-mono text-[0.7rem] tracking-widest uppercase px-4 py-2 rounded-sm bg-zinc-900 border-2 select-none active:translate-y-[2px] active:shadow-none transition-transform',
        toneClass[b.tone],
      )}
      onpointerdown={(e) => press(e, b.key, true)}
      onpointerup={(e) => press(e, b.key, false)}
      onpointercancel={(e) => press(e, b.key, false)}
    >
      {b.label}
    </button>
  {/each}
</div>

<style>
  .arcade-btn { touch-action: none; -webkit-tap-highlight-color: transparent; }
</style>
