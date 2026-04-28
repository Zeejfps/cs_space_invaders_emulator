// Reactive touch-device detection.
class Touch {
  isTouch = $state(detect());
  constructor() {
    if (typeof window === 'undefined') return;
    const mq = window.matchMedia('(pointer: coarse)');
    mq.addEventListener('change', () => { this.isTouch = detect(); });
  }
}

function detect(): boolean {
  if (typeof window === 'undefined') return false;
  return ('ontouchstart' in window) || window.matchMedia('(pointer: coarse)').matches;
}

export const touch = new Touch();
