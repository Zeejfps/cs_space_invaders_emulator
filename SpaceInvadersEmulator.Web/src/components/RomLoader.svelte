<script lang="ts">
  import { loadRom } from '../lib/emulator';

  let { onloaded }: { onloaded: () => void } = $props();

  let error = $state('');
  let dragging = $state(false);

  const MAX_ROM_SIZE = 0x10000; // 64 KB — full 8080 address space

  // Known per-file load offsets for supported MAME ROM sets. When all dropped
  // files match an entry here, each is placed at its known address and the
  // gaps (RAM/VRAM regions on real hardware) are left as zeros.
  const ROM_OFFSETS: Record<string, number> = {
    // Space Invaders (invaders.zip)
    'invaders.h': 0x0000,
    'invaders.g': 0x0800,
    'invaders.f': 0x1000,
    'invaders.e': 0x1800,
    // Balloon Bomber (ballbomb.zip)
    'tn01':   0x0000,
    'tn02':   0x0800,
    'tn03':   0x1000,
    'tn04':   0x1800,
    'tn05':   0x4000,
    'tn05-1': 0x4000,
    'tn06':   0x4800,
    'tn07':   0x5000,
  };

  async function handleFiles(files: FileList): Promise<void> {
    error = '';

    let romData: Uint8Array;

    if (files.length === 1) {
      const data = new Uint8Array(await files[0].arrayBuffer());
      if (data.byteLength > MAX_ROM_SIZE) {
        error = `ROM is ${data.byteLength} bytes; max ${MAX_ROM_SIZE} (64 KB).`;
        return;
      }
      romData = data;
    } else {
      const arr = Array.from(files);
      const offsets = arr.map(f => ROM_OFFSETS[f.name.toLowerCase()]);
      const unknown = arr.filter((_, i) => offsets[i] === undefined).map(f => f.name);
      if (unknown.length) {
        error = `Unknown ROM file(s): ${unknown.join(', ')}. Drop a recognized set or a single merged .bin.`;
        return;
      }

      const buffers = await Promise.all(arr.map(f => f.arrayBuffer()));
      const totalSize = Math.max(
        ...arr.map((_, i) => (offsets[i] as number) + buffers[i].byteLength),
      );
      if (totalSize > MAX_ROM_SIZE) {
        error = `Combined ROM is ${totalSize} bytes; max ${MAX_ROM_SIZE} (64 KB).`;
        return;
      }

      romData = new Uint8Array(totalSize);
      for (let i = 0; i < arr.length; i++) {
        romData.set(new Uint8Array(buffers[i]), offsets[i] as number);
      }
    }

    loadRom(romData);
    onloaded();
  }

  function onDrop(e: DragEvent): void {
    e.preventDefault();
    dragging = false;
    if (e.dataTransfer?.files.length) handleFiles(e.dataTransfer.files);
  }

  function onDragOver(e: DragEvent): void {
    e.preventDefault();
    dragging = true;
  }

  function onFileChange(e: Event): void {
    const input = e.target as HTMLInputElement;
    if (input.files?.length) handleFiles(input.files);
  }
</script>

<div class="loader">
  <h1>SPACE INVADERS</h1>
  <div
    class="dropzone"
    class:dragging
    role="button"
    tabindex="0"
    ondrop={onDrop}
    ondragover={onDragOver}
    ondragleave={() => (dragging = false)}
  >
    <p class="primary">Drop ROM files here</p>
    <p class="hint">
      single .bin (up to 64 KB) &nbsp;·&nbsp;
      invaders.e/.f/.g/.h &nbsp;·&nbsp;
      ballbomb tn01–tn07
    </p>
    <label class="browse-btn">
      Browse files
      <input type="file" multiple onchange={onFileChange} />
    </label>
  </div>
  {#if error}
    <p class="error">{error}</p>
  {/if}
</div>

<style>
  .loader {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1.5rem;
    color: #0f0;
    font-family: monospace;
  }

  h1 {
    font-size: 2.5rem;
    letter-spacing: 0.25em;
    text-shadow: 0 0 20px #0f0;
  }

  .dropzone {
    border: 2px dashed #0f0;
    border-radius: 4px;
    padding: 3rem 4rem;
    text-align: center;
    cursor: pointer;
    transition: background 0.15s, border-color 0.15s;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.75rem;
  }

  .dropzone.dragging {
    background: rgba(0, 255, 0, 0.08);
    border-color: #fff;
  }

  .primary {
    font-size: 1.25rem;
  }

  .hint {
    font-size: 0.8rem;
    opacity: 0.6;
  }

  .browse-btn {
    margin-top: 0.5rem;
    padding: 0.4rem 1.2rem;
    border: 1px solid #0f0;
    border-radius: 3px;
    cursor: pointer;
    font-family: inherit;
    font-size: 0.85rem;
    transition: background 0.15s;
  }

  .browse-btn:hover {
    background: rgba(0, 255, 0, 0.15);
  }

  .browse-btn input {
    display: none;
  }

  .error {
    color: #f44;
    font-size: 0.85rem;
    max-width: 30rem;
    text-align: center;
  }
</style>
