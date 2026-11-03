import { Component } from '@angular/core';
import { IToast, ToastService } from '../../Services/toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss'],
})
export class ToastComponent {
  toasts: IToast[] = [];

  constructor(private toastService: ToastService) {
    this.toastService.toasts$.subscribe((toasts) => (this.toasts = toasts));
  }

  close(id: number): void {
    this.toastService.remove(id);
  }

  iconFor(type: string): string {
    if (type === 'success') {
      return 'fa-check-circle';
    }

    return 'fa-times-circle';
  }
}
