export function isInArray<T>(value: any, array: readonly T[]): value is T {
  return array.includes(value);
}
