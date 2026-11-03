import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { PaymentMethodService } from '../../../Services/payment-method.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-payment-methods',
  templateUrl: './admin-payment-methods.component.html',
})
export class AdminPaymentMethodsComponent implements OnInit {
  headers: any;
  paymentMethods: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  includeInactive: boolean = false;

  showCreateForm: boolean = false;
  creating: boolean = false;
  createName: string = '';

  editingId: number | null = null;
  saving: boolean = false;
  editName: string = '';

  constructor(
    private paymentMethodService: PaymentMethodService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.load();
  }

  load(): void {
    this.loading = true;

    withMinDuration(
      this.paymentMethodService.getPaymentMethods(
        this.headers,
        this.includeInactive,
      ),
    ).subscribe({
      next: (data: any) => {
        this.paymentMethods = data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load payment methods.');
      },
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;

    if (this.showCreateForm) {
      this.editingId = null;
      this.createName = '';
    }
  }

  createPaymentMethod(): void {
    if (!this.createName.trim()) {
      this.toastService.error('Name is required.');
      return;
    }

    this.creating = true;

    this.paymentMethodService
      .create({ Name: this.createName }, this.headers)
      .subscribe({
        next: () => {
          this.creating = false;
          this.showCreateForm = false;
          this.toastService.success('Payment method created.');
          this.load();
        },
        error: (err: any) => {
          this.creating = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error ||
              'Could not create payment method.',
          );
        },
      });
  }

  startEdit(pm: any): void {
    this.showCreateForm = false;
    this.editingId = pm.id;
    this.editName = pm.name;
  }

  cancelEdit(): void {
    this.editingId = null;
  }

  saveEdit(): void {
    if (!this.editName.trim()) {
      this.toastService.error('Name is required.');
      return;
    }

    this.saving = true;

    this.paymentMethodService
      .update(this.editingId as number, { Name: this.editName }, this.headers)
      .subscribe({
        next: () => {
          this.saving = false;
          this.editingId = null;
          this.toastService.success('Payment method updated.');
          this.load();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error ||
              'Could not update payment method.',
          );
        },
      });
  }

  async toggleActive(pm: any): Promise<void> {
    const message = pm.isActive
      ? `Deactivate "${pm.name}"? It will no longer be offered at checkout.`
      : `Reactivate "${pm.name}"?`;

    const confirmed = await this.confirmService.confirm(
      message,
      pm.isActive ? 'Deactivate payment method' : 'Reactivate payment method',
      pm.isActive ? 'Deactivate' : 'Reactivate',
      pm.isActive,
    );

    if (!confirmed) {
      return;
    }

    this.paymentMethodService.delete(pm.id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          pm.isActive
            ? 'Payment method deactivated.'
            : 'Payment method reactivated.',
        );
        this.load();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error || 'Could not update payment method.',
        );
      },
    });
  }
}
