<script lang="ts">
  const keys: { k: string; label: string }[] = [
    { k: '←  →', label: 'MOVE' },
    { k: 'SPACE', label: 'FIRE' },
    { k: 'C',     label: 'COIN' },
    { k: '1',     label: 'START' },
  ];
</script>

<div class="flex items-center justify-center flex-wrap gap-3 py-2 font-mono text-[0.65rem] text-zinc-500">
  {#each keys as e (e.k)}
    <span class="inline-flex items-center gap-1.5">
      <kbd class="px-1.5 py-0.5 rounded border border-zinc-700 bg-zinc-900 text-zinc-300">{e.k}</kbd>
      <span class="tracking-widest uppercase">{e.label}</span>
    </span>
  {/each}
</div>
