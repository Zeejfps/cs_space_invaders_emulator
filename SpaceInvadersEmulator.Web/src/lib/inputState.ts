// Single source of truth for player inputs. Both keyboard handlers (GameCanvas)
// and on-screen controls (ControlPanel, MobileControlPad) write here; the RAF
// loop reads it once per frame and forwards to the emulator.

import {
  writeP1Left, writeP1Right, writeP1Fire,
  writeCoin, writeP1Start, writeP2Start,
} from './emulator';

export type InputKey = 'left' | 'right' | 'fire' | 'coin' | 'p1Start' | 'p2Start';

const writers: Record<InputKey, (p: boolean) => void> = {
  left:    writeP1Left,
  right:   writeP1Right,
  fire:    writeP1Fire,
  coin:    writeCoin,
  p1Start: writeP1Start,
  p2Start: writeP2Start,
};

const state: Record<InputKey, boolean> = {
  left: false, right: false, fire: false,
  coin: false, p1Start: false, p2Start: false,
};

export function setInput(key: InputKey, pressed: boolean): void {
  if (state[key] === pressed) return;
  state[key] = pressed;
  writers[key](pressed);
}

export function clearAllInputs(): void {
  for (const key of Object.keys(state) as InputKey[]) {
    if (state[key]) setInput(key, false);
  }
}
