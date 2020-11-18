import {Observable, of, throwError} from 'rxjs';
import {delay, mergeMap, retryWhen} from 'rxjs/operators';

const DEFAULT_MAX_RETRIES = 5;
const DEFAULT_DELAY_MS = 1000;

export function delayRetry(delayMs: number = DEFAULT_DELAY_MS, maxRetry: number = DEFAULT_MAX_RETRIES) {
  let retries = maxRetry;
  return (src: Observable<any>) =>
    src.pipe(
      retryWhen((errors: Observable<any>) => errors.pipe(
        delay(delayMs),
        mergeMap(error => --retries > 0 ? of(error) : throwError('Max retries'))
      ))
    );
}
