import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CouponService } from '../../../Services/coupon.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-coupons',
  templateUrl: './admin-coupons.component.html',
  styleUrls: ['./admin-coupons.component.scss'],
})
export class AdminCouponsComponent implements OnInit {
  headers: any;
  coupons: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  includeInactive: boolean = false;
  keyword: string = '';

  showCreateForm: boolean = false;
  createForm!: FormGroup;
  creating: boolean = false;

  editingCouponId: number | null = null;
  editForm!: FormGroup;
  saving: boolean = false;

  expandedCouponIds: Set<number> = new Set<number>();
  sortColumn: string | null = null;
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(
    private couponService: CouponService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.createForm = new FormGroup<any>({
      code: new FormControl('', [Validators.required, Validators.minLength(3)]),
      discountPercentage: new FormControl(10, [
        Validators.required,
        Validators.min(1),
        Validators.max(100),
      ]),
      validUntil: new FormControl('', [Validators.required]),
    });

    this.loadCoupons();
  }

  loadCoupons(): void {
    this.loading = true;

    withMinDuration(
      this.couponService.search(
        this.keyword || undefined,
        this.includeInactive,
        this.headers,
      ),
    ).subscribe({
      next: (data: any) => {
        this.coupons = data.data;
        this.applySort();
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load coupons.');
      },
    });
  }

  search(): void {
    this.loadCoupons();
  }

  toggleExpand(couponId: number): void {
    if (this.expandedCouponIds.has(couponId)) {
      this.expandedCouponIds.delete(couponId);
    } else {
      this.expandedCouponIds.add(couponId);
    }
  }

  sortBy(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.applySort();
  }

  private applySort(): void {
    if (!this.sortColumn) {
      return;
    }

    const column = this.sortColumn;
    const dir = this.sortDirection === 'asc' ? 1 : -1;

    this.coupons = [...this.coupons].sort((a, b) => {
      let valueA = a[column];
      let valueB = b[column];

      if (column === 'validUntil') {
        valueA = new Date(valueA).getTime();
        valueB = new Date(valueB).getTime();
      }

      if (typeof valueA === 'string') {
        valueA = valueA.toLowerCase();
        valueB = (valueB || '').toLowerCase();
      }

      if (valueA < valueB) {
        return -1 * dir;
      }

      if (valueA > valueB) {
        return 1 * dir;
      }

      return 0;
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;

    if (this.showCreateForm) {
      this.editingCouponId = null;
    }
  }

  createCoupon(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.creating = true;

    const dataToSend = {
      Code: this.createForm.get('code')?.value,
      DiscountPercentage: this.createForm.get('discountPercentage')?.value,
      ValidUntil: this.createForm.get('validUntil')?.value,
    };

    this.couponService.create(dataToSend, this.headers).subscribe({
      next: () => {
        this.creating = false;
        this.showCreateForm = false;
        this.createForm.reset({ discountPercentage: 10 });
        this.toastService.success('Coupon created.');
        this.loadCoupons();
      },
      error: (err: any) => {
        this.creating = false;

        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            'Could not create coupon. Code might already be in use.',
        );
      },
    });
  }

  startEdit(coupon: any): void {
    this.showCreateForm = false;
    this.editingCouponId = coupon.id;

    this.editForm = new FormGroup<any>({
      code: new FormControl(coupon.code, [
        Validators.required,
        Validators.minLength(3),
      ]),
      discountPercentage: new FormControl(coupon.discountPercentage, [
        Validators.required,
        Validators.min(1),
        Validators.max(100),
      ]),
      validUntil: new FormControl(
        coupon.validUntil ? coupon.validUntil.substring(0, 10) : '',
        [Validators.required],
      ),
    });
  }

  cancelEdit(): void {
    this.editingCouponId = null;
  }

  saveEdit(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.saving = true;

    const dataToSend = {
      Code: this.editForm.get('code')?.value,
      DiscountPercentage: this.editForm.get('discountPercentage')?.value,
      ValidUntil: this.editForm.get('validUntil')?.value,
    };

    this.couponService
      .updateCoupon(this.editingCouponId as number, dataToSend, this.headers)
      .subscribe({
        next: () => {
          this.saving = false;
          this.editingCouponId = null;
          this.toastService.success('Coupon updated.');
          this.loadCoupons();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not update coupon.',
          );
        },
      });
  }

  async deleteCoupon(
    id: number,
    code: string,
    isActive: boolean,
  ): Promise<void> {
    const message = isActive
      ? `Delete coupon "${code}"? It will no longer be usable at checkout unless you show inactive coupons.`
      : `Restore coupon "${code}"?`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Delete coupon' : 'Restore coupon',
      isActive ? 'Delete' : 'Restore',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.couponService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Coupon deleted.' : 'Coupon restored.',
        );
        this.loadCoupons();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error || 'Could not update coupon.',
        );
      },
    });
  }
}
