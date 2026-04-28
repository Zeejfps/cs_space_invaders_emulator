<script lang="ts">
  import RepoBadge from '../lib/ui/RepoBadge.svelte';

  // Popover content. Sectioned so future blocks (game info, dipswitches)
  // can drop in alongside the controls section.
  const keys: { k: string; label: string }[] = [
    { k: '←  →', label: 'Move' },
    { k: 'Space', label: 'Fire' },
    { k: 'C',     label: 'Insert coin' },
    { k: '1',     label: 'Player 1 start' },
    { k: '2',     label: 'Player 2 start' },
  ];
</script>

<div class="font-mono text-zinc-200 text-sm flex flex-col gap-3">
  <section>
    <div class="text-[0.65rem] tracking-widest uppercase text-zinc-500 mb-2">Controls</div>
    <ul class="flex flex-col gap-1.5">
      {#each keys as e (e.k)}
        <li class="flex items-center justify-between gap-3">
          <kbd class="shrink-0 px-2 py-0.5 rounded border border-zinc-700 bg-zinc-900 text-zinc-100 text-xs font-mono">{e.k}</kbd>
          <span class="text-xs text-zinc-300">{e.label}</span>
        </li>
      {/each}
    </ul>
  </section>

  <section class="border-t border-zinc-800 pt-3">
    <div class="text-[0.65rem] tracking-widest uppercase text-zinc-500 mb-2">About</div>
    <RepoBadge label="Source" />
  </section>
</div>
