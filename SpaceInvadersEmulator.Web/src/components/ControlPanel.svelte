<script lang="ts">
  import { writeCoin, writeP1Start, writeP2Start } from '../lib/emulator';
</script>

<div class="control-panel">
  <button
    class="btn"
    onpointerdown={() => writeCoin(true)}
    onpointerup={() => writeCoin(false)}
    onpointerleave={() => writeCoin(false)}
  >
    INSERT COIN
  </button>
  <button
    class="btn"
    onpointerdown={() => writeP1Start(true)}
    onpointerup={() => writeP1Start(false)}
    onpointerleave={() => writeP1Start(false)}
  >
    1P START
  </button>
  <button
    class="btn"
    onpointerdown={() => writeP2Start(true)}
    onpointerup={() => writeP2Start(false)}
    onpointerleave={() => writeP2Start(false)}
  >
    2P START
  </button>
  <span class="keys">← → Space · C = Coin · 1 2 = Start</span>
</div>

<style>
  .control-panel {
    background: #111;
    border: 6px solid #444;
    border-top: none;
    border-radius: 0 0 8px 8px;
    padding: 0.75rem 1.5rem;
    display: flex;
    align-items: center;
    gap: 1rem;
    width: 100%;
    justify-content: center;
  }

  .btn {
    background: #222;
    color: #ccc;
    border: 2px solid #555;
    border-radius: 4px;
    padding: 0.35rem 0.9rem;
    font-family: monospace;
    font-size: 0.75rem;
    letter-spacing: 0.08em;
    cursor: pointer;
    user-select: none;
    transition: background 0.1s, color 0.1s;
  }

  .btn:active,
  .btn:hover {
    background: #444;
    color: #fff;
  }

  .keys {
    font-family: monospace;
    font-size: 0.7rem;
    color: #555;
    margin-left: 0.5rem;
  }
</style>
