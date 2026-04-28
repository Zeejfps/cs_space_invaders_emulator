<script lang="ts">
  import { loadRom } from '../lib/emulator';

  let { onloaded }: { onloaded: () => void } = $props();

  let error = $state('');
  let dragging = $state(false);

  const ROM_SIZE = 8192;
  // MAME invaders.zip loads h→g→f→e (0x0000→0x0800→0x1000→0x1800).
  const EXT_ORDER = ['h', 'g', 'f', 'e'];

  async function handleFiles(files: FileList): Promise<void> {
    error = '';

    let romData: Uint8Array;

    if (files.length === 1) {
      romData = new Uint8Array(await files[0].arrayBuffer());
    } else {
      const sorted = Array.from(files).sort((a, b) => {
        const extA = a.name.split('.').pop()?.toLowerCase() ?? '';
        const extB = b.name.split('.').pop()?.toLowerCase() ?? '';
        return EXT_ORDER.indexOf(extA) - EXT_ORDER.indexOf(extB);
      });
      const buffers = await Promise.all(sorted.map(f => f.arrayBuffer()));
      const total = buffers.reduce((n, b) => n + b.byteLength, 0);
      romData = new Uint8Array(total);
      let offset = 0;
      for (const buf of buffers) {
        romData.set(new Uint8Array(buf), offset);
        offset += buf.byteLength;
      }
    }

    if (romData.byteLength !== ROM_SIZE) {
      error = `Expected ${ROM_SIZE} bytes but got ${romData.byteLength}. Drop all 4 ROM parts (invaders.e .f .g .h) or a single 8 KB .bin.`;
      return;
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
    <p class="hint">invaders.e + .f + .g + .h &nbsp;·&nbsp; or a single 8 KB invaders.bin</p>
    <label class="browse-btn">
      Browse files
      <input type="file" multiple accept=".bin,.e,.f,.g,.h" onchange={onFileChange} />
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
