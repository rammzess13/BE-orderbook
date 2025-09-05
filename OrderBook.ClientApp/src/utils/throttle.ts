export function throttle<T extends (...args: any[]) => any>(func: T, limit: number): T {
  let lastFunc: ReturnType<typeof setTimeout>;
  let lastRan: number;

  const context = this;

  return function(this: any, ...args: Parameters<T>): ReturnType<T> | undefined {
    const now = Date.now();

    if (lastRan && now - lastRan < limit) {
      clearTimeout(lastFunc);
      lastFunc = setTimeout(() => {
        lastRan = now;
        func.apply(context, args);
      }, limit - (now - lastRan));
    } else {
      lastRan = now;
      return func.apply(context, args);
    }
  } as T;
}
