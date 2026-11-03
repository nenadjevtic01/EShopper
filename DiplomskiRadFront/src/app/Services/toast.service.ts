import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface IToast {
  id: number;
  type: 'success' | 'danger';
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private nextId: number = 1;
  private toastsSubject: BehaviorSubject<IToast[]> = new BehaviorSubject<
    IToast[]
  >([]);
  toasts$ = this.toastsSubject.asObservable();

  private show(type: IToast['type'], message: string): void {
    const toast: IToast = { id: this.nextId++, type, message };
    this.toastsSubject.next([...this.toastsSubject.value, toast]);
    setTimeout(() => this.remove(toast.id), 4000);
  }

  success(message: string): void {
    this.show('success', message);
  }

  error(message: string): void {
    this.show('danger', message);
  }

  errorFrom(err: any, fallback: string): void {
    this.error(
      err?.error?.errors?.[0]?.error || err?.error?.message || fallback,
    );
  }

  remove(id: number): void {
    this.toastsSubject.next(
      this.toastsSubject.value.filter((t) => t.id !== id),
    );
  }
}
