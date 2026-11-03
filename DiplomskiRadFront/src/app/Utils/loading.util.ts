import { Observable, timer, zip, map } from 'rxjs';

export function withMinDuration<T>(
  source$: Observable<T>,
  ms: number = 400,
): Observable<T> {
  return zip(source$, timer(ms)).pipe(map(([result]) => result));
}
