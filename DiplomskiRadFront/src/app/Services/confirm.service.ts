import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface IConfirmRequest {
  title: string;
  message: string;
  confirmLabel: string;
  danger: boolean;
  resolve: (result: boolean) => void;
}

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  private requestSubject: BehaviorSubject<IConfirmRequest | null> =
    new BehaviorSubject<IConfirmRequest | null>(null);
  request$ = this.requestSubject.asObservable();

  confirm(
    message: string,
    title: string = 'Please confirm',
    confirmLabel: string = 'Delete',
    danger: boolean = true,
  ): Promise<boolean> {
    return new Promise<boolean>((resolve) => {
      this.requestSubject.next({
        title,
        message,
        confirmLabel,
        danger,
        resolve,
      });
    });
  }

  respond(result: boolean): void {
    const current = this.requestSubject.value;

    if (current) {
      current.resolve(result);
    }

    this.requestSubject.next(null);
  }
}
